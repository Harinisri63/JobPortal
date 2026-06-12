using System;
using System.Collections.Generic;
namespace JobPortal.UI;

internal static class PaginationHelper
{
    public static void Show<T>(List<T> items, Action<T, int> printRow, int pageSize = 5)
    {
        int pageNumber = 1;
        while (true)
        {
            int totalPages = (int)Math.Ceiling((double)items.Count / pageSize);
            if (totalPages == 0) totalPages = 1;
            int start = (pageNumber - 1) * pageSize;
            for (int i = start; i < Math.Min(start + pageSize, items.Count); i++)
            {
                printRow(items[i], i);
            }
            Console.WriteLine();
            Console.WriteLine($"Page {pageNumber} of {totalPages}");

            Console.WriteLine("[N] Next  [P] Previous  [Q] Quit");

            char choice = char.ToUpper(Console.ReadKey(true).KeyChar);

            switch (choice)
            {
                case 'N':
                    if (pageNumber < totalPages)
                        pageNumber++;
                    else
                    {
                        Console.WriteLine("You are already in the last page.");
                    }
                    break;

                case 'P':
                    if (pageNumber > 1)
                        pageNumber--;
                    else
                    {
                        Console.WriteLine("You are already in the first page.");
                    }
                    break;

                case 'Q':
                    return;
            }
        }
    }

    public static int ShowWithSelection<T>(List<T> items, Action<T, int> printRow, string promptMsg = "Enter selection number", int pageSize = 5)
    {
        int pageNumber = 1;
        while (true)
        {
            int totalPages = (int)Math.Ceiling((double)items.Count / pageSize);
            if (totalPages == 0) totalPages = 1;
            
            int start = (pageNumber - 1) * pageSize;
            int end = Math.Min(start + pageSize, items.Count);
            
            for (int i = start; i < end; i++)
            {
                printRow(items[i], i);
            }
            Console.WriteLine();
            Console.WriteLine($"Page {pageNumber} of {totalPages} (Total {items.Count} items)");
            Console.WriteLine("Commands: [N] Next Page  ·  [P] Previous Page  ·  [0] Go back");
            Console.WriteLine();
            
            Console.Write($"{promptMsg} (or page navigation command): ");
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
                if (sel == 0) return 0;
                if (sel >= 1 && sel <= items.Count)
                {
                    return sel;
                }
                Console.WriteLine($"Invalid selection. Enter a number between 1 and {items.Count}. Press any key...");
                Console.ReadKey(true);
            }
        }
    }
}