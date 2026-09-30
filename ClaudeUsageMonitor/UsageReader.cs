using System.Globalization;
using System.Text;
using System.Text.Json;

namespace ClaudeUsageMonitor;

public sealed class UsageEntry
{
    public DateTime Timestamp { get; init; }   // UTC
    public string Model { get; init; } = "";
    public string Project { get; init; } = "";
    public long Input { get; init; }
    public long Output { get; init; }
    public long CacheWrite5m { get; init; }
    public long CacheWrite1h { get; init; }
    public long CacheRead { get; init; }
    public bool Fast { get; init; }
    public decimal Cost { get; set; }

    public long CacheWrite => CacheWrite5m + CacheWrite1h;
}

/// <summary>
/// Lê os logs .jsonl que o Claude Code grava em ~/.claude/projects.
/// A leitura é incremental: cada arquivo guarda o offset já processado, então
/// chamar <see cref="Poll"/> a cada poucos segundos só lê as linhas novas.
/// </summary>
public sealed class UsageReader
{
    private static readonly TimeSpan DirRescanInterval = TimeSpan.FromSeconds(10);

    private readonly object _lock = new();
    private readonly Dictionary<string, long> _offsets = new();
    // Chave = message.id + requestId. O Claude Code grava uma linha por bloco de conteúdo
    // com o mesmo usage, então sem deduplicar os tokens seriam contados várias vezes.
    private readonly Dictionary<string, UsageEntry> _entries = new();
    private List<string> _files = new();
    private DateTime _lastDirScan = DateTime.MinValue;

    public IReadOnlyList<string> Roots { get; }

    public UsageReader(IReadOnlyList<string>? roots = null)
    {
        Roots = roots ?? FindRoots();
    }

    public int EntryCount
    {
        get { lock (_lock) return _entries.Count; }
    }

    /// <summary>Pastas "projects" do Claude Code que existem nesta máquina.</summary>
    public static List<string> FindRoots()
    {
        var candidates = new List<string>();
        var env = Environment.GetEnvironmentVariable("CLAUDE_CONFIG_DIR");
        if (!string.IsNullOrWhiteSpace(env))
        {
            foreach (var dir in env.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                candidates.Add(Path.Combine(dir, "projects"));
        }
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        candidates.Add(Path.Combine(home, ".claude", "projects"));
        candidates.Add(Path.Combine(home, ".config", "claude", "projects"));

        return candidates
            .Select(Path.GetFullPath)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(Directory.Exists)
            .ToList();
    }

    /// <summary>Lê o que foi acrescentado aos logs desde a última chamada. Retorna quantas entradas mudaram.</summary>
    public int Poll()
    {
        if (DateTime.UtcNow - _lastDirScan > DirRescanInterval)
        {
            _files = Roots
                .SelectMany(r => SafeEnumerate(r))
                .ToList();
            _lastDirScan = DateTime.UtcNow;
        }

        int changed = 0;
        foreach (var file in _files)
            changed += ReadNewLines(file);
        return changed;
    }

    public List<UsageEntry> Snapshot()
    {
        lock (_lock)
            return _entries.Values.ToList();
    }

    private static IEnumerable<string> SafeEnumerate(string root)
    {
        try
        {
            return Directory.EnumerateFiles(root, "*.jsonl", SearchOption.AllDirectories).ToList();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return Array.Empty<string>();
        }
    }

    private int ReadNewLines(string file)
    {
        _offsets.TryGetValue(file, out long offset);
        byte[] buffer;
        try
        {
            using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            long length = fs.Length;
            if (length < offset)
                offset = 0; // arquivo foi truncado/reescrito
            if (length == offset)
                return 0;

            fs.Seek(offset, SeekOrigin.Begin);
            buffer = new byte[length - offset];
            fs.ReadExactly(buffer);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return 0;
        }

        // Só processa até a última quebra de linha: a linha final pode estar sendo escrita agora.
        int end = Array.LastIndexOf(buffer, (byte)'\n');
        if (end < 0)
            return 0;
        _offsets[file] = offset + end + 1;

        int changed = 0;
        var text = Encoding.UTF8.GetString(buffer, 0, end);
        foreach (var line in text.Split('\n'))
        {
            var parsed = ParseLine(line, file);
            if (parsed is null)
                continue;
            var (key, entry) = parsed.Value;
            entry.Cost = Pricing.Cost(entry);
            lock (_lock)
                _entries[key] = entry;
            changed++;
        }
        return changed;
    }

    internal static (string Key, UsageEntry Entry)? ParseLine(string line, string file)
    {
        if (line.Length == 0 || !line.Contains("\"usage\"", StringComparison.Ordinal))
            return null;
        try
        {
            using var doc = JsonDocument.Parse(line);
            var root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object
                || !root.TryGetProperty("message", out var msg) || msg.ValueKind != JsonValueKind.Object
                || !msg.TryGetProperty("usage", out var usage) || usage.ValueKind != JsonValueKind.Object)
                return null;

            var tsText = GetString(root, "timestamp");
            if (tsText is null || !DateTime.TryParse(tsText, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var ts))
                return null;

            long input = GetLong(usage, "input_tokens");
            long output = GetLong(usage, "output_tokens");
            long cacheRead = GetLong(usage, "cache_read_input_tokens");
            long cacheCreate = GetLong(usage, "cache_creation_input_tokens");
            long write5m = cacheCreate, write1h = 0;
            if (usage.TryGetProperty("cache_creation", out var cc) && cc.ValueKind == JsonValueKind.Object)
            {
                write5m = GetLong(cc, "ephemeral_5m_input_tokens");
                write1h = GetLong(cc, "ephemeral_1h_input_tokens");
            }
            if (input + output + cacheRead + write5m + write1h == 0)
                return null;

            string model = GetString(msg, "model") ?? "desconhecido";
            string? msgId = GetString(msg, "id");
            string key = msgId is not null
                ? msgId + ":" + GetString(root, "requestId")
                : GetString(root, "uuid") ?? file + ":" + line.GetHashCode();

            return (key, new UsageEntry
            {
                Timestamp = ts,
                Model = model,
                Project = ProjectName(GetString(root, "cwd"), file),
                Input = input,
                Output = output,
                CacheWrite5m = write5m,
                CacheWrite1h = write1h,
                CacheRead = cacheRead,
                Fast = GetString(usage, "speed") == "fast",
            });
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string ProjectName(string? cwd, string file)
    {
        if (!string.IsNullOrEmpty(cwd))
        {
            var name = cwd.TrimEnd('/', '\\').Split('/', '\\')[^1];
            if (name.Length > 0)
                return name;
        }
        return Path.GetFileName(Path.GetDirectoryName(file)) ?? "";
    }

    private static string? GetString(JsonElement el, string name) =>
        el.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.String ? p.GetString() : null;

    private static long GetLong(JsonElement el, string name) =>
        el.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.Number && p.TryGetInt64(out var v) ? v : 0;
}
