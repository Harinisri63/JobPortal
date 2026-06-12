using Spectre.Console;

namespace JobPortal.UI;

/// <summary>
/// Reusable panel and layout factory for HIRE OPS.
/// Presentation-only. No business logic, no service calls.
/// </summary>
internal static class PanelFactory
{
    // ── Screen Header ──────────────────────────────────────────────
    /// <summary>
    /// Renders the full-width branded screen header with breadcrumb, user and time.
    /// </summary>
    public static void RenderScreenHeader(
        string module,
        string breadcrumb,
        string userName,
        string role)
    {
        AnsiConsole.WriteLine();

        // Top Rule with App Name
        var topRule = new Rule($"[bold {UITheme.Primary}] {UITheme.AppName} [/]  [bold {UITheme.Dim}]|[/]  [{UITheme.Neutral}]{Markup.Escape(module)}[/]")
            .RuleStyle(UITheme.Primary)
            .LeftJustified();
        AnsiConsole.Write(topRule);

        // Meta line: breadcrumb | user | time
        var grid = new Grid()
            .AddColumn(new GridColumn().NoWrap())
            .AddColumn(new GridColumn().NoWrap().RightAligned());

        grid.AddRow(
            $"[{UITheme.Dim}] ❯ {Markup.Escape(breadcrumb)}[/]",
            $"[{UITheme.Dim}]{Markup.Escape(userName)} [[{Markup.Escape(role)}]]  {UITheme.Today()}  {UITheme.Now()}[/]"
        );
        AnsiConsole.Write(grid);

        var bottomRule = new Rule().RuleStyle(UITheme.Dim);
        AnsiConsole.Write(bottomRule);
        AnsiConsole.WriteLine();
    }

    // ── Footer ─────────────────────────────────────────────────────
    public static void RenderFooter(string userName = "")
    {
        AnsiConsole.WriteLine();
        var rule = new Rule($"[{UITheme.Dim}] {UITheme.AppName}  {UITheme.AppVersion}  ·  {UITheme.AppEnv}  ·  {Markup.Escape(userName)}  ·  {UITheme.Now()} [/]")
            .RuleStyle(UITheme.Dim);
        AnsiConsole.Write(rule);
    }

    // ── Alert Panels ───────────────────────────────────────────────
    public static void RenderSuccess(string message)
    {
        AnsiConsole.Write(
            new Panel($"[bold {UITheme.Success}]  ✓  {Markup.Escape(message)}[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse(UITheme.Success))
                .Padding(1, 0));
    }

    public static void RenderError(string message)
    {
        AnsiConsole.Write(
            new Panel($"[bold {UITheme.Error}]  ✗  {Markup.Escape(message)}[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse(UITheme.Error))
                .Padding(1, 0));
    }

    public static void RenderWarning(string message)
    {
        AnsiConsole.Write(
            new Panel($"[bold {UITheme.Warning}]  ⚠  {Markup.Escape(message)}[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse(UITheme.Warning))
                .Padding(1, 0));
    }

    public static void RenderInfo(string message)
    {
        AnsiConsole.Write(
            new Panel($"[{UITheme.Info}]  ℹ  {Markup.Escape(message)}[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse(UITheme.Info))
                .Padding(1, 0));
    }

    // ── Info Card ──────────────────────────────────────────────────
    /// <summary>
    /// Renders a key-value info card inside a styled panel.
    /// </summary>
    public static void RenderInfoCard(string title, IEnumerable<(string Label, string Value)> fields, string borderColor = "")
    {
        borderColor = string.IsNullOrWhiteSpace(borderColor) ? UITheme.Primary : borderColor;
        var table = new Table()
            .HideHeaders()
            .Border(TableBorder.None)
            .AddColumn(new TableColumn("").Width(24))
            .AddColumn(new TableColumn(""));

        foreach (var (label, value) in fields)
        {
            table.AddRow(
                $"[{UITheme.Dim}]{Markup.Escape(label)}[/]",
                $"[bold {UITheme.White}]{Markup.Escape(value)}[/]"
            );
        }

        AnsiConsole.Write(
            new Panel(table)
                .Header($"[bold {borderColor}]  {Markup.Escape(title)}  [/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse(borderColor))
                .Padding(1, 0));
    }

    // ── Welcome Banner ─────────────────────────────────────────────
    public static void RenderWelcomeBanner(string userName, string role)
    {
        string roleColor = role.ToUpperInvariant() switch
        {
            "ADMIN"     => UITheme.Admin,
            "CANDIDATE" => UITheme.Candidate,
            _           => UITheme.Primary,
        };

        string roleIcon = role.ToUpperInvariant() switch
        {
            "ADMIN"     => "⬡",
            "CANDIDATE" => "◈",
            _           => "●",
        };

        AnsiConsole.Write(
            new Panel(
                $"[bold white]Welcome back,[/] [bold {roleColor}]{Markup.Escape(userName)}[/]   [{roleColor}]{roleIcon} {Markup.Escape(role)}[/]\n" +
                $"[{UITheme.Dim}]{UITheme.Today()}  ·  {UITheme.Now()}[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse(roleColor))
                .Padding(2, 0));
    }

    // ── Section Sub-header ─────────────────────────────────────────
    public static void RenderSectionTitle(string title, string icon = "◆")
    {
        AnsiConsole.MarkupLine($"\n[bold {UITheme.Primary}]{icon}  {Markup.Escape(title)}[/]");
        AnsiConsole.Write(new Rule().RuleStyle(UITheme.Dim));
    }

    // ── Metric Card ────────────────────────────────────────────────
    public static Panel MetricPanel(string title, string value, string subtitle, string color)
    {
        var content = new Markup(
            $"[bold {color}]{Markup.Escape(value)}[/]\n" +
            $"[{UITheme.Dim}]{Markup.Escape(subtitle)}[/]");

        return new Panel(content)
            .Header($"[bold {color}] {Markup.Escape(title)} [/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse(color))
            .Padding(1, 0);
    }

    // ── OTP display ────────────────────────────────────────────────
    public static void RenderOTPBox(string otp)
    {
        AnsiConsole.Write(
            new Panel($"[bold {UITheme.Warning}]{Markup.Escape(otp)}[/]\n[{UITheme.Dim}]Copy this code — in production it would be emailed[/]")
                .Header($"[bold {UITheme.Warning}] OTP Code [/]")
                .Border(BoxBorder.Double)
                .BorderStyle(Style.Parse(UITheme.Warning))
                .Padding(2, 0));
    }

    // ── Credentials hint box ───────────────────────────────────────
    public static void RenderCredentialsHint(string email, string password)
    {
        var table = new Table()
            .HideHeaders()
            .Border(TableBorder.None)
            .AddColumn(new TableColumn("").Width(14))
            .AddColumn(new TableColumn(""));
        table.AddRow($"[{UITheme.Dim}]Email[/]",    $"[bold {UITheme.Warning}]{Markup.Escape(email)}[/]");
        table.AddRow($"[{UITheme.Dim}]Password[/]", $"[bold {UITheme.Warning}]{Markup.Escape(password)}[/]");

        AnsiConsole.Write(
            new Panel(table)
                .Header($"[bold {UITheme.Warning}] Default Admin Credentials [/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse(UITheme.Warning))
                .Padding(1, 0));
        AnsiConsole.WriteLine();
    }
}
