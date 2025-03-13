using System.Numerics;
using Mathe;
namespace Taschenrechner1YA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string userview = "";
            var trigonometry = new Stack<char>();
            ConsoleKeyInfo keyInfo;
            Taschenrechner rechner = new Taschenrechner();

            Console.WriteLine("Real-time Calculator. Type an expression:");

            do
            {
                keyInfo = Console.ReadKey(true);
                char keyChar = keyInfo.KeyChar;

                if (rechner.IsValidInput(keyChar))
                {
                    userview += keyChar;
                    UpdateConsole(userview);
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    try
                    {
                        decimal result = rechner.EvaluateString(userview);
                        Console.WriteLine($"\nResult: {result}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\nError: {ex.Message}");
                    }
                    userview = "";
                    Console.WriteLine("Type another expression:");
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && userview.Length > 0)
                {
                    userview = userview.Substring(0, userview.Length - 1);
                    UpdateConsole(userview);
                }
                else
                {
                    Console.WriteLine("\nInvalid input. Try again.");
                }
            } while (keyInfo.Key != ConsoleKey.Escape);

        }

        static void UpdateConsole(string expression)
        {
            Taschenrechner rechner = new Taschenrechner();
            Console.Clear();
            if (rechner.IsValidExpression(expression))
            {
                try
                {
                    decimal result = rechner.EvaluateString(expression);
                    Console.WriteLine($"Result: {result}");
                }
                catch
                {
                    Console.WriteLine("Result: Error");
                }
            }
            else
            {
                Console.WriteLine("Result: ");
            }
            Console.WriteLine(expression);
        }

    }
}
