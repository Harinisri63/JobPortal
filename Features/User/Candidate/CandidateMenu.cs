using JobPortal.Features.Interview;
using JobPortal.Shared.Exceptions;
using JobPortal.Features.JobPosting;
using JobPortal.FileStorage;
using JobPortal.UI;
using AppModel = JobPortal.Features.Application.Application1;
using JobPortal.Features.Application;
using JobPortal.Features.User.Candidate;
using Spectre.Console;

namespace JobPortal.Features.Candidate;

internal class CandidateMenu
{
    private readonly JobSeeker _seeker;
    public CandidateMenu(JobSeeker seeker) => _seeker = seeker;
    private void ShowHeader(string module, string breadcrumb)
    {
        Console.Clear();
        PanelFactory.RenderScreenHeader(module, breadcrumb, _seeker.Name, "CANDIDATE");
    }

    public void Show()
    {
        bool running = true;
        while (running)
        {
            ShowHeader("Candidate Dashboard",$"Dashboard > {_seeker.Name}");
            int unread =DataStore.NotificationService.GetPendingCount(_seeker.SeekerId);
            int applied = 0;
            foreach (Application1 application in DataStore.Applications)
            {
                if (application.JobSeekerId == _seeker.SeekerId)
                {
                    applied++;
                }
            }
            int saved = _seeker.SavedJobs.Count;
            bool hasResume =!string.IsNullOrWhiteSpace(_seeker.ResumeUrl);
            HashSet<string> myAppIds =new HashSet<string>();
            foreach (Application1 application in DataStore.Applications)
            {
                if (application.JobSeekerId == _seeker.SeekerId)
                {
                    myAppIds.Add(
                        application.ApplicationId);
                }
            }
            int interviews = 0;
            foreach (Interview1 interview in DataStore.InterviewScheduler.GetAll())
            {
                if (myAppIds.Contains(interview.ApplicationId))
                {
                    interviews++;
                }
            }

            DashboardRenderer.RenderCandidateDashboard(_seeker.Name,unread,applied,saved,hasResume,interviews,_seeker.SeekerId);
            string menuContent =
                $"[bold {UITheme.Primary}]  1.[/]  [white]Browse & Search Jobs[/]\n" +
                $"[bold {UITheme.Candidate}]  2.[/]  [white]Apply for Job[/]\n" +
                $"[bold {UITheme.Secondary}]  3.[/]  [white]View Applied Jobs[/]\n" +
                $"[bold {UITheme.Secondary}]  4.[/]  [white]Saved Jobs[/]\n" +
                $"[bold {UITheme.Info}]  5.[/]  [white]My Profile[/]\n" +
                $"[bold {UITheme.Warning}]  6.[/]  [white]Upload Resume[/]\n" +
                $"[bold {UITheme.Admin}]  7.[/]  [white]Interview Schedule[/]\n" +
                $"[bold {(unread > 0 ? UITheme.Warning : UITheme.Neutral)}]  8.[/]  [white]Notifications[/]" +
                (unread > 0? $"  [bold {UITheme.Warning}][[{unread} NEW]][/]": "") +"\n" +
                $"[bold {UITheme.Info}]  9.[/]  [white]Complaints[/]\n" +
                $"[{UITheme.Dim}]  0.  Logout[/]";
            AnsiConsole.Write(new Panel(menuContent).Header($"[bold {UITheme.Candidate}]  Candidate Hub  [/]").Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse(UITheme.Candidate)).Padding(2, 0));
            PanelFactory.RenderFooter(_seeker.Name);
            AnsiConsole.WriteLine();
            int choice;
            while (true)
            {
                string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-9]]:[/]").PromptStyle(UITheme.Candidate).AllowEmpty());
                if (!int.TryParse(raw.Trim(),out choice) ||choice < 0 ||choice > 9)
                {
                    PanelFactory.RenderError("Invalid choice. Enter 0 to 9.");
                    continue;
                }

                break;
            }

            switch (choice)
            {
                case 1:
                    BrowseJobs();
                    break;

                case 2:
                    ApplyForJob();
                    break;

                case 3:
                    ViewAppliedJobs();
                    break;

                case 4:
                    ShowSavedJobs();
                    break;

                case 5:
                    MyProfile();
                    break;

                case 6:
                    UploadResume();
                    break;

                case 7:
                    InterviewSchedule();
                    break;

                case 8:
                    ShowNotifications();
                    break;

                case 9:
                    ShowComplaints();
                    break;

                case 0:
                    running = false;
                    break;
            }
        }

        PanelFactory.RenderSuccess($"Goodbye, {_seeker.Name}! Session ended.");
    }

    private void BrowseJobs()
    {
        try
        {
            bool running = true;
            while (running)
            {
                ShowHeader("Browse & Search Jobs", "Dashboard > Browse Jobs");

                var menuContent =
                    $"[bold {UITheme.Primary}]  1.[/]  [white]Keyword Search[/]\n" +
                    $"[bold {UITheme.Primary}]  2.[/]  [white]By Location[/]\n" +
                    $"[bold {UITheme.Primary}]  3.[/]  [white]By Salary Range[/]\n" +
                    $"[bold {UITheme.Primary}]  4.[/]  [white]By Skills[/]\n" +
                    $"[bold {UITheme.Primary}]  5.[/]  [white]By Experience[/]\n" +
                    $"[bold {UITheme.Primary}]  6.[/]  [white]By Company[/]\n" +
                    $"[bold {UITheme.Candidate}]  7.[/]  [white]View All Jobs[/]\n" +
                    $"[{UITheme.Dim}]  0.  Back[/]";

                AnsiConsole.Write(new Panel(menuContent).Header($"[bold {UITheme.Primary}]  Job Search  [/]")
                        .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Primary)).Padding(2, 0));
                AnsiConsole.WriteLine();
                int choice;
                while (true)
                {
                    string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-7]]:[/]").PromptStyle(UITheme.Primary).AllowEmpty());
                    if (!int.TryParse(raw.Trim(), out choice) || choice < 0 || choice > 7)
                    {
                        PanelFactory.RenderError("Invalid choice. Enter 0 to 7.");
                        continue;
                    }
                    break;
                }

                List<JobListing>? results = null;
                switch (choice)
                {
                    case 0: running = false; break;
                    case 1: results = SearchByKeyword();     break;
                    case 2: results = SearchByLocation();    break;
                    case 3: results = SearchBySalary();      break;
                    case 4: results = SearchBySkills();      break;
                    case 5: results = SearchByExperience();  break;
                    case 6: results = SearchByCompany();     break;
                    default: results = JobSearch.SearchJobs(DataStore.Jobs); break;
                }
                if (results != null)
                    DisplayJobList(results, allowSave: true, allowApply: true);
            }
        }
        catch (ValidationException ex)
        {
            PanelFactory.RenderError(ex.Message); ConsoleHelper.Pause();
        }
        catch (JPNSException ex)
        {
            PanelFactory.RenderError(ex.Message); ConsoleHelper.Pause();
        }
        catch (Exception)
        {
            PanelFactory.RenderError("Unexpected error. Please try again."); ConsoleHelper.Pause();
        }
    }

    private List<JobListing> SearchByKeyword()
    {
        string keyword = ConsoleHelper.ReadNonEmpty("Enter keyword");
        return JobSearch.SearchJobs(DataStore.Jobs, keyword);
    }

    private List<JobListing> SearchByLocation()
    {
        string loc = ConsoleHelper.ReadNonEmpty("Enter location");
        return JobSearch.FilterByLocation(DataStore.Jobs, loc);
    }

    private List<JobListing> SearchBySalary()
    {
        decimal min = ConsoleHelper.ReadPositiveDecimal("Min salary (LPA)");
        decimal max;
        while (true)
        {
            max = ConsoleHelper.ReadPositiveDecimal("Max salary (LPA)");
            if (max >= min) break;
            PanelFactory.RenderError("Max salary must be >= Min salary.");
        }
        var range = new Shared.Structs.SalaryRange(min, max);
        return JobSearch.SearchJobs(DataStore.Jobs, "", range);
    }

    private List<JobListing> SearchBySkills()
    {
        string raw = ConsoleHelper.ReadNonEmpty("Enter skills (comma-separated)");
        string[] skillArray = raw.Split(',');
        List<string> skills = new List<string>();
        foreach (string skill in skillArray)
        {
            string trimmedSkill = skill.Trim();
            if (trimmedSkill.Length > 0)
            {
                skills.Add(trimmedSkill);
            }
        }
        SkillsFilter filter = new SkillsFilter(skills);
        return filter.FilterJobs(DataStore.Jobs);
    }

    private List<JobListing> SearchByExperience()
    {
        int exp = ConsoleHelper.ReadPositiveInt("Max years of experience");
        return JobSearch.FilterByExperience(DataStore.Jobs, exp);
    }

    private List<JobListing> SearchByCompany()
    {
        string company = ConsoleHelper.ReadNonEmpty("Enter company name");
        return JobSearch.FilterByCompany(DataStore.Jobs, company);
    }

    private void DisplayJobList(List<JobListing> jobs, bool allowSave = false, bool allowApply = false)
    {
        if (jobs.Count == 0)
        {
            PanelFactory.RenderWarning("No jobs found matching your criteria.");
            ConsoleHelper.Pause();
            return;
        }

        int pageSize = 5;
        int pageNumber = 1;
        while (true)
        {
            ShowHeader("Browse & Search Jobs", "Dashboard > Browse Jobs");
            int totalPages = (int)Math.Ceiling((double)jobs.Count / pageSize);
            if (totalPages == 0) totalPages = 1;

            int start = (pageNumber - 1) * pageSize;
            int end = Math.Min(start + pageSize, jobs.Count);

            var pageJobs = new List<JobListing>();
            for (int i = start; i < end; i++)
            {
                pageJobs.Add(jobs[i]);
            }

            AnsiConsole.WriteLine();
            PanelFactory.RenderSectionTitle($"Search Results  ({jobs.Count} jobs found - Page {pageNumber} of {totalPages})", "◈");
            TableFactory.RenderJobTable(pageJobs, showStatus: false, startIndex: start);
            AnsiConsole.WriteLine();
            Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back");
            Console.WriteLine();

            Console.Write("Enter job number to view details (or page navigation command): ");
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
                if (sel >= 1 && sel <= jobs.Count)
                {
                    var selected = jobs[sel - 1];
                    PanelFactory.RenderSectionTitle("Job Details");
                    selected.Display();
                    AnsiConsole.WriteLine();

                    if (allowApply || allowSave)
                    {
                        var actionMenu =
                            $"[bold {UITheme.Candidate}]  1.[/]  [white]Apply Now[/]\n" +
                            $"[bold {UITheme.Secondary}]  2.[/]  [white]Save to Watchlist[/]\n" +
                            $"[{UITheme.Dim}]  0.  Back[/]";

                        AnsiConsole.Write(new Panel(actionMenu).Header($"[bold {UITheme.Primary}]  Job Actions  [/]")
                                .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Primary)).Padding(2, 0));
                        AnsiConsole.WriteLine();
                        int action;
                        while (true)
                        {
                            string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-2]]:[/]").PromptStyle(UITheme.Primary).AllowEmpty());
                            if (!int.TryParse(raw.Trim(), out action) || action < 0 || action > 2)
                            {
                                PanelFactory.RenderError("Invalid choice. Enter 0, 1, or 2.");
                                continue;
                            }
                            break;
                        }
                        if (action == 1)
                        {
                            try
                            {
                                SubmitApplication(selected);
                            }
                            catch (JPNSException ex)
                            {
                                PanelFactory.RenderError(ex.Message);
                            }
                        }
                        if (action == 2) SaveJob(selected.JobId);
                    }
                    ConsoleHelper.Pause();
                }
                else
                {
                    Console.WriteLine($"Invalid selection. Enter a number between 1 and {jobs.Count}. Press any key...");
                    Console.ReadKey(true);
                }
            }
        }
    }

    private void ApplyForJob()
    {
        try
        {
            ShowHeader("Apply for Job", "Dashboard > Apply");
            AnsiConsole.MarkupLine($"  [{UITheme.Dim}]Enter a Job ID directly, or press Enter to browse all open jobs.[/]");
            AnsiConsole.WriteLine();

            string jobId = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.White}]Job ID (or Enter to browse):[/]").PromptStyle(UITheme.Primary).AllowEmpty());
            jobId = jobId.Trim();
            if (string.IsNullOrWhiteSpace(jobId))
            {
                var list = JobSearch.SearchJobs(DataStore.Jobs);
                DisplayJobList(list, allowApply: true);
                return;
            }
            JobListing? job = null;
            foreach (JobListing j in DataStore.Jobs)
            {
                if (j.JobId == jobId)
                {
                    job = j;
                    break;
                }
            }
            if (job == null)
                throw new JobNotAvailableException("Job not found with that ID.");

            PanelFactory.RenderSectionTitle("Job Details");
            job.Display();
            AnsiConsole.WriteLine();

            if (ConsoleHelper.Confirm("Apply for this job?"))
            {
                try
                {
                    SubmitApplication(job);
                }
                catch (JPNSException ex)
                {
                    PanelFactory.RenderError(ex.Message);
                    ConsoleHelper.Pause();
                }
            }
        }
        catch (ValidationException ex)
        {
            PanelFactory.RenderError(ex.Message); ConsoleHelper.Pause();
        }
        catch (DuplicateApplicationException ex)
        {
            PanelFactory.RenderError(ex.Message); ConsoleHelper.Pause();
        }
        catch (JobExpiredException ex)
        {
            PanelFactory.RenderError(ex.Message); ConsoleHelper.Pause();
        }
        catch (ResumeNotUploadedException ex)
        {
            PanelFactory.RenderError(ex.Message);
            if (ConsoleHelper.Confirm("Upload a resume now?"))
            {
                UploadResume();
                if (!string.IsNullOrWhiteSpace(_seeker.ResumeUrl))
                {
                    ApplyForJob();
                }
                return;
            }
            ConsoleHelper.PrintInfo("Returning to menu...");
        }
        catch (JPNSException ex)
        {
            PanelFactory.RenderError(ex.Message); ConsoleHelper.Pause();
        }
        catch (Exception)
        {
            PanelFactory.RenderError("Unexpected error. Try again."); ConsoleHelper.Pause();
        }
    }

    private void SubmitApplication(JobListing job)
    {
        if (!_seeker.IsActive)
            throw new JPNSException("Your account is not active. Please contact support.");
        if (string.IsNullOrWhiteSpace(_seeker.ResumeUrl))
        {
            PanelFactory.RenderError("Please upload your resume before applying.");
            if (ConsoleHelper.Confirm("Do you want to upload your resume now?"))
            {
                UploadResume();
                if (string.IsNullOrWhiteSpace(_seeker.ResumeUrl))
                {
                    throw new ResumeNotUploadedException();
                }
            }
            else
            {
                throw new ResumeNotUploadedException();
            }
        }
        if (job.IsExpired())
            throw new JobExpiredException();
        if (!job.IsActive)
            throw new JobNotAvailableException();

        // Terms and conditions policy check
        ShowHeader("Terms and Conditions", "Dashboard > Apply > Policy");
        
        string termsText = 
            "[bold white]By using this Job Portal, you agree to:[/]\n\n" +
            "  • Provide accurate information.\n" +
            "  • Keep your account credentials secure.\n" +
            "  • Use the platform only for lawful job-search and recruitment purposes.\n" +
            "  • Not post fraudulent jobs or fake profiles.\n" +
            "  • Respect the privacy of other users.\n" +
            "  • Accept that the portal does not guarantee employment or hiring.\n\n" +
            "[bold yellow]Violation of these terms may result in account suspension or removal.[/]\n";

        AnsiConsole.Write(new Panel(termsText)
            .Header("[bold yellow] Platform Policy [/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse(UITheme.Warning))
            .Padding(2, 1));
        AnsiConsole.WriteLine();

        if (!ConsoleHelper.Confirm("Do you accept these terms and conditions?"))
        {
            throw new JPNSException("You must accept the terms and conditions to apply for jobs.");
        }

        bool alreadyApplied = false;

        foreach (Application1 application in DataStore.Applications)
        {
            if (application.JobSeekerId == _seeker.SeekerId &&
                application.JobId == job.JobId)
            {
                alreadyApplied = true;
                break;
            }
        }
        if (alreadyApplied)
            throw new DuplicateApplicationException();

        string cover = ConsoleHelper.ReadLine("Cover letter (optional — press Enter to skip)");
        var app = new AppModel(_seeker.SeekerId, job.JobId, _seeker.ResumeUrl, cover);
        DataStore.Applications.Add(app);
        DataStore.SaveApplications();

        DataStore.NotificationService.Send(_seeker.SeekerId, "Application Submitted",
            $"Your application for '{job.Title}' has been submitted. Application ID: {app.ApplicationId}",
            "APPLICATION_STATUS");
        PanelFactory.RenderSuccess($"Application submitted! ID: {app.ApplicationId}");
        AnsiConsole.WriteLine();
        StatusRenderer.RenderApplicationPipeline("PENDING");
    }

    private void SaveJob(string jobId)
    {
        try
        {
            _seeker.SaveJob(jobId);
            DataStore.SaveJobSeekers();
            PanelFactory.RenderSuccess("Job saved to your watchlist.");
        }
        catch (JPNSException ex)
        {
            PanelFactory.RenderError(ex.Message);
        }
    }

    private void ViewAppliedJobs()
    {
        try
        {
            ShowHeader("My Applications","Dashboard > My Applications");
            List<AppModel> apps =new List<AppModel>();
            foreach (AppModel app in DataStore.Applications)
            {
                if (app.JobSeekerId ==_seeker.SeekerId)
                {
                    apps.Add(app);
                }
            }

            if (apps.Count == 0)
            {
                PanelFactory.RenderInfo("No applications yet. Start browsing jobs!");
                ConsoleHelper.Pause();
                return;
            }

            int pageSize = 5;
            int pageNumber = 1;
            while (true)
            {
                ShowHeader("My Applications", "Dashboard > My Applications");
                int totalPages = (int)Math.Ceiling((double)apps.Count / pageSize);
                if (totalPages == 0) totalPages = 1;

                int start = (pageNumber - 1) * pageSize;
                int end = Math.Min(start + pageSize, apps.Count);

                var pageApps = new List<AppModel>();
                for (int i = start; i < end; i++)
                {
                    pageApps.Add(apps[i]);
                }

                AnsiConsole.WriteLine();
                PanelFactory.RenderSectionTitle($"My Applications  ({apps.Count} total - Page {pageNumber} of {totalPages})", "▶");
                TableFactory.RenderApplicationTable(pageApps, DataStore.Jobs, startIndex: start);
                AnsiConsole.WriteLine();
                Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back");
                Console.WriteLine();

                Console.Write("Enter application number to view pipeline details (or page navigation command): ");
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
                    if (sel >= 1 && sel <= apps.Count)
                    {
                        AppModel selected = apps[sel - 1];
                        JobListing? selJob = null;
                        foreach (JobListing job in DataStore.Jobs)
                        {
                            if (job.JobId == selected.JobId)
                            {
                                selJob = job;
                                break;
                            }
                        }
                        PanelFactory.RenderSectionTitle("Application Detail");
                        selected.Display(selJob != null ? selJob.Title : "", selJob != null ? selJob.Company : "");
                        AnsiConsole.WriteLine();
                        StatusRenderer.RenderApplicationPipeline(selected.Status.ToString());
                        ConsoleHelper.Pause();
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid selection. Enter a number between 1 and {apps.Count}. Press any key...");
                        Console.ReadKey(true);
                    }
                }
            }
        }
        catch (JPNSException ex)
        {
            PanelFactory.RenderError(ex.Message);
            ConsoleHelper.Pause();
        }
        catch (Exception)
        {
            PanelFactory.RenderError("Unable to retrieve applications.");
            ConsoleHelper.Pause();
        }
    }

    private void ShowSavedJobs()
    {
        try
        {
            int pageSize = 5;
            int pageNumber = 1;
            while (true)
            {
                ShowHeader("Saved Jobs", "Dashboard > Watchlist");
                
                List<JobListing> savedList = new List<JobListing>();
                foreach (JobListing job in DataStore.Jobs)
                {
                    if (_seeker.SavedJobs.Contains(job.JobId))
                    {
                        savedList.Add(job);
                    }
                }

                if (savedList.Count == 0)
                {
                    PanelFactory.RenderInfo("No saved jobs. Browse jobs and save ones you like!");
                    ConsoleHelper.Pause();
                    return;
                }

                int totalPages = (int)Math.Ceiling((double)savedList.Count / pageSize);
                if (totalPages == 0) totalPages = 1;
                if (pageNumber > totalPages) pageNumber = totalPages;

                int start = (pageNumber - 1) * pageSize;
                int end = Math.Min(start + pageSize, savedList.Count);

                var pageJobs = new List<JobListing>();
                for (int i = start; i < end; i++)
                {
                    pageJobs.Add(savedList[i]);
                }

                PanelFactory.RenderSectionTitle($"Your Watchlist  ({savedList.Count} jobs - Page {pageNumber} of {totalPages})", "★");
                TableFactory.RenderSavedJobTable(pageJobs, startIndex: start);
                AnsiConsole.WriteLine();

                string menuContent =
                    $"[bold {UITheme.Primary}]  1.[/]  [white]Remove a saved job[/]\n" +
                    $"[bold {UITheme.Primary}]  2.[/]  [white]Next Page[/]\n" +
                    $"[bold {UITheme.Primary}]  3.[/]  [white]Previous Page[/]\n" +
                    $"[{UITheme.Dim}]  0.  Back[/]";

                AnsiConsole.Write(new Panel(menuContent).Header($"[bold {UITheme.Secondary}]  Watchlist Actions  [/]")
                        .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Secondary)).Padding(2, 0));
                AnsiConsole.WriteLine();

                int action;
                while (true)
                {
                    string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-3]]:[/]").PromptStyle(UITheme.Secondary).AllowEmpty());
                    if (!int.TryParse(raw.Trim(), out action) || action < 0 || action > 3)
                    {
                        PanelFactory.RenderError("Invalid choice. Enter 0 to 3.");
                        continue;
                    }
                    break;
                }

                if (action == 0) return;
                else if (action == 2)
                {
                    if (pageNumber < totalPages)
                        pageNumber++;
                    else
                    {
                        Console.WriteLine("You are already on the last page. Press any key...");
                        Console.ReadKey(true);
                    }
                }
                else if (action == 3)
                {
                    if (pageNumber > 1)
                        pageNumber--;
                    else
                    {
                        Console.WriteLine("You are already on the first page. Press any key...");
                        Console.ReadKey(true);
                    }
                }
                else if (action == 1)
                {
                    AnsiConsole.MarkupLine($"  [{UITheme.Dim}]Enter job number to remove:[/]");
                    int sel;
                    while (true)
                    {
                        string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Job #:[/]").PromptStyle(UITheme.Error).AllowEmpty());
                        if (!int.TryParse(raw.Trim(), out sel) || sel < 1 || sel > savedList.Count)
                        {
                            PanelFactory.RenderError($"Enter a number between 1 and {savedList.Count}.");
                            continue;
                        }
                        break;
                    }
                    if (ConsoleHelper.Confirm($"Remove '{savedList[sel - 1].Title}' from watchlist?"))
                    {
                        _seeker.RemoveSavedJob(savedList[sel - 1].JobId);
                        DataStore.SaveJobSeekers();
                        PanelFactory.RenderSuccess("Job removed from watchlist.");
                        ConsoleHelper.Pause();
                    }
                }
            }
        }
        catch (JPNSException ex)
        {
            PanelFactory.RenderError(ex.Message);
            ConsoleHelper.Pause();
        }
    }

    private void MyProfile()
    {
        try
        {
            bool running = true;
            while (running)
            {
                ShowHeader("My Profile", "Dashboard > My Profile");
                PanelFactory.RenderInfoCard("Candidate Profile", new[]
                {
                    ("Seeker ID",   _seeker.SeekerId),
                    ("Name",        _seeker.Name),
                    ("Email",       _seeker.Email),
                    ("Phone",       _seeker.Phone),
                    ("Location",    string.IsNullOrWhiteSpace(_seeker.Location) ? "Not set" : _seeker.Location),
                    ("Skills",      string.IsNullOrWhiteSpace(_seeker.Skills)   ? "Not set" : _seeker.Skills),
                    ("Education",   string.IsNullOrWhiteSpace(_seeker.Education) ? "Not set" : _seeker.Education),
                    ("Experience",  string.IsNullOrWhiteSpace(_seeker.Experience) ? "Not set" : _seeker.Experience),
                    ("Resume",      string.IsNullOrWhiteSpace(_seeker.ResumeUrl) ? "Not uploaded" : _seeker.ResumeUrl),
                }, UITheme.Candidate);

                AnsiConsole.WriteLine();
                var menuContent =
                    $"[bold {UITheme.Primary}]  1.[/]  [white]Update Name[/]\n" +
                    $"[bold {UITheme.Primary}]  2.[/]  [white]Update Location[/]\n" +
                    $"[bold {UITheme.Primary}]  3.[/]  [white]Update Phone[/]\n" +
                    $"[bold {UITheme.Secondary}]  4.[/]  [white]Update Skills Summary[/]\n" +
                    $"[bold {UITheme.Secondary}]  5.[/]  [white]Update Education Summary[/]\n" +
                    $"[bold {UITheme.Secondary}]  6.[/]  [white]Update Experience Summary[/]\n" +
                    $"[bold {UITheme.Warning}]  7.[/]  [white]Change Password[/]\n" +
                    $"[bold {UITheme.Info}]  8.[/]  [white]Manage Projects[/]\n" +
                    $"[bold {UITheme.Info}]  9.[/]  [white]Manage Certificates[/]\n" +
                    $"[bold {UITheme.Info}]  10.[/]  [white]Manage Detailed Education[/]\n" +
                    $"[bold {UITheme.Info}]  11.[/]  [white]Manage Detailed Work Experience[/]\n" +
                    $"[bold {UITheme.Info}]  12.[/]  [white]Manage Detailed Skills[/]\n" +
                    $"[bold {UITheme.Info}]  13.[/]  [white]Give Company Feedback[/]\n" +
                    $"[{UITheme.Dim}]  0.  Back[/]";

                AnsiConsole.Write(new Panel(menuContent).Header($"[bold {UITheme.Candidate}]  Profile Actions  [/]")
                        .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Candidate)).Padding(2, 0));
                AnsiConsole.WriteLine();
                int choice;
                while (true)
                {
                    string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-13]]:[/]").PromptStyle(UITheme.Candidate).AllowEmpty());
                    if (!int.TryParse(raw.Trim(), out choice) || choice < 0 || choice > 13)
                    {
                        PanelFactory.RenderError("Invalid choice. Enter 0 to 13.");
                        continue;
                    }
                    break;
                }

                switch (choice)
                {
                    case 0: running = false; break;
                    case 1:
                        string name = ConsoleHelper.ReadNonEmpty("New name");
                        if (name == "0") continue;
                        if (ConsoleHelper.Confirm("Update name?"))
                        {
                            _seeker.UpdateName(name);
                            DataStore.SaveJobSeekers();
                            PanelFactory.RenderSuccess("Name updated.");
                        }
                        break;
                    case 2:
                        string loc = ConsoleHelper.ReadNonEmpty("New location");
                        if (loc == "0") continue;
                        if (ConsoleHelper.Confirm("Update location?"))
                        {
                            _seeker.UpdateLocation(loc);
                            DataStore.SaveJobSeekers();
                            PanelFactory.RenderSuccess("Location updated.");
                        }
                        break;
                    case 3:
                            string phone;

                            while (true)
                            {
                                phone = ConsoleHelper.ReadPhone("New phone");

                                bool phoneExists = false;

                                foreach (JobSeeker seeker in DataStore.JobSeekers)
                                {
                                    if (seeker.SeekerId != _seeker.SeekerId &&
                                        seeker.Phone == phone)
                                    {
                                        phoneExists = true;
                                        break;
                                    }
                                }

                                if (phoneExists)
                                {
                                    PanelFactory.RenderError("This phone number is already in use.");
                                    continue;
                                }

                                break;
                            }

                            if (ConsoleHelper.Confirm("Update phone number?"))
                            {
                                _seeker.UpdatePhone(phone);
                                DataStore.SaveJobSeekers();
                                PanelFactory.RenderSuccess("Phone updated.");
                            }
                            break;
                        
                    case 4:
                        string skills = ConsoleHelper.ReadNonEmpty("Skills (comma-separated)");
                        if (skills == "0") continue;
                        if (ConsoleHelper.Confirm("Update skills?"))
                        {
                            _seeker.UpdateSkills(skills);
                            DataStore.SaveJobSeekers();
                            PanelFactory.RenderSuccess("Skills updated.");
                        }
                        break;
                    case 5:
                        string edu = ConsoleHelper.ReadNonEmpty("Education details");
                        if (edu == "0") continue;
                        if (ConsoleHelper.Confirm("Update education?"))
                        {
                            _seeker.UpdateEducation(edu);
                            DataStore.SaveJobSeekers();
                            PanelFactory.RenderSuccess("Education updated.");
                        }
                        break;
                    case 6:
                        string exp = ConsoleHelper.ReadNonEmpty("Work experience details");
                        if (exp == "0") continue;
                        if (ConsoleHelper.Confirm("Update work experience?"))
                        {
                            _seeker.UpdateExperience(exp);
                            DataStore.SaveJobSeekers();
                            PanelFactory.RenderSuccess("Experience updated.");
                        }
                        break;
                    case 7:
                        string np = ConsoleHelper.ReadValidatedPassword("New password");
                        if (np == "0") continue;
                        ConsoleHelper.ReadConfirmPassword(np);
                        if (ConsoleHelper.Confirm("Update password?"))
                        {
                            _seeker.UpdatePassword(np);
                            DataStore.SaveJobSeekers();
                            PanelFactory.RenderSuccess("Password changed.");
                        }
                        break;
                    case 8:
                        ManageProjects();
                        break;
                    case 9:
                        ManageCertificates();
                        break;
                    case 10:
                        ManageEducationDetails();
                        break;
                    case 11:
                        ManageWorkExperienceDetails();
                        break;
                    case 12:
                        ManageSkillsDetails();
                        break;
                    case 13:
                        GiveCompanyFeedback();
                        break;
                }
                ConsoleHelper.Pause();
            }
        }
        catch (ValidationException ex)
        {
            PanelFactory.RenderError(ex.Message);
        }
        catch (JPNSException ex)
        {
            PanelFactory.RenderError(ex.Message);
        }
        catch (Exception)
        {
            PanelFactory.RenderError("Profile update failed.");
        }
    }

    private void UploadResume()
    {
        try
        {
            ShowHeader("Upload Resume", "Dashboard > Resume");
            PanelFactory.RenderInfo("Supported formats: PDF, DOC, DOCX  |  Max size: 5MB");
            AnsiConsole.WriteLine();

            string path = ConsoleHelper.ReadNonEmpty("Resume file path (or a name like 'my_resume.pdf')");
            if (ConsoleHelper.Confirm("Upload this resume?"))
            {
                string stored = ResumeService.UploadResume(path, _seeker.SeekerId);
                _seeker.SetResumeUrl(stored);
                DataStore.SaveJobSeekers();
                PanelFactory.RenderSuccess($"Resume uploaded: {stored}");
            }
        }
        catch (ValidationException ex)
        {
            PanelFactory.RenderError(ex.Message);
        }
        catch (JPNSException ex)
        {
            PanelFactory.RenderError(ex.Message);
        }
        catch (Exception)
        {
            PanelFactory.RenderError("Failed to upload resume.");
        }
        ConsoleHelper.Pause();
    }

    private void InterviewSchedule()
    {
        try
        {
            ShowHeader("My Interview Schedule","Dashboard > Interviews");
            HashSet<string> myAppIds =new HashSet<string>();
            foreach (AppModel app in DataStore.Applications)
            {
                if (app.JobSeekerId ==_seeker.SeekerId)
                {
                    myAppIds.Add(app.ApplicationId);
                }
            }
            List<Interview1> interviews =new List<Interview1>();
            foreach (Interview1 interview in DataStore.InterviewScheduler.GetAll())
            {
                if (myAppIds.Contains(interview.ApplicationId))
                {
                    interviews.Add(interview);
                }
            }

            for (int i = 0; i < interviews.Count - 1; i++)
            {
                for (int j = i + 1; j < interviews.Count; j++)
                {
                    if (interviews[i].ScheduledAt >interviews[j].ScheduledAt)
                    {
                        Interview1 temp = interviews[i];
                        interviews[i] = interviews[j];
                        interviews[j] = temp;
                    }
                }
            }

            if (interviews.Count == 0)
            {
                PanelFactory.RenderInfo("No interviews scheduled yet.");
                ConsoleHelper.Pause();
                return;
            }
            int pageSize = 5;
            int pageNumber = 1;
            while (true)
            {
                ShowHeader("Interview Schedule", "Dashboard > Interviews");
                int totalPages = (int)Math.Ceiling((double)interviews.Count / pageSize);
                if (totalPages == 0) totalPages = 1;

                int start = (pageNumber - 1) * pageSize;
                int end = Math.Min(start + pageSize, interviews.Count);

                var pageIvs = new List<Interview1>();
                for (int i = start; i < end; i++)
                {
                    pageIvs.Add(interviews[i]);
                }

                PanelFactory.RenderSectionTitle($"Upcoming Interviews  ({interviews.Count} total - Page {pageNumber} of {totalPages})", "📅");
                TableFactory.RenderInterviewTable(pageIvs, DataStore.Applications, DataStore.Jobs, startIndex: start);
                AnsiConsole.WriteLine();
                Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back");
                Console.WriteLine();

                Console.Write("Enter choice: ");
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
                    return;
                }
            }
        }
        catch (JPNSException ex)
        {
            PanelFactory.RenderError(ex.Message);
        }
        ConsoleHelper.Pause();
    }

    private void ShowNotifications()
    {
        try
        {
            int pageSize = 5;
            int pageNumber = 1;
            while (true)
            {
                ShowHeader("Notification Center", "Dashboard > Notifications");

                var notifications = DataStore.NotificationService.GetAllForUser(_seeker.SeekerId);
                int unread = DataStore.NotificationService.GetPendingCount(_seeker.SeekerId);

                int totalPages = (int)Math.Ceiling((double)notifications.Count / pageSize);
                if (totalPages == 0) totalPages = 1;

                int start = (pageNumber - 1) * pageSize;
                int end = Math.Min(start + pageSize, notifications.Count);

                var pageNotifications = new List<JobPortal.Features.Notifications.Notification>();
                for (int i = start; i < end; i++)
                {
                    pageNotifications.Add(notifications[i]);
                }

                PanelFactory.RenderSectionTitle($"Inbox  ({unread} unread)  (Page {pageNumber} of {totalPages})", "◎");

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
                        DataStore.NotificationService.MarkAllRead(_seeker.SeekerId);
                        PanelFactory.RenderSuccess("All notifications marked as read.");
                        ConsoleHelper.Pause();
                    }
                }
                else if (input == "0")
                {
                    break;
                }
            }
        }
        catch (JPNSException ex)
        {
            PanelFactory.RenderError(ex.Message);
            ConsoleHelper.Pause();
        }
    }

    private void ShowComplaints()
    {
        bool inComplaints = true;
        while (inComplaints)
        {
            ShowHeader("Complaints System", "Dashboard > Complaints");

            string menuContent =
                $"[bold {UITheme.Primary}]  1.[/]  [white]Raise a Complaint[/]\n" +
                $"[bold {UITheme.Primary}]  2.[/]  [white]View My Complaints[/]\n" +
                $"[{UITheme.Dim}]  0.  Back[/]";

            AnsiConsole.Write(new Panel(menuContent).Header($"[bold {UITheme.Candidate}]  Complaints Menu  [/]")
                    .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Candidate)).Padding(2, 0));
            AnsiConsole.WriteLine();

            int choice;
            while (true)
            {
                string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-2]]:[/]").PromptStyle(UITheme.Candidate).AllowEmpty());
                if (!int.TryParse(raw.Trim(), out choice) || choice < 0 || choice > 2)
                {
                    PanelFactory.RenderError("Invalid choice. Enter 0 to 2.");
                    continue;
                }
                break;
            }

            switch (choice)
            {
                case 0:
                    inComplaints = false;
                    break;
                case 1:
                    RaiseComplaint();
                    break;
                case 2:
                    ViewMyComplaints();
                    break;
            }
        }
    }

    private void RaiseComplaint()
    {
        try
        {
            ShowHeader("Raise a Complaint", "Dashboard > Complaints > Raise");
            string subject = ConsoleHelper.ReadNonEmpty("Enter complaint subject");
            string description = ConsoleHelper.ReadNonEmpty("Enter complaint description");

            int dbUserId = 0;
            string[] parts = _seeker.SeekerId.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int parsedId))
            {
                dbUserId = parsedId;
            }

            var complaint = new Complaint
            {
                SubmittedByUserId = dbUserId,
                AgainstUserId = null,
                Subject = subject,
                Description = description,
                Status = "OPEN",
                CreatedAt = DateTime.Now
            };

            DataStore.Complaints.Add(complaint);
            DataStore.SaveComplaints();

            PanelFactory.RenderSuccess("Complaint raised successfully! Our admins will review it.");
        }
        catch (Exception ex)
        {
            PanelFactory.RenderError($"Failed to raise complaint: {ex.Message}");
        }
        ConsoleHelper.Pause();
    }

    private void ManageProjects()
    {
        bool running = true;
        while (running)
        {
            ShowHeader("My Projects", "Dashboard > My Profile > Projects");
            
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderStyle(Style.Parse(UITheme.Candidate))
                .Expand()
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Title[/]"))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Technologies[/]"))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Project URL[/]"));

            for (int i = 0; i < _seeker.Projects.Count; i++)
            {
                var p = _seeker.Projects[i];
                table.AddRow(
                    $"[{UITheme.Dim}]{i + 1}[/]",
                    $"[bold white]{Markup.Escape(p.Title)}[/]",
                    $"[{UITheme.Neutral}]{Markup.Escape(p.Technologies ?? "—")}[/]",
                    $"[{UITheme.Dim}]{Markup.Escape(p.ProjectUrl ?? "—")}[/]"
                );
            }
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();

            var menu = 
                $"[bold {UITheme.Primary}]  1.[/]  [white]Add Project[/]\n" +
                $"[bold {UITheme.Error}]  2.[/]  [white]Remove Project[/]\n" +
                $"[{UITheme.Dim}]  0.  Back[/]";
            AnsiConsole.Write(new Panel(menu).Header($"[bold {UITheme.Candidate}]  Actions  [/]")
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

            if (choice == 0) return;
            if (choice == 1)
            {
                string title = ConsoleHelper.ReadNonEmpty("Project Title (or 0 to cancel)");
                if (title == "0") continue;
                string desc = ConsoleHelper.ReadLine("Description (press Enter to skip)");
                string tech = ConsoleHelper.ReadLine("Technologies (comma-separated, press Enter to skip)");
                string url = ConsoleHelper.ReadLine("Project URL (press Enter to skip)");
                
                var proj = new User.Candidate.Project
                {
                    Title = title,
                    Description = string.IsNullOrWhiteSpace(desc) ? null : desc,
                    Technologies = string.IsNullOrWhiteSpace(tech) ? null : tech,
                    ProjectUrl = string.IsNullOrWhiteSpace(url) ? null : url,
                    StartDate = DateTime.Now.AddMonths(-6),
                    EndDate = DateTime.Now
                };
                _seeker.Projects.Add(proj);
                DataStore.SaveJobSeekers();
                PanelFactory.RenderSuccess("Project added successfully!");
                ConsoleHelper.Pause();
            }
            if (choice == 2)
            {
                if (_seeker.Projects.Count == 0)
                {
                    PanelFactory.RenderError("No projects to remove.");
                    ConsoleHelper.Pause();
                    continue;
                }
                int idx = ConsoleHelper.ReadPositiveInt("Enter project number to remove (or 0 to cancel)");
                if (idx == 0) continue;
                if (idx >= 1 && idx <= _seeker.Projects.Count)
                {
                    if (ConsoleHelper.Confirm($"Remove project '{_seeker.Projects[idx - 1].Title}'?"))
                    {
                        _seeker.Projects.RemoveAt(idx - 1);
                        DataStore.SaveJobSeekers();
                        PanelFactory.RenderSuccess("Project removed.");
                        ConsoleHelper.Pause();
                    }
                }
                else
                {
                    PanelFactory.RenderError("Invalid project number.");
                    ConsoleHelper.Pause();
                }
            }
        }
    }

    private void ManageCertificates()
    {
        bool running = true;
        while (running)
        {
            ShowHeader("My Certificates", "Dashboard > My Profile > Certificates");
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderStyle(Style.Parse(UITheme.Candidate))
                .Expand()
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Cert Name[/]"))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Issuing Organization[/]"))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Credential ID[/]"));

            for (int i = 0; i < _seeker.Certificates.Count; i++)
            {
                var c = _seeker.Certificates[i];
                table.AddRow(
                    $"[{UITheme.Dim}]{i + 1}[/]",
                    $"[bold white]{Markup.Escape(c.CertName)}[/]",
                    $"[{UITheme.Neutral}]{Markup.Escape(c.IssuingOrg ?? "—")}[/]",
                    $"[{UITheme.Dim}]{Markup.Escape(c.CredentialId ?? "—")}[/]"
                );
            }
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();

            var menu = 
                $"[bold {UITheme.Primary}]  1.[/]  [white]Add Certificate[/]\n" +
                $"[bold {UITheme.Error}]  2.[/]  [white]Remove Certificate[/]\n" +
                $"[{UITheme.Dim}]  0.  Back[/]";
            AnsiConsole.Write(new Panel(menu).Header($"[bold {UITheme.Candidate}]  Actions  [/]")
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

            if (choice == 0) return;
            if (choice == 1)
            {
                string name = ConsoleHelper.ReadNonEmpty("Certificate Name (or 0 to cancel)");
                if (name == "0") continue;
                string org = ConsoleHelper.ReadLine("Issuing Org (press Enter to skip)");
                string url = ConsoleHelper.ReadLine("Certificate URL (press Enter to skip)");
                string cred = ConsoleHelper.ReadLine("Credential ID (press Enter to skip)");

                var cert = new User.Candidate.Certificate
                {
                    CertName = name,
                    IssuingOrg = string.IsNullOrWhiteSpace(org) ? null : org,
                    CertificateUrl = string.IsNullOrWhiteSpace(url) ? null : url,
                    CredentialId = string.IsNullOrWhiteSpace(cred) ? null : cred,
                    IssueDate = DateTime.Now
                };
                _seeker.Certificates.Add(cert);
                DataStore.SaveJobSeekers();
                PanelFactory.RenderSuccess("Certificate added successfully!");
                ConsoleHelper.Pause();
            }
            if (choice == 2)
            {
                if (_seeker.Certificates.Count == 0)
                {
                    PanelFactory.RenderError("No certificates to remove.");
                    ConsoleHelper.Pause();
                    continue;
                }
                int idx = ConsoleHelper.ReadPositiveInt("Enter certificate number to remove (or 0 to cancel)");
                if (idx == 0) continue;
                if (idx >= 1 && idx <= _seeker.Certificates.Count)
                {
                    if (ConsoleHelper.Confirm($"Remove certificate '{_seeker.Certificates[idx - 1].CertName}'?"))
                    {
                        _seeker.Certificates.RemoveAt(idx - 1);
                        DataStore.SaveJobSeekers();
                        PanelFactory.RenderSuccess("Certificate removed.");
                        ConsoleHelper.Pause();
                    }
                }
                else
                {
                    PanelFactory.RenderError("Invalid certificate number.");
                    ConsoleHelper.Pause();
                }
            }
        }
    }

    private void ManageEducationDetails()
    {
        bool running = true;
        while (running)
        {
            ShowHeader("Education Details", "Dashboard > My Profile > Education Details");
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderStyle(Style.Parse(UITheme.Candidate))
                .Expand()
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Degree[/]"))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Institution[/]"))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Grade[/]"));

            for (int i = 0; i < _seeker.EducationList.Count; i++)
            {
                var edu = _seeker.EducationList[i];
                table.AddRow(
                    $"[{UITheme.Dim}]{i + 1}[/]",
                    $"[bold white]{Markup.Escape(edu.Degree)}[/]",
                    $"[{UITheme.Neutral}]{Markup.Escape(edu.Institution)}[/]",
                    $"[{UITheme.Dim}]{Markup.Escape(edu.Grade ?? "—")}[/]"
                );
            }
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();

            var menu = 
                $"[bold {UITheme.Primary}]  1.[/]  [white]Add Education Details[/]\n" +
                $"[bold {UITheme.Error}]  2.[/]  [white]Remove Education Details[/]\n" +
                $"[{UITheme.Dim}]  0.  Back[/]";
            AnsiConsole.Write(new Panel(menu).Header($"[bold {UITheme.Candidate}]  Actions  [/]")
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

            if (choice == 0) return;
            if (choice == 1)
            {
                string degree = ConsoleHelper.ReadNonEmpty("Degree (or 0 to cancel)");
                if (degree == "0") continue;
                string inst = ConsoleHelper.ReadNonEmpty("Institution (or 0 to cancel)");
                if (inst == "0") continue;
                string grade = ConsoleHelper.ReadLine("Grade (press Enter to skip)");

                var edu = new User.Candidate.Education
                {
                    Degree = degree,
                    Institution = inst,
                    Grade = string.IsNullOrWhiteSpace(grade) ? null : grade
                };
                _seeker.EducationList.Add(edu);
                DataStore.SaveJobSeekers();
                PanelFactory.RenderSuccess("Education details added!");
                ConsoleHelper.Pause();
            }
            if (choice == 2)
            {
                if (_seeker.EducationList.Count == 0)
                {
                    PanelFactory.RenderError("No education records to remove.");
                    ConsoleHelper.Pause();
                    continue;
                }
                int idx = ConsoleHelper.ReadPositiveInt("Enter record number to remove (or 0 to cancel)");
                if (idx == 0) continue;
                if (idx >= 1 && idx <= _seeker.EducationList.Count)
                {
                    if (ConsoleHelper.Confirm($"Remove education record for '{_seeker.EducationList[idx - 1].Degree}'?"))
                    {
                        _seeker.EducationList.RemoveAt(idx - 1);
                        DataStore.SaveJobSeekers();
                        PanelFactory.RenderSuccess("Education record removed.");
                        ConsoleHelper.Pause();
                    }
                }
                else
                {
                    PanelFactory.RenderError("Invalid record number.");
                    ConsoleHelper.Pause();
                }
            }
        }
    }

    private void ManageWorkExperienceDetails()
    {
        bool running = true;
        while (running)
        {
            ShowHeader("Work Experience Details", "Dashboard > My Profile > Work Experience");
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderStyle(Style.Parse(UITheme.Candidate))
                .Expand()
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Job Title[/]"))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Company[/]"))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Current[/]"));

            for (int i = 0; i < _seeker.ExperienceList.Count; i++)
            {
                var exp = _seeker.ExperienceList[i];
                table.AddRow(
                    $"[{UITheme.Dim}]{i + 1}[/]",
                    $"[bold white]{Markup.Escape(exp.JobTitle)}[/]",
                    $"[{UITheme.Neutral}]{Markup.Escape(exp.Company)}[/]",
                    $"[{UITheme.Dim}]{(exp.IsCurrent ? "Yes" : "No")}[/]"
                );
            }
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();

            var menu = 
                $"[bold {UITheme.Primary}]  1.[/]  [white]Add Work Experience[/]\n" +
                $"[bold {UITheme.Error}]  2.[/]  [white]Remove Work Experience[/]\n" +
                $"[{UITheme.Dim}]  0.  Back[/]";
            AnsiConsole.Write(new Panel(menu).Header($"[bold {UITheme.Candidate}]  Actions  [/]")
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

            if (choice == 0) return;
            if (choice == 1)
            {
                string title = ConsoleHelper.ReadNonEmpty("Job Title (or 0 to cancel)");
                if (title == "0") continue;
                string comp = ConsoleHelper.ReadNonEmpty("Company (or 0 to cancel)");
                if (comp == "0") continue;
                bool isCurrent = ConsoleHelper.Confirm("Is this your current job?");

                var exp = new User.Candidate.WorkExperience
                {
                    JobTitle = title,
                    Company = comp,
                    IsCurrent = isCurrent,
                    StartDate = DateTime.Now.AddYears(-1)
                };
                _seeker.ExperienceList.Add(exp);
                DataStore.SaveJobSeekers();
                PanelFactory.RenderSuccess("Work experience details added!");
                ConsoleHelper.Pause();
            }
            if (choice == 2)
            {
                if (_seeker.ExperienceList.Count == 0)
                {
                    PanelFactory.RenderError("No experience records to remove.");
                    ConsoleHelper.Pause();
                    continue;
                }
                int idx = ConsoleHelper.ReadPositiveInt("Enter record number to remove (or 0 to cancel)");
                if (idx == 0) continue;
                if (idx >= 1 && idx <= _seeker.ExperienceList.Count)
                {
                    if (ConsoleHelper.Confirm($"Remove experience record for '{_seeker.ExperienceList[idx - 1].JobTitle}'?"))
                    {
                        _seeker.ExperienceList.RemoveAt(idx - 1);
                        DataStore.SaveJobSeekers();
                        PanelFactory.RenderSuccess("Experience record removed.");
                        ConsoleHelper.Pause();
                    }
                }
                else
                {
                    PanelFactory.RenderError("Invalid record number.");
                    ConsoleHelper.Pause();
                }
            }
        }
    }

    private void ManageSkillsDetails()
    {
        bool running = true;
        while (running)
        {
            ShowHeader("Skills Details", "Dashboard > My Profile > Skills Details");
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderStyle(Style.Parse(UITheme.Candidate))
                .Expand()
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Skill Name[/]"))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Proficiency Level[/]"))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Exp Years[/]"));

            for (int i = 0; i < _seeker.SkillsList.Count; i++)
            {
                var skill = _seeker.SkillsList[i];
                table.AddRow(
                    $"[{UITheme.Dim}]{i + 1}[/]",
                    $"[bold white]{Markup.Escape(skill.SkillName)}[/]",
                    $"[{UITheme.Neutral}]{Markup.Escape(skill.ProficiencyLevel)}[/]",
                    $"[{UITheme.Dim}]{(skill.YearsExperience.HasValue ? skill.YearsExperience.Value.ToString() : "—")}[/]"
                );
            }
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();

            var menu = 
                $"[bold {UITheme.Primary}]  1.[/]  [white]Add Skill[/]\n" +
                $"[bold {UITheme.Error}]  2.[/]  [white]Remove Skill[/]\n" +
                $"[{UITheme.Dim}]  0.  Back[/]";
            AnsiConsole.Write(new Panel(menu).Header($"[bold {UITheme.Candidate}]  Actions  [/]")
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

            if (choice == 0) return;
            if (choice == 1)
            {
                string name = ConsoleHelper.ReadNonEmpty("Skill Name (or 0 to cancel)");
                if (name == "0") continue;
                
                string prof = "Beginner";
                var levelMenu = $"[bold {UITheme.Primary}]  1.[/]  [white]Beginner[/]\n" +
                                $"[bold {UITheme.Primary}]  2.[/]  [white]Intermediate[/]\n" +
                                $"[bold {UITheme.Primary}]  3.[/]  [white]Expert[/]";
                AnsiConsole.Write(new Panel(levelMenu).Header($"[bold {UITheme.Candidate}]  Select Proficiency  [/]")
                        .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Candidate)).Padding(2, 0));
                AnsiConsole.WriteLine();
                int levelChoice;
                while (true)
                {
                    string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Select choice [[1-3]]:[/]").PromptStyle(UITheme.Candidate).AllowEmpty());
                    if (!int.TryParse(raw.Trim(), out levelChoice) || levelChoice < 1 || levelChoice > 3)
                    {
                        PanelFactory.RenderError("Enter a number between 1 and 3.");
                        continue;
                    }
                    break;
                }
                if (levelChoice == 1) prof = "Beginner";
                if (levelChoice == 2) prof = "Intermediate";
                if (levelChoice == 3) prof = "Expert";

                int exp = ConsoleHelper.ReadPositiveInt("Years of Experience (optional, or 99 to skip)");
                
                var skill = new User.Candidate.CandidateSkill
                {
                    SkillName = name,
                    ProficiencyLevel = prof,
                    YearsExperience = exp == 99 ? null : (int?)exp
                };
                _seeker.SkillsList.Add(skill);
                DataStore.SaveJobSeekers();
                PanelFactory.RenderSuccess("Skill details added!");
                ConsoleHelper.Pause();
            }
            if (choice == 2)
            {
                if (_seeker.SkillsList.Count == 0)
                {
                    PanelFactory.RenderError("No skills to remove.");
                    ConsoleHelper.Pause();
                    continue;
                }
                int idx = ConsoleHelper.ReadPositiveInt("Enter record number to remove (or 0 to cancel)");
                if (idx == 0) continue;
                if (idx >= 1 && idx <= _seeker.SkillsList.Count)
                {
                    if (ConsoleHelper.Confirm($"Remove skill '{_seeker.SkillsList[idx - 1].SkillName}'?"))
                    {
                        _seeker.SkillsList.RemoveAt(idx - 1);
                        DataStore.SaveJobSeekers();
                        PanelFactory.RenderSuccess("Skill removed.");
                        ConsoleHelper.Pause();
                    }
                }
                else
                {
                    PanelFactory.RenderError("Invalid record number.");
                    ConsoleHelper.Pause();
                }
            }
        }
    }

    private void GiveCompanyFeedback()
    {
        bool running = true;
        while (running)
        {
            ShowHeader("Company Feedbacks", "Dashboard > My Profile > Company Feedbacks");
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderStyle(Style.Parse(UITheme.Candidate))
                .Expand()
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]#[/]").Width(4))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Company Name[/]"))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Rating[/]"))
                .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Review[/]"));

            for (int i = 0; i < _seeker.Feedbacks.Count; i++)
            {
                var fb = _seeker.Feedbacks[i];
                table.AddRow(
                    $"[{UITheme.Dim}]{i + 1}[/]",
                    $"[bold white]{Markup.Escape(fb.CompanyName)}[/]",
                    $"[{UITheme.Secondary}]{fb.Rating}★[/]",
                    $"[{UITheme.Dim}]{Markup.Escape(fb.Review ?? "—")}[/]"
                );
            }
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();

            var menu = 
                $"[bold {UITheme.Primary}]  1.[/]  [white]Give Feedback[/]\n" +
                $"[bold {UITheme.Error}]  2.[/]  [white]Remove Feedback[/]\n" +
                $"[{UITheme.Dim}]  0.  Back[/]";
            AnsiConsole.Write(new Panel(menu).Header($"[bold {UITheme.Candidate}]  Actions  [/]")
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

            if (choice == 0) return;
            if (choice == 1)
            {
                string comp = ConsoleHelper.ReadNonEmpty("Company Name (or 0 to cancel)");
                if (comp == "0") continue;
                if (comp.Trim().ToUpperInvariant() != "JPNS")
                {
                    PanelFactory.RenderError("Feedback is only allowed for the company 'JPNS'.");
                    ConsoleHelper.Pause();
                    continue;
                }
                int rating;
                while (true)
                {
                    rating = ConsoleHelper.ReadPositiveInt("Rating [1-5]");
                    if (rating >= 1 && rating <= 5) break;
                    PanelFactory.RenderError("Rating must be between 1 and 5.");
                }
                string review = ConsoleHelper.ReadLine("Review/Comment (press Enter to skip)");
                bool isAnon = ConsoleHelper.Confirm("Submit anonymously?");

                var fb = new User.Candidate.Feedback
                {
                    CompanyName = comp,
                    Rating = (byte)rating,
                    Review = string.IsNullOrWhiteSpace(review) ? null : review,
                    IsAnonymous = isAnon,
                    CreatedAt = DateTime.Now
                };
                _seeker.Feedbacks.Add(fb);
                DataStore.SaveJobSeekers();
                PanelFactory.RenderSuccess("Feedback submitted successfully!");
                ConsoleHelper.Pause();
            }
            if (choice == 2)
            {
                if (_seeker.Feedbacks.Count == 0)
                {
                    PanelFactory.RenderError("No feedbacks to remove.");
                    ConsoleHelper.Pause();
                    continue;
                }
                int idx = ConsoleHelper.ReadPositiveInt("Enter feedback number to remove (or 0 to cancel)");
                if (idx == 0) continue;
                if (idx >= 1 && idx <= _seeker.Feedbacks.Count)
                {
                    if (ConsoleHelper.Confirm($"Remove feedback for '{_seeker.Feedbacks[idx - 1].CompanyName}'?"))
                    {
                        _seeker.Feedbacks.RemoveAt(idx - 1);
                        DataStore.SaveJobSeekers();
                        PanelFactory.RenderSuccess("Feedback removed.");
                        ConsoleHelper.Pause();
                    }
                }
                else
                {
                    PanelFactory.RenderError("Invalid feedback number.");
                    ConsoleHelper.Pause();
                }
            }
        }
    }

    private void ViewMyComplaints()
    {
        try
        {
            int dbUserId = 0;
            string[] parts = _seeker.SeekerId.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int parsedId))
            {
                dbUserId = parsedId;
            }

            int pageSize = 5;
            int pageNumber = 1;
            while (true)
            {
                ShowHeader("My Complaints", "Dashboard > Complaints > View");

                List<Complaint> myComplaints = new List<Complaint>();
                foreach (Complaint c in DataStore.Complaints)
                {
                    if (c.SubmittedByUserId == dbUserId)
                    {
                        myComplaints.Add(c);
                    }
                }

                if (myComplaints.Count == 0)
                {
                    PanelFactory.RenderInfo("You have not raised any complaints yet.");
                    ConsoleHelper.Pause();
                    return;
                }

                int totalPages = (int)Math.Ceiling((double)myComplaints.Count / pageSize);
                if (totalPages == 0) totalPages = 1;
                if (pageNumber > totalPages) pageNumber = totalPages;

                int start = (pageNumber - 1) * pageSize;
                int end = Math.Min(start + pageSize, myComplaints.Count);

                var pageComplaints = new List<Complaint>();
                for (int i = start; i < end; i++)
                {
                    pageComplaints.Add(myComplaints[i]);
                }

                PanelFactory.RenderSectionTitle($"My Complaints  ({myComplaints.Count} total - Page {pageNumber} of {totalPages})", "▶");
                AnsiConsole.MarkupLine("  [yellow]Instruction: You can only update or delete a complaint within 5 minutes of raising it.[/]");
                AnsiConsole.WriteLine();
                TableFactory.RenderComplaintTable(pageComplaints, startIndex: start);
                AnsiConsole.WriteLine();
                Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back");
                Console.WriteLine();

                Console.Write("Enter complaint number to view details (or page navigation command): ");
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
                    if (sel >= 1 && sel <= myComplaints.Count)
                    {
                        Complaint selected = myComplaints[sel - 1];
                        
                        while (true)
                        {
                            ShowHeader("Complaint Details", "Dashboard > Complaints > Details");

                            PanelFactory.RenderInfoCard("Complaint Details", new[]
                            {
                                ("Complaint ID", selected.ComplaintId.ToString()),
                                ("Subject", selected.Subject),
                                ("Description", selected.Description),
                                ("Status", selected.Status),
                                ("Admin Notes", string.IsNullOrWhiteSpace(selected.AdminNotes) ? "None" : selected.AdminNotes),
                                ("Submitted At", selected.CreatedAt.ToString("g")),
                                ("Resolved At", selected.ResolvedAt.HasValue ? selected.ResolvedAt.Value.ToString("g") : "N/A"),
                            }, UITheme.Candidate);

                            double elapsed = (DateTime.Now - selected.CreatedAt).TotalSeconds;
                            bool editable = elapsed < 300 && selected.Status.ToUpperInvariant() == "OPEN";

                            if (editable)
                            {
                                double remaining = 300 - elapsed;
                                int min = (int)(remaining / 60);
                                int sec = (int)(remaining % 60);
                                AnsiConsole.MarkupLine($"  [yellow]You can update or delete this complaint for another {min}m {sec}s.[/]");
                                AnsiConsole.WriteLine();

                                var actionMenu = 
                                    $"[bold {UITheme.Primary}]  1.[/]  [white]Update Complaint[/]\n" +
                                    $"[bold {UITheme.Error}]  2.[/]  [white]Delete Complaint[/]\n" +
                                    $"[{UITheme.Dim}]  0.  Back[/]";
                                AnsiConsole.Write(new Panel(actionMenu).Header($"[bold {UITheme.Candidate}]  Complaint Actions  [/]")
                                        .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Candidate)).Padding(2, 0));
                                AnsiConsole.WriteLine();
                                
                                int act;
                                while (true)
                                {
                                    string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-2]]:[/]").PromptStyle(UITheme.Candidate).AllowEmpty());
                                    if (!int.TryParse(raw.Trim(), out act) || act < 0 || act > 2)
                                    {
                                        PanelFactory.RenderError("Invalid choice. Enter 0, 1, or 2.");
                                        continue;
                                    }
                                    break;
                                }

                                if (act == 0)
                                {
                                    break;
                                }
                                else if (act == 1) // Update
                                {
                                    AnsiConsole.WriteLine();
                                    var colMenu = 
                                        $"[bold {UITheme.Primary}]  1.[/]  [white]Update Subject[/]\n" +
                                        $"[bold {UITheme.Primary}]  2.[/]  [white]Update Description[/]\n" +
                                        $"[{UITheme.Dim}]  0.  Cancel[/]";
                                    AnsiConsole.Write(new Panel(colMenu).Header($"[bold {UITheme.Candidate}]  Select Field  [/]")
                                            .Border(BoxBorder.Rounded).BorderStyle(Style.Parse(UITheme.Candidate)).Padding(2, 0));
                                    AnsiConsole.WriteLine();
                                    
                                    int field;
                                    while (true)
                                    {
                                        string raw = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-2]]:[/]").PromptStyle(UITheme.Candidate).AllowEmpty());
                                        if (!int.TryParse(raw.Trim(), out field) || field < 0 || field > 2)
                                        {
                                            PanelFactory.RenderError("Invalid choice. Enter 0, 1, or 2.");
                                            continue;
                                        }
                                        break;
                                    }

                                    if (field == 0) continue;

                                    if ((DateTime.Now - selected.CreatedAt).TotalSeconds >= 300)
                                    {
                                        PanelFactory.RenderError("Time limit exceeded! You cannot update this complaint anymore.");
                                        ConsoleHelper.Pause();
                                        break;
                                    }

                                    if (field == 1)
                                    {
                                        string newSub = ConsoleHelper.ReadNonEmpty("New subject (or 0 to cancel)");
                                        if (newSub == "0") continue;

                                        if ((DateTime.Now - selected.CreatedAt).TotalSeconds < 300)
                                        {
                                            selected.Subject = newSub;
                                            DataStore.SaveComplaints();
                                            PanelFactory.RenderSuccess("Subject updated successfully.");
                                            ConsoleHelper.Pause();
                                        }
                                        else
                                        {
                                            PanelFactory.RenderError("Time limit exceeded! You cannot update this complaint anymore.");
                                            ConsoleHelper.Pause();
                                            break;
                                        }
                                    }
                                    else if (field == 2)
                                    {
                                        string newDesc = ConsoleHelper.ReadNonEmpty("New description (or 0 to cancel)");
                                        if (newDesc == "0") continue;

                                        if ((DateTime.Now - selected.CreatedAt).TotalSeconds < 300)
                                        {
                                            selected.Description = newDesc;
                                            DataStore.SaveComplaints();
                                            PanelFactory.RenderSuccess("Description updated successfully.");
                                            ConsoleHelper.Pause();
                                        }
                                        else
                                        {
                                            PanelFactory.RenderError("Time limit exceeded! You cannot update this complaint anymore.");
                                            ConsoleHelper.Pause();
                                            break;
                                        }
                                    }
                                }
                                else if (act == 2) // Delete
                                {
                                    if (ConsoleHelper.Confirm("Are you sure you want to delete this complaint?"))
                                    {
                                        if ((DateTime.Now - selected.CreatedAt).TotalSeconds < 300)
                                        {
                                            DataStore.Complaints.Remove(selected);
                                            DataStore.SaveComplaints();
                                            PanelFactory.RenderSuccess("Complaint deleted successfully.");
                                            ConsoleHelper.Pause();
                                            break;
                                        }
                                        else
                                        {
                                            PanelFactory.RenderError("Time limit exceeded! You cannot delete this complaint anymore.");
                                            ConsoleHelper.Pause();
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                AnsiConsole.MarkupLine("  [grey]Note: This complaint is closed, resolved, or was created more than 5 minutes ago and cannot be modified.[/]");
                                ConsoleHelper.Pause();
                                break;
                            }
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid selection. Enter a number between 1 and {myComplaints.Count}. Press any key...");
                        Console.ReadKey(true);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            PanelFactory.RenderError($"Failed to display complaints: {ex.Message}");
            ConsoleHelper.Pause();
        }
    }
}