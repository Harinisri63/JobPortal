using Spectre.Console;

namespace JobPortal.UI;

/// <summary>
/// Central design token registry for HIRE OPS — JobPortal 2077.
/// Presentation-only. No business logic, no service calls.
/// </summary>
internal static class UITheme
{
    // ── App Identity ──────────────────────────────────────────────
    public const string AppName    = "JobPortal 2077";
    public const string AppTagline = "THE FUTURE OF TALENT DISCOVERY";
    public const string AppVersion = "v2.0.0";
    public const string AppEnv     = "PRODUCTION";

    // ── Color Palette ─────────────────────────────────────────────
    public const string Primary   = "cyan1";
    public const string Secondary = "steelblue1";
    public const string Success   = "springgreen1";
    public const string Warning   = "gold1";
    public const string Error     = "indianred1";
    public const string Info      = "aqua";
    public const string Employer  = "mediumpurple";
    public const string Candidate = "deepskyblue1";
    public const string Admin     = "magenta1";
    public const string Neutral   = "grey70";
    public const string Dim       = "grey50";
    public const string White     = "white";

    // ── Status Colors ─────────────────────────────────────────────
    public static string StatusColor(string status) => status.ToUpperInvariant() switch
    {
        "PENDING"              => Warning,
        "PENDING_APPROVAL"     => Warning,
        "SHORTLISTED"          => Primary,
        "INTERVIEW_SCHEDULED"  => Secondary,
        "HIRED"                => Success,
        "APPROVED"             => Success,
        "COMPLETED"            => Success,
        "REJECTED"             => Error,
        "CANCELLED"            => Error,
        "CLOSED"               => Dim,
        "EXPIRED"              => Error,
        "OPEN"                 => Success,
        "ACTIVE"               => Success,
        "BLOCKED"              => Error,
        "SCHEDULED"            => Secondary,
        "ONLINE"               => Info,
        "OFFLINE"              => Neutral,
        _                      => White,
    };

    public static string StatusIcon(string status) => status.ToUpperInvariant() switch
    {
        "PENDING"             => "⏳",
        "PENDING_APPROVAL"    => "⏳",
        "SHORTLISTED"         => "✦",
        "INTERVIEW_SCHEDULED" => "📅",
        "HIRED"               => "🏆",
        "APPROVED"            => "✓",
        "COMPLETED"           => "✓",
        "REJECTED"            => "✗",
        "CANCELLED"           => "✗",
        "CLOSED"              => "■",
        "EXPIRED"             => "✗",
        "OPEN"                => "●",
        "ACTIVE"              => "●",
        _                     => "○",
    };

    // ── Markup helpers ────────────────────────────────────────────
    public static string Accent(string text)   => $"[bold {Primary}]{text}[/]";
    public static string Muted(string text)    => $"[{Dim}]{text}[/]";
    public static string Bold(string text)     => $"[bold white]{text}[/]";
    public static string Ok(string text)       => $"[bold {Success}]{text}[/]";
    public static string Warn(string text)     => $"[bold {Warning}]{text}[/]";
    public static string Err(string text)      => $"[bold {Error}]{text}[/]";
    public static string Inf(string text)      => $"[{Info}]{text}[/]";

    public static string Badge(string status)
    {
        string c    = StatusColor(status);
        string icon = StatusIcon(status);
        return $"[bold {c}]{icon} {Markup.Escape(status)}[/]";
    }

    // ── Border styles ─────────────────────────────────────────────
    public static Style PrimaryStyle   => Style.Parse($"bold {Primary}");
    public static Style SecondaryStyle => Style.Parse(Secondary);
    public static Style SuccessStyle   => Style.Parse(Success);
    public static Style WarningStyle   => Style.Parse(Warning);
    public static Style ErrorStyle     => Style.Parse(Error);
    public static Style NeutralStyle   => Style.Parse(Neutral);
    public static Style DimStyle       => Style.Parse(Dim);

    // ── Separator line ─────────────────────────────────────────────
    public static Rule SectionRule(string title = "") =>
        string.IsNullOrWhiteSpace(title)
            ? new Rule().RuleStyle(Dim)
            : new Rule($"[bold {Primary}]{Markup.Escape(title)}[/]").RuleStyle(Primary);

    // ── Current time string ────────────────────────────────────────
    public static string Now() => DateTime.Now.ToString("HH:mm:ss");
    public static string Today() => DateTime.Now.ToString("ddd, dd MMM yyyy");
}
