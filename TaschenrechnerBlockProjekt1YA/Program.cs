using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mathe;

namespace TaschenrechnerBlockProjekt1YA
{
    internal class Program
    {
        private static List<string> calculationHistory = new List<string>(); // Stores calculation history
        private static string angleMode = "DEG"; // Tracks angle mode (DEG or RAD)

        static void Main(string[] args)
        {
            char pastInput = 's';
            string temp = "";
            Stack<char> operators = new Stack<char>();
            Stack<decimal> values = new Stack<decimal>();
            string userView = "";
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            operators.Push('+'); // Default starting operator
            values.Push(0);
            Taschenrechner rechner = new Taschenrechner();

            Console.WriteLine("Welcome to the calculator!\n\n");
            Console.WriteLine("1. Einheitenumrechner");
            Console.WriteLine("2. Taschenrechner");
            Console.WriteLine("\n Press ESC to close the Program");
            keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.D1:
                    Einheitenrechner();
                    break;
                case ConsoleKey.D2:
                    break;
            }




            while (true)
            {
                Console.Clear();
                UpdateUserView(userView, values.Peek());

                keyInfo = Console.ReadKey(true);
                char keyChar = keyInfo.KeyChar;

                // Exit on Escape key
                if (keyInfo.Key == ConsoleKey.Escape)
                    break;

                // Show history on 'H' key
                if (keyInfo.Key == ConsoleKey.H)
                {
                    ShowHistory();
                    continue;
                }

                // Toggle angle mode on 'M' key
                if (keyInfo.Key == ConsoleKey.M)
                {
                    angleMode = angleMode == "DEG" ? "RAD" : "DEG";
                    Console.WriteLine($"\nAngle mode: {angleMode}");
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }
                // Handle equal sign
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    if (!string.IsNullOrEmpty(temp))
                    {
                        values.Push(decimal.Parse(temp, CultureInfo.InvariantCulture));
                        temp = "";
                    }

                    while (operators.Count > 0)
                    {
                        if (operators.Peek() == '(')
                        {
                            Console.WriteLine("Error: Unmatched parenthesis");
                            break;
                        }

                        decimal second = values.Pop();
                        decimal first = values.Pop();
                        try
                        {
                            values.Push(rechner.ApplyOperator(operators.Pop(), second, first));
                        }
                        catch (DivideByZeroException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            return;
                        }
                    }

                    // Add to history
                    calculationHistory.Add($"{userView.Replace("◘", "")} = {values.Peek()}");

                    Console.Clear();
                    Console.WriteLine(values.Peek());
                    userView = values.Peek().ToString();
                    pastInput = 's';
                }
                // Handle backspace
                if (keyInfo.Key == ConsoleKey.Backspace && userView.Length > 0)
                {
                    userView = userView.Remove(userView.Length - 1);
                    pastInput = userView.Length > 0 ? userView[^1] : 's';

                    if (userView.Length == 0)
                    {
                        operators.Clear();
                        values.Clear();
                        operators.Push('+');
                        values.Push(0);
                    }
                    else if (rechner.IsOperator(pastInput) && operators.Count > 1)
                    {
                        operators.Pop();
                    }
                    else if (!string.IsNullOrEmpty(temp))
                    {
                        temp = temp.Remove(temp.Length - 1);
                    }
                    continue;
                }



                // Validate input

                if (keyInfo.Key == ConsoleKey.D6 && keyInfo.Modifiers == ConsoleModifiers.Shift)
                {
                    keyChar = '^';
                }
                else if (!rechner.IsValidInput(keyChar))
                    continue;
                userView += keyChar;

                // Handle digits and negative numbers
                if (char.IsDigit(keyChar) || keyChar == '-' || keyChar == '.' || keyChar == ',')
                {
                    temp += keyChar;
                    pastInput = keyChar;
                    continue;
                }
                if (char.IsLetter(keyChar))
                {
                    operators.Push(keyChar);
                    pastInput = keyChar;
                    continue;
                }

                // Handle opening bracket
                if (keyChar == '(')
                {
                    operators.Push(keyChar);
                    pastInput = keyChar;
                    continue;
                }

                // Handle closing bracket
                if (keyChar == ')')
                {
                    if (!string.IsNullOrEmpty(temp))
                    {
                        values.Push(decimal.Parse(temp, CultureInfo.InvariantCulture));
                        temp = "";
                    }

                    while (operators.Count > 0 && operators.Peek() != '(')
                    {
                        decimal second = values.Pop();
                        decimal first = values.Pop();
                        try
                        {
                            values.Push(rechner.ApplyOperator(operators.Pop(), second, first));
                        }
                        catch (DivideByZeroException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            return;
                        }
                    }

                    if (operators.Count > 0 && operators.Peek() == '(')
                    {
                        operators.Pop(); // Remove '('
                    }

                    // Apply trigonometric function if present
                    if (operators.Count > 0 && "sctSCT".Contains(operators.Peek().ToString()))
                    {
                        char func = operators.Pop();
                        decimal arg = values.Pop();
                        values.Push(ApplyTrigFunction(func, arg, angleMode));
                    }

                    pastInput = keyChar;
                    continue;
                }

                // Handle operators (+, -, *, /, ^)
                if (rechner.IsOperator(keyChar))
                {
                    if (!string.IsNullOrEmpty(temp))
                    {
                        values.Push(decimal.Parse(temp, CultureInfo.InvariantCulture));
                        temp = "";
                    }

                    // Handle right-associativity for '^'
                    bool isRightAssociative = keyChar == '^';
                    int currentPrecedence = rechner.Precedence(keyChar);

                    while (operators.Count > 0 && operators.Peek() != '(' &&
                           (rechner.Precedence(operators.Peek()) > currentPrecedence ||
                           (!isRightAssociative && rechner.Precedence(operators.Peek()) >= currentPrecedence)))
                    {
                        decimal second = values.Pop();
                        decimal first = values.Pop();
                        try
                        {
                            values.Push(rechner.ApplyOperator(operators.Pop(), second, first));
                        }
                        catch (DivideByZeroException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            return;
                        }
                    }

                    operators.Push(keyChar);
                    pastInput = keyChar;
                    continue;
                }


            }
        }

        // Apply trigonometric functions
        private static decimal ApplyTrigFunction(char func, decimal arg, string mode)
        {
            double angle = (double)arg;
            if (mode == "DEG") angle *= Math.PI / 180.0;

            return func switch
            {
                'S' => (decimal)Math.Sin(angle),
                'C' => (decimal)Math.Cos(angle),
                'T' => (decimal)Math.Tan(angle),
                's' => (decimal)Math.Sin(angle),
                'c' => (decimal)Math.Cos(angle),
                't' => (decimal)Math.Tan(angle),
                _ => throw new InvalidOperationException()
            };
        }

        // Show calculation history
        private static void ShowHistory()
        {
            Console.Clear();
            Console.WriteLine("Calculation History:\n");
            foreach (string entry in calculationHistory)
            {
                Console.WriteLine($"  {entry}");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        // Update user view
        private static void UpdateUserView(string userView, decimal result)
        {
            Console.WriteLine($"Result: {result}\t({angleMode})");
            Console.WriteLine($"Input: {userView.Replace("◘", "")}");
        }

        private static void Einheitenrechner()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            do
            {
                Console.Clear();
                Console.WriteLine("Einheitenumrechner");
                Console.WriteLine("1. Längen");
                Console.WriteLine("2. Gewicht");
                Console.WriteLine("3. Temperatur");
                Console.WriteLine("Drücken Sie die Escape-Taste, wenn Sie zum vorherigen Menü zurückkehren möchten");

                
                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
                        Laengen();
                        break;
                    case ConsoleKey.D2:
                        Gewicht();
                        break;
                    case ConsoleKey.D3:
                        Temperatur();
                        break;
                }
            } while(keyInfo.Key != ConsoleKey.Escape);
        }

        private static void Laengen()
        {
            decimal eingegebenelaenge = 0;
            bool error = false;
            Console.WriteLine("Laengen");
            Console.WriteLine("Was wollen Sie umrechnen?");
            Console.WriteLine("1. Meter");
            Console.WriteLine("2. Kilometer");
            Console.WriteLine("3. Meilen");

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            Console.Clear();
            Console.WriteLine("Geben Sie den Wert ein:");
            switch (keyInfo.Key)
            {
                case ConsoleKey.D1:
                    Console.WriteLine("Meter:");
                    do
                    {
                        try
                        {
                            eingegebenelaenge = decimal.Parse(Console.ReadLine());
                            error = false;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Ungültige Eingabe");
                            error = true;
                        }
                    }while(error);
                    Console.WriteLine($"Kilometer: {eingegebenelaenge/1000}");
                    Console.WriteLine($"Meilen: {eingegebenelaenge / 1609,344}");

                    break;
                case ConsoleKey.D2:
                    Console.WriteLine("Kilometer:");
                    do
                    {
                        try
                        {
                            eingegebenelaenge = decimal.Parse(Console.ReadLine());
                            error = false;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Ungültige Eingabe");
                            error = true;
                        }
                    } while (error);
                    Console.WriteLine($"Meter: {eingegebenelaenge * 1000}");
                    Console.WriteLine($"Meilen: {eingegebenelaenge / 1,609344}");
                    break;
                case ConsoleKey.D3:
                    Console.WriteLine("Meilen:");
                    do
                    {
                        try
                        {
                            eingegebenelaenge = decimal.Parse(Console.ReadLine());
                            error = false;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Ungültige Eingabe");
                            error = true;
                        }
                    } while (error);
                    Console.WriteLine($"Meter: {eingegebenelaenge * 1609,344}");
                    Console.WriteLine($"Kilometer: {eingegebenelaenge * 1,609344}");

                    break;
            }

            Console.Clear();
            

        }
        private static void Gewicht()
        {
            ConsoleKeyInfo keyInfo = new();
            do
            {
                decimal eingegebeneGewicht = 0;
                Console.Clear();
                Console.WriteLine("Gewicht");
                Console.WriteLine("Was wollen Sie umrechnen?");
                Console.WriteLine("1. Kilogramm");
                Console.WriteLine("2. Pfund");

                keyInfo = Console.ReadKey(true);
                Console.Clear();
                Console.WriteLine("Geben Sie den Wert ein:");
                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
                        Console.Write("Kilogramm:");
                        eingegebeneGewicht = decimal.Parse(Console.ReadLine());
                        Console.Write($"Pfund: {eingegebeneGewicht * 2,20462}");
                        break;
                    case ConsoleKey.D2:
                        Console.WriteLine("Pfund:");
                        eingegebeneGewicht = decimal.Parse(Console.ReadLine());
                        Console.WriteLine($"Kilogramm: {eingegebeneGewicht / 2,20462}");
                        break;
                }
                Console.ReadKey();
            } while (keyInfo.Key != ConsoleKey.Escape);
            
        }

        private static void Temperatur()
        {
            decimal eingegebeneTemperatur = 0;
            Console.Clear();
            Console.WriteLine("Temperatur");
            Console.WriteLine("Was wollen Sie umrechnen?");
            Console.WriteLine("1. Celsius");
            Console.WriteLine("2. Fahrenheit");
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            Console.Clear();
            Console.WriteLine("Geben Sie den Wert ein:");
            switch( keyInfo.Key)
            {
                case ConsoleKey.D1:
                    Console.Write("1. Celsius: ");
                    eingegebeneTemperatur = decimal.Parse(Console.ReadLine());
                    Console.WriteLine($"Fahrenheit: {eingegebeneTemperatur * 9 / 5 + 32}");
                    break;
                case ConsoleKey.D2:
                    Console.Write("2. Fahrenheit: ");
                    eingegebeneTemperatur = decimal.Parse(Console.ReadLine());
                    Console.WriteLine($"Celsius: {(eingegebeneTemperatur - 32) * 5 / 9}");
                    break;
            }
            Console.ReadKey();
        }
    }
}