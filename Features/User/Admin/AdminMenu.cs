using JobPortal.Shared.Exceptions;
using JobPortal.Shared.Enums;
using JobPortal.Features.Candidate;
using JobPortal.Features.JobPosting;
using JobPortal.Features.Reports;
using JobPortal.FileStorage;
using JobPortal.UI;
using JobPortal.Shared.Structs;
using JobPortal.Shared.DTOs;
using JobPortal.Features.Application;
using JobPortal.Features.Interview;
using JobPortal.Features.User.Candidate;
using JobPortal.Database;
using System.Data;
using Spectre.Console;

namespace JobPortal.Features.Admin;

internal class AdminMenu
{
    private readonly Admin1 _admin;
    public AdminMenu(Admin1 admin) => _admin = admin;

    private void ShowHeader(string module, string breadcrumb)
    {
        Console.Clear();
        PanelFactory.RenderScreenHeader(module, breadcrumb, _admin.Name, "ADMIN");
    }

    private static readonly string[] _menuItems =
    [
        "1.  Manage Job Seekers",
        "2.  Manage Admins",
        "3.  Approve Job Postings",
        "4.  Notifications",
        "5.  Manage Complaints",
        "6.  Send Announcement",
        "7.  Post New Job",
        "8.  Manage My Job Listings",
        "9.  View Applications",
        "10. Shortlist / Reject Applicant",
        "11. Schedule Interview",
        "12. Hiring Report",
        "13. My Profile",
        "0.  Logout",
    ];

    public void Show()
    {
        bool running = true;
        while (running)
        {
            ShowHeader("Admin Dashboard", $"Dashboard > {_admin.Company}");
            int unread = DataStore.NotificationService.GetPendingCount(_admin.AdminId);
            int myJobs = 0;
            int pendingJobs = 0;
            int closedJobs = 0;
            foreach (var job in DataStore.Jobs)
            {
                if (job.AdminId == _admin.AdminId)
                    myJobs++;
                if (job.Status == JobStatus.PENDING_APPROVAL)
                    pendingJobs++;
                if (job.Status == JobStatus.CLOSED || job.Status == JobStatus.EXPIRED)
                    closedJobs++;
            }
            int myApps = 0;
            foreach (var app in DataStore.Applications)
            {
                bool belongsToAdmin = false;
                foreach (var job in DataStore.Jobs)
                {
                    if (job.JobId == app.JobId && job.AdminId == _admin.AdminId)
                    {
                        belongsToAdmin = true;
                        break;
                    }
                }
                if (belongsToAdmin)
                    myApps++;
            }
            int myShortlisted = 0;
            foreach (var app in DataStore.Applications)
            {
                if (app.Status != ApplicationStatus.SHORTLISTED)
                    continue;
                bool belongsToAdmin = false;
                foreach (var job in DataStore.Jobs)
                {
                    if (job.JobId == app.JobId && job.AdminId == _admin.AdminId)
                    {
                        belongsToAdmin = true;
                        break;
                    }
                }
                if (belongsToAdmin)
                    myShortlisted++;
            }
            int myInterviews = 0;
            foreach (var interview in DataStore.InterviewScheduler.GetAll())
            {
                bool belongsToAdmin = false;
                foreach (var app in DataStore.Applications)
                {
                    if (app.ApplicationId == interview.ApplicationId)
                    {
                        foreach (var job in DataStore.Jobs)
                        {
                            if (job.JobId == app.JobId && job.AdminId == _admin.AdminId)
                            {
                                belongsToAdmin = true;
                                break;
                            }
                        }
                        if (belongsToAdmin)
                            break;
                    }
                }
                if (belongsToAdmin)
                    myInterviews++;
            }
            List<string> menuLines = new List<string>();
            for (int i = 0; i < _menuItems.Length; i++)
            {
                string item = _menuItems[i];
                string color;
                if (i == _menuItems.Length - 1)
                {
                    color = UITheme.Dim;
                }
                else if (i == 3 && unread > 0)
                {
                    color = UITheme.Warning;
                }
                else
                {
                    color = UITheme.White;
                }
                string notifBadge = "";
                if (i == 3 && unread > 0)
                {
                    notifBadge = $" [bold {UITheme.Warning}][[{unread} NEW]][/]";
                }
                menuLines.Add($"[{color}]  {Markup.Escape(item)}[/]{notifBadge}");
            }
            string menuContent = string.Join("\n", menuLines);
            AnsiConsole.Write(new Panel(menuContent).Header($"[bold {UITheme.Admin}]  Admin Command Center  [/]").Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Admin)).Padding(2, 0));
            PanelFactory.RenderFooter(_admin.Name);
            AnsiConsole.WriteLine();
            int choice;
            try
            {
                while (true)
                {
                    string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-13]]:[/]").PromptStyle(UITheme.Admin).AllowEmpty());
                    if (!int.TryParse(raw.Trim(), out choice) || choice < 0 || choice > 13)
                    {
                        PanelFactory.RenderError("Please enter a number between 0 and 13.");
                        continue;
                    }
                    break;
                }
                switch (choice)
                {
                    case 1:
                        ManageJobSeekers();
                        break;

                    case 2:
                        ManageAdmins();
                        break;

                    case 3:
                        ApproveJobPostings();
                        break;

                    case 4:
                        ShowNotifications();
                        break;

                    case 5:
                        ManageComplaints();
                        break;

                    case 6:
                        SendAnnouncement();
                        break;

                    case 7:
                        PostNewJob();
                        break;

                    case 8:
                        ManageMyListings();
                        break;

                    case 9:
                        ViewApplications();
                        break;

                    case 10:
                        ShortlistReject();
                        break;

                    case 11:
                        InterviewManagementMenu();
                        break;

                    case 12:
                        HiringReport();
                        break;

                    case 13:
                        MyProfile();
                        break;

                    case 0:
                        running = false;
                        break;
                }
            }
            catch (ValidationException ex)
            {
                PanelFactory.RenderError(ex.Message);
                ConsoleHelper.Pause();
            }
            catch (JPNSException ex)
            {
                PanelFactory.RenderError(ex.Message);
                ConsoleHelper.Pause();
            }
            catch (Exception ex)
            {
                PanelFactory.RenderError($"Unexpected error: {ex.Message}");
                ConsoleHelper.Pause();
            }
        }
        PanelFactory.RenderSuccess($"Goodbye, {_admin.Name}! Session ended.");
    }

    private void ManageJobSeekers()
    {
        bool running = true;
        while (running)
        {
            ShowHeader("Manage Job Seekers", "Dashboard > Job Seekers");

            var menuContent = $"[bold {UITheme.Candidate}]  1.[/]  [white]View All Candidates[/]\n" +
                $"[bold {UITheme.Error}]  2.[/]  [white]Block Account[/]\n" +
                $"[bold {UITheme.Success}]  3.[/]  [white]Activate Account[/]\n" +
                $"[{UITheme.Dim}]  0.  Back[/]";

            AnsiConsole.Write(new Panel(menuContent).Header($"[bold {UITheme.Candidate}]  Candidate Management  [/]")
                    .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Candidate)).Padding(2, 0));
            AnsiConsole.WriteLine();
            int choice;
            while (true)
            {
                string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-3]]:[/]").PromptStyle(UITheme.Candidate).AllowEmpty());
                if (!int.TryParse(raw.Trim(), out choice) || choice < 0 || choice > 3)
                {
                    PanelFactory.RenderError("Invalid choice. Enter 0, 1, 2, or 3.");
                    continue;
                }

                break;
            }

            switch (choice)
            {
                case 0:
                    running = false;
                    break;

                case 1:
                    if (DataStore.JobSeekers.Count == 0)
                    {
                        PanelFactory.RenderInfo("No job seekers registered.");
                        ConsoleHelper.Pause();
                        break;
                    }
                    int viewPageSize = 5;
                    int viewPageNumber = 1;
                    while (true)
                    {
                        ShowHeader("Manage Job Seekers", "Dashboard > Job Seekers > View All");
                        int totalPages = (int)Math.Ceiling((double)DataStore.JobSeekers.Count / viewPageSize);
                        if (totalPages == 0) totalPages = 1;

                        int start = (viewPageNumber - 1) * viewPageSize;
                        int end = Math.Min(start + viewPageSize, DataStore.JobSeekers.Count);

                        var pageSeekers = new List<JobSeeker>();
                        for (int i = start; i < end; i++)
                        {
                            pageSeekers.Add(DataStore.JobSeekers[i]);
                        }

                        PanelFactory.RenderSectionTitle($"All Registered Candidates  (Page {viewPageNumber} of {totalPages})", "▶");
                        TableFactory.RenderCandidateTable(pageSeekers, startIndex: start);
                        AnsiConsole.WriteLine();
                        Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back");
                        Console.WriteLine();

                        Console.Write("Enter command: ");
                        string input = Console.ReadLine()?.Trim() ?? "";
                        if (string.IsNullOrEmpty(input)) continue;

                        if (input.Equals("N", StringComparison.OrdinalIgnoreCase))
                        {
                            if (viewPageNumber < totalPages)
                                viewPageNumber++;
                            else
                            {
                                Console.WriteLine("You are already on the last page. Press any key...");
                                Console.ReadKey(true);
                            }
                        }
                        else if (input.Equals("P", StringComparison.OrdinalIgnoreCase))
                        {
                            if (viewPageNumber > 1)
                                viewPageNumber--;
                            else
                            {
                                Console.WriteLine("You are already on the first page. Press any key...");
                                Console.ReadKey(true);
                            }
                        }
                        else if (input == "0")
                        {
                            break;
                        }
                        else if (int.TryParse(input, out int sel))
                        {
                            if (sel >= 1 && sel <= DataStore.JobSeekers.Count)
                            {
                                JobSeeker selectedSeeker = DataStore.JobSeekers[sel - 1];
                                EditCandidateRounds(selectedSeeker);
                            }
                            else
                            {
                                Console.WriteLine($"Invalid selection. Enter a number between 1 and {DataStore.JobSeekers.Count}. Press any key...");
                                Console.ReadKey(true);
                            }
                        }
                    }
                    break;

                case 2:
                    {
                        if (DataStore.JobSeekers.Count == 0)
                        {
                            PanelFactory.RenderInfo("No job seekers available to block.");
                            ConsoleHelper.Pause();
                            break;
                        }
                        int pageSize = 5;
                        int pageNumber = 1;
                        bool done = false;
                        while (!done)
                        {
                            ShowHeader("Manage Job Seekers", "Dashboard > Job Seekers > Block");
                            int totalPages = (int)Math.Ceiling((double)DataStore.JobSeekers.Count / pageSize);
                            if (totalPages == 0) totalPages = 1;

                            int start = (pageNumber - 1) * pageSize;
                            int end = Math.Min(start + pageSize, DataStore.JobSeekers.Count);

                            var pageSeekers = new List<JobSeeker>();
                            for (int i = start; i < end; i++)
                            {
                                pageSeekers.Add(DataStore.JobSeekers[i]);
                            }

                            PanelFactory.RenderSectionTitle($"Block Candidate  (Page {pageNumber} of {totalPages})", "▶");
                            TableFactory.RenderCandidateTable(pageSeekers, startIndex: start);
                            AnsiConsole.WriteLine();
                            Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back");
                            Console.WriteLine();

                            Console.Write("Enter candidate number to block or Seeker ID (or page navigation command): ");
                            string input = Console.ReadLine()?.Trim() ?? "";
                            if (string.IsNullOrEmpty(input)) continue;

                            if (input.Equals("N", StringComparison.OrdinalIgnoreCase))
                            {
                                if (pageNumber < totalPages)
                                    pageNumber++;
                                else
                                {
                                    Console.WriteLine("You are already on the last page. Press any key...");
                                    Console.ReadKey(true);
                                }
                            }
                            else if (input.Equals("P", StringComparison.OrdinalIgnoreCase))
                            {
                                if (pageNumber > 1)
                                    pageNumber--;
                                else
                                {
                                    Console.WriteLine("You are already on the first page. Press any key...");
                                    Console.ReadKey(true);
                                }
                            }
                            else
                            {
                                JobSeeker? targetSeeker = null;
                                if (input == "0") { done = true; break; }

                                foreach (var seeker in DataStore.JobSeekers)
                                {
                                    if (seeker.SeekerId.Equals(input, StringComparison.OrdinalIgnoreCase))
                                    {
                                        targetSeeker = seeker;
                                        break;
                                    }
                                }

                                if (targetSeeker == null && int.TryParse(input, out int sel))
                                {
                                    if (sel >= 1 && sel <= DataStore.JobSeekers.Count)
                                    {
                                        targetSeeker = DataStore.JobSeekers[sel - 1];
                                    }
                                }

                                if (targetSeeker != null)
                                {
                                    if (ConsoleHelper.Confirm($"Are you sure you want to block account [{targetSeeker.SeekerId}] ({targetSeeker.Name})?"))
                                    {
                                        _admin.BlockAccount(targetSeeker.SeekerId);
                                        PanelFactory.RenderSuccess("Account blocked successfully.");
                                    }
                                    ConsoleHelper.Pause();
                                }
                                else
                                {
                                    Console.WriteLine($"Invalid selection or Seeker ID '{input}'. Press any key...");
                                    Console.ReadKey(true);
                                }
                            }
                        }
                        break;
                    }

                case 3:
                    {
                        if (DataStore.JobSeekers.Count == 0)
                        {
                            PanelFactory.RenderInfo("No job seekers available to activate.");
                            ConsoleHelper.Pause();
                            break;
                        }
                        int pageSize = 5;
                        int pageNumber = 1;
                        bool done = false;
                        while (!done)
                        {
                            ShowHeader("Manage Job Seekers", "Dashboard > Job Seekers > Activate");
                            int totalPages = (int)Math.Ceiling((double)DataStore.JobSeekers.Count / pageSize);
                            if (totalPages == 0) totalPages = 1;

                            int start = (pageNumber - 1) * pageSize;
                            int end = Math.Min(start + pageSize, DataStore.JobSeekers.Count);

                            var pageSeekers = new List<JobSeeker>();
                            for (int i = start; i < end; i++)
                            {
                                pageSeekers.Add(DataStore.JobSeekers[i]);
                            }

                            PanelFactory.RenderSectionTitle($"Activate Candidate  (Page {pageNumber} of {totalPages})", "▶");
                            TableFactory.RenderCandidateTable(pageSeekers, startIndex: start);
                            AnsiConsole.WriteLine();
                            Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back");
                            Console.WriteLine();

                            Console.Write("Enter candidate number to activate or Seeker ID (or page navigation command): ");
                            string input = Console.ReadLine()?.Trim() ?? "";
                            if (string.IsNullOrEmpty(input)) continue;

                            if (input.Equals("N", StringComparison.OrdinalIgnoreCase))
                            {
                                if (pageNumber < totalPages)
                                    pageNumber++;
                                else
                                {
                                    Console.WriteLine("You are already on the last page. Press any key...");
                                    Console.ReadKey(true);
                                }
                            }
                            else if (input.Equals("P", StringComparison.OrdinalIgnoreCase))
                            {
                                if (pageNumber > 1)
                                    pageNumber--;
                                else
                                {
                                    Console.WriteLine("You are already on the first page. Press any key...");
                                    Console.ReadKey(true);
                                }
                            }
                            else
                            {
                                JobSeeker? targetSeeker = null;
                                if (input == "0") { done = true; break; }

                                foreach (var seeker in DataStore.JobSeekers)
                                {
                                    if (seeker.SeekerId.Equals(input, StringComparison.OrdinalIgnoreCase))
                                    {
                                        targetSeeker = seeker;
                                        break;
                                    }
                                }

                                if (targetSeeker == null && int.TryParse(input, out int sel))
                                {
                                    if (sel >= 1 && sel <= DataStore.JobSeekers.Count)
                                    {
                                        targetSeeker = DataStore.JobSeekers[sel - 1];
                                    }
                                }

                                if (targetSeeker != null)
                                {
                                    if (targetSeeker.IsActive)
                                    {
                                        PanelFactory.RenderWarning("The candidate is already in an active state.");
                                        ConsoleHelper.Pause();
                                    }
                                    else
                                    {
                                        if (ConsoleHelper.Confirm($"Do you want to activate the account {targetSeeker.SeekerId}?"))
                                        {
                                            _admin.ActivateAccount(targetSeeker.SeekerId);
                                            PanelFactory.RenderSuccess("Account activated successfully.");
                                        }
                                        ConsoleHelper.Pause();
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Invalid selection or Seeker ID '{input}'. Press any key...");
                                    Console.ReadKey(true);
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }

    private void EditCandidateRounds(JobSeeker seeker)
    {
        bool editing = true;
        while (editing)
        {
            ShowHeader("Edit Candidate Rounds", $"Dashboard > Job Seekers > Edit Rounds > {seeker.Name}");
            Console.WriteLine($"  Candidate: {seeker.Name} ({seeker.SeekerId})");
            Console.WriteLine($"  Current Cleared Rounds: {seeker.ClearedRounds}");
            Console.WriteLine($"  Current Total Rounds  : {seeker.TotalRounds}");
            Console.WriteLine();

            var menu = $"[bold {UITheme.Candidate}]  1.[/]  [white]Update Cleared Rounds[/]\n" +
                       $"[bold {UITheme.Candidate}]  2.[/]  [white]Update Total Rounds[/]\n" +
                       $"[{UITheme.Dim}]  0.  Back[/]";

            AnsiConsole.Write(new Panel(menu).Header($"[bold {UITheme.Candidate}]  Round Operations  [/]")
                    .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Candidate)).Padding(2, 0));
            AnsiConsole.WriteLine();

            int choice;
            while (true)
            {
                string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-2]]:[/]").PromptStyle(UITheme.Candidate).AllowEmpty());
                if (!int.TryParse(raw.Trim(), out choice) || choice < 0 || choice > 2)
                {
                    PanelFactory.RenderError("Invalid choice. Enter 0, 1, or 2.");
                    continue;
                }
                break;
            }

            if (choice == 0)
            {
                editing = false;
            }
            else if (choice == 1)
            {
                int newCleared;
                while (true)
                {
                    string input = ConsoleHelper.ReadNonEmpty("Enter cleared rounds");
                    if (int.TryParse(input, out newCleared) && newCleared >= 0)
                    {
                        if (newCleared <= seeker.TotalRounds)
                        {
                            break;
                        }
                        PanelFactory.RenderError($"Cleared rounds cannot exceed total rounds ({seeker.TotalRounds}).");
                    }
                    else
                    {
                        PanelFactory.RenderError("Please enter a valid non-negative number.");
                    }
                }
                seeker.ClearedRounds = newCleared;
                DataStore.SaveJobSeekers();
                PanelFactory.RenderSuccess("Cleared rounds updated successfully.");
                ConsoleHelper.Pause();
            }
            else if (choice == 2)
            {
                int newTotal;
                while (true)
                {
                    string input = ConsoleHelper.ReadNonEmpty("Enter total rounds");
                    if (int.TryParse(input, out newTotal) && newTotal > 0)
                    {
                        if (newTotal >= seeker.ClearedRounds)
                        {
                            break;
                        }
                        PanelFactory.RenderError($"Total rounds cannot be less than cleared rounds ({seeker.ClearedRounds}).");
                    }
                    else
                    {
                        PanelFactory.RenderError("Please enter a valid positive number.");
                    }
                }
                seeker.TotalRounds = newTotal;
                DataStore.SaveJobSeekers();
                PanelFactory.RenderSuccess("Total rounds updated successfully.");
                ConsoleHelper.Pause();
            }
        }
    }

    private void ManageAdmins()
    {
        bool running = true;

        while (running)
        {
            ShowHeader("Manage Admins", "Dashboard > Admin Management");

            var menuContent =
                $"[bold {UITheme.Admin}]  1.[/]  [white]View All Admins[/]\n" +
                $"[bold {UITheme.Success}]  2.[/]  [white]Approve Admin[/]\n" +
                $"[bold {UITheme.Error}]  3.[/]  [white]Reject Admin[/]\n" +
                $"[{UITheme.Dim}]  0.  Back[/]";

            AnsiConsole.Write(new Panel(menuContent).Header($"[bold {UITheme.Admin}]  Admin Account Management  [/]")
                    .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Admin)).Padding(2, 0));
            AnsiConsole.WriteLine();
            int choice;
            while (true)
            {
                string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-3]]:[/]").PromptStyle(UITheme.Admin).AllowEmpty());
                if (!int.TryParse(raw.Trim(), out choice) || choice < 0 || choice > 3)
                {
                    PanelFactory.RenderError("Invalid choice. Enter 0, 1, 2, or 3.");
                    continue;
                }
                break;
            }

            switch (choice)
            {
                case 0:
                    running = false;
                    break;

                case 1:
                    if (DataStore.Admins.Count == 0)
                    {
                        PanelFactory.RenderInfo("No admin accounts found.");
                        ConsoleHelper.Pause();
                        break;
                    }
                    {
                        int pageSize = 5;
                        int pageNumber = 1;
                        while (true)
                        {
                            ShowHeader("Manage Admins", "Dashboard > Admin Management > View All");
                            int totalPages = (int)Math.Ceiling((double)DataStore.Admins.Count / pageSize);
                            if (totalPages == 0) totalPages = 1;
                            if (pageNumber > totalPages) pageNumber = totalPages;

                            int start = (pageNumber - 1) * pageSize;
                            int end = Math.Min(start + pageSize, DataStore.Admins.Count);

                            var table = new Table()
                                .Border(TableBorder.Rounded)
                                .BorderStyle(Style.Parse(UITheme.Admin))
                                .AddColumn(new TableColumn($"[{UITheme.Dim}]#[/]").Centered())
                                .AddColumn(new TableColumn("[bold white]Name[/]"))
                                .AddColumn(new TableColumn("[bold white]Email[/]"))
                                .AddColumn(new TableColumn("[bold white]Company[/]"))
                                .AddColumn(new TableColumn("[bold white]Approved[/]").Centered());

                            for (int i = start; i < end; i++)
                            {
                                var admin = DataStore.Admins[i];
                                string approvedText = admin.IsApproved ? $"[green]✓ YES[/]" : $"[yellow]⏳ PENDING[/]";
                                table.AddRow(
                                    $"[{UITheme.Dim}]{i + 1}[/]",
                                    $"[white]{Markup.Escape(admin.Name)}[/]",
                                    $"[cyan]{Markup.Escape(admin.Email)}[/]",
                                    $"[magenta]{Markup.Escape(admin.Company)}[/]",
                                    approvedText
                                );
                            }

                            PanelFactory.RenderSectionTitle($"All Admin Accounts  (Page {pageNumber} of {totalPages})", "◆");
                            AnsiConsole.Write(table);
                            AnsiConsole.WriteLine();
                            Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back");
                            Console.WriteLine();

                            Console.Write("Enter command (N/P/0): ");
                            string input = Console.ReadLine()?.Trim() ?? "";
                            if (string.IsNullOrEmpty(input)) continue;

                            if (input.Equals("N", StringComparison.OrdinalIgnoreCase))
                            {
                                if (pageNumber < totalPages)
                                    pageNumber++;
                                else
                                {
                                    Console.WriteLine("You are already on the last page. Press any key...");
                                    Console.ReadKey(true);
                                }
                            }
                            else if (input.Equals("P", StringComparison.OrdinalIgnoreCase))
                            {
                                if (pageNumber > 1)
                                    pageNumber--;
                                else
                                {
                                    Console.WriteLine("You are already on the first page. Press any key...");
                                    Console.ReadKey(true);
                                }
                            }
                            else if (input == "0")
                            {
                                break;
                            }
                        }
                        break;
                    }
 
                 case 2:
                     {
                         while (true)
                         {
                             ShowHeader("Manage Admins", "Dashboard > Admin Management > Approve");
                             
                             var filteredAdmins = new List<Admin1>();
                             foreach (var admin in DataStore.Admins)
                             {
                                 if (admin.Email.Trim().ToLowerInvariant() != "admin@jpns.com")
                                 {
                                     filteredAdmins.Add(admin);
                                 }
                             }
                             TableFactory.RenderAdminTable(filteredAdmins);
                             AnsiConsole.WriteLine();
                             string email = ConsoleHelper.ReadEmail("Enter admin email to approve (or 0 to go back)");
                             if (email == "0") break;
 
                             Admin1? target = DataStore.FindAdmin(email);
                             if (target == null || target.Email.Trim().ToLowerInvariant() == "admin@jpns.com")
                             {
                                 PanelFactory.RenderError(target == null ? "Admin account not found." : "You cannot modify the super admin account.");
                                 ConsoleHelper.Pause();
                                 continue;
                             }
 
                             ShowHeader("Admin Account Details", "Dashboard > Admin Management > Approve > Details");
                             PanelFactory.RenderInfoCard("Admin Details", new[]
                             {
                                 ("Name", target.Name),
                                 ("Email", target.Email),
                                 ("Company", target.Company),
                                 ("Approved", target.IsApproved ? "Yes" : "No"),
                                 ("Active", target.IsActive ? "Yes" : "No")
                             }, UITheme.Admin);
                             AnsiConsole.WriteLine();
 
                             if (ConsoleHelper.Confirm($"Do you want to approve the admin [{email}]?"))
                             {
                                 target.Approve();
                                 DataStore.SaveAdmins();
                                 PanelFactory.RenderSuccess("Admin approved.");
                             }
                             else
                             {
                                 PanelFactory.RenderInfo("Approval cancelled.");
                             }
                             ConsoleHelper.Pause();
                         }
                         break;
                     }
 
                 case 3:
                     {
                         while (true)
                         {
                             ShowHeader("Manage Admins", "Dashboard > Admin Management > Reject");
                             
                             var filteredAdmins = new List<Admin1>();
                             foreach (var admin in DataStore.Admins)
                             {
                                 if (admin.Email.Trim().ToLowerInvariant() != "admin@jpns.com")
                                 {
                                     filteredAdmins.Add(admin);
                                 }
                             }
                             TableFactory.RenderAdminTable(filteredAdmins);
                             AnsiConsole.WriteLine();
                             string email = ConsoleHelper.ReadEmail("Enter admin email to reject (or 0 to go back)");
                             if (email == "0") break;
 
                             Admin1? target = DataStore.FindAdmin(email);
                             if (target == null || target.Email.Trim().ToLowerInvariant() == "admin@jpns.com")
                             {
                                 PanelFactory.RenderError(target == null ? "Admin account not found." : "You cannot modify the super admin account.");
                                 ConsoleHelper.Pause();
                                 continue;
                             }
 
                             ShowHeader("Admin Account Details", "Dashboard > Admin Management > Reject > Details");
                             PanelFactory.RenderInfoCard("Admin Details", new[]
                             {
                                 ("Name", target.Name),
                                 ("Email", target.Email),
                                 ("Company", target.Company),
                                 ("Approved", target.IsApproved ? "Yes" : "No"),
                                 ("Active", target.IsActive ? "Yes" : "No")
                             }, UITheme.Admin);
                             AnsiConsole.WriteLine();
 
                             if (ConsoleHelper.Confirm($"Do you want to reject the admin [{email}]?"))
                             {
                                 target.Reject();
                                 DataStore.SaveAdmins();
                                 PanelFactory.RenderSuccess("Admin rejected.");
                             }
                             else
                             {
                                 PanelFactory.RenderInfo("Rejection cancelled.");
                             }
                             ConsoleHelper.Pause();
                         }
                         break;
                     }
            }
        }
    }

    private void ApproveJobPostings()
    {
        bool running = true;
        while (running)
        {
            ShowHeader("Approve Job Postings", "Dashboard > Job Approvals");
            List<JobListing> pending = new List<JobListing>();
            foreach (JobListing job in DataStore.Jobs)
            {
                if (job.Status == JobStatus.PENDING_APPROVAL)
                {
                    pending.Add(job);
                }
            }
            if (pending.Count == 0)
            {
                PanelFactory.RenderInfo("No pending job postings.");
                ConsoleHelper.Pause();
                return;
            }
            int pageSize = 5;
            int pageNumber = 1;
            bool done = false;
            JobListing? selected = null;
            while (!done)
            {
                ShowHeader("Approve Job Postings", "Dashboard > Job Approvals");
                int totalPages = (int)Math.Ceiling((double)pending.Count / pageSize);
                if (totalPages == 0) totalPages = 1;

                int start = (pageNumber - 1) * pageSize;
                int end = Math.Min(start + pageSize, pending.Count);

                var pagePending = new List<JobListing>();
                for (int i = start; i < end; i++)
                {
                    pagePending.Add(pending[i]);
                }

                PanelFactory.RenderSectionTitle($"Pending Approval ({pending.Count} jobs - Page {pageNumber} of {totalPages})", "⏳");
                TableFactory.RenderPendingJobTable(pagePending, startIndex: start);
                AnsiConsole.WriteLine();
                Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back");
                Console.WriteLine();

                Console.Write("Select job number to review (or page navigation command): ");
                string input = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrEmpty(input)) continue;

                if (input.Equals("N", StringComparison.OrdinalIgnoreCase))
                {
                    if (pageNumber < totalPages)
                        pageNumber++;
                    else
                    {
                        Console.WriteLine("You are already on the last page. Press any key...");
                        Console.ReadKey(true);
                    }
                }
                else if (input.Equals("P", StringComparison.OrdinalIgnoreCase))
                {
                    if (pageNumber > 1)
                        pageNumber--;
                    else
                    {
                        Console.WriteLine("You are already on the first page. Press any key...");
                        Console.ReadKey(true);
                    }
                }
                else if (int.TryParse(input, out int sel))
                {
                    if (sel == 0) return;
                    if (sel >= 1 && sel <= pending.Count)
                    {
                        selected = pending[sel - 1];
                        done = true;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid selection. Enter a number between 1 and {pending.Count}. Press any key...");
                        Console.ReadKey(true);
                    }
                }
            }
            PanelFactory.RenderSectionTitle("Job Detail");
            selected?.Display();
            AnsiConsole.WriteLine();
            string actionMenu = $"[bold {UITheme.Success}]  1.[/]  [white]Approve[/]\n" + 
                                $"[bold {UITheme.Error}]  2.[/]  [white]Reject[/]\n" + 
                                $"[bold {UITheme.Primary}]  3.[/]  [white]Edit Job Listing[/]\n" +
                                $"[{UITheme.Dim}]  0.  Back[/]";
            AnsiConsole.Write(new Panel(actionMenu).Header($"[bold {UITheme.Warning}]  Review Action  [/]")
                    .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Warning)).Padding(2, 0));
            AnsiConsole.WriteLine();
            int choice;
            while (true)
            {
                string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-3]]:[/]").PromptStyle(UITheme.Warning).AllowEmpty());
                if (!int.TryParse(raw.Trim(), out choice) || choice < 0 || choice > 3)
                {
                    PanelFactory.RenderError("Invalid choice. Enter 0, 1, 2, or 3.");
                    continue;
                }
                break;
            }
            switch (choice)
            {
                case 0:
                    running = false;
                    break;

                case 1:
                    if (ConsoleHelper.Confirm("Approve this job posting?"))
                    {
                        selected!.Approve();
                        DataStore.SaveJobs();
                        DataStore.NotificationService.Send(selected!.AdminId, "Job Approved", $"Your job '{selected.Title}' has been approved and is now live.", "ANNOUNCEMENT");
                        PanelFactory.RenderSuccess("Job approved and published.");
                    }
                    break;

                case 2:
                    if (ConsoleHelper.Confirm("Reject this job posting?"))
                    {
                        string reason = ConsoleHelper.ReadNonEmpty("Rejection reason");
                        selected!.Reject();
                        DataStore.SaveJobs();
                        DataStore.NotificationService.Send(selected!.AdminId, "Job Rejected", $"Your job '{selected.Title}' was rejected. Reason: {reason}", "ANNOUNCEMENT");
                        PanelFactory.RenderWarning("Job rejected.");
                    }
                    break;

                case 3:
                    {
                        while (true)
                        {
                            ShowHeader("Edit Job Posting", "Dashboard > Job Approvals > Edit");
                            var editMenu =
                                $"[bold {UITheme.Primary}]  1.[/]  [white]Job Title[/]\n" +
                                $"[{UITheme.Dim}]  2.  Back[/]";
                            AnsiConsole.Write(new Panel(editMenu).Header($"[bold {UITheme.Warning}]  Edit Fields  [/]")
                                    .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Warning)).Padding(2, 0));
                            AnsiConsole.WriteLine();
                            int editChoice;
                            while (true)
                            {
                                string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[1-2]]:[/]").PromptStyle(UITheme.Warning).AllowEmpty());
                                if (!int.TryParse(raw.Trim(), out editChoice) || editChoice < 1 || editChoice > 2)
                                {
                                    PanelFactory.RenderError("Invalid choice. Enter 1 or 2.");
                                    continue;
                                }
                                break;
                            }
                            if (editChoice == 2) break;
                            if (editChoice == 1)
                            {
                                string newTitle = ConsoleHelper.ReadLine("Enter new job title (or 0 to cancel)");
                                if (newTitle != "0" && !string.IsNullOrWhiteSpace(newTitle))
                                {
                                    selected!.Title = newTitle.Trim();
                                    DataStore.SaveJobs();
                                    PanelFactory.RenderSuccess("Job title updated successfully.");
                                }
                                break;
                            }
                        }
                    }
                    break;
            }
            ConsoleHelper.Pause();
        }
    }

    private void ShowNotifications()
    {
        int pageSize = 5;
        int pageNumber = 1;
        while (true)
        {
            ShowHeader("Notification Center", "Dashboard > Notifications");
            var notifications = DataStore.NotificationService.GetAllForUser(_admin.AdminId);
            int unread = DataStore.NotificationService.GetPendingCount(_admin.AdminId);

            int totalPages = (int)Math.Ceiling((double)notifications.Count / pageSize);
            if (totalPages == 0) totalPages = 1;

            int start = (pageNumber - 1) * pageSize;
            int end = Math.Min(start + pageSize, notifications.Count);

            var pageNotifications = new List<JobPortal.Features.Notifications.Notification>();
            for (int i = start; i < end; i++)
            {
                pageNotifications.Add(notifications[i]);
            }

            PanelFactory.RenderSectionTitle($"Inbox ({unread} unread)  (Page {pageNumber} of {totalPages})", "◎");

            if (pageNotifications.Count == 0)
            {
                PanelFactory.RenderInfo("Your inbox is empty.");
            }
            else
            {
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

                for (int i = 0; i < pageNotifications.Count; i++)
                {
                    var n = pageNotifications[i];
                    bool isUnread = n.IsUnread();

                    string rowStyle = isUnread ? "bold white" : UITheme.Dim;
                    string readMark = isUnread ? $"[bold {UITheme.Warning}]● NEW[/]" : $"[{UITheme.Dim}]✓[/]";

                    string type = n.Type ?? "SYSTEM";
                    string typeColor = type.ToUpperInvariant() switch
                    {
                        "INTERVIEW_INVITE" => UITheme.Secondary,
                        "APPLICATION_STATUS" => UITheme.Candidate,
                        "ANNOUNCEMENT" => UITheme.Warning,
                        _ => UITheme.Info
                    };

                    table.AddRow(
                        $"[{UITheme.Dim}]{start + i + 1}[/]",
                        $"[{typeColor}]{Markup.Escape(type)}[/]",
                        $"[{rowStyle}]{Markup.Escape(n.Title ?? "—")}[/]",
                        $"[{rowStyle}]{Markup.Escape(n.Message ?? "—")}[/]",
                        $"[{UITheme.Dim}]{n.CreatedAt:dd MMM HH:mm}[/]",
                        readMark
                    );
                }
                AnsiConsole.Write(table);
            }

            AnsiConsole.WriteLine();
            Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [1] Mark all as read  ·  [0] Go back");
            Console.WriteLine();

            Console.Write("Enter command: ");
            string input = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(input)) continue;

            if (input.Equals("N", StringComparison.OrdinalIgnoreCase))
            {
                if (pageNumber < totalPages)
                    pageNumber++;
                else
                {
                    Console.WriteLine("You are already on the last page. Press any key...");
                    Console.ReadKey(true);
                }
            }
            else if (input.Equals("P", StringComparison.OrdinalIgnoreCase))
            {
                if (pageNumber > 1)
                    pageNumber--;
                else
                {
                    Console.WriteLine("You are already on the first page. Press any key...");
                    Console.ReadKey(true);
                }
            }
            else if (input == "1")
            {
                if (ConsoleHelper.Confirm("Mark all notifications as read?"))
                {
                    DataStore.NotificationService.MarkAllRead(_admin.AdminId);
                    PanelFactory.RenderSuccess("Marked all as read.");
                    ConsoleHelper.Pause();
                }
            }
            else if (input == "0")
            {
                break;
            }
        }
    }

    private void ManageComplaints()
    {
        try
        {
            if (DataStore.Complaints.Count == 0)
            {
                ShowHeader("Manage Complaints", "Dashboard > Complaints");
                PanelFactory.RenderInfo("No complaints have been submitted.");
                ConsoleHelper.Pause();
                return;
            }

            int pageSize = 5;
            int pageNumber = 1;
            while (true)
            {
                ShowHeader("Manage Complaints", "Dashboard > Complaints");
                int totalPages = (int)Math.Ceiling((double)DataStore.Complaints.Count / pageSize);
                if (totalPages == 0) totalPages = 1;

                int start = (pageNumber - 1) * pageSize;
                int end = Math.Min(start + pageSize, DataStore.Complaints.Count);

                var pageComplaints = new List<Complaint>();
                for (int i = start; i < end; i++)
                {
                    pageComplaints.Add(DataStore.Complaints[i]);
                }

                PanelFactory.RenderSectionTitle($"Complaints Inbox  ({DataStore.Complaints.Count} total - Page {pageNumber} of {totalPages})", "▲");
                TableFactory.RenderComplaintTable(pageComplaints, startIndex: start);
                AnsiConsole.WriteLine();
                Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back");
                Console.WriteLine();

                Console.Write("Enter complaint number to resolve / view (or page navigation command): ");
                string input = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrEmpty(input)) continue;

                if (input.Equals("N", StringComparison.OrdinalIgnoreCase))
                {
                    if (pageNumber < totalPages)
                        pageNumber++;
                    else
                    {
                        Console.WriteLine("You are already on the last page. Press any key...");
                        Console.ReadKey(true);
                    }
                }
                else if (input.Equals("P", StringComparison.OrdinalIgnoreCase))
                {
                    if (pageNumber > 1)
                        pageNumber--;
                    else
                    {
                        Console.WriteLine("You are already on the first page. Press any key...");
                        Console.ReadKey(true);
                    }
                }
                else if (int.TryParse(input, out int sel))
                {
                    if (sel == 0) return;
                    if (sel >= 1 && sel <= DataStore.Complaints.Count)
                    {
                        Complaint selected = DataStore.Complaints[sel - 1];
                        ShowHeader("Complaint Details", "Dashboard > Complaints > Details");

                        JobSeeker? seeker = null;
                        foreach (JobSeeker s in DataStore.JobSeekers)
                        {
                            string[] parts = s.SeekerId.Split('-');
                            if (parts.Length == 3 && int.TryParse(parts[2], out int parsedId) && parsedId == selected.SubmittedByUserId)
                            {
                                seeker = s;
                                break;
                            }
                        }
                        string applicantName = seeker != null ? seeker.Name : $"User ID {selected.SubmittedByUserId}";

                        PanelFactory.RenderInfoCard("Complaint details", new[]
                        {
                            ("Complaint ID", selected.ComplaintId.ToString()),
                            ("Submitted By", applicantName),
                            ("Subject", selected.Subject),
                            ("Description", selected.Description),
                            ("Status", selected.Status),
                            ("Admin Notes", string.IsNullOrWhiteSpace(selected.AdminNotes) ? "None" : selected.AdminNotes),
                            ("Submitted At", selected.CreatedAt.ToString("g")),
                            ("Resolved At", selected.ResolvedAt.HasValue ? selected.ResolvedAt.Value.ToString("g") : "N/A"),
                        }, UITheme.Admin);

                        if (selected.Status.ToUpperInvariant() != "RESOLVED")
                        {
                            var actionMenu =
                                $"[bold {UITheme.Success}]  1.[/]  [white]Resolve Complaint[/]\n" +
                                $"[{UITheme.Dim}]  0.  Back[/]";
                            
                            AnsiConsole.Write(new Panel(actionMenu).Header($"[bold {UITheme.Admin}]  Actions  [/]")
                                    .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Admin)).Padding(2, 0));
                            AnsiConsole.WriteLine();

                            int actionChoice;
                            while (true)
                            {
                                string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-1]]:[/]").PromptStyle(UITheme.Admin).AllowEmpty());
                                if (!int.TryParse(raw.Trim(), out actionChoice) || actionChoice < 0 || actionChoice > 1)
                                {
                                    PanelFactory.RenderError("Invalid choice. Enter 0 or 1.");
                                    continue;
                                }
                                break;
                            }

                            if (actionChoice == 1)
                            {
                                string notes = ConsoleHelper.ReadNonEmpty("Enter resolution notes");
                                selected.Status = "RESOLVED";
                                selected.AdminNotes = notes;
                                selected.ResolvedAt = DateTime.Now;

                                DataStore.SaveComplaints();

                                if (seeker != null)
                                {
                                    DataStore.NotificationService.Send(seeker.SeekerId, "Complaint Resolved",
                                        $"Your complaint regarding '{selected.Subject}' has been resolved. Notes: {notes}",
                                        "COMPLAINT_STATUS");
                                }

                                PanelFactory.RenderSuccess("Complaint resolved successfully!");
                                ConsoleHelper.Pause();
                                break;
                            }
                        }
                        else
                        {
                            ConsoleHelper.Pause();
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Invalid selection. Enter a number between 1 and {DataStore.Complaints.Count}. Press any key...");
                        Console.ReadKey(true);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            PanelFactory.RenderError($"Error managing complaints: {ex.Message}");
            ConsoleHelper.Pause();
        }
    }

    private void SendAnnouncement()
    {
        ShowHeader("Send Announcement", "Dashboard > Announcement");
        string msg = ConsoleHelper.ReadNonEmpty("Announcement message");
        if (ConsoleHelper.Confirm("Send this announcement to all users?"))
        {
            _admin.SendAnnouncement(msg);
            PanelFactory.RenderSuccess($"Announcement sent to {DataStore.JobSeekers.Count} user(s).");
        }
        ConsoleHelper.Pause();
    }

    private void PostNewJob()
    {
        ShowHeader("Post New Job", "Dashboard > Post Job");
        if (!_admin.IsApproved)
        {
            throw new AccessDeniedException("Your admin account is not yet approved.");
        }
        PanelFactory.RenderInfo("Each field is validated immediately. Fill in the details below.");
        AnsiConsole.WriteLine();
        string title = ConsoleHelper.ReadJobTitle("Job Title");
        string desc = ConsoleHelper.ReadNonEmpty("Description");
        string location = ConsoleHelper.ReadNonEmpty("Location (city or Remote)");
        string skillsRaw = ConsoleHelper.ReadNonEmpty("Required Skills (comma-separated)");
        string[] skillArray = skillsRaw.Split(',');
        List<string> skills = new List<string>();
        foreach (string skill in skillArray)
        {
            string trimmedSkill = skill.Trim();
            if (trimmedSkill.Length > 0)
            {
                skills.Add(trimmedSkill);
            }
        }
        decimal min = ConsoleHelper.ReadPositiveDecimal("Min Salary (LPA)");
        decimal max;
        while (true)
        {
            max = ConsoleHelper.ReadPositiveDecimal("Max Salary (LPA)");
            if (max >= min)
            {
                break;
            }
            PanelFactory.RenderError("Max salary must be greater than or equal to Min salary.");
        }
        SalaryRange salary = new SalaryRange(min, max);
        int exp = ConsoleHelper.ReadPositiveInt("Experience Required (years)");
        DateTime expiry = ConsoleHelper.ReadExpiryDate("Expiry Date (yyyy-MM-dd)");
        
        int rounds = 3;
        while (true)
        {
            string rawRounds = ConsoleHelper.ReadLine("Number of Interview Rounds (default 3)");
            if (string.IsNullOrWhiteSpace(rawRounds))
            {
                rounds = 3;
                break;
            }
            if (int.TryParse(rawRounds, out int parsedRounds) && parsedRounds > 0)
            {
                rounds = parsedRounds;
                break;
            }
            PanelFactory.RenderError("Please enter a positive integer for interview rounds.");
        }

        if (ConsoleHelper.Confirm("Submit this job posting for approval?"))
        {
            JobListing job = new JobListing(_admin.AdminId, title, desc, _admin.Company, location, salary, exp, skills, expiry, rounds);
            _admin.AddJobListing(job);
            PanelFactory.RenderSuccess($"Job '{title}' submitted for approval. ID: {job.JobId}");
        }
        ConsoleHelper.Pause();
    }

    private void ManageMyListings()
    {
        bool running = true;
        while (running)
        {
            ShowHeader("My Job Listings", "Dashboard > My Listings");
            List<JobListing> mine = new List<JobListing>();
            foreach (JobListing job in DataStore.Jobs)
            {
                if (job.AdminId == _admin.AdminId)
                {
                    mine.Add(job);
                }
            }
            if (mine.Count == 0)
            {
                PanelFactory.RenderInfo("No job listings found.");
                ConsoleHelper.Pause();
                return;
            }
            int pageSize = 5;
            int pageNumber = 1;
            bool done = false;
            JobListing? selected = null;
            while (!done)
            {
                ShowHeader("My Job Listings", "Dashboard > My Listings");
                int totalPages = (int)Math.Ceiling((double)mine.Count / pageSize);
                if (totalPages == 0) totalPages = 1;

                int start = (pageNumber - 1) * pageSize;
                int end = Math.Min(start + pageSize, mine.Count);

                var pageMine = new List<JobListing>();
                for (int i = start; i < end; i++)
                {
                    pageMine.Add(mine[i]);
                }

                PanelFactory.RenderSectionTitle($"My Listings ({mine.Count} total - Page {pageNumber} of {totalPages})", "◈");
                TableFactory.RenderJobTable(pageMine, showStatus: true, startIndex: start);
                AnsiConsole.WriteLine();
                
                int activeCount = 0;
                int pendingCount = 0;
                int closedCount = 0;
                int expiredCount = 0;
                foreach (JobListing job in mine)
                {
                    if (job.IsActive && !job.IsExpired()) activeCount++;
                    if (job.Status == JobStatus.PENDING_APPROVAL) pendingCount++;
                    if (job.Status == JobStatus.CLOSED) closedCount++;
                    if (job.Status == JobStatus.EXPIRED) expiredCount++;
                }
                StatusRenderer.RenderJobStatusSummary(activeCount, pendingCount, closedCount, expiredCount);
                AnsiConsole.WriteLine();

                Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back");
                Console.WriteLine();

                Console.Write("Select listing number to manage (or page navigation command): ");
                string input = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrEmpty(input)) continue;

                if (input.Equals("N", StringComparison.OrdinalIgnoreCase))
                {
                    if (pageNumber < totalPages)
                        pageNumber++;
                    else
                    {
                        Console.WriteLine("You are already on the last page. Press any key...");
                        Console.ReadKey(true);
                    }
                }
                else if (input.Equals("P", StringComparison.OrdinalIgnoreCase))
                {
                    if (pageNumber > 1)
                        pageNumber--;
                    else
                    {
                        Console.WriteLine("You are already on the first page. Press any key...");
                        Console.ReadKey(true);
                    }
                }
                else if (int.TryParse(input, out int sel))
                {
                    if (sel == 0) return;
                    if (sel >= 1 && sel <= mine.Count)
                    {
                        selected = mine[sel - 1];
                        done = true;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid selection. Enter a number between 1 and {mine.Count}. Press any key...");
                        Console.ReadKey(true);
                    }
                }
            }
            PanelFactory.RenderSectionTitle("Manage Listing");
            selected?.Display();
            AnsiConsole.WriteLine();
            string actionMenu = $"[bold {UITheme.Info}]  1.[/]  [white]Update Description[/]\n" + $"[bold {UITheme.Warning}]  2.[/]  [white]Close Listing[/]\n" +
                $"[bold {UITheme.Error}]  3.[/]  [white]Delete Listing[/]\n" + $"[{UITheme.Dim}]  0.  Back[/]";
            AnsiConsole.Write(new Panel(actionMenu).Header($"[bold {UITheme.Primary}]  Listing Actions  [/]")
                    .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Primary)).Padding(2, 0));
            AnsiConsole.WriteLine();
            int action;
            while (true)
            {
                string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-3]]:[/]").PromptStyle(UITheme.Primary).AllowEmpty());
                if (!int.TryParse(raw.Trim(), out action) || action < 0 || action > 3)
                {
                    PanelFactory.RenderError("Invalid choice. Enter 0, 1, 2, or 3.");
                    continue;
                }
                break;
            }
            switch (action)
            {
                case 0:
                    running = false;
                    break;

                case 1:
                    {
                        string newDesc = ConsoleHelper.ReadNonEmpty("New description");
                        if (ConsoleHelper.Confirm("Update description?"))
                        {
                            _admin.UpdateJobListing(selected!.JobId, selected!.Title, newDesc);
                            PanelFactory.RenderSuccess("Listing updated.");
                        }
                        break;
                    }

                case 2:
                    {
                        if (ConsoleHelper.Confirm("Close this listing?"))
                        {
                            _admin.CloseJobListing(selected!.JobId);
                            PanelFactory.RenderSuccess("Listing closed.");
                        }
                        break;
                    }

                case 3:
                    {
                        if (ConsoleHelper.Confirm("Permanently delete this listing?"))
                        {
                            _admin.DeleteJobListing(selected!.JobId);
                            PanelFactory.RenderSuccess("Listing deleted.");
                        }
                        break;
                    }
            }
            ConsoleHelper.Pause();
        }
    }

    private void ViewApplications()
    {
        ShowHeader("View Applications", "Dashboard > Applications");
        List<JobListing> mine = new List<JobListing>();
        foreach (JobListing job in DataStore.Jobs)
        {
            if (job.AdminId == _admin.AdminId)
            {
                mine.Add(job);
            }
        }
        if (mine.Count == 0)
        {
            PanelFactory.RenderInfo("No jobs found.");
            ConsoleHelper.Pause();
            return;
        }
        int pageSize = 5;
        int pageNumber = 1;
        bool done = false;
        while (!done)
        {
            ShowHeader("View Applications", "Dashboard > Applications");
            int totalPages = (int)Math.Ceiling((double)mine.Count / pageSize);
            if (totalPages == 0) totalPages = 1;

            int start = (pageNumber - 1) * pageSize;
            int end = Math.Min(start + pageSize, mine.Count);

            Table table = new Table()
                .Border(TableBorder.Rounded)
                .BorderStyle(Style.Parse(UITheme.Admin))
                .Expand()
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Job Title[/]"))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Status[/]").Centered())
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Applications[/]").Centered());

            for (int i = start; i < end; i++)
            {
                int applicationCount = 0;
                foreach (var application in DataStore.Applications)
                {
                    if (application.JobId == mine[i].JobId)
                    {
                        applicationCount++;
                    }
                }
                table.AddRow(
                    $"[{UITheme.Dim}]{i + 1}[/]",
                    $"[bold white]{Markup.Escape(mine[i].Title)}[/]",
                    UITheme.Badge(mine[i].Status.ToString()),
                    applicationCount > 0
                        ? $"[bold {UITheme.Candidate}]{applicationCount}[/]"
                        : $"[{UITheme.Dim}]0[/]"
                );
            }
            PanelFactory.RenderSectionTitle($"My Jobs  (Page {pageNumber} of {totalPages})", "⚙");
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
            Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back");
            Console.WriteLine();

            Console.Write("Select job number to view applicants (or page navigation command): ");
            string input = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(input)) continue;

            if (input.Equals("N", StringComparison.OrdinalIgnoreCase))
            {
                if (pageNumber < totalPages)
                    pageNumber++;
                else
                {
                    Console.WriteLine("You are already on the last page. Press any key...");
                    Console.ReadKey(true);
                }
            }
            else if (input.Equals("P", StringComparison.OrdinalIgnoreCase))
            {
                if (pageNumber > 1)
                    pageNumber--;
                else
                {
                    Console.WriteLine("You are already on the first page. Press any key...");
                    Console.ReadKey(true);
                }
            }
            else if (int.TryParse(input, out int sel))
            {
                if (sel == 0) return;
                if (sel >= 1 && sel <= mine.Count)
                {
                    JobListing selectedJob = mine[sel - 1];
                    List<Application1> apps = _admin.ViewApplications(selectedJob.JobId);
                    
                    if (apps.Count == 0)
                    {
                        PanelFactory.RenderInfo("No applications for this job.");
                        ConsoleHelper.Pause();
                    }
                    else
                    {
                        int appPageSize = 5;
                        int appPageNumber = 1;
                        while (true)
                        {
                            ShowHeader("View Applications", $"Dashboard > Applications > {selectedJob.Title}");
                            int appTotalPages = (int)Math.Ceiling((double)apps.Count / appPageSize);
                            if (appTotalPages == 0) appTotalPages = 1;

                            int appStart = (appPageNumber - 1) * appPageSize;
                            int appEnd = Math.Min(appStart + appPageSize, apps.Count);

                            var pageApps = new List<Application1>();
                            for (int i = appStart; i < appEnd; i++)
                            {
                                pageApps.Add(apps[i]);
                            }

                            PanelFactory.RenderSectionTitle($"Applicants for: {selectedJob.Title}  (Page {appPageNumber} of {appTotalPages})");
                            TableFactory.RenderApplicationsByJobTable(pageApps, DataStore.JobSeekers, startIndex: appStart);
                            AnsiConsole.WriteLine();
                            Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Back");
                            Console.WriteLine();

                            Console.Write("Enter command: ");
                            string appInput = Console.ReadLine()?.Trim() ?? "";
                            if (string.IsNullOrEmpty(appInput)) continue;

                            if (appInput.Equals("N", StringComparison.OrdinalIgnoreCase))
                            {
                                if (appPageNumber < appTotalPages)
                                    appPageNumber++;
                                else
                                {
                                    Console.WriteLine("You are already on the last page. Press any key...");
                                    Console.ReadKey(true);
                                }
                            }
                            else if (appInput.Equals("P", StringComparison.OrdinalIgnoreCase))
                            {
                                if (appPageNumber > 1)
                                    appPageNumber--;
                                else
                                {
                                    Console.WriteLine("You are already on the first page. Press any key...");
                                    Console.ReadKey(true);
                                }
                            }
                            else if (appInput == "0")
                            {
                                break;
                            }
                        }
                    }
                    done = true;
                }
                else
                {
                    Console.WriteLine($"Invalid selection. Enter a number between 1 and {mine.Count}. Press any key...");
                    Console.ReadKey(true);
                }
            }
        }
    }

    private void ShortlistReject()
    {
        int pageSize = 5;
        int pageNumber = 1;
        int filterChoice = 6; 

        while (true)
        {
            ShowHeader("Review Applications - Select Filter", "Dashboard > Review Applications > Filter");

            var filterMenu =
                $"[bold {UITheme.Primary}]  1.[/]  [white]Hired[/]\n" +
                $"[bold {UITheme.Primary}]  2.[/]  [white]Shortlisted[/]\n" +
                $"[bold {UITheme.Primary}]  3.[/]  [white]Pending[/]\n" +
                $"[bold {UITheme.Primary}]  4.[/]  [white]Rejected[/]\n" +
                $"[bold {UITheme.Primary}]  5.[/]  [white]Interview Scheduled[/]\n" +
                $"[bold {UITheme.Secondary}]  6.[/]  [white]All[/]\n" +
                $"[{UITheme.Dim}]  0.  Back[/]";

            AnsiConsole.Write(new Panel(filterMenu).Header($"[bold {UITheme.Primary}]  Status Filter  [/]")
                    .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Primary)).Padding(2, 0));
            AnsiConsole.WriteLine();

            while (true)
            {
                string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Select filter choice [[0-6]]:[/]").PromptStyle(UITheme.Primary).AllowEmpty());
                if (!int.TryParse(raw.Trim(), out filterChoice) || filterChoice < 0 || filterChoice > 6)
                {
                    PanelFactory.RenderError("Invalid choice. Enter 0 to 6.");
                    continue;
                }
                break;
            }

            if (filterChoice == 0) return; 
            pageNumber = 1;
            while (true)
            {
                ShowHeader("Shortlist / Reject Applicant", "Dashboard > Review Applications");
                List<Application1> allApps = new List<Application1>();
                foreach (Application1 application in DataStore.Applications)
                {
                    bool belongsToAdmin = false;
                    foreach (JobListing job in DataStore.Jobs)
                    {
                        if (job.JobId == application.JobId && job.AdminId == _admin.AdminId)
                        {
                            belongsToAdmin = true;
                            break;
                        }
                    }

                    if (belongsToAdmin)
                    {
                        bool matchesFilter = filterChoice switch
                        {
                            1 => application.Status == ApplicationStatus.HIRED,
                            2 => application.Status == ApplicationStatus.SHORTLISTED,
                            3 => application.Status == ApplicationStatus.PENDING,
                            4 => application.Status == ApplicationStatus.REJECTED,
                            5 => application.Status == ApplicationStatus.INTERVIEW_SCHEDULED,
                            6 => true, 
                            _ => true
                        };

                        if (matchesFilter)
                        {
                            allApps.Add(application);
                        }
                    }
                }

                if (allApps.Count == 0)
                {
                    PanelFactory.RenderInfo("No applications found matching this status filter.");
                    ConsoleHelper.Pause();
                    break; // Go back to filter selection menu
                }

                int totalPages = (int)Math.Ceiling((double)allApps.Count / pageSize);
                if (totalPages == 0) totalPages = 1;
                if (pageNumber > totalPages) pageNumber = totalPages;

                int start = (pageNumber - 1) * pageSize;
                int end = Math.Min(start + pageSize, allApps.Count);

                var pageApps = new List<Application1>();
                for (int i = start; i < end; i++)
                {
                    pageApps.Add(allApps[i]);
                }

                string filterText = filterChoice switch
                {
                    1 => "Hired",
                    2 => "Shortlisted",
                    3 => "Pending",
                    4 => "Rejected",
                    5 => "Interview Scheduled",
                    _ => "All"
                };

                PanelFactory.RenderSectionTitle($"Applications ({filterText})  (Page {pageNumber} of {totalPages})", "▶");
                TableFactory.RenderApplicationsByJobTable(pageApps, DataStore.JobSeekers, startIndex: start);
                AnsiConsole.WriteLine();
                Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back to filter menu");
                Console.WriteLine();

                Console.Write("Select application number (or page navigation command): ");
                string input = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrEmpty(input)) continue;

                if (input.Equals("N", StringComparison.OrdinalIgnoreCase))
                {
                    if (pageNumber < totalPages)
                        pageNumber++;
                    else
                    {
                        Console.WriteLine("You are already on the last page. Press any key...");
                        Console.ReadKey(true);
                    }
                    continue;
                }
                else if (input.Equals("P", StringComparison.OrdinalIgnoreCase))
                {
                    if (pageNumber > 1)
                        pageNumber--;
                    else
                    {
                        Console.WriteLine("You are already on the first page. Press any key...");
                        Console.ReadKey(true);
                    }
                    continue;
                }
                else if (int.TryParse(input, out int sel))
                {
                    if (sel == 0) break; 
                    if (sel >= 1 && sel <= allApps.Count)
                    {
                        Application1 selected = allApps[sel - 1];

                        
                        if (selected.Status == ApplicationStatus.REJECTED)
                        {
                            PanelFactory.RenderError("This application has already been rejected.");
                            ConsoleHelper.Pause();
                            continue;
                        }
                        if (selected.Status == ApplicationStatus.HIRED)
                        {
                            PanelFactory.RenderError("This candidate has already been hired.");
                            ConsoleHelper.Pause();
                            continue;
                        }

                        AnsiConsole.WriteLine();
                        string actionMenu = $"[bold {UITheme.Candidate}]  1.[/]  [white]Shortlist[/]\n" + $"[bold {UITheme.Error}]  2.[/]  [white]Reject[/]\n" + $"[{UITheme.Dim}]  0.  Cancel[/]";
                        AnsiConsole.Write(new Panel(actionMenu).Header($"[bold {UITheme.Admin}]  Review Decision  [/]")
                                .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Admin)).Padding(2, 0));
                        AnsiConsole.WriteLine();
                        int action;
                        while (true)
                        {
                            string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-2]]:[/]").PromptStyle(UITheme.Admin).AllowEmpty());
                            if (!int.TryParse(raw.Trim(), out action) || action < 0 || action > 2)
                            {
                                PanelFactory.RenderError("Invalid choice. Enter 0, 1, or 2.");
                                continue;
                            }
                            break;
                        }

                        if (action == 0) continue;

                        if (action == 1)
                        {
                            if (selected.Status == ApplicationStatus.SHORTLISTED)
                            {
                                if (ConsoleHelper.Confirm("This candidate is already shortlisted. Do you want to schedule the interview?"))
                                {
                                    ScheduleInterview(selected);
                                }
                                continue;
                            }
                            if (selected.Status == ApplicationStatus.INTERVIEW_SCHEDULED)
                            {
                                Interview1? scheduledInv = null;
                                foreach (Interview1 iv in DataStore.Interviews)
                                {
                                    if (iv.ApplicationId == selected.ApplicationId && iv.Status == InterviewStatus.SCHEDULED)
                                    {
                                        scheduledInv = iv;
                                        break;
                                    }
                                }

                                if (scheduledInv != null)
                                {
                                    string timeStr = scheduledInv.ScheduledAt.ToString("dd MMM yyyy hh:mm tt");
                                    if (scheduledInv.ScheduledAt > DateTime.Now)
                                    {
                                        PanelFactory.RenderError($"This candidate has an interview scheduled in the future at {timeStr}. You cannot shortlist/reject this candidate yet.");
                                        ConsoleHelper.Pause();
                                        continue;
                                    }
                                    else
                                    {
                                        if (ConsoleHelper.Confirm("Shortlist this candidate?"))
                                        {
                                            _admin.ShortlistCandidate(selected.ApplicationId);
                                            PanelFactory.RenderSuccess("Candidate shortlisted.");
                                            Thread.Sleep(800);
                                        }
                                        continue;
                                    }
                                }
                            }

                            if (ConsoleHelper.Confirm("Shortlist this candidate?"))
                            {
                                _admin.ShortlistCandidate(selected.ApplicationId);
                                PanelFactory.RenderSuccess("Candidate shortlisted.");
                                if (ConsoleHelper.Confirm("Do you want to schedule the interview for the candidate?"))
                                {
                                    ScheduleInterview(selected);
                                }
                                else
                                {
                                    Thread.Sleep(800);
                                }
                            }
                        }
                        else if (action == 2)
                        {
                            if (selected.Status == ApplicationStatus.INTERVIEW_SCHEDULED)
                            {
                                Interview1? scheduledInv = null;
                                foreach (Interview1 iv in DataStore.Interviews)
                                {
                                    if (iv.ApplicationId == selected.ApplicationId && iv.Status == InterviewStatus.SCHEDULED)
                                    {
                                        scheduledInv = iv;
                                        break;
                                    }
                                }

                                if (scheduledInv != null && scheduledInv.ScheduledAt > DateTime.Now)
                                {
                                    string timeStr = scheduledInv.ScheduledAt.ToString("dd MMM yyyy hh:mm tt");
                                    PanelFactory.RenderError($"This candidate has an interview scheduled in the future at {timeStr}. You cannot shortlist/reject this candidate yet.");
                                    ConsoleHelper.Pause();
                                    continue;
                                }
                            }

                            if (ConsoleHelper.Confirm("Reject this candidate?"))
                            {
                                _admin.RejectCandidate(selected.ApplicationId);
                                PanelFactory.RenderSuccess("Candidate rejected.");
                                Thread.Sleep(800);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Invalid selection. Enter a number between 1 and {allApps.Count}. Press any key...");
                        Console.ReadKey(true);
                    }
                }
            }
        }
    }

    private void InterviewManagementMenu()
    {
        bool running = true;
        while (running)
        {
            ShowHeader("Interview Management", "Dashboard > Interviews");

            var menuContent =
                $"[bold {UITheme.Primary}]  1.[/]  [white]Schedule Interview[/]\n" +
                $"[bold {UITheme.Secondary}]  2.[/]  [white]Update Scheduled Interview Status[/]\n" +
                $"[{UITheme.Dim}]  0.  Back[/]";

            AnsiConsole.Write(new Panel(menuContent).Header($"[bold {UITheme.Secondary}]  Interview Management  [/]")
                    .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Secondary)).Padding(2, 0));
            AnsiConsole.WriteLine();

            int choice;
            while (true)
            {
                string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-2]]:[/]").PromptStyle(UITheme.Secondary).AllowEmpty());
                if (!int.TryParse(raw.Trim(), out choice) || choice < 0 || choice > 2)
                {
                    PanelFactory.RenderError("Invalid choice. Enter 0, 1, or 2.");
                    continue;
                }
                break;
            }

            switch (choice)
            {
                case 0:
                    running = false;
                    break;
                case 1:
                    ScheduleInterview();
                    break;
                case 2:
                    UpdateInterviewStatus();
                    break;
            }
        }
    }

    private void ScheduleInterview(Application1? targetApp = null)
    {
        ShowHeader("Schedule Interview", "Dashboard > Interviews");
        Application1? app = targetApp;
        if (app == null)
        {
            List<Application1> shortlisted = new List<Application1>();
            foreach (Application1 application in DataStore.Applications)
            {
                if (application.Status != ApplicationStatus.SHORTLISTED)
                {
                    continue;
                }
                bool belongsToAdmin = false;
                foreach (JobListing job in DataStore.Jobs)
                {
                    if (job.JobId == application.JobId && job.AdminId == _admin.AdminId)
                    {
                        belongsToAdmin = true;
                        break;
                    }
                }

                if (belongsToAdmin)
                {
                    shortlisted.Add(application);
                }
            }

            if (shortlisted.Count == 0)
            {
                PanelFactory.RenderInfo("No shortlisted applications.");
                ConsoleHelper.Pause();
                return;
            }

            int pageSize = 5;
            int pageNumber = 1;
            bool done = false;
            while (!done)
            {
                ShowHeader("Schedule Interview", "Dashboard > Interviews");
                int totalPages = (int)Math.Ceiling((double)shortlisted.Count / pageSize);
                if (totalPages == 0) totalPages = 1;

                int start = (pageNumber - 1) * pageSize;
                int end = Math.Min(start + pageSize, shortlisted.Count);

                var pageApps = new List<Application1>();
                for (int i = start; i < end; i++)
                {
                    pageApps.Add(shortlisted[i]);
                }

                PanelFactory.RenderSectionTitle($"Shortlisted Candidates  (Page {pageNumber} of {totalPages})", "▶");
                TableFactory.RenderApplicationsByJobTable(pageApps, DataStore.JobSeekers, startIndex: start);
                AnsiConsole.WriteLine();
                Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back");
                Console.WriteLine();

                Console.Write("Select application number (or page navigation command): ");
                string input = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrEmpty(input)) continue;

                if (input.Equals("N", StringComparison.OrdinalIgnoreCase))
                {
                    if (pageNumber < totalPages)
                        pageNumber++;
                    else
                    {
                        Console.WriteLine("You are already on the last page. Press any key...");
                        Console.ReadKey(true);
                    }
                }
                else if (input.Equals("P", StringComparison.OrdinalIgnoreCase))
                {
                    if (pageNumber > 1)
                        pageNumber--;
                    else
                    {
                        Console.WriteLine("You are already on the first page. Press any key...");
                        Console.ReadKey(true);
                    }
                }
                else if (int.TryParse(input, out int sel))
                {
                    if (sel == 0) return;
                    if (sel >= 1 && sel <= shortlisted.Count)
                    {
                        app = shortlisted[sel - 1];
                        done = true;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid selection. Enter a number between 1 and {shortlisted.Count}. Press any key...");
                        Console.ReadKey(true);
                    }
                }
            }
        }
        if (app == null) return;

        if (app.Status == ApplicationStatus.HIRED)
        {
            PanelFactory.RenderError("This candidate has already cleared all rounds and is HIRED. No more interviews can be scheduled.");
            ConsoleHelper.Pause();
            return;
        }

        // Block scheduling if seeker is already in HIRED status
        var preCheckSeeker = DataStore.FindJobSeekerById(app.JobSeekerId);
        if (preCheckSeeker != null && preCheckSeeker.CandidateStatus == "HIRED")
        {
            PanelFactory.RenderError("This candidate has already been HIRED and no more interviews can be scheduled.");
            ConsoleHelper.Pause();
            return;
        }
        AnsiConsole.WriteLine();
        string modeMenu = $"[bold {UITheme.Info}]  1.[/]  [white]Online (Video Call)[/]\n" + $"[bold {UITheme.Neutral}]  2.[/]  [white]Offline (In-Person)[/]";
        AnsiConsole.Write(new Panel(modeMenu).Header($"[bold {UITheme.Secondary}]  Interview Mode  [/]")
                .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Secondary)).Padding(2, 0));
        AnsiConsole.WriteLine();
        int modeChoice;
        while (true)
        {
            string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Mode [[1-2]]:[/]").PromptStyle(UITheme.Secondary).AllowEmpty());
            if (!int.TryParse(raw.Trim(), out modeChoice) || modeChoice < 1 || modeChoice > 2)
            {
                PanelFactory.RenderError("Enter 1 for Online or 2 for Offline.");
                continue;
            }
            break;
        }
        string mode = modeChoice == 1 ? "Online" : "Offline";
        string link = "";
        string venue = "";
        if (mode == "Online")
        {
            link = ConsoleHelper.ReadNonEmpty("Meeting link (e.g. https://meet.google.com/xxx)");
        }
        if (mode == "Offline")
        {
            venue = ConsoleHelper.ReadNonEmpty("Venue address");
        }
        DateTime scheduledAt = ConsoleHelper.ReadFutureDateTime("Interview date & time (yyyy-MM-dd HH:mm)");
        if (ConsoleHelper.Confirm("Schedule this interview?"))
        {
            Interview1 interview = DataStore.InterviewScheduler.Schedule(app!.ApplicationId, scheduledAt, mode, link, venue);
            app!.UpdateStatus("INTERVIEW_SCHEDULED");
            DataStore.SaveApplications();
            JobListing? selectedJob = null;
            foreach (JobListing job in DataStore.Jobs)
            {
                if (job.JobId == app!.JobId)
                {
                    selectedJob = job;
                    break;
                }
            }

            // If scheduling this interview maxes out the round counter, auto-hire the candidate
            var scheduledSeeker = DataStore.FindJobSeekerById(app.JobSeekerId);
            if (scheduledSeeker != null && scheduledSeeker.CurrentRound >= scheduledSeeker.TotalRounds)
            {
                app.UpdateStatus("HIRED");
                DataStore.SaveApplications();
                DataStore.NotificationService.Send(app.JobSeekerId, "Congratulations! You are Hired",
                    $"You have been selected for all {scheduledSeeker.TotalRounds} rounds for '{selectedJob?.Title}' and have been HIRED!",
                    "APPLICATION_STATUS");
                PanelFactory.RenderSuccess($"Interview scheduled for the final round. Candidate has been automatically marked as HIRED!");
            }
            else
            {
                DataStore.NotificationService.Send(app!.JobSeekerId, "Interview Scheduled", $"Interview for '{selectedJob?.Title}' scheduled on {scheduledAt:dd MMM yyyy HH:mm}. Mode: {mode}" +
                    (mode == "Online" ? $" | Link: {link}" : $" | Venue: {venue}"), "INTERVIEW_INVITE");
                PanelFactory.RenderSuccess($"Interview scheduled. ID: {interview.InterviewId}");
            }
        }
        ConsoleHelper.Pause();
    }

    private void UpdateInterviewStatus()
    {
        try
        {
            int pageSize = 5;
            int pageNumber = 1;
            while (true)
            {
                List<Interview1> adminInterviews = new List<Interview1>();
                foreach (Interview1 iv in DataStore.Interviews)
                {
                    if (iv.Status == InterviewStatus.SCHEDULED)
                    {
                        Application1? app = null;
                        foreach (Application1 a in DataStore.Applications)
                        {
                            if (a.ApplicationId == iv.ApplicationId)
                            {
                                app = a;
                                break;
                            }
                        }
                        if (app != null)
                        {
                            bool belongsToAdmin = false;
                            foreach (JobListing job in DataStore.Jobs)
                            {
                                if (job.JobId == app.JobId && job.AdminId == _admin.AdminId)
                                {
                                    belongsToAdmin = true;
                                    break;
                                }
                            }
                            if (belongsToAdmin)
                            {
                                adminInterviews.Add(iv);
                            }
                        }
                    }
                }

                if (adminInterviews.Count == 0)
                {
                    ShowHeader("Update Interview Status", "Dashboard > Interviews > Update");
                    PanelFactory.RenderInfo("No scheduled interviews found.");
                    ConsoleHelper.Pause();
                    return;
                }

                ShowHeader("Update Interview Status", "Dashboard > Interviews > Update");
                int totalPages = (int)Math.Ceiling((double)adminInterviews.Count / pageSize);
                if (totalPages == 0) totalPages = 1;
                if (pageNumber > totalPages) pageNumber = totalPages;

                int start = (pageNumber - 1) * pageSize;
                int end = Math.Min(start + pageSize, adminInterviews.Count);

                var pageIvs = new List<Interview1>();
                for (int i = start; i < end; i++)
                {
                    pageIvs.Add(adminInterviews[i]);
                }

                PanelFactory.RenderSectionTitle($"Scheduled Interviews  ({adminInterviews.Count} total - Page {pageNumber} of {totalPages})", "▶");
                TableFactory.RenderInterviewTable(pageIvs, DataStore.Applications, DataStore.Jobs, startIndex: start);
                AnsiConsole.WriteLine();
                Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back");
                Console.WriteLine();

                Console.Write("Enter interview number to update status (or page navigation command): ");
                string input = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrEmpty(input)) continue;

                if (input.Equals("N", StringComparison.OrdinalIgnoreCase))
                {
                    if (pageNumber < totalPages)
                        pageNumber++;
                    else
                    {
                        Console.WriteLine("You are already on the last page. Press any key...");
                        Console.ReadKey(true);
                    }
                }
                else if (input.Equals("P", StringComparison.OrdinalIgnoreCase))
                {
                    if (pageNumber > 1)
                        pageNumber--;
                    else
                    {
                        Console.WriteLine("You are already on the first page. Press any key...");
                        Console.ReadKey(true);
                    }
                }
                else if (int.TryParse(input, out int sel))
                {
                    if (sel == 0) return;
                    if (sel >= 1 && sel <= adminInterviews.Count)
                    {
                        Interview1 selectedIv = adminInterviews[sel - 1];
                        UpdateSelectedInterview(selectedIv);
                        ConsoleHelper.Pause();
                    }
                    else
                    {
                        Console.WriteLine($"Invalid selection. Enter a number between 1 and {adminInterviews.Count}. Press any key...");
                        Console.ReadKey(true);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            PanelFactory.RenderError($"Error: {ex.Message}");
            ConsoleHelper.Pause();
        }
    }

    private void UpdateSelectedInterview(Interview1 iv)
    {
        ShowHeader("Update Interview Details", "Dashboard > Interviews > Update > Details");
        var statusMenu = 
            $"[bold {UITheme.Success}]  1.[/]  [white]Mark Completed[/]\n" +
            $"[bold {UITheme.Error}]  2.[/]  [white]Mark Cancelled[/]\n" +
            $"[{UITheme.Dim}]  0.  Back[/]";
        AnsiConsole.Write(new Panel(statusMenu).Header($"[bold {UITheme.Secondary}]  Select Status  [/]")
                .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Secondary)).Padding(2, 0));
        AnsiConsole.WriteLine();
        int statusChoice;
        while (true)
        {
            string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-2]]:[/]").PromptStyle(UITheme.Secondary).AllowEmpty());
            if (!int.TryParse(raw.Trim(), out statusChoice) || statusChoice < 0 || statusChoice > 2)
            {
                PanelFactory.RenderError("Invalid choice. Enter 0, 1, or 2.");
                continue;
            }
            break;
        }
        if (statusChoice == 0) return;

        Application1? app = null;
        foreach (Application1 a in DataStore.Applications)
        {
            if (a.ApplicationId == iv.ApplicationId)
            {
                app = a;
                break;
            }
        }

        JobListing? job = null;
        if (app != null)
        {
            foreach (JobListing j in DataStore.Jobs)
            {
                if (j.JobId == app.JobId)
                {
                    job = j;
                    break;
                }
            }
        }

        if (statusChoice == 1) // Mark Completed
        {
            string feedback = ConsoleHelper.ReadLine("Enter interview feedback/notes (optional)");
            
            // Invoke DB Stored Procedure for interview status sync
            int ivId = 0;
            if (DatabaseSync.IsEnabled && int.TryParse(iv.InterviewId, out ivId))
            {
                try
                {
                    using var conn = new Microsoft.Data.SqlClient.SqlConnection(System.IO.File.ReadAllText("appsettings.json").Contains("ConnectionStrings") ? 
                        System.Text.Json.JsonDocument.Parse(System.IO.File.ReadAllText("appsettings.json")).RootElement.GetProperty("ConnectionStrings").GetProperty("DefaultConnection").GetString() : "");
                    conn.Open();
                    using var cmd = new Microsoft.Data.SqlClient.SqlCommand("JPNS.usp_UpdateInterviewStatus", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@InterviewId", ivId);
                    cmd.Parameters.AddWithValue("@Status", "COMPLETED");
                    cmd.Parameters.AddWithValue("@Feedback", string.IsNullOrWhiteSpace(feedback) ? DBNull.Value : feedback);
                    cmd.ExecuteNonQuery();
                }
                catch {}
            }

            iv.Status = InterviewStatus.COMPLETED;

            int roundsRequired = job != null ? job.NumberOfRounds : 3;
            int completedCount = 0;
            foreach (Interview1 i in DataStore.Interviews)
            {
                if (i.ApplicationId == iv.ApplicationId && i.Status == InterviewStatus.COMPLETED)
                {
                    completedCount++;
                }
            }

            if (app != null)
            {
                // If the application is already HIRED (e.g. auto-hired at scheduling), just save and show info.
                if (app.Status == ApplicationStatus.HIRED)
                {
                    DataStore.SaveInterviews();
                    PanelFactory.RenderSuccess("Interview marked completed. Candidate is already HIRED — no further status change needed.");
                    return;
                }

                // Always transition to SHORTLISTED — UpdateStatus internally auto-promotes
                // to HIRED if CurrentRound reaches TotalRounds after advancing by 1.
                app.UpdateStatus("SHORTLISTED");
                DataStore.SaveApplications();

                if (app.Status == ApplicationStatus.HIRED)
                {
                    DataStore.NotificationService.Send(app.JobSeekerId, "Congratulations! You are Hired",
                        $"You have successfully cleared all rounds of interviews for '{job?.Title}' and have been hired!",
                        "APPLICATION_STATUS");
                    PanelFactory.RenderSuccess("Interview completed! Candidate cleared all rounds — automatically marked as HIRED.");
                }
                else
                {
                    DataStore.NotificationService.Send(app.JobSeekerId, "Interview Round Completed",
                        $"You have completed an interview round for '{job?.Title}' and have been shortlisted for the next round.",
                        "APPLICATION_STATUS");
                    var s = FileStorage.DataStore.FindJobSeekerById(app.JobSeekerId);
                    string roundInfo = s != null ? $"{s.CurrentRound}/{s.TotalRounds}" : $"{completedCount + 1}";
                    PanelFactory.RenderSuccess($"Interview completed! Round {roundInfo} — status reset to SHORTLISTED for next round.");
                }
            }
            DataStore.SaveInterviews();

        }
        else if (statusChoice == 2) // Mark Cancelled
        {
            int ivId = 0;
            if (DatabaseSync.IsEnabled && int.TryParse(iv.InterviewId, out ivId))
            {
                try
                {
                    using var conn = new Microsoft.Data.SqlClient.SqlConnection(System.IO.File.ReadAllText("appsettings.json").Contains("ConnectionStrings") ? 
                        System.Text.Json.JsonDocument.Parse(System.IO.File.ReadAllText("appsettings.json")).RootElement.GetProperty("ConnectionStrings").GetProperty("DefaultConnection").GetString() : "");
                    conn.Open();
                    using var cmd = new Microsoft.Data.SqlClient.SqlCommand("JPNS.usp_UpdateInterviewStatus", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@InterviewId", ivId);
                    cmd.Parameters.AddWithValue("@Status", "CANCELLED");
                    cmd.Parameters.AddWithValue("@Feedback", DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
                catch {}
            }

            iv.Status = InterviewStatus.CANCELLED;

            if (app != null)
            {
                app.UpdateStatus("SHORTLISTED");
                DataStore.SaveApplications();
                DataStore.NotificationService.Send(app.JobSeekerId, "Interview Cancelled",
                    $"Your interview for '{job?.Title}' scheduled on {iv.ScheduledAt:g} has been cancelled.",
                    "APPLICATION_STATUS");
            }
            DataStore.SaveInterviews();
            PanelFactory.RenderSuccess("Interview status set to CANCELLED.");
        }
    }

    private void HiringReport()
    {
        bool running = true;
        while (running)
        {
            ShowHeader("Hiring Report", "Dashboard > Analytics");
            string menuContent = 
                $"[bold {UITheme.Primary}]  1.[/]  [white]Hiring Funnel  (BarChart)[/]\n" + 
                $"[bold {UITheme.Secondary}]  2.[/]  [white]Job Analytics  (BarChart)[/]\n" +
                $"[bold {UITheme.Warning}]  3.[/]  [white]Export Reports[/]\n" + 
                $"[{UITheme.Dim}]  0.  Back[/]";
            AnsiConsole.Write(new Panel(menuContent).Header($"[bold {UITheme.Primary}]  Analytics Center  [/]")
                    .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Primary)).Padding(2, 0));
            AnsiConsole.WriteLine();
            int choice;
            while (true)
            {
                string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-3]]:[/]").PromptStyle(UITheme.Primary).AllowEmpty());
                if (!int.TryParse(raw.Trim(), out choice) || choice < 0 || choice > 3)
                {
                    PanelFactory.RenderError("Invalid choice. Enter 0, 1, 2, or 3.");
                    continue;
                }
                break;
            }
            switch (choice)
            {
                case 0:
                    running = false;
                    break;

                case 1:
                    {
                        List<Application1> apps = new List<Application1>();
                        foreach (Application1 app in DataStore.Applications)
                        {
                            bool belongsToAdmin = false;
                            foreach (JobListing job in DataStore.Jobs)
                            {
                                if (job.JobId == app.JobId && job.AdminId == _admin.AdminId)
                                {
                                    belongsToAdmin = true;
                                    break;
                                }
                            }

                            if (belongsToAdmin)
                            {
                                apps.Add(app);
                            }
                        }

                        var report = ReportManager.HiringReport(apps, _admin.AdminId);
                        ReportManager.DisplayReport(report);
                        int pending = 0;
                        int shortlisted = 0;
                        int interviewScheduled = 0;
                        int hired = 0;
                        int rejected = 0;
                        foreach (Application1 app in apps)
                        {
                            if (app.Status == ApplicationStatus.PENDING)
                            {
                                pending++;
                            }

                            if (app.Status == ApplicationStatus.SHORTLISTED)
                            {
                                shortlisted++;
                            }

                            if (app.Status == ApplicationStatus.INTERVIEW_SCHEDULED)
                            {
                                interviewScheduled++;
                            }

                            if (app.Status == ApplicationStatus.HIRED)
                            {
                                hired++;
                            }

                            if (app.Status == ApplicationStatus.REJECTED)
                            {
                                rejected++;
                            }
                        }

                        ChartRenderer.RenderHiringFunnel(pending, shortlisted, interviewScheduled, hired);
                        ChartRenderer.RenderApplicationBreakdown(pending, shortlisted, interviewScheduled, hired, rejected);
                        ConsoleHelper.Pause();
                        break;
                    }

                case 2:
                    {
                        List<JobListing> jobs = new List<JobListing>();
                        foreach (JobListing job in DataStore.Jobs)
                        {
                            if (job.AdminId == _admin.AdminId)
                            {
                                jobs.Add(job);
                            }
                        }
                        var report1 = ReportManager.JobAnalytics(jobs, _admin.AdminId);
                        ReportManager.DisplayReport(report1);
                        int activeJobs = 0;
                        int pendingJobs = 0;
                        int closedJobs = 0;
                        int expiredJobs = 0;
                        foreach (JobListing job in jobs)
                        {
                            if (job.IsActive && !job.IsExpired())
                            {
                                activeJobs++;
                            }

                            if (job.Status == JobStatus.PENDING_APPROVAL)
                            {
                                pendingJobs++;
                            }

                            if (job.Status == JobStatus.CLOSED)
                            {
                                closedJobs++;
                            }

                            if (job.Status == JobStatus.EXPIRED)
                            {
                                expiredJobs++;
                            }
                        }
                        ChartRenderer.RenderJobAnalytics(activeJobs, pendingJobs, closedJobs, expiredJobs);
                        ConsoleHelper.Pause();
                        break;
                    }

                case 3:
                    {
                        RunReportExportFlow();
                        ConsoleHelper.Pause();
                        break;
                    }
            }
        }
    }

    private void RunReportExportFlow()
    {
        ShowHeader("Export Reports", "Dashboard > Analytics > Export");
        var reportChoiceMenu = 
            $"[bold {UITheme.Primary}]  1.[/]  [white]Hiring Funnel Report[/]\n" +
            $"[bold {UITheme.Secondary}]  2.[/]  [white]Job Analytics Report[/]\n" +
            $"[{UITheme.Dim}]  0.  Back[/]";
        AnsiConsole.Write(new Panel(reportChoiceMenu).Header($"[bold {UITheme.Primary}]  Select Report  [/]")
                .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Primary)).Padding(2, 0));
        AnsiConsole.WriteLine();
        int repChoice;
        while (true)
        {
            string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-2]]:[/]").PromptStyle(UITheme.Primary).AllowEmpty());
            if (!int.TryParse(raw.Trim(), out repChoice) || repChoice < 0 || repChoice > 2)
            {
                PanelFactory.RenderError("Invalid choice. Enter 0, 1, or 2.");
                continue;
            }
            break;
        }
        if (repChoice == 0) return;

        ReportDTO report;
        string baseName;
        if (repChoice == 1)
        {
            List<Application1> apps = new List<Application1>();
            foreach (Application1 app in DataStore.Applications)
            {
                bool belongsToAdmin = false;
                foreach (JobListing job in DataStore.Jobs)
                {
                    if (job.JobId == app.JobId && job.AdminId == _admin.AdminId)
                    {
                        belongsToAdmin = true;
                        break;
                    }
                }
                if (belongsToAdmin) apps.Add(app);
            }
            report = ReportManager.HiringReport(apps, _admin.AdminId);
            baseName = "hiring_report";
        }
        else
        {
            List<JobListing> jobs = new List<JobListing>();
            foreach (JobListing job in DataStore.Jobs)
            {
                if (job.AdminId == _admin.AdminId) jobs.Add(job);
            }
            report = ReportManager.JobAnalytics(jobs, _admin.AdminId);
            baseName = "job_analytics";
        }

        AnsiConsole.WriteLine();
        ReportManager.RenderPreview(report);
        AnsiConsole.WriteLine();

        var formatMenu =
            $"[bold {UITheme.Primary}]  1.[/]  [white]CSV (Comma Separated Values)[/]\n" +
            $"[bold {UITheme.Secondary}]  2.[/]  [white]TXT (Plain Text)[/]\n" +
            $"[bold {UITheme.Info}]  3.[/]  [white]DOCX (Word Document)[/]\n" +
            $"[bold {UITheme.Warning}]  4.[/]  [white]PDF (Portable Document Format)[/]\n" +
            $"[{UITheme.Dim}]  0.  Cancel[/]";
        AnsiConsole.Write(new Panel(formatMenu).Header($"[bold {UITheme.Primary}]  Select Format  [/]")
                .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Primary)).Padding(2, 0));
        AnsiConsole.WriteLine();
        int fmtChoice;
        while (true)
        {
            string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-4]]:[/]").PromptStyle(UITheme.Primary).AllowEmpty());
            if (!int.TryParse(raw.Trim(), out fmtChoice) || fmtChoice < 0 || fmtChoice > 4)
            {
                PanelFactory.RenderError("Invalid choice. Enter 0 to 4.");
                continue;
            }
            break;
        }
        if (fmtChoice == 0) return;

        string ext = fmtChoice switch
        {
            1 => "csv",
            2 => "txt",
            3 => "docx",
            4 => "pdf",
            _ => "csv"
        };

        string folder = @"D:\Synergech\Project\JPNS_FINAL\Files";
        string filePath = Path.Combine(folder, $"{baseName}_{DateTime.Now:yyyyMMddHHmmss}.{ext}");

        if (ConsoleHelper.Confirm($"Export report to: {filePath}?"))
        {
            try
            {
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                switch (fmtChoice)
                {
                    case 1:
                        ReportManager.ExportToCsv(report, filePath);
                        break;
                    case 2:
                        ReportManager.ExportToTxt(report, filePath);
                        break;
                    case 3:
                        ReportManager.ExportToDocx(report, filePath);
                        break;
                    case 4:
                        ReportManager.ExportToPdf(report, filePath);
                        break;
                }
                PanelFactory.RenderSuccess($"Report successfully exported! Location: {filePath}");
            }
            catch (Exception ex)
            {
                PanelFactory.RenderError($"Export failed: {ex.Message}");
            }
        }
    }

    private void MyProfile()
    {
        bool running = true;

        while (running)
        {
            ShowHeader("My Profile", "Dashboard > My Profile");

            PanelFactory.RenderInfoCard(
                "Admin Profile",
                new[]
                {
                ("Name", _admin.Name),
                ("Email", _admin.Email),
                ("Company", _admin.Company),
                ("Admin ID", _admin.AdminId),
                ("Approved", _admin.IsApproved ? "Yes ✓" : "Pending")
                },
                UITheme.Admin);

            AnsiConsole.WriteLine();

            string menuContent =
                $"[bold {UITheme.Primary}]  1.[/]  [white]Update Name[/]\n" +
                $"[bold {UITheme.Employer}]  2.[/]  [white]Update Company[/]\n" +
                $"[bold {UITheme.Warning}]  3.[/]  [white]Change Password[/]\n" +
                $"[{UITheme.Dim}]  0.  Back[/]";

            AnsiConsole.Write(new Panel(menuContent).Header($"[bold {UITheme.Admin}]  Profile Actions  [/]")
                    .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Admin)).Padding(2, 0));
            AnsiConsole.WriteLine();
            int choice;
            while (true)
            {
                string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-3]]:[/]").PromptStyle(UITheme.Admin).AllowEmpty());
                if (!int.TryParse(raw.Trim(), out choice) || choice < 0 || choice > 3)
                {
                    PanelFactory.RenderError("Invalid choice. Enter 0, 1, 2, or 3.");
                    continue;
                }
                break;
            }
            switch (choice)
            {
                case 0:
                    running = false;
                    break;

                case 1:
                    {
                        string name = ConsoleHelper.ReadNonEmpty("New name");
                        if (ConsoleHelper.Confirm("Update name?"))
                        {
                            _admin.UpdateName(name);
                            DataStore.SaveAdmins();
                            PanelFactory.RenderSuccess("Name updated.");
                        }
                        break;
                    }

                case 2:
                    {
                        string company = ConsoleHelper.ReadNonEmpty("New company name");
                        if (company.Trim().ToUpperInvariant() != "JPNS")
                        {
                            PanelFactory.RenderError("Only the company 'JPNS' is supported.");
                            ConsoleHelper.Pause();
                            break;
                        }
                        if (ConsoleHelper.Confirm("Update company name?"))
                        {
                            _admin.UpdateCompany(company);
                            DataStore.SaveAdmins();
                            PanelFactory.RenderSuccess("Company updated.");
                        }
                        break;
                    }

                case 3:
                    {
                        string newPassword = ConsoleHelper.ReadValidatedPassword("New password");
                        if (ConsoleHelper.Confirm("Update password?"))
                        {
                            ConsoleHelper.ReadConfirmPassword(newPassword);
                            _admin.UpdatePassword(newPassword);
                            DataStore.SaveAdmins();
                            PanelFactory.RenderSuccess("Password changed.");
                        }
                        break;
                    }
            }

            ConsoleHelper.Pause();
        }
    }
}