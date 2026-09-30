namespace ClaudeUsageMonitor;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Pricing.LoadOverrides(Path.Combine(AppContext.BaseDirectory, "precos.json"));
        Application.Run(new MainForm());
    }
}
