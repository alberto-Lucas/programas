using System.Drawing.Drawing2D;

namespace ClaudeUsageMonitor;

internal static class Theme
{
    public static readonly Color Background = Color.FromArgb(24, 24, 27);
    public static readonly Color Card = Color.FromArgb(36, 36, 41);
    public static readonly Color Border = Color.FromArgb(58, 58, 66);
    public static readonly Color Text = Color.FromArgb(236, 236, 240);
    public static readonly Color Muted = Color.FromArgb(150, 150, 160);
    public static readonly Color Accent = Color.FromArgb(217, 119, 87);   // laranja Claude

    public static readonly Font Title = new("Segoe UI Semibold", 9.5f);
    public static readonly Font Big = new("Segoe UI Semibold", 20f);
    public static readonly Font Body = new("Segoe UI", 9f);
}

/// <summary>Cartão com título, valor em destaque, barra opcional e linhas de detalhe.</summary>
internal sealed class StatCard : Panel
{
    private readonly Label _title = new() { Dock = DockStyle.Top, Height = 20, ForeColor = Theme.Muted, Font = Theme.Title };
    private readonly Label _value = new() { Dock = DockStyle.Top, Height = 40, ForeColor = Theme.Text, Font = Theme.Big };
    private readonly Label _detail = new() { Dock = DockStyle.Fill, ForeColor = Theme.Text, Font = Theme.Body };
    private readonly ProgressStrip _bar = new() { Dock = DockStyle.Top, Height = 8, Visible = false };

    public StatCard(string title)
    {
        BackColor = Theme.Card;
        Padding = new Padding(12, 10, 12, 8);
        Margin = new Padding(6);
        Dock = DockStyle.Fill;
        _title.Text = title.ToUpperInvariant();
        Controls.Add(_detail);
        Controls.Add(new Panel { Dock = DockStyle.Top, Height = 6 });
        Controls.Add(_bar);
        Controls.Add(_value);
        Controls.Add(_title);
    }

    public void SetValue(string value, Color? color = null)
    {
        _value.Text = value;
        _value.ForeColor = color ?? Theme.Text;
    }

    public void SetDetail(string text) => _detail.Text = text;

    public void SetProgress(double? fraction, Color color)
    {
        _bar.Visible = fraction.HasValue;
        _bar.Fraction = fraction ?? 0;
        _bar.BarColor = color;
        _bar.Invalidate();
    }
}

internal sealed class ProgressStrip : Control
{
    public double Fraction { get; set; }
    public Color BarColor { get; set; } = Theme.Accent;

    public ProgressStrip()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(Parent?.BackColor ?? Theme.Card);
        using var bg = new SolidBrush(Theme.Border);
        e.Graphics.FillRectangle(bg, 0, 0, Width, Height);
        int w = (int)(Width * Math.Clamp(Fraction, 0, 1));
        using var fg = new SolidBrush(BarColor);
        e.Graphics.FillRectangle(fg, 0, 0, w, Height);
    }
}

/// <summary>Gráfico de barras com os tokens por minuto da última hora.</summary>
internal sealed class MinuteChart : Control
{
    private long[] _values = new long[60];

    public MinuteChart()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BackColor = Theme.Card;
        Margin = new Padding(6);
        Dock = DockStyle.Fill;
    }

    public void SetData(long[] values)
    {
        _values = values;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(BackColor);
        g.SmoothingMode = SmoothingMode.AntiAlias;

        const int padLeft = 12, padRight = 12, padTop = 28, padBottom = 20;
        long max = Math.Max(1, _values.Max());
        using var muted = new SolidBrush(Theme.Muted);
        g.DrawString("TOKENS POR MINUTO (ÚLTIMA HORA)", Theme.Title, muted, padLeft, 6);
        var maxText = "pico: " + Fmt.Tokens(max);
        var sz = g.MeasureString(maxText, Theme.Body);
        g.DrawString(maxText, Theme.Body, muted, Width - padRight - sz.Width, 7);

        float plotW = Width - padLeft - padRight;
        float plotH = Height - padTop - padBottom;
        if (plotW <= 0 || plotH <= 0)
            return;

        float slot = plotW / _values.Length;
        float barW = Math.Max(1, slot - 2);
        using var bar = new SolidBrush(Theme.Accent);
        using var baseline = new Pen(Theme.Border);
        g.DrawLine(baseline, padLeft, padTop + plotH, padLeft + plotW, padTop + plotH);
        for (int i = 0; i < _values.Length; i++)
        {
            if (_values[i] <= 0)
                continue;
            float h = Math.Max(2, (float)(_values[i] / (double)max * plotH));
            g.FillRectangle(bar, padLeft + i * slot + 1, padTop + plotH - h, barW, h);
        }
        g.DrawString("-60 min", Theme.Body, muted, padLeft, padTop + plotH + 2);
        var agora = "agora";
        sz = g.MeasureString(agora, Theme.Body);
        g.DrawString(agora, Theme.Body, muted, padLeft + plotW - sz.Width, padTop + plotH + 2);
    }
}
