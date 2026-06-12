//using System.Text;
//using System.Text.RegularExpressions;

//namespace JobPortal.UI;

//internal static class ConsoleHelper
//{
//    private const int ColField = 25;
//    private const int ColAmount = 30;

//    public static void ColorsInformation()
//    {
//        PrintHeader("Console Color Guide");

//        Console.ForegroundColor = ConsoleColor.Cyan;
//        Console.WriteLine("  Cyan        -> Header / Section Title");

//        Console.ForegroundColor = ConsoleColor.Green;
//        Console.WriteLine("  Green       -> Success / Accepted / Hired / Approved / Completed");

//        Console.ForegroundColor = ConsoleColor.Red;
//        Console.WriteLine("  Red         -> Error / Rejected / Cancelled");

//        Console.ForegroundColor = ConsoleColor.Yellow;
//        Console.WriteLine("  Yellow      -> Warning / Pending / Pending Approval");

//        Console.ForegroundColor = ConsoleColor.DarkCyan;
//        Console.WriteLine("  Dark Cyan   -> Info Message");

//        Console.ForegroundColor = ConsoleColor.White;
//        Console.WriteLine("  White       -> Field Values / Default");

//        Console.ForegroundColor = ConsoleColor.DarkYellow;
//        Console.WriteLine("  Dark Yellow -> Table Header");

//        Console.ForegroundColor = ConsoleColor.Blue;
//        Console.WriteLine("  Blue        -> Interview Scheduled");

//        Console.ForegroundColor = ConsoleColor.DarkGray;
//        Console.WriteLine("  Dark Gray   -> Closed");

//        Console.ForegroundColor = ConsoleColor.DarkRed;
//        Console.WriteLine("  Dark Red    -> Expired");

//        //PrintSeparator();
//    }

//    public static void PrintHeader(string title)
//    {
//        Console.WriteLine();
//        Console.ForegroundColor = ConsoleColor.Cyan;
//        Console.WriteLine(new string('=', 60));
//        Console.WriteLine($"  {title}");
//        Console.WriteLine(new string('=', 60));
//        Console.ResetColor();
//    }

//    public static void PrintSeparator() => Console.WriteLine(new string('-', 60));

//    public static void PrintSuccess(string message)
//    {
//        Console.ForegroundColor = ConsoleColor.Green;
//        Console.WriteLine($"  [OK] {message}");
//        Console.ResetColor();
//    }

//    public static void PrintError(string message)
//    {
//        Console.ForegroundColor = ConsoleColor.Red;
//        Console.WriteLine($"  [ERR] {message}");
//        Console.ResetColor();
//    }

//    public static void PrintWarning(string message)
//    {
//        Console.ForegroundColor = ConsoleColor.Yellow;
//        Console.WriteLine($"  [!] {message}");
//        Console.ResetColor();
//    }

//    public static void PrintInfo(string message)
//    {
//        Console.ForegroundColor = ConsoleColor.DarkCyan;
//        Console.WriteLine($"  {message}");
//        Console.ResetColor();
//    }

//    public static void PrintField(string label, string value)
//    {
//        Console.Write($"  {label,-ColField}: ");
//        Console.ForegroundColor = ConsoleColor.White;
//        Console.WriteLine(value);
//        Console.ResetColor();
//    }

//    public static void PrintBadge(string status)
//    {
//        ConsoleColor color;
//        switch (status.ToUpperInvariant())
//        {
//            case "PENDING":
//                color = ConsoleColor.Yellow;
//                break;

//            case "SHORTLISTED":
//                color = ConsoleColor.Cyan;
//                break;

//            case "INTERVIEW_SCHEDULED":
//                color = ConsoleColor.Blue;
//                break;

//            case "REJECTED":
//                color = ConsoleColor.Red;
//                break;

//            case "HIRED":
//            case "APPROVED":
//            case "COMPLETED":
//                color = ConsoleColor.Green;
//                break;

//            case "PENDING_APPROVAL":
//                color = ConsoleColor.Yellow;
//                break;

//            case "CLOSED":
//                color = ConsoleColor.DarkGray;
//                break;

//            case "EXPIRED":
//                color = ConsoleColor.DarkRed;
//                break;

//            case "SCHEDULED":
//                color = ConsoleColor.Cyan;
//                break;

//            case "CANCELLED":
//                color = ConsoleColor.Red;
//                break;

//            default:
//                color = ConsoleColor.White;
//                break;
//        }
//        Console.ForegroundColor = color;
//        Console.Write($"[{status}]");
//        Console.ResetColor();
//    }

//    public static void PrintTableRow(int index, params string[] cols)
//    {
//        Console.Write($"  {index,3}. ");
//        foreach (string c in cols)
//            Console.Write($"{c,-ColAmount}");
//        Console.WriteLine();
//    }

//    public static void PrintTableHeader(params string[] cols)
//    {
//        Console.Write("  " + "    ");
//        Console.ForegroundColor = ConsoleColor.DarkYellow;
//        foreach (string c in cols)
//            Console.Write($"{c,-ColAmount}");
//        Console.ResetColor();
//        Console.WriteLine();
//        PrintSeparator();
//    }

//    public static void Pause()
//    {
//        Console.WriteLine();
//        Console.WriteLine("  Press any key to continue...");
//        Console.ReadKey(true);
//    }


//    public static string ReadLine(string prompt)
//    {
//        Console.Write($"  {prompt}: ");
//        return Console.ReadLine()?.Trim() ?? "";
//    }

//    public static string ReadPassword(
//    string prompt = "Password",
//    bool showRules = false)
//    {
//        string password = "";
//        bool showPassword = false;

//        if (showRules)
//        {
//            PrintSeparator();
//            Console.ForegroundColor = ConsoleColor.DarkCyan;
//            Console.WriteLine(
//                " Password Rules:\n" +
//                "  • Minimum 8 characters\n" +
//                "  • At least one uppercase letter\n" +
//                "  • At least one lowercase letter\n" +
//                "  • At least one digit\n" +
//                "  • At least one special character");
//            Console.ResetColor();
//            PrintSeparator();
//        }

//        Console.Write($" {prompt} (press Tab to show/hide): ");

//        int startLeft = Console.CursorLeft;
//        int startTop = Console.CursorTop;

//        while (true)
//        {
//            ConsoleKeyInfo key = Console.ReadKey(true);

//            if (key.Key == ConsoleKey.Enter)
//            {
//                Console.WriteLine();
//                break;
//            }
//            else if (key.Key == ConsoleKey.Backspace)
//            {
//                if (password.Length > 0)
//                {
//                    password = password[..^1];
//                    Console.SetCursorPosition(startLeft, startTop);
//                    Console.Write(new string(' ', password.Length + 1));
//                    Console.SetCursorPosition(startLeft, startTop);
//                    Console.Write(showPassword ? password : new string('*', password.Length));
//                }
//            }
//            else if (key.Key == ConsoleKey.Tab)
//            {
//                showPassword = !showPassword;
//                Console.SetCursorPosition(startLeft, startTop);
//                Console.Write(new string(' ', password.Length + 1));
//                Console.SetCursorPosition(startLeft, startTop);
//                Console.Write(showPassword ? password : new string('*', password.Length));
//            }
//            else if (!char.IsControl(key.KeyChar))
//            {
//                password += key.KeyChar;
//                Console.Write(showPassword ? key.KeyChar : '*');
//            }
//        }

//        return password;
//    }

//    public static int ReadMenuChoice(int min, int max)
//    {
//        while (true)
//        {
//            Console.Write("  Enter choice: ");
//            string raw = Console.ReadLine()?.Trim() ?? "";
//            int choice;
//            if (!int.TryParse(raw, out choice))
//            {
//                PrintError("Invalid choice. Please enter a number.");
//                continue;
//            }
//            if (choice < min || choice > max)
//            {
//                PrintError($"Please enter a number between {min} and {max}.");
//                continue;
//            }
//            return choice;
//        }
//    }

//    public static string ReadNonEmpty(string prompt)
//    {
//        while (true)
//        {
//            Console.Write($"  {prompt}: ");
//            string input = Console.ReadLine()?.Trim() ?? "";
//            if (!string.IsNullOrWhiteSpace(input))
//                return input;
//            PrintError($"{prompt} cannot be empty.");
//        }
//    }

//    public static string ReadEmail(string prompt = "Email")
//    {
//        while (true)
//        {
//            Console.Write($"  {prompt} [eg. user@gmail.com] : ");
//            string input = Console.ReadLine()?.Trim() ?? "";
//            if (input.Equals("0")) return "0";

//            if (string.IsNullOrWhiteSpace(input))
//            {
//                PrintError("Email cannot be empty.");
//                continue;
//            }

//            if (input.Length > 100)
//            {
//                PrintError("Email must not exceed 100 characters.");
//                continue;
//            }
//            if (!System.Text.RegularExpressions.Regex.IsMatch(input, @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$"))
//            {
//                PrintError("Invalid Email Format. Example: user@gmail.com");
//                continue;
//            }

//            PrintSuccess("Accepted.");
//            return input;
//        }
//    }

//    public static string ReadPhone(string prompt = "Phone")
//    {
//        while (true)
//        {
//            Console.Write($"  {prompt} [ Mobile number should contain 10 digits ] : ");
//            string input = Console.ReadLine()?.Trim() ?? "";

//            if (string.IsNullOrWhiteSpace(input))
//            {
//                PrintError("Phone number cannot be empty.");
//                continue;
//            }

//            if (!Regex.IsMatch(input, @"^[89]\d{9}$"))
//            {
//                PrintError("Phone number must contain digits only and it starts either 8 or 9.");
//                continue;
//            }

//            if (input.Length != 10)
//            {
//                PrintError("Phone number must contain exactly 10 digits.");
//                continue;
//            }

//            PrintSuccess("Accepted.");
//            return input;
//        }
//    }

//    public static string ReadValidatedPassword(string prompt = "Confirm Password")
//    {
//        while (true)
//        {
//            string pw = ReadPassword(prompt, true);
//            List<string> errors = new List<string>();
//            bool hasUpper = false;
//            bool hasLower = false;
//            bool hasDigit = false;
//            bool hasSpecial = false;
//            string specialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?";
//            foreach (char c in pw)
//            {
//                if (char.IsUpper(c))
//                    hasUpper = true;
//                if (char.IsLower(c))
//                    hasLower = true;
//                if (char.IsDigit(c))
//                    hasDigit = true;
//                if (specialChars.Contains(c))
//                    hasSpecial = true;
//            }
//            if (pw.Length < 8)
//                errors.Add("Minimum 8 characters");
//            if (!hasUpper)
//                errors.Add("At least one uppercase letter");
//            if (!hasLower)
//                errors.Add("At least one lowercase letter");
//            if (!hasDigit)
//                errors.Add("At least one digit");
//            if (!hasSpecial)
//                errors.Add("At least one special character");
//            if (errors.Count == 0)
//                return pw;
//            PrintError("Password must contain:");
//            foreach (string e in errors)
//            {
//                Console.WriteLine($"    - {e}");
//            }
//        }
//    }

//    public static void ReadConfirmPassword(string original)
//    {
//        while (true)
//        {
//            string confirm = ReadPassword("Confirm Password");
//            if (confirm == original)
//            {
//                PrintSuccess("Passwords match.");
//                return;
//            }
//            PrintError("Passwords do not match. Please try again.");
//        }
//    }

//    public static string ReadJobTitle(string prompt = "Job Title")
//    {
//        while (true)
//        {
//            Console.Write($"  {prompt}: ");
//            string input = Console.ReadLine()?.Trim() ?? "";

//            if (input.Length < 3)
//            { PrintError("Job title must be at least 3 characters."); continue; }

//            if (input.Length > 100)
//            { PrintError("Job title must not exceed 100 characters."); continue; }

//            PrintSuccess("Accepted.");
//            return input;
//        }
//    }

//    public static decimal ReadPositiveDecimal(string prompt)
//    {
//        while (true)
//        {
//            Console.Write($"  {prompt}: ");
//            string input = Console.ReadLine()?.Trim() ?? "";

//            if (!decimal.TryParse(input, out decimal val))
//            { PrintError("Must be a numeric value. Example: 5.5"); continue; }

//            if (val <= 0)
//            { PrintError("Value must be greater than 0."); continue; }

//            PrintSuccess("Accepted.");
//            return val;
//        }
//    }

//    public static int ReadPositiveInt(string prompt)
//    {
//        while (true)
//        {
//            Console.Write($"  {prompt}: ");
//            string input = Console.ReadLine()?.Trim() ?? "";

//            if (!int.TryParse(input, out int val))
//            { PrintError("Must be a whole number. Example: 30"); continue; }

//            if (val <= 0)
//            { PrintError("Value must be greater than 0."); continue; }

//            PrintSuccess("Accepted.");
//            return val;
//        }
//    }

//    public static DateTime ReadFutureDateTime(string prompt)
//    {
//        while (true)
//        {
//            Console.Write($"  {prompt}: ");
//            string input = Console.ReadLine()?.Trim() ?? "";

//            if (!DateTime.TryParse(input, out DateTime dt))
//            { PrintError("Invalid date format. Example: 2025-12-31 14:00"); continue; }

//            if (dt <= DateTime.Now)
//            { PrintError("Date must be a future date/time."); continue; }

//            PrintSuccess("Accepted.");
//            return dt;
//        }
//    }

//    public static DateTime ReadDate(string prompt)
//    {
//        while (true)
//        {
//            Console.Write($"  {prompt}: ");
//            string input = Console.ReadLine()?.Trim() ?? "";

//            if (!DateTime.TryParse(input, out DateTime dt))
//            { PrintError("Invalid date. Example: 2025-12-31"); continue; }

//            PrintSuccess("Accepted.");
//            return dt;
//        }
//    }

//    public static DateTime ReadExpiryDate(string prompt)
//    {
//        while (true)
//        {
//            Console.Write($"  {prompt}: ");
//            string input = Console.ReadLine()?.Trim() ?? "";

//            if (!DateTime.TryParse(input, out DateTime dt))
//            { PrintError("Invalid date. Example: 2025-12-31"); continue; }

//            if (dt.Date <= DateTime.Today)
//            { PrintError("Expiry date cannot be before or equal to today."); continue; }

//            PrintSuccess("Accepted.");
//            return dt;
//        }
//    }

//}






using System.Text.RegularExpressions;
using Spectre.Console;

namespace JobPortal.UI;

internal static class ConsoleHelper
{
    private const int ColField = 25;
    private const int ColAmount = 30;


    public static void ColorsInformation()
    {
        PrintHeader("Color Guide (JobPortal 2077)");

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse(UITheme.Primary))
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Color[/]").Centered())
            .AddColumn(new TableColumn($"[bold {UITheme.Warning}]Usage[/]"));

        table.AddRow($"[{UITheme.Primary}]■ Cyan[/]", "Header / Section Title");
        table.AddRow($"[{UITheme.Success}]■ Green[/]", "Success / Accepted / Hired / Approved / Completed");
        table.AddRow($"[{UITheme.Error}]■ Red[/]", "Error / Rejected / Cancelled");
        table.AddRow($"[{UITheme.Warning}]■ Yellow[/]", "Warning / Pending / Pending Approval");
        table.AddRow($"[{UITheme.Info}]■ Dark Cyan[/]", "Info Message");
        table.AddRow($"[{UITheme.White}]■ White[/]", "Field Values / Default");
        table.AddRow($"[{UITheme.Warning}]■ Gold[/]", "Table Header");
        table.AddRow($"[{UITheme.Candidate}]■ Blue[/]", "Interview Scheduled");
        table.AddRow($"[{UITheme.Dim}]■ Dark Gray[/]", "Closed");
        table.AddRow($"[{UITheme.Error}]■ Dark Red[/]", "Expired");

        AnsiConsole.Write(table);
    }

    public static void PrintHeader(string title)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(
            new Rule($"[bold cyan]{Markup.Escape(title)}[/]")
                .RuleStyle("cyan")
                .LeftJustified());
        AnsiConsole.WriteLine();
    }

    public static void PrintSeparator()
        => AnsiConsole.Write(new Rule().RuleStyle("grey dim"));

    public static void PrintPanel(string title, string content, string borderColor = "cyan")
    {
        AnsiConsole.Write(
            new Panel(Markup.Escape(content))
                .Header($"[bold {borderColor}] {Markup.Escape(title)} [/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse(borderColor))
                .Padding(1, 0));
    }


    public static void PrintSuccess(string message)
        => AnsiConsole.MarkupLine($"  [bold green][[OK]][/] [green]{Markup.Escape(message)}[/]");

    public static void PrintError(string message)
        => AnsiConsole.MarkupLine($"  [bold red][[ERR]][/] [red]{Markup.Escape(message)}[/]");

    public static void PrintWarning(string message)
        => AnsiConsole.MarkupLine($"  [bold yellow][[!]][/] [yellow]{Markup.Escape(message)}[/]");

    public static void PrintInfo(string message)
        => AnsiConsole.MarkupLine($"  [darkcyan]{Markup.Escape(message)}[/]");


    public static void PrintField(string label, string value)
        => AnsiConsole.MarkupLine(
            $"  [grey]{Markup.Escape($"{label,-ColField}")}:[/] [white]{Markup.Escape(value)}[/]");

    public static void PrintFields(params (string Label, string Value)[] fields)
    {
        var table = new Table()
            .HideHeaders()
            .Border(TableBorder.None)
            .AddColumn(new TableColumn("").Width(ColField))
            .AddColumn(new TableColumn(""));

        foreach (var (label, value) in fields)
            table.AddRow(
                $"[grey]{Markup.Escape(label)}[/]",
                $"[white]{Markup.Escape(value)}[/]");

        AnsiConsole.Write(table);
    }


    public static void PrintBadge(string status)
    {
        string color = status.ToUpperInvariant() switch
        {
            "PENDING" => "yellow",
            "SHORTLISTED" => "cyan",
            "INTERVIEW_SCHEDULED" => "blue",
            "REJECTED" => "red",
            "HIRED" or
            "APPROVED" or
            "COMPLETED" => "green",
            "PENDING_APPROVAL" => "yellow",
            "CLOSED" => "grey",
            "EXPIRED" => "maroon",
            "SCHEDULED" => "cyan",
            "CANCELLED" => "red",
            _ => "white",
        };
        AnsiConsole.Markup($"[bold {color}][[{Markup.Escape(status)}]][/]");
    }

    public static void PrintTableRow(int index, params string[] cols)
    {
        AnsiConsole.Markup($"  [white]{index,3}. [/]");
        foreach (string c in cols)
            AnsiConsole.Markup($"[white]{Markup.Escape($"{c,-ColAmount}")}[/]");
        AnsiConsole.WriteLine();
    }

    public static void PrintTableHeader(params string[] cols)
    {
        AnsiConsole.Markup("       ");
        foreach (string c in cols)
            AnsiConsole.Markup($"[gold1]{Markup.Escape($"{c,-ColAmount}")}[/]");
        AnsiConsole.WriteLine();
        PrintSeparator();
    }

    public static void BuildSpectreTable(string[] headers,IEnumerable<string[]> rows,Justify[]? alignments = null)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse("grey"))
            .Expand();

        for (int i = 0; i < headers.Length; i++)
        {
            var col = new TableColumn($"[bold gold1]{Markup.Escape(headers[i])}[/]");
            if (alignments != null && i < alignments.Length)
                col.Alignment(alignments[i]);
            table.AddColumn(col);
        }

        foreach (string[] row in rows)
        {
            string[] escapedRow = new string[row.Length];

            for (int i = 0; i < row.Length; i++)
            {
                escapedRow[i] = Markup.Escape(row[i]);
            }

            table.AddRow(escapedRow);
        }

        AnsiConsole.Write(table);
    }


    public static void Pause()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("  [grey italic]Press any key to continue...[/]");
        Console.ReadKey(true);
    }


    public static string ReadLine(string prompt)
        => AnsiConsole.Prompt(
            new TextPrompt<string>($"  [white]{Markup.Escape(prompt)}:[/]")
                .PromptStyle("white")
                .AllowEmpty()) ?? "";


    public static string ReadPassword(string prompt = "Password",bool showRules = false)
    {
        if (showRules)
        {
            PrintSeparator();
            AnsiConsole.Write(
                new Panel(
                        "[darkcyan]• Minimum 8 characters\n" +
                        "• At least one uppercase letter\n" +
                        "• At least one lowercase letter\n" +
                        "• At least one digit\n" +
                        "• At least one special character[/]")
                    .Header("[bold darkcyan] Password Rules [/]")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("darkcyan")));
            PrintSeparator();
        }

        AnsiConsole.Markup(
            $" [white]{Markup.Escape(prompt)}[/] [grey](Tab = show/hide):[/] ");
        return ReadPasswordRawLoop();
    }

    private static string ReadPasswordRawLoop()
    {
        string password = "";
        bool show = false;
        int startLeft = Console.CursorLeft;
        int startTop = Console.CursorTop;

        while (true)
        {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            { Console.WriteLine(); break; }

            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[..^1];
                Console.SetCursorPosition(startLeft, startTop);
                Console.Write(new string(' ', password.Length + 1));
                Console.SetCursorPosition(startLeft, startTop);
                Console.Write(show ? password : new string('*', password.Length));
            }
            else if (key.Key == ConsoleKey.Tab)
            {
                show = !show;
                Console.SetCursorPosition(startLeft, startTop);
                Console.Write(new string(' ', password.Length + 1));
                Console.SetCursorPosition(startLeft, startTop);
                Console.Write(show ? password : new string('*', password.Length));
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password += key.KeyChar;
                Console.Write(show ? key.KeyChar : '*');
            }
        }
        return password;
    }

    public static int ReadMenuChoice(string title, params string[] options)
    {
        string chosen = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[bold cyan]{Markup.Escape(title)}[/]")
                .PageSize(10)
                .HighlightStyle(Style.Parse("bold cyan"))
                .AddChoices(options));

        return Array.IndexOf(options, chosen) + 1;
    }

    public static int ReadMenuChoice(int min, int max)
        => AnsiConsole.Prompt(
            new TextPrompt<int>("  [white]Enter choice:[/]")
                .PromptStyle("white")
                .ValidationErrorMessage("[red]Please enter a valid number.[/]")
                .Validate(n => n >= min && n <= max
                    ? ValidationResult.Success()
                    : ValidationResult.Error(
                        $"[red]Enter a number between {min} and {max}.[/]")));


    public static string ReadNonEmpty(string prompt)
        => AnsiConsole.Prompt(
            new TextPrompt<string>($"  [white]{Markup.Escape(prompt)}:[/]")
                .PromptStyle("white")
                .Validate(s => !string.IsNullOrWhiteSpace(s)
                    ? ValidationResult.Success()
                    : ValidationResult.Error(
                        $"[red]{Markup.Escape(prompt)} cannot be empty.[/]")));

    public static string ReadEmail(string prompt = "Email")
    {
        while (true)
        {
            string input = AnsiConsole.Prompt(new TextPrompt<string>($"  [white]{Markup.Escape(prompt)}[/] [grey][[eg. user@gmail.com]][/]:").PromptStyle("white").AllowEmpty());

            if (input.Trim() == "0") return "0";
            input = input.Trim();

            if (string.IsNullOrWhiteSpace(input))
            { PrintError("Email cannot be empty."); continue; }

            if (input.Length > 100)
            { PrintError("Email must not exceed 100 characters."); continue; }

            if (!Regex.IsMatch(input,@"^[a-zA-Z0-9._%+\-]+@(gmail\.com|jpns\.com|[a-zA-Z0-9.\-]+\.in\.edu)$",RegexOptions.IgnoreCase))
            {
                PrintError("Invalid Email Format. Example: user@gmail.com");
                continue;
            }

            PrintSuccess("Accepted.");
            return input;
        }
    }


    public static string ReadPhone(string prompt = "Phone")
        => AnsiConsole.Prompt(
            new TextPrompt<string>(
                    $"  [white]{Markup.Escape(prompt)}[/] [grey][[10 digits, starts with 8 or 9]][/]:")
                .PromptStyle("white")
                .Validate(s =>
                {
                    s = s.Trim();
                    if (string.IsNullOrWhiteSpace(s))
                        return ValidationResult.Error("[red]Phone number cannot be empty.[/]");
                    if (!Regex.IsMatch(s, @"^[89]\d{9}$"))
                        return ValidationResult.Error(
                            "[red]Must be 10 digits starting with 8 or 9.[/]");
                    return ValidationResult.Success();
                }));


    public static string ReadValidatedPassword(string prompt = "Password")
    {
        while (true)
        {
            string pw = ReadPassword(prompt, showRules: true);
            if (pw == "0") return "0";

            List<string> errors = new List<string>();

            bool upper = false;
            bool lower = false;
            bool digit = false;
            bool special = false;

            const string specials =
                "!@#$%^&*()_+-=[]{}|;':\",./<>?";

            foreach (char c in pw)
            {
                if (char.IsUpper(c))
                    upper = true;

                if (char.IsLower(c))
                    lower = true;

                if (char.IsDigit(c))
                    digit = true;

                if (specials.Contains(c))
                    special = true;
            }

            if (pw.Length < 8)
                errors.Add("Minimum 8 characters");

            if (!upper)
                errors.Add("At least one uppercase letter");

            if (!lower)
                errors.Add("At least one lowercase letter");

            if (!digit)
                errors.Add("At least one digit");

            if (!special)
                errors.Add("At least one special character");

            if (errors.Count == 0)
                return pw;

            string body = "";

            for (int i = 0; i < errors.Count; i++)
            {
                body +=
                    $"  [red]• {Markup.Escape(errors[i])}[/]";

                if (i < errors.Count - 1)
                {
                    body += "\n";
                }
            }

            AnsiConsole.Write(
                new Panel(body)
                    .Header("[bold red] Password Requirements Not Met [/]")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("red")));
        }
    }


    public static void ReadConfirmPassword(string original)
    {
        while (true)
        {
            string confirm = ReadPassword("Confirm Password");

            if (confirm == original)
            { PrintSuccess("Passwords match."); return; }

            PrintError("Passwords do not match. Please try again.");
        }
    }


    public static string ReadJobTitle(string prompt = "Job Title")
        => AnsiConsole.Prompt(
            new TextPrompt<string>($"  [white]{Markup.Escape(prompt)}:[/]")
                .PromptStyle("white")
                .Validate(s =>
                {
                    s = s.Trim();
                    if (s.Length < 3) return ValidationResult.Error("[red]Job title must be at least 3 characters.[/]");
                    if (s.Length > 100) return ValidationResult.Error("[red]Job title must not exceed 100 characters.[/]");
                    return ValidationResult.Success();
                }));

    public static decimal ReadPositiveDecimal(string prompt)
        => AnsiConsole.Prompt(
            new TextPrompt<decimal>($"  [white]{Markup.Escape(prompt)}:[/]")
                .PromptStyle("white")
                .ValidationErrorMessage("[red]Must be a positive number. Example: 5.5[/]")
                .Validate(v => v > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Value must be greater than 0.[/]")));

    public static int ReadPositiveInt(string prompt)
        => AnsiConsole.Prompt(
            new TextPrompt<int>($"  [white]{Markup.Escape(prompt)}:[/]")
                .PromptStyle("white")
                .ValidationErrorMessage("[red]Must be a positive whole number. Example: 30[/]")
                .Validate(v => v > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Value must be greater than 0.[/]")));


    public static DateTime ReadFutureDateTime(string prompt)
    {
        string raw = AnsiConsole.Prompt(
            new TextPrompt<string>(
                    $"  [white]{Markup.Escape(prompt)}[/] [grey][[eg. 2025-12-31 02:00 PM]][/]:")
                .PromptStyle("white")
                .Validate(s =>
                {
                    string input = s.Trim();
                    var match = Regex.Match(input, @"^(\d{4}-\d{2}-)(\d{2}):(\d{2})\s*(AM|PM)$", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        input = $"{match.Groups[1].Value}{match.Groups[2].Value} {match.Groups[2].Value}:{match.Groups[3].Value} {match.Groups[4].Value}";
                    }

                    string[] formats = new string[] {
                        "yyyy-MM-dd HH:mm",
                        "yyyy-MM-dd hh:mm tt",
                        "yyyy-MM-dd h:mm tt",
                        "yyyy-MM-dd h:mmtt",
                        "yyyy-MM-dd hh:mmtt",
                        "yyyy-MM-dd H:mm",
                        "yyyy-MM-dd HH:mm:ss",
                        "yyyy-MM-dd hh:mm:ss tt"
                    };
                    bool ok = DateTime.TryParseExact(input, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt);
                    if (!ok)
                    {
                        ok = DateTime.TryParse(input, out dt);
                    }
                    if (!ok)
                        return ValidationResult.Error("[red]Invalid format. Example: 2025-12-31 02:00 PM[/]");
                    if (dt <= DateTime.Now)
                        return ValidationResult.Error("[red]Date must be in the future.[/]");
                    return ValidationResult.Success();
                }));

        string processed = raw.Trim();
        var match2 = Regex.Match(processed, @"^(\d{4}-\d{2}-)(\d{2}):(\d{2})\s*(AM|PM)$", RegexOptions.IgnoreCase);
        if (match2.Success)
        {
            processed = $"{match2.Groups[1].Value}{match2.Groups[2].Value} {match2.Groups[2].Value}:{match2.Groups[3].Value} {match2.Groups[4].Value}";
        }

        string[] parseFormats = new string[] {
            "yyyy-MM-dd HH:mm",
            "yyyy-MM-dd hh:mm tt",
            "yyyy-MM-dd h:mm tt",
            "yyyy-MM-dd h:mmtt",
            "yyyy-MM-dd hh:mmtt",
            "yyyy-MM-dd H:mm",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd hh:mm:ss tt"
        };
        if (DateTime.TryParseExact(processed, parseFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime result))
        {
            return result;
        }
        return DateTime.Parse(processed);
    }

    public static DateTime ReadDate(string prompt)
    {
        string raw = AnsiConsole.Prompt(
            new TextPrompt<string>(
                    $"  [white]{Markup.Escape(prompt)}[/] [grey][[eg. 2025-12-31]][/]:")
                .PromptStyle("white")
                .Validate(s => DateTime.TryParse(s, out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Invalid date. Example: 2025-12-31[/]")));
        return DateTime.Parse(raw);
    }

    public static DateTime ReadExpiryDate(string prompt)
    {
        string raw = AnsiConsole.Prompt(
            new TextPrompt<string>(
                    $"  [white]{Markup.Escape(prompt)}[/] [grey][[eg. 2025-12-31]][/]:")
                .PromptStyle("white")
                .Validate(s =>
                {
                    if (!DateTime.TryParse(s, out DateTime dt))
                        return ValidationResult.Error("[red]Invalid date. Example: 2025-12-31[/]");
                    if (dt.Date <= DateTime.Today)
                        return ValidationResult.Error("[red]Expiry date must be after today.[/]");
                    return ValidationResult.Success();
                }));
        return DateTime.Parse(raw);
    }

    public static List<string> ReadMultiSelect(string title, params string[] options)
        => AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title($"[bold cyan]{Markup.Escape(title)}[/]")
                .PageSize(12)
                .HighlightStyle(Style.Parse("bold cyan"))
                .InstructionsText("[grey](Space = select, Enter = confirm)[/]")
                .AddChoices(options));


    public static bool Confirm(string question)
    {
        while (true)
        {
            var prompt = new TextPrompt<string>($"  [yellow]{Markup.Escape(question)}[/] [grey][[y/n]][/]:")
                .PromptStyle("white")
                .AllowEmpty();
            var result = AnsiConsole.Prompt(prompt);
            if (string.IsNullOrWhiteSpace(result))
            {
                continue;
            }
            string cleaned = result.Trim().ToLowerInvariant();
            if (cleaned == "y" || cleaned == "yes")
            {
                return true;
            }
            if (cleaned == "n" || cleaned == "no")
            {
                return false;
            }
            AnsiConsole.MarkupLine("  [red]Please select one of the available options[/]");
        }
    }

    public static void WithSpinner(string message, Action action)
        => AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots2)
            .SpinnerStyle(Style.Parse("cyan"))
            .Start(message, _ => action());

    public static async Task WithSpinnerAsync(string message, Func<Task> action)
        => await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots2)
            .SpinnerStyle(Style.Parse("cyan"))
            .StartAsync(message, async _ => await action());
}