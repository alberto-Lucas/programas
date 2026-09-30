using System.Diagnostics;
using System.Globalization;

namespace ClaudeUsageMonitor;

internal static class Fmt
{
    public static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    public static string Tokens(long n) => n switch
    {
        >= 1_000_000_000 => (n / 1_000_000_000d).ToString("0.##", PtBr) + " B",
        >= 1_000_000 => (n / 1_000_000d).ToString("0.##", PtBr) + " M",
        >= 10_000 => (n / 1_000d).ToString("0.#", PtBr) + " mil",
        _ => n.ToString("N0", PtBr),
    };

    public static string Money(decimal v) => "US$ " + v.ToString("N2", PtBr);

    public static string Duration(TimeSpan t) =>
        t.TotalHours >= 1 ? $"{(int)t.TotalHours}h{t.Minutes:00}min" : $"{Math.Max(0, t.Minutes)}min";

    public static string Time(DateTime utc) => utc.ToLocalTime().ToString("HH:mm", PtBr);
}

internal sealed class MainForm : Form
{
    private readonly UsageReader _reader = new();
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 2000 };
    private bool _busy;

    private readonly StatCard _blockCard = new("Bloco atual (5 h)");
    private readonly StatCard _todayCard = new("Hoje");
    private readonly StatCard _monthCard = new("Este mês");
    private readonly MinuteChart _chart = new();
    private readonly ListView _models = CreateList(("Modelo (hoje)", 170), ("Req.", 50), ("Entrada", 75), ("Saída", 75), ("Cache", 80), ("Custo", 85));
    private readonly ListView _recent = CreateList(("Hora", 65), ("Modelo", 150), ("Projeto", 130), ("Entrada", 70), ("Saída", 70), ("Cache", 80), ("Custo", 75));
    private readonly Label _status = new() { Dock = DockStyle.Fill, ForeColor = Theme.Muted, Font = Theme.Body, TextAlign = ContentAlignment.MiddleLeft, AutoEllipsis = true };

    public MainForm()
    {
        Text = "Claude Usage Monitor";
        BackColor = Theme.Background;
        ForeColor = Theme.Text;
        Font = Theme.Body;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(960, 720);
        MinimumSize = new Size(760, 560);

        var cards = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, Margin = Padding.Empty };
        for (int i = 0; i < 3; i++)
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        cards.Controls.Add(_blockCard, 0, 0);
        cards.Controls.Add(_todayCard, 1, 0);
        cards.Controls.Add(_monthCard, 2, 0);

        var lists = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = Padding.Empty };
        lists.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
        lists.RowStyles.Add(new RowStyle(SizeType.Percent, 65));
        lists.Controls.Add(_models, 0, 0);
        lists.Controls.Add(_recent, 0, 1);

        var topMost = new CheckBox { Text = "Sempre visível", AutoSize = true, ForeColor = Theme.Text, Margin = new Padding(6, 8, 6, 0) };
        topMost.CheckedChanged += (_, _) => TopMost = topMost.Checked;
        var openFolder = new Button { Text = "Abrir pasta de logs", AutoSize = true, FlatStyle = FlatStyle.Flat, ForeColor = Theme.Text, BackColor = Theme.Card, Margin = new Padding(6, 4, 6, 0) };
        openFolder.FlatAppearance.BorderColor = Theme.Border;
        openFolder.Click += (_, _) => OpenLogsFolder();

        var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, Margin = Padding.Empty };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        footer.Controls.Add(_status, 0, 0);
        footer.Controls.Add(topMost, 1, 0);
        footer.Controls.Add(openFolder, 2, 0);

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(8) };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 130));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        root.Controls.Add(cards, 0, 0);
        root.Controls.Add(_chart, 0, 1);
        root.Controls.Add(lists, 0, 2);
        root.Controls.Add(footer, 0, 3);
        Controls.Add(root);

        _status.Text = _reader.Roots.Count == 0
            ? "Nenhuma pasta de logs do Claude Code encontrada (~/.claude/projects)."
            : "Carregando logs…";

        _timer.Tick += async (_, _) => await RefreshAsync();
        Shown += async (_, _) => { await RefreshAsync(); _timer.Start(); };
    }

    private static ListView CreateList(params (string Name, int Width)[] columns)
    {
        var lv = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            HeaderStyle = ColumnHeaderStyle.Nonclickable,
            BackColor = Theme.Card,
            ForeColor = Theme.Text,
            BorderStyle = BorderStyle.None,
            Margin = new Padding(6),
        };
        foreach (var (name, width) in columns)
            lv.Columns.Add(name, width, name is "Hora" or "Modelo" or "Projeto" or "Modelo (hoje)" ? HorizontalAlignment.Left : HorizontalAlignment.Right);
        return lv;
    }

    private async Task RefreshAsync()
    {
        if (_busy)
            return;
        _busy = true;
        try
        {
            var now = DateTime.UtcNow;
            var stats = await Task.Run(() =>
            {
                _reader.Poll();
                return UsageStats.Compute(_reader.Snapshot(), now);
            });
            Render(stats, now);
        }
        catch (Exception ex)
        {
            _status.Text = "Erro ao ler os logs: " + ex.Message;
        }
        finally
        {
            _busy = false;
        }
    }

    private void Render(UsageStats s, DateTime nowUtc)
    {
        RenderBlock(s, nowUtc);

        _todayCard.SetValue(Fmt.Money(s.Today.Cost));
        _todayCard.SetDetail(Details(s.Today));
        _monthCard.SetValue(Fmt.Money(s.Month.Cost));
        _monthCard.SetDetail(Details(s.Month));
        _chart.SetData(s.LastHourPerMinute);

        _models.BeginUpdate();
        _models.Items.Clear();
        foreach (var (model, t) in s.TodayByModel.OrderByDescending(kv => kv.Value.Cost))
            _models.Items.Add(new ListViewItem(new[]
            {
                model, t.Requests.ToString("N0", Fmt.PtBr), Fmt.Tokens(t.Input), Fmt.Tokens(t.Output),
                Fmt.Tokens(t.CacheWrite + t.CacheRead), Fmt.Money(t.Cost),
            }));
        _models.EndUpdate();

        _recent.BeginUpdate();
        _recent.Items.Clear();
        foreach (var e in s.Recent)
            _recent.Items.Add(new ListViewItem(new[]
            {
                e.Timestamp.ToLocalTime().ToString("HH:mm:ss", Fmt.PtBr), e.Model, e.Project,
                Fmt.Tokens(e.Input), Fmt.Tokens(e.Output), Fmt.Tokens(e.CacheWrite + e.CacheRead), Fmt.Money(e.Cost),
            }));
        _recent.EndUpdate();

        _status.Text = $"Atualizado às {DateTime.Now:HH:mm:ss} · {s.TotalEntries:N0} requisições lidas · " +
                       "custo estimado com preços da API · " + string.Join("; ", _reader.Roots);
    }

    private void RenderBlock(UsageStats s, DateTime nowUtc)
    {
        var b = s.CurrentBlock;
        if (b is null)
        {
            _blockCard.SetValue("Sem bloco ativo", Theme.Muted);
            _blockCard.SetProgress(null, Theme.Accent);
            _blockCard.SetDetail("Um novo bloco de 5 h começa na próxima mensagem enviada ao Claude.");
            return;
        }

        var remaining = b.End - nowUtc;
        double elapsedMin = Math.Max(1, (nowUtc - b.FirstEntry).TotalMinutes);
        double tokensPerMin = b.Totals.InOut / elapsedMin;
        decimal costPerHour = b.Totals.Cost / (decimal)(elapsedMin / 60);
        decimal projected = b.Totals.Cost + costPerHour * (decimal)Math.Max(0, remaining.TotalHours);

        // Referência de "limite": o maior bloco já registrado. Não é o limite oficial do plano.
        double? usedFraction = s.MaxPreviousBlockTokens > 0 ? b.Totals.InOut / (double)s.MaxPreviousBlockTokens : null;
        _blockCard.SetValue(Fmt.Money(b.Totals.Cost), Theme.Accent);
        _blockCard.SetProgress(1 - remaining.TotalMinutes / UsageBlock.Duration.TotalMinutes, Theme.Accent);
        _blockCard.SetDetail(
            $"{Fmt.Time(b.Start)} → {Fmt.Time(b.End)} · reinicia em {Fmt.Duration(remaining)}\n" +
            $"Tokens: {Fmt.Tokens(b.Totals.InOut)} (entrada+saída) · {b.Totals.Requests:N0} req.\n" +
            $"Ritmo: {Fmt.Tokens((long)tokensPerMin)}/min · {Fmt.Money(costPerHour)}/h\n" +
            $"Projeção até o fim: {Fmt.Money(projected)}" +
            (usedFraction.HasValue ? $"\nvs. maior bloco anterior: {usedFraction.Value:P0}" : ""));
    }

    private static string Details(TokenTotals t) =>
        $"Entrada: {Fmt.Tokens(t.Input)}  ·  Saída: {Fmt.Tokens(t.Output)}\n" +
        $"Cache escrita: {Fmt.Tokens(t.CacheWrite)}\n" +
        $"Cache leitura: {Fmt.Tokens(t.CacheRead)}\n" +
        $"Requisições: {t.Requests:N0}";

    private void OpenLogsFolder()
    {
        var dir = _reader.Roots.FirstOrDefault();
        if (dir is null)
        {
            MessageBox.Show(this, "Nenhuma pasta de logs encontrada.", Text);
            return;
        }
        Process.Start(new ProcessStartInfo { FileName = dir, UseShellExecute = true });
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _timer.Dispose();
        base.OnFormClosed(e);
    }
}
