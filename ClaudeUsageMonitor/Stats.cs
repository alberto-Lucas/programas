namespace ClaudeUsageMonitor;

public sealed class TokenTotals
{
    public long Input { get; private set; }
    public long Output { get; private set; }
    public long CacheWrite { get; private set; }
    public long CacheRead { get; private set; }
    public decimal Cost { get; private set; }
    public int Requests { get; private set; }

    /// <summary>Entrada + saída (sem cache).</summary>
    public long InOut => Input + Output;
    public long All => Input + Output + CacheWrite + CacheRead;

    public void Add(UsageEntry e)
    {
        Input += e.Input;
        Output += e.Output;
        CacheWrite += e.CacheWrite;
        CacheRead += e.CacheRead;
        Cost += e.Cost;
        Requests++;
    }
}

/// <summary>
/// Janela de 5 horas, a mesma unidade usada pelos limites de uso dos planos Pro/Max.
/// Começa na hora cheia da primeira mensagem e termina 5 h depois.
/// </summary>
public sealed class UsageBlock
{
    public static readonly TimeSpan Duration = TimeSpan.FromHours(5);

    public DateTime Start { get; init; }       // UTC
    public DateTime End => Start + Duration;
    public DateTime FirstEntry { get; init; }
    public DateTime LastEntry { get; set; }
    public TokenTotals Totals { get; } = new();

    public bool IsActive(DateTime nowUtc) => nowUtc < End;
}

public sealed class UsageStats
{
    public TokenTotals Today { get; } = new();
    public TokenTotals Month { get; } = new();
    public UsageBlock? CurrentBlock { get; private set; }
    /// <summary>Maior bloco anterior (em tokens entrada+saída), usado como referência de "limite".</summary>
    public long MaxPreviousBlockTokens { get; private set; }
    public Dictionary<string, TokenTotals> TodayByModel { get; } = new();
    /// <summary>Tokens (entrada+saída) por minuto nos últimos 60 minutos; o último item é o minuto atual.</summary>
    public long[] LastHourPerMinute { get; } = new long[60];
    public List<UsageEntry> Recent { get; private set; } = new();
    public int TotalEntries { get; private set; }

    public static UsageStats Compute(List<UsageEntry> entries, DateTime nowUtc, int recentCount = 100)
    {
        var s = new UsageStats { TotalEntries = entries.Count };
        entries.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));

        var nowLocal = nowUtc.ToLocalTime();
        var todayStart = nowLocal.Date.ToUniversalTime();
        var monthStart = new DateTime(nowLocal.Year, nowLocal.Month, 1, 0, 0, 0, DateTimeKind.Local).ToUniversalTime();
        var currentMinute = new DateTime(nowUtc.Ticks - nowUtc.Ticks % TimeSpan.TicksPerMinute, DateTimeKind.Utc);

        UsageBlock? block = null;
        foreach (var e in entries)
        {
            if (block is null || e.Timestamp >= block.End)
            {
                if (block is not null)
                    s.MaxPreviousBlockTokens = Math.Max(s.MaxPreviousBlockTokens, block.Totals.InOut);
                var hour = new DateTime(e.Timestamp.Year, e.Timestamp.Month, e.Timestamp.Day, e.Timestamp.Hour, 0, 0, DateTimeKind.Utc);
                block = new UsageBlock { Start = hour, FirstEntry = e.Timestamp };
            }
            block.Totals.Add(e);
            block.LastEntry = e.Timestamp;

            if (e.Timestamp >= monthStart)
                s.Month.Add(e);
            if (e.Timestamp >= todayStart)
            {
                s.Today.Add(e);
                if (!s.TodayByModel.TryGetValue(e.Model, out var t))
                    s.TodayByModel[e.Model] = t = new TokenTotals();
                t.Add(e);
            }

            int minutesAgo = e.Timestamp >= currentMinute
                ? 0
                : (int)Math.Ceiling((currentMinute - e.Timestamp).TotalMinutes);
            if (minutesAgo < 60)
                s.LastHourPerMinute[59 - minutesAgo] += e.Input + e.Output;
        }

        if (block is not null)
        {
            if (block.IsActive(nowUtc))
                s.CurrentBlock = block;
            else
                s.MaxPreviousBlockTokens = Math.Max(s.MaxPreviousBlockTokens, block.Totals.InOut);
        }

        s.Recent = entries.Skip(Math.Max(0, entries.Count - recentCount)).Reverse().ToList();
        return s;
    }
}
