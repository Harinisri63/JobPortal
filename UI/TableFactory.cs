using JobPortal.Features.Application;
using JobPortal.Features.Candidate;
using JobPortal.Features.Interview;
using JobPortal.Features.JobPosting;
using JobPortal.Features.Notifications;
using JobPortal.Features.Admin;
using JobPortal.Features.User.Candidate;
using JobPortal.Shared.Enums;
using JobPortal.Shared.Structs;
using JobPortal.UI;
using Spectre.Console;

namespace JobPortal.UI;
internal static class TableFactory
{
    public static void RenderJobTable(IList<JobListing> jobs, bool showStatus = true, int startIndex = 0)
    {
        if (jobs.Count == 0)
        {
            PanelFactory.RenderWarning("No job listings found.");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse(UITheme.Primary))
            .Expand()
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").RightAligned().Width(4))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Job Title[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Company[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Location[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Salary (LPA)[/]").Centered())
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Exp.[/]").Centered());

        if (showStatus)
        {
            table.AddColumn(new TableColumn($"[bold {UITheme.Warning}]Status[/]").Centered());
        }

        table.AddColumn(new TableColumn($"[bold {UITheme.Warning}]Expires[/]").Centered());

        for (int i = 0; i < jobs.Count; i++)
        {
            var j = jobs[i];
            string statusMarkup = UITheme.Badge(j.Status.ToString());
            bool expired = j.IsExpired();
            string rowColor = expired ? UITheme.Dim : UITheme.White;

            if (showStatus)
            {
                table.AddRow(
                    $"[{UITheme.Dim}]{startIndex + i + 1}[/]",
                    $"[bold {rowColor}]{Markup.Escape(Truncate(j.Title, 28))}[/]",
                    $"[{rowColor}]{Markup.Escape(Truncate(j.Company, 20))}[/]",
                    $"[{rowColor}]{Markup.Escape(j.Location)}[/]",
                    $"[{UITheme.Secondary}]{Markup.Escape(j.SalaryRange.ToString())}[/]",
                    $"[{rowColor}]{j.ExperienceRequired}y[/]",
                    statusMarkup,
                    $"[{UITheme.Dim}]{j.ExpiryDate:dd MMM yy}[/]"
                );
            }
            else
            {
                table.AddRow(
                    $"[{UITheme.Dim}]{startIndex + i + 1}[/]",
                    $"[bold {rowColor}]{Markup.Escape(Truncate(j.Title, 28))}[/]",
                    $"[{rowColor}]{Markup.Escape(Truncate(j.Company, 20))}[/]",
                    $"[{rowColor}]{Markup.Escape(j.Location)}[/]",
                    $"[{UITheme.Secondary}]{Markup.Escape(j.SalaryRange.ToString())}[/]",
                    $"[{rowColor}]{j.ExperienceRequired}y[/]",
                    $"[{UITheme.Dim}]{j.ExpiryDate:dd MMM yy}[/]"
                );
            }
        }

        AnsiConsole.Write(table);
    }

    public static void RenderApplicationTable(
        IList<Application1> apps,
        IList<JobListing> allJobs,
        int startIndex = 0)
    {
        if (apps.Count == 0)
        {
            PanelFactory.RenderInfo("No applications found.");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse(UITheme.Candidate))
            .Expand()
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]App ID[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Job Title[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Company[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Applied[/]").Centered())
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Status[/]").Centered());

        for (int i = 0; i < apps.Count; i++)
        {
            var a = apps[i];
            JobListing? job = null;

            for (int k = 0; k < allJobs.Count; k++)
            {
                if (allJobs[k].JobId == a.JobId)
                {
                    job = allJobs[k];
                    break;
                }
            }
            string title   = Truncate(job?.Title   ?? "N/A", 22);
            string company = Truncate(job?.Company ?? "N/A", 18);

            table.AddRow(
                $"[{UITheme.Dim}]{startIndex + i + 1}[/]",
                $"[{UITheme.Dim}]{Markup.Escape(Truncate(a.ApplicationId, 12))}[/]",
                $"[bold white]{Markup.Escape(title)}[/]",
                $"[{UITheme.Neutral}]{Markup.Escape(company)}[/]",
                $"[{UITheme.Dim}]{a.AppliedDate:dd MMM yy}[/]",
                UITheme.Badge(a.Status.ToString())
            );
        }

        AnsiConsole.Write(table);
    }

    public static void RenderCandidateTable(IList<JobSeeker> seekers, int startIndex = 0)
    {
        if (seekers.Count == 0)
        {
            PanelFactory.RenderInfo("No candidates registered.");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse(UITheme.Candidate))
            .Expand()
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Seeker ID[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Name[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Email[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Phone[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Cleared/Total Rounds[/]").Centered())
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Status[/]").Centered());

        for (int i = 0; i < seekers.Count; i++)
        {
            var s = seekers[i];
            
            // 1. Determine Display Rounds
            // REJECTED: show the round at which rejection happened, e.g. "1 / 3 [REJECTED]"
            // All other statuses (including INTERVIEW_SCHEDULED): show CurrentRound / TotalRounds
            // The Status column already conveys whether an interview is pending.
            string displayRounds;
            if (s.CandidateStatus == "REJECTED")
            {
                int rejectedRound = s.RejectedRound ?? s.CurrentRound;
                displayRounds = $"{rejectedRound} / {s.TotalRounds} [REJECTED]";
            }
            else
            {
                displayRounds = $"{s.CurrentRound} / {s.TotalRounds}";
            }

            // 2. Determine Display Status
            string displayStatus;
            if (!s.IsActive)
            {
                displayStatus = $"[bold {UITheme.Error}]✕ BLOCKED[/]";
            }
            else if (s.CandidateStatus == "REJECTED")
            {
                displayStatus = $"[bold {UITheme.Error}]✖ REJECTED[/]";
            }
            else if (s.CandidateStatus == "SHORTLISTED")
            {
                displayStatus = $"[bold {UITheme.Primary}]✦ SHORTLISTED[/]";
            }
            else if (s.CandidateStatus == "INTERVIEW_SCHEDULED")
            {
                // Safety net: if rounds are maxed, always show HIRED regardless of stored status.
                if (s.CurrentRound >= s.TotalRounds)
                    displayStatus = $"[bold {UITheme.Success}]● HIRED[/]";
                else
                    displayStatus = $"[bold {UITheme.Secondary}]📅 INTERVIEW_SCHEDULED[/]";
            }
            else if (s.CandidateStatus == "HIRED")
            {
                displayStatus = $"[bold {UITheme.Success}]● HIRED[/]";
            }
            else if (!string.IsNullOrEmpty(s.CandidateStatus))
            {
                string color = UITheme.StatusColor(s.CandidateStatus);
                string icon = UITheme.StatusIcon(s.CandidateStatus);
                displayStatus = $"[bold {color}]{icon} {Markup.Escape(s.CandidateStatus)}[/]";
            }
            else
            {
                displayStatus = $"[bold {UITheme.Success}]● ACTIVE[/]";
            }

            table.AddRow(
                $"[{UITheme.Dim}]{startIndex + i + 1}[/]",
                $"[{UITheme.Secondary}]{Markup.Escape(s.SeekerId)}[/]",
                $"[bold white]{Markup.Escape(s.Name)}[/]",
                $"[{UITheme.Neutral}]{Markup.Escape(s.Email)}[/]",
                $"[{UITheme.Neutral}]{Markup.Escape(s.Phone)}[/]",
                $"[white]{Markup.Escape(displayRounds)}[/]",
                displayStatus
            );
        }

        AnsiConsole.Write(table);
    }

    public static void RenderAdminTable(IList<Admin1> admins, int startIndex = 0)
    {
        if (admins.Count == 0)
        {
            PanelFactory.RenderInfo("No admin accounts found.");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse(UITheme.Admin))
            .Expand()
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Name[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Email[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Company[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Approved[/]").Centered());

        for (int i = 0; i < admins.Count; i++)
        {
            var a = admins[i];
            string approvedMark = a.IsApproved
                ? $"[bold {UITheme.Success}]✓ YES[/]"
                : $"[bold {UITheme.Warning}]⏳ PENDING[/]";

            table.AddRow(
                $"[{UITheme.Dim}]{startIndex + i + 1}[/]",
                $"[bold white]{Markup.Escape(a.Name)}[/]",
                $"[{UITheme.Neutral}]{Markup.Escape(a.Email)}[/]",
                $"[{UITheme.Employer}]{Markup.Escape(a.Company)}[/]",
                approvedMark
            );
        }

        AnsiConsole.Write(table);
    }

    public static void RenderInterviewTable(
    IList<Interview1> interviews,
    IList<Application1> allApps,
    IList<JobListing> allJobs,
    int startIndex = 0)
    {
        if (interviews.Count == 0)
        {
            PanelFactory.RenderInfo("No interviews scheduled.");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse(UITheme.Secondary))
            .Expand()
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Job Title[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Scheduled[/]").Centered())
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Mode[/]").Centered())
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Status[/]").Centered())
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Link / Venue[/]"));

        for (int i = 0; i < interviews.Count; i++)
        {
            var iv = interviews[i];

            Application1? app = null;
            for (int j = 0; j < allApps.Count; j++)
            {
                if (allApps[j].ApplicationId == iv.ApplicationId)
                {
                    app = allApps[j];
                    break;
                }
            }

            JobListing? job = null;
            if (app != null)
            {
                for (int k = 0; k < allJobs.Count; k++)
                {
                    if (allJobs[k].JobId == app.JobId)
                    {
                        job = allJobs[k];
                        break;
                    }
                }
            }

            string modeColor = iv.Mode?.ToUpperInvariant() == "ONLINE"
                ? UITheme.Info
                : UITheme.Neutral;

            string location = iv.Mode?.ToUpperInvariant() == "ONLINE"
                ? Truncate(iv.MeetingLink ?? "—", 30)
                : Truncate(iv.Venue ?? "—", 30);

            table.AddRow(
                $"[{UITheme.Dim}]{startIndex + i + 1}[/]",
                $"[bold white]{Markup.Escape(Truncate(job?.Title ?? "N/A", 24))}[/]",
                $"[{UITheme.Secondary}]{iv.ScheduledAt:dd MMM yy  HH:mm}[/]",
                $"[bold {modeColor}]{Markup.Escape(iv.Mode ?? "—")}[/]",
                UITheme.Badge(iv.Status.ToString()),
                $"[{UITheme.Dim}]{Markup.Escape(location)}[/]"
            );
        }

        AnsiConsole.Write(table);
    }
    public static void RenderNotificationTable(IList<Notification> notifications)
    {
        if (notifications.Count == 0)
        {
            PanelFactory.RenderInfo("Your inbox is empty.");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse(UITheme.Info))
            .Expand()
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Type[/]").Width(20))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Title[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Message[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Date[/]").Centered())
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Read[/]").Centered());

        for (int i = 0; i < notifications.Count; i++)
        {
            var n = notifications[i];

            bool isUnread = n.IsUnread();

            string rowStyle;
            string readMark;

            if (isUnread)
            {
                rowStyle = "bold white";
                readMark = $"[{UITheme.Warning}]● NEW[/]";
            }
            else
            {
                rowStyle = UITheme.Dim;
                readMark = $"[{UITheme.Dim}]✓[/]";
            }

            string type = n.Type ?? "SYSTEM";
            string typeColor;

            if (type.ToUpperInvariant() == "INTERVIEW_INVITE")
            {
                typeColor = UITheme.Secondary;
            }
            else if (type.ToUpperInvariant() == "APPLICATION_STATUS")
            {
                typeColor = UITheme.Candidate;
            }
            else if (type.ToUpperInvariant() == "ANNOUNCEMENT")
            {
                typeColor = UITheme.Warning;
            }
            else
            {
                typeColor = UITheme.Info;
            }

            string title = Truncate(n.Title ?? "—", 22);
            string message = Truncate(n.Message ?? "—", 40);

            table.AddRow(
                $"[{UITheme.Dim}]{i + 1}[/]",
                $"[{typeColor}]{Markup.Escape(type)}[/]",
                $"[{rowStyle}]{Markup.Escape(title)}[/]",
                $"[{rowStyle}]{Markup.Escape(message)}[/]",
                $"[{UITheme.Dim}]{n.CreatedAt:dd MMM HH:mm}[/]",
                readMark
            );
        }

        AnsiConsole.Write(table);
    }

    public static void RenderPendingJobTable(IList<JobListing> jobs, int startIndex = 0)
    {
        if (jobs == null || jobs.Count == 0)
        {
            PanelFactory.RenderInfo("No pending job postings.");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse(UITheme.Warning))
            .Expand()
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Job Title[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Company[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Location[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Salary[/]").Centered())
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Posted[/]").Centered())
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Expires[/]").Centered());

        for (int i = 0; i < jobs.Count; i++)
        {
            var j = jobs[i];

            string title = Truncate(j.Title ?? "—", 26);
            string company = Truncate(j.Company ?? "—", 18);
            string location = j.Location ?? "—";
            string salary = j.SalaryRange.ToString() ?? "—";

            string posted = j.PostedDate == default? "—": j.PostedDate.ToString("dd MMM");

            string expiry = j.ExpiryDate == default
                ? "—"
                : j.ExpiryDate.ToString("dd MMM");

            table.AddRow(
                $"[{UITheme.Dim}]{startIndex + i + 1}[/]",
                $"[bold white]{Markup.Escape(title)}[/]",
                $"[{UITheme.Employer}]{Markup.Escape(company)}[/]",
                $"[{UITheme.Neutral}]{Markup.Escape(location)}[/]",
                $"[{UITheme.Secondary}]{Markup.Escape(salary)}[/]",
                $"[{UITheme.Dim}]{posted}[/]",
                $"[{UITheme.Dim}]{expiry}[/]"
            );
        }

        AnsiConsole.Write(table);
    }

    public static void RenderApplicationsByJobTable(
    IList<Application1> apps,
    IList<JobSeeker> seekers,
    int startIndex = 0)
    {
        if (apps == null || apps.Count == 0)
        {
            PanelFactory.RenderInfo("No applications for this job.");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse(UITheme.Admin))
            .Expand()
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]App ID[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Applicant[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Applied[/]").Centered())
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Status[/]").Centered());

        for (int i = 0; i < apps.Count; i++)
        {
            var a = apps[i];

            JobSeeker sk = null;

            if (seekers != null)
            {
                for (int j = 0; j < seekers.Count; j++)
                {
                    if (seekers[j].SeekerId == a.JobSeekerId)
                    {
                        sk = seekers[j];
                        break;
                    }
                }
            }

            string appId = Truncate(a.ApplicationId ?? "—", 14);

            string applicantName = sk != null
                ? sk.Name ?? a.JobSeekerId
                : a.JobSeekerId;

            string appliedDate = a.AppliedDate == default
                ? "—"
                : a.AppliedDate.ToString("dd MMM yy");

            table.AddRow(
                $"[{UITheme.Dim}]{startIndex + i + 1}[/]",
                $"[{UITheme.Dim}]{Markup.Escape(appId)}[/]",
                $"[bold white]{Markup.Escape(applicantName)}[/]",
                $"[{UITheme.Dim}]{appliedDate}[/]",
                UITheme.Badge(a.Status.ToString())
            );
        }

        AnsiConsole.Write(table);
    }

    public static void RenderSavedJobTable(IList<JobListing> jobs, int startIndex = 0)
    {
        if (jobs.Count == 0)
        {
            PanelFactory.RenderInfo("No saved jobs.");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse(UITheme.Secondary))
            .Expand()
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Job Title[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Company[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Location[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Salary[/]").Centered());

        for (int i = 0; i < jobs.Count; i++)
        {
            var j = jobs[i];
            table.AddRow(
                $"[{UITheme.Dim}]{startIndex + i + 1}[/]",
                $"[bold white]{Markup.Escape(Truncate(j.Title, 26))}[/]",
                $"[{UITheme.Employer}]{Markup.Escape(Truncate(j.Company, 18))}[/]",
                $"[{UITheme.Neutral}]{Markup.Escape(j.Location)}[/]",
                $"[{UITheme.Secondary}]{Markup.Escape(j.SalaryRange.ToString())}[/]"
            );
        }

        AnsiConsole.Write(table);
    }

    public static void RenderComplaintTable(IList<Complaint> complaints, int startIndex = 0)
    {
        if (complaints.Count == 0)
        {
            PanelFactory.RenderInfo("No complaints recorded.");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse(UITheme.Warning))
            .Expand()
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Subject[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Description[/]"))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Status[/]").Centered())
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Created Date[/]").Centered());

        for (int i = 0; i < complaints.Count; i++)
        {
            var c = complaints[i];
            string statusColor = c.Status.ToUpperInvariant() switch
            {
                "OPEN" => UITheme.Warning,
                "RESOLVED" => UITheme.Success,
                _ => UITheme.Neutral
            };

            table.AddRow(
                $"[{UITheme.Dim}]{startIndex + i + 1}[/]",
                $"[bold white]{Markup.Escape(Truncate(c.Subject, 30))}[/]",
                $"[{UITheme.Neutral}]{Markup.Escape(Truncate(c.Description, 45))}[/]",
                $"[bold {statusColor}]{c.Status}[/]",
                $"[{UITheme.Dim}]{c.CreatedAt:dd MMM yy}[/]"
            );
        }

        AnsiConsole.Write(table);
    }

    private static string Truncate(string s, int max)
    {
        if (string.IsNullOrEmpty(s))
            return "—";

        return s.Length > max ? s[..max] + "…" : s;
    }
}
