using JobPortal.Shared.Exceptions;
using JobPortal.Database;
using JobPortal.Features.Admin;
using JobPortal.Features.Auth;
using JobPortal.Features.Candidate;
using JobPortal.FileStorage;
using JobPortal.UI;
using Spectre.Console;

namespace JobPortal;

internal class Program
{
    private static readonly OTPService _otpService = new();

    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "JobPortal — Talent Discovery Engine";

        RunBootSequence();

        bool running = true;
        while (running)
        {
            ShowMainMenu();
            string choice = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.Dim}]Enter choice [[0-2]]:[/]").PromptStyle(UITheme.Primary).AllowEmpty());
            switch (choice.Trim())
            {
                case "1":
                    await HandleLogin();
                    break;
                case "2":
                    await HandleRegister();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    PanelFactory.RenderError("Choice must be 0, 1, or 2.");
                    ConsoleHelper.Pause();
                    break;
            }
        }
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new FigletText("Goodbye!").Centered().Color(Color.Cyan1));
        AnsiConsole.MarkupLine($"[{UITheme.Dim}]  Thank you for using {UITheme.AppName} · {UITheme.Today()}[/]");
        AnsiConsole.WriteLine();
    }

    private static void RunBootSequence()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new FigletText("JOBPORTAL").Centered().Color(Color.Cyan1));    
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule($"[bold {UITheme.Dim}]{UITheme.AppTagline}[/]").RuleStyle(UITheme.Dim));
        AnsiConsole.WriteLine();
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse(UITheme.Primary))
            .Start($"[{UITheme.Primary}]Initializing Talent Discovery Engine...[/]", ctx =>
            {
                Thread.Sleep(300);

                ctx.Status($"[{UITheme.Primary}]Loading Candidate Registry...[/]");
                Thread.Sleep(250);

                ctx.Status($"[{UITheme.Primary}]Loading Employer Directory...[/]");
                Thread.Sleep(200);

                ctx.Status($"[{UITheme.Primary}]Loading Recruitment Services...[/]");
                Thread.Sleep(200);

                ctx.Status($"[{UITheme.Primary}]Loading Resume Analytics...[/]");
                Thread.Sleep(200);

                ctx.Status($"[{UITheme.Warning}]Connecting Database...[/]");
                DatabaseSync.Initialize();
                Thread.Sleep(250);

                ctx.Status($"[{UITheme.Primary}]Loading Authentication Services...[/]");
                DataStore.LoadAll();
                Thread.Sleep(250);

                ctx.Status($"[{UITheme.Primary}]Loading Matching Engine...[/]");
                Thread.Sleep(200);

                ctx.Status($"[{UITheme.Primary}]Verifying Modules...[/]");
                Thread.Sleep(200);
            });

        AnsiConsole.MarkupLine($"[bold {UITheme.Success}]  ✓  System Ready[/]");
        AnsiConsole.WriteLine();
        Thread.Sleep(400);
    }

    private static void ShowMainMenu()
    {
        Console.Clear();
        AnsiConsole.WriteLine();
        var topRule = new Rule($"[bold {UITheme.Primary}] {UITheme.AppName} [/]  [{UITheme.Dim}]·  {UITheme.AppTagline}[/]")
            .RuleStyle(UITheme.Primary);
        AnsiConsole.Write(topRule);
        AnsiConsole.WriteLine();
        var menuContent =
            $"[bold {UITheme.Primary}]  1.[/]  [{UITheme.White}]Login[/]\n" +
            $"[bold {UITheme.Candidate}]  2.[/]  [{UITheme.White}]Register as Job Seeker[/]\n" +
            $"[{UITheme.Dim}]  0.  Exit[/]";

        AnsiConsole.Write(
            new Panel(menuContent)
                .Header($"[bold {UITheme.Primary}]  Main Menu  [/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse(UITheme.Primary))
                .Padding(2, 1));
        AnsiConsole.WriteLine();
        ConsoleHelper.ColorsInformation();
        PanelFactory.RenderFooter();
        AnsiConsole.WriteLine();
    }

    private static async Task HandleLogin()
    {
        try
        {
            Console.Clear();
            PanelFactory.RenderScreenHeader("Login", "Main > Login", "Guest", "PUBLIC");
            PanelFactory.RenderCredentialsHint("admin@jpns.com", "Admin@123");

            ConsoleHelper.PrintInfo("If you want to leave the current page enter 0.");
            AnsiConsole.WriteLine();

            string identifier;
            while (true)
            {
                AnsiConsole.Write(UITheme.SectionRule("Identifier"));
                AnsiConsole.MarkupLine($"[{UITheme.Dim}]  Options: Email [[eg. user@gmail.com]]  ·  Seeker ID [[eg. JSK-2026-0000]][/]");

                identifier = AnsiConsole.Prompt(
                    new TextPrompt<string>($"  [{UITheme.White}]Identifier (email / Seeker ID):[/]")
                        .PromptStyle(UITheme.Primary)
                        .AllowEmpty());

                identifier = identifier.Trim();
                if (identifier == "0") return;

                if (identifier.Contains('@'))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(identifier,@"^[a-zA-Z0-9._%+\-]+@(gmail\.com|jpns\.com|[a-zA-Z0-9.\-]+\.in\.edu)$",System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        PanelFactory.RenderError("Only @gmail.com, @jpns.com, and .in.edu email addresses are allowed.");
                        continue;
                    }
                }

                if (!string.IsNullOrWhiteSpace(identifier)) break;
                PanelFactory.RenderError("Identifier cannot be empty.");
            }

            string password;
            while (true)
            {
                password = ConsoleHelper.ReadPassword("Password", false);
                if (!string.IsNullOrWhiteSpace(password)) break;
                PanelFactory.RenderError("Password cannot be empty.");
            }

            Admin1? admin = null;
            JobSeeker? seeker = null;

            admin = DataStore.FindAdmin(identifier);
            if (admin != null)
            {
                try
                {
                    admin = AuthService.LoginAdmin(identifier, password);
                }
                catch (InvalidCredentialException)
                {
                    seeker = TryLoginAsSeeker(identifier, password);
                    if (seeker == null)
                        throw new InvalidCredentialException("Invalid credentials. Please try again.");
                }
            }
            else
            {
                seeker = TryLoginAsSeeker(identifier, password);
            }

            if (admin != null)
            {
                PanelFactory.RenderSuccess($"Welcome back, {admin.Name}! Redirecting to Admin Dashboard...");
                await Task.Delay(500);
                new AdminMenu(admin).Show();
            }
            else if (seeker != null)
            {
                if (!seeker.IsVerified)
                {
                    PanelFactory.RenderWarning("Email not verified. Please complete OTP verification.");
                    VerifyOTP(seeker);
                }
                PanelFactory.RenderSuccess($"Welcome back, {seeker.Name}! Redirecting to Candidate Dashboard...");
                await Task.Delay(500);
                new CandidateMenu(seeker).Show();
            }
            else
            {
                PanelFactory.RenderError("Invalid credentials. Please try again.");
                ConsoleHelper.Pause();
            }
        }
        catch (AccountLockedException ex)
        {
            PanelFactory.RenderError(ex.Message);
            ConsoleHelper.Pause();
        }
        catch (InvalidCredentialException ex)
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
            PanelFactory.RenderError($"Unexpected error during login: {ex}");
            ConsoleHelper.Pause();
        }
    }

    private static JobSeeker? TryLoginAsSeeker(string identifier, string password)
    {
        try
        {
            return AuthService.LoginJobSeeker(identifier, password);
        }
        catch (AccountLockedException)
        {
            throw;
        }
        catch
        {
            return null;
        }
    }

    private static async Task HandleRegister()
    {
        try
        {
            Console.Clear();
            PanelFactory.RenderScreenHeader("New Candidate Registration", "Main > Register", "Guest", "PUBLIC");
            PanelFactory.RenderInfo("Each field is validated immediately after you press Enter.");
            AnsiConsole.WriteLine();
            ConsoleHelper.PrintInfo("If you want to go to the previous menu enter 0.");
            AnsiConsole.WriteLine();

            string name = ConsoleHelper.ReadNonEmpty("Full Name");
            if (name.Equals("0")) return;

            string email;
            while (true)
            {
                email = ConsoleHelper.ReadEmail("Email");
                if (email.Equals("0")) return;
                if (DataStore.FindJobSeeker(email) != null)
                {
                    PanelFactory.RenderError("This email is already registered. Please use a different email.");
                    continue;
                }
                break;
            }

            string phone;
            while (true)
            {
                phone = ConsoleHelper.ReadPhone("Phone");
                bool phoneExists = false;
                foreach (JobSeeker existingSeeker in DataStore.JobSeekers)
                {
                    if (existingSeeker.Phone == phone)
                    {
                        phoneExists = true;
                        break;
                    }
                }
                if (phoneExists)
                {
                    PanelFactory.RenderError("This phone number is already registered. Please use a different number.");
                    continue;
                }
                break;
            }

            string password = ConsoleHelper.ReadValidatedPassword("Password");
            ConsoleHelper.ReadConfirmPassword(password);

            var seeker = new JobSeeker(name, email, phone, password);
            DataStore.JobSeekers.Add(seeker);
            DataStore.SaveJobSeekers();

            PanelFactory.RenderSuccess($"Account created!  Seeker ID: {seeker.SeekerId}");
            PanelFactory.RenderInfo("Please verify your email via OTP.");

            VerifyOTP(seeker);

            if (seeker.IsVerified)
            {
                PanelFactory.RenderSuccess("Email verified! You can now log in.");
                DataStore.NotificationService.Send(seeker.SeekerId, "Welcome to JPNS",$"Hello {seeker.Name}, welcome to the Job Portal! Start browsing jobs today.", "ANNOUNCEMENT");
                PanelFactory.RenderSuccess("Registration completed successfully.");
                PanelFactory.RenderInfo("Redirecting to Candidate Dashboard...");
                await Task.Delay(500);

                CandidateMenu candidateMenu = new CandidateMenu(seeker);
                candidateMenu.Show();
                return;
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
        catch (Exception)
        {
            PanelFactory.RenderError("Registration failed. Please try again.");
            ConsoleHelper.Pause();
        }
    }

    private static void VerifyOTP(JobSeeker seeker)
    {
        try
        {
            string otp = _otpService.GenerateOTP(seeker.Email);
            AnsiConsole.WriteLine();
            PanelFactory.RenderOTPBox(otp);

            for (int attempt = 1; attempt <= 3; attempt++)
            {
                string input = AnsiConsole.Prompt(new TextPrompt<string>($"  [{UITheme.White}]Enter OTP (attempt {attempt}/3):[/]").PromptStyle(UITheme.Warning));
                try
                {
                    _otpService.ValidateOTP(seeker.Email, input);
                    seeker.Verify();
                    DataStore.SaveJobSeekers();
                    PanelFactory.RenderSuccess("OTP verified successfully!");
                    ConsoleHelper.Pause();
                    return;
                }
                catch (OTPExpiredException ex)
                {
                    PanelFactory.RenderError(ex.Message);
                    ConsoleHelper.Pause();
                    return;
                }
                catch (ValidationException ex)
                {
                    PanelFactory.RenderError(ex.Message);
                    if (attempt == 3)
                        PanelFactory.RenderWarning("Maximum OTP attempts reached.");
                }
            }
        }
        catch (JPNSException ex)
        {
            PanelFactory.RenderError(ex.Message);
        }
        ConsoleHelper.Pause();
    }
}
