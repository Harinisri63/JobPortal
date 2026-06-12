using Spectre.Console;

namespace JobPortal.UI;

internal static class ChartRenderer
{
    public static void RenderHiringFunnel(int pending, int shortlisted, int interviewed, int hired)
    {
        if (pending + shortlisted + interviewed + hired == 0)
        {
            PanelFactory.RenderInfo("No application data available for the funnel.");
            return;
        }

        var chart = new BarChart()
            .Width(60)
            .Label($"[bold {UITheme.Primary}]Hiring Funnel[/]")
            .CenterLabel()
            .AddItem("Applied",    pending,     Color.DeepSkyBlue1)
            .AddItem("Shortlisted", shortlisted, Color.Cyan1)
            .AddItem("Interviewed", interviewed, Color.SteelBlue1)
            .AddItem("Hired",      hired,        Color.SpringGreen1);

        AnsiConsole.Write(
            new Panel(chart)
                .Header($"[bold {UITheme.Primary}]  Recruitment Funnel Analytics  [/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse(UITheme.Primary))
                .Padding(1, 0));
    }

    public static void RenderJobAnalytics(int approved, int pending, int closed, int expired)
    {
        if (approved + pending + closed + expired == 0)
        {
            PanelFactory.RenderInfo("No job data available.");
            return;
        }

        var chart = new BarChart()
            .Width(60)
            .Label($"[bold {UITheme.Primary}]Job Status Overview[/]")
            .CenterLabel()
            .AddItem("Approved", approved, Color.SpringGreen1)
            .AddItem("Pending",  pending,  Color.Gold1)
            .AddItem("Closed",   closed,   Color.Grey50)
            .AddItem("Expired",  expired,  Color.IndianRed1);

        AnsiConsole.Write(
            new Panel(chart)
                .Header($"[bold {UITheme.Primary}]  Job Listings Analytics  [/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse(UITheme.Primary))
                .Padding(1, 0));
    }

    public static void RenderApplicationBreakdown(int pending, int shortlisted, int interviewed, int hired, int rejected)
    {
        int total = pending + shortlisted + interviewed + hired + rejected;
        if (total == 0)
        {
            PanelFactory.RenderInfo("No application data to chart.");
            return;
        }

        try
        {
            var chart = new BreakdownChart()
                .Width(60)
                .AddItem("Pending",     pending,     Color.Gold1)
                .AddItem("Shortlisted", shortlisted, Color.Cyan1)
                .AddItem("Interviewed", interviewed, Color.SteelBlue1)
                .AddItem("Hired",       hired,       Color.SpringGreen1)
                .AddItem("Rejected",    rejected,    Color.IndianRed1);

            AnsiConsole.Write(
                new Panel(chart)
                    .Header($"[bold {UITheme.Secondary}]  Application Status Breakdown  [/]")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse(UITheme.Secondary))
                    .Padding(1, 0));
        }
        catch {  }
    }
}
