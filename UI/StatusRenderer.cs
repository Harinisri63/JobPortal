using Spectre.Console;

namespace JobPortal.UI;

/// <summary>
/// Visual application pipeline tracker and status badge renderer.
/// Presentation-only. No business logic.
/// </summary>
internal static class StatusRenderer
{
    // ── Application Pipeline ───────────────────────────────────────
    private static readonly (string Key, string Label)[] _pipeline =
    [
        ("PENDING",              "Application Submitted"),
        ("SHORTLISTED",          "Resume Reviewed & Shortlisted"),
        ("INTERVIEW_SCHEDULED",  "Interview Scheduled"),
        ("HIRED",                "Offer Extended  ·  Hired"),
    ];

    public static void RenderApplicationPipeline(string currentStatus)
    {
        string upper = currentStatus.ToUpperInvariant();
        bool rejected = upper is "REJECTED" or "CANCELLED";

        // Compute current step index
        int currentIdx = -1;
        for (int i = 0; i < _pipeline.Length; i++)
        {
            if (_pipeline[i].Key == upper) { currentIdx = i; break; }
        }
        // If hired then all steps done
        if (upper == "HIRED") currentIdx = _pipeline.Length - 1;

        var rows = new List<string>();
        for (int i = 0; i < _pipeline.Length; i++)
        {
            string icon, color;
            if (rejected && i == 0)
            {
                // Show submitted step as done
                icon  = "✓"; color = UITheme.Success;
            }
            else if (rejected && i > 0)
            {
                // Rest are irrelevant
                icon  = "□"; color = UITheme.Dim;
            }
            else if (i < currentIdx)
            {
                icon  = "✓"; color = UITheme.Success;
            }
            else if (i == currentIdx)
            {
                icon  = "➜"; color = UITheme.Primary;
            }
            else
            {
                icon  = "□"; color = UITheme.Dim;
            }

            rows.Add($"[bold {color}]{icon}[/]  [{color}]{Markup.Escape(_pipeline[i].Label)}[/]");
        }

        if (rejected)
        {
            rows.Add($"[bold {UITheme.Error}]✗[/]  [{UITheme.Error}]Application {Markup.Escape(currentStatus.ToUpperInvariant())}[/]");
        }

        string body = string.Join("\n", rows);
        string borderColor = rejected ? UITheme.Error
            : upper == "HIRED" ? UITheme.Success
            : UITheme.Primary;

        AnsiConsole.Write(
            new Panel(body)
                .Header($"[bold {borderColor}]  Application Progress  [/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse(borderColor))
                .Padding(2, 0));
    }

    // ── Inline Status Badge ────────────────────────────────────────
    public static void PrintStatusBadge(string status)
    {
        string color = UITheme.StatusColor(status);
        string icon  = UITheme.StatusIcon(status);
        AnsiConsole.Markup($"[bold {color}]{icon} {Markup.Escape(status)}[/]");
    }

    // ── Job Status Summary Bar ─────────────────────────────────────
    public static void RenderJobStatusSummary(
        int open, int pending, int closed, int expired)
    {
        int total = open + pending + closed + expired;
        if (total == 0) return;

        try
        {
            var chart = new BreakdownChart()
                .Width(60)
                .AddItem("Open",    open,    Color.SpringGreen1)
                .AddItem("Pending", pending, Color.Gold1)
                .AddItem("Closed",  closed,  Color.Grey50)
                .AddItem("Expired", expired, Color.IndianRed1);

            AnsiConsole.Write(
                new Panel(chart)
                    .Header($"[bold {UITheme.Primary}]  Job Status Distribution  [/]")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse(UITheme.Primary))
                    .Padding(1, 0));
        }
        catch
        {
            // BreakdownChart requires > 0 total; fallback gracefully
        }
    }
}
