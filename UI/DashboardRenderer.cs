using Spectre.Console;

namespace JobPortal.UI;

/// <summary>
/// Renders multi-panel metric dashboards for Admin and Candidate roles.
/// Presentation-only. All metric values are passed in — no service calls.
/// </summary>
internal static class DashboardRenderer
{
    // ── Admin Dashboard ────────────────────────────────────────────
    public static void RenderAdminDashboard(
        string adminName,
        string company,
        int    unreadNotifications,
        int    totalJobs,
        int    pendingApprovals,
        int    closedJobs,
        int    totalSeekers,
        int    activeSeekers,
        int    totalAdmins,
        int    totalApplications,
        int    shortlisted,
        int    interviews)
    {
        AnsiConsole.WriteLine();
        PanelFactory.RenderWelcomeBanner(adminName, "ADMIN");
        AnsiConsole.WriteLine();

        // Row 1: Candidate | Job | Application metrics
        var row1 = new Columns(
            PanelFactory.MetricPanel(
                "⬡  Candidates",
                totalSeekers.ToString(),
                $"{activeSeekers} active",
                UITheme.Candidate),

            PanelFactory.MetricPanel(
                "◈  Job Listings",
                totalJobs.ToString(),
                $"{pendingApprovals} pending approval  ·  {closedJobs} closed",
                UITheme.Primary),

            PanelFactory.MetricPanel(
                "▶  Applications",
                totalApplications.ToString(),
                $"{shortlisted} shortlisted  ·  {interviews} interviews",
                UITheme.Secondary)
        );
        AnsiConsole.Write(row1);
        AnsiConsole.WriteLine();

        // Row 2: Admin count | Notifications | Platform
        var row2 = new Columns(
            PanelFactory.MetricPanel(
                "⬡  Admin Accounts",
                totalAdmins.ToString(),
                $"Company: {company}",
                UITheme.Admin),

            PanelFactory.MetricPanel(
                "◎  Notifications",
                unreadNotifications.ToString(),
                "unread messages",
                unreadNotifications > 0 ? UITheme.Warning : UITheme.Dim),

            PanelFactory.MetricPanel(
                "●  System",
                UITheme.AppEnv,
                $"{UITheme.Today()}",
                UITheme.Dim)
        );
        AnsiConsole.Write(row2);
        AnsiConsole.WriteLine();
    }

    // ── Candidate Dashboard ────────────────────────────────────────
    public static void RenderCandidateDashboard(
        string seekerName,
        int    unreadNotifications,
        int    appliedJobs,
        int    savedJobs,
        bool   hasResume,
        int    interviewCount,
        string seekerId)
    {
        AnsiConsole.WriteLine();
        PanelFactory.RenderWelcomeBanner(seekerName, "CANDIDATE");
        AnsiConsole.WriteLine();

        var row = new Columns(
            PanelFactory.MetricPanel(
                "▶  Applications",
                appliedJobs.ToString(),
                "jobs applied",
                UITheme.Candidate),

            PanelFactory.MetricPanel(
                "★  Saved Jobs",
                savedJobs.ToString(),
                "in watchlist",
                UITheme.Secondary),

            PanelFactory.MetricPanel(
                "📅  Interviews",
                interviewCount.ToString(),
                "upcoming",
                interviewCount > 0 ? UITheme.Primary : UITheme.Dim),

            PanelFactory.MetricPanel(
                "◎  Notifications",
                unreadNotifications.ToString(),
                "unread",
                unreadNotifications > 0 ? UITheme.Warning : UITheme.Dim)
        );
        AnsiConsole.Write(row);

        // Resume status
        AnsiConsole.WriteLine();
        string resumeStatus = hasResume
            ? $"[bold {UITheme.Success}]  ✓  Resume on file[/]"
            : $"[bold {UITheme.Warning}]  ⚠  No resume uploaded — you cannot apply until a resume is added[/]";
        AnsiConsole.Write(
            new Panel(resumeStatus + $"\n[{UITheme.Dim}]ID: {Markup.Escape(seekerId)}[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse(hasResume ? UITheme.Success : UITheme.Warning))
                .Padding(1, 0));
        AnsiConsole.WriteLine();
    }
}
