using System.Text.Json;

namespace ClaudeUsageMonitor;

/// <summary>Preços em US$ por milhão de tokens.</summary>
public sealed record ModelPrice(decimal Input, decimal Output, decimal CacheWrite5m, decimal CacheWrite1h, decimal CacheRead);

/// <summary>
/// Tabela de preços da API usada para estimar o custo. Para assinantes Pro/Max o valor
/// é apenas o "equivalente em API" — não é o que você paga.
/// Os valores podem ser sobrescritos com um arquivo precos.json ao lado do .exe.
/// </summary>
public static class Pricing
{
    // Escrita em cache: 1,25x (5 min) e 2x (1 h) o preço de entrada; leitura: 0,1x, salvo exceções.
    private static ModelPrice P(decimal input, decimal output, decimal? cacheRead = null) =>
        new(input, output, input * 1.25m, input * 2m, cacheRead ?? input * 0.1m);

    private static readonly Dictionary<string, ModelPrice> Table = new(StringComparer.OrdinalIgnoreCase)
    {
        ["claude-fable-5-1"] = P(10m, 50m, 0.25m),
        ["claude-mythos-5-1"] = P(10m, 50m, 0.25m),
        ["claude-fable-5"] = P(10m, 50m),
        ["claude-mythos-5"] = P(10m, 50m),
        ["claude-opus-5-5"] = P(4m, 20m, 0.20m),
        ["claude-opus-5"] = P(5m, 25m),
        ["claude-opus-4-8"] = P(5m, 25m),
        ["claude-opus-4-7"] = P(5m, 25m),
        ["claude-opus-4-6"] = P(5m, 25m),
        ["claude-opus-4-5"] = P(5m, 25m),
        ["claude-opus-4"] = P(15m, 75m),
        ["claude-sonnet-5-5"] = P(2m, 10m, 0.20m),
        ["claude-sonnet-5"] = P(2m, 10m),
        ["claude-sonnet-4"] = P(3m, 15m),
        ["claude-3-7-sonnet"] = P(3m, 15m),
        ["claude-3-5-sonnet"] = P(3m, 15m),
        ["claude-haiku-4-5"] = P(1m, 5m),
        ["claude-3-5-haiku"] = P(0.8m, 4m),
        ["claude-3-haiku"] = P(0.25m, 1.25m),
    };

    /// <summary>Usado quando o modelo não está na tabela.</summary>
    private static readonly ModelPrice Fallback = P(3m, 15m);

    /// <summary>
    /// Formato do precos.json:
    /// { "claude-opus-5-5": { "input": 4, "output": 20, "cacheWrite5m": 5, "cacheWrite1h": 8, "cacheRead": 0.2 } }
    /// Campos de cache omitidos são calculados a partir do preço de entrada.
    /// </summary>
    public static void LoadOverrides(string path)
    {
        if (!File.Exists(path))
            return;
        try
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(path));
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                var v = prop.Value;
                decimal input = Get(v, "input") ?? 0m;
                decimal output = Get(v, "output") ?? 0m;
                Table[prop.Name] = new ModelPrice(
                    input,
                    output,
                    Get(v, "cacheWrite5m") ?? input * 1.25m,
                    Get(v, "cacheWrite1h") ?? input * 2m,
                    Get(v, "cacheRead") ?? input * 0.1m);
            }
        }
        catch (Exception ex) when (ex is JsonException or IOException or InvalidOperationException)
        {
            // Arquivo inválido: mantém a tabela padrão.
        }

        static decimal? Get(JsonElement el, string name) =>
            el.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.Number ? p.GetDecimal() : null;
    }

    /// <summary>Procura pelo prefixo mais longo, ignorando prefixos de provedor (ex.: "us.anthropic.").</summary>
    public static ModelPrice Find(string model)
    {
        int idx = model.IndexOf("claude-", StringComparison.OrdinalIgnoreCase);
        string name = idx > 0 ? model[idx..] : model;

        ModelPrice? best = null;
        int bestLen = -1;
        foreach (var (prefix, price) in Table)
        {
            if (prefix.Length > bestLen && name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                best = price;
                bestLen = prefix.Length;
            }
        }
        return best ?? Fallback;
    }

    public static decimal Cost(UsageEntry e)
    {
        var p = Find(e.Model);
        decimal cost = (e.Input * p.Input
                        + e.Output * p.Output
                        + e.CacheWrite5m * p.CacheWrite5m
                        + e.CacheWrite1h * p.CacheWrite1h
                        + e.CacheRead * p.CacheRead) / 1_000_000m;
        // Fast mode custa o dobro do padrão.
        return e.Fast ? cost * 2m : cost;
    }
}
