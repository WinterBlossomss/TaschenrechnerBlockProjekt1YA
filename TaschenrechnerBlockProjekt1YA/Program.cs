using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mathe;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaschenrechnerBlockProjekt1YA
{
    internal class Program
    {
        private static List<string> calculationHistory = new List<string>(); // Stores calculation history
        private static List<decimal> calculatedResults = new List<decimal>(); // Stores calculated results
        private static string angleMode = "DEG"; // Tracks angle mode (DEG or RAD)

        static void Main(string[] args)
        {
            char pastInput = 's';
            string temp = "";
            Stack<char> operators = new Stack<char>();
            Stack<decimal> values = new Stack<decimal>();
            string userView = "";
            bool error = false;
            bool shouldReset = false;
            ConsoleKeyInfo keyInfo = new();

            operators.Push('+'); // Default starting operator
            values.Push(0);
            Taschenrechner rechner = new Taschenrechner();

            Console.WriteLine("Welcome to the calculator!\n");
            Console.WriteLine("1. Einheitenumrechner");
            Console.WriteLine("2. Taschenrechner");
            Console.WriteLine("\n Press ESC to close the Program");
            do
            {
                keyInfo = Console.ReadKey();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
                        Einheitenrechner();
                        error = false;
                        break;
                    case ConsoleKey.D2:
                        error = false;
                        break; ;
                    default:
                        Console.WriteLine("Ungültige Eingabe");
                        error = true;
                        return;
                }
            } while (error);

            //Taschenrechner
            do
            {
                Console.Clear();
                if (values.Count != 0)
                    UpdateUserView(userView, values.Peek(), temp);
                else
                    UpdateUserView(userView, 0, temp);
                keyInfo = Console.ReadKey();
                char keyChar = keyInfo.KeyChar;

                // Show history on 'H' key
                if (keyInfo.Key == ConsoleKey.H)
                {
                    Console.Clear();
                    Console.WriteLine("---------------------------------------------");
                    Console.WriteLine("Verlauf der Berechnungen:\n");
                    foreach (string entry in calculationHistory)
                    {
                        Console.WriteLine($"  {entry}");
                    }
                    Console.WriteLine("---------------------------------------------");
                    Console.WriteLine("Wollen Sie eine Resultat auswählen? (J/N)");
                    keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.J)
                    {
                        Console.WriteLine("Geben Sie die Nummer des Resultats ein:");
                        string input = Console.ReadLine();
                        if (!int.TryParse(input, out int selectedIndex) || selectedIndex < 1 || selectedIndex > calculatedResults.Count)
                        {
                            Console.WriteLine("Ungültige Index.");
                            continue;
                        }
                        decimal result = calculatedResults[selectedIndex - 1];

                        Console.WriteLine($"Resultat: {result}\n");
                        values.Push(result);
                        userView = $"{result}";
                    }
                    else
                    {
                        Console.Clear();
                    }
                    Console.WriteLine("\nDrücken Sie eine Taste um fortzusetzen...");
                    Console.ReadKey();
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
                // Handle enter key
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
                        if (values.Count == 0)
                            break;
                        decimal second = values.Pop();
                        if (values.Count == 0)
                            break;
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
                    if (values.Count == 0)
                        continue;
                    // Add to history
                    calculationHistory.Add($"{calculationHistory.Count + 1}: {userView.Replace("◘", "")} = {values.Peek()}");
                    calculatedResults.Add(values.Peek());

                    Console.Clear();
                    Console.WriteLine(values.Peek());
                    userView = values.Peek().ToString();
                    shouldReset = true;
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

                // Circumflex für trigonometrische Funktionen
                if (keyInfo.Key == ConsoleKey.D6 && keyInfo.Modifiers == ConsoleModifiers.Shift)
                {
                    keyChar = '^';
                }
                //Wurzelzeichen für Wurzelziehen
                if(keyInfo.Key == ConsoleKey.D3 && keyInfo.Modifiers == ConsoleModifiers.Shift)
                {
                    keyChar = '√';
                }
                else if (!rechner.IsValidInput(keyChar))
                    continue;
                userView += keyChar;

                // Handling für Zahlen
                if (char.IsDigit(keyChar))
                {
                    if (shouldReset)
                    {
                        // Reset calculator state
                        operators.Clear();
                        values.Clear();
                        operators.Push('+');
                        values.Push(0);
                        temp = "";
                        userView = "";
                        userView += keyChar;
                        shouldReset = false;
                    }

                    temp += keyChar;
                    pastInput = keyChar;
                    continue;
                }

                // Handling für Dezimalzahlen
                if (keyChar == '.' || keyChar == ',')
                {
                    if (temp.Contains('.'))
                    {
                        continue; // Ignore duplicate decimal points
                    }
                    temp += '.';
                    userView += keyChar == ',' ? '.' : keyChar;
                }

                // Handling für Fakultät
                else if (keyChar == '!')
                {
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (decimal.TryParse(temp, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal number))
                        {
                            if (number == Math.Floor(number) && number >= 0)
                            {
                                try
                                {
                                    int n = (int)number;
                                    decimal result = rechner.Fakultaet(n);
                                    temp = result.ToString(CultureInfo.InvariantCulture); // Store result in temp
                                }
                                catch (OverflowException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    userView = userView.Remove(userView.Length - 1); // Remove '!' on error
                                }
                                catch (ArgumentOutOfRangeException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    userView = userView.Remove(userView.Length - 1);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Factorial requires a non-negative integer.");
                                userView = userView.Remove(userView.Length - 1);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid number for factorial.");
                            userView = userView.Remove(userView.Length - 1);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Factorial requires a preceding number.");
                        userView = userView.Remove(userView.Length - 1);
                    }
                    continue;
                }

                // Handling für negative Zahlen
                else if (keyChar == '-' && (temp.Length == 0 || rechner.IsOperator(pastInput) || pastInput == '('))
                {
                    temp += keyChar;
                    userView += keyChar;
                }

                // Handling für Buchstaben
                if (char.IsLetter(keyChar))
                {
                    operators.Push(keyChar);
                    pastInput = keyChar;
                    continue;
                }

                // Handling für offene Klammern
                if (keyChar == '(')
                {
                    operators.Push(keyChar);
                    pastInput = keyChar;
                    continue;
                }

                // Handling für geschlossene Klammern
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
                        operators.Pop(); // Entfernt '('
                    }

                    // Berechnung der trigonometrischen Funktionen wenn vorhanden
                    if (operators.Count > 0 && "sctSCT".Contains(operators.Peek().ToString()))
                    {
                        char func = operators.Pop();
                        decimal arg = values.Pop();
                        values.Push(rechner.ApplyTrigFunction(func, arg, angleMode));
                    }

                    pastInput = keyChar;
                    continue;
                }

                // Handling von Operatoren (+, -, *, /, ^)
                if (rechner.IsOperator(keyChar))
                {
                    if (!string.IsNullOrEmpty(temp))
                    {
                        values.Push(decimal.Parse(temp, CultureInfo.InvariantCulture));
                        temp = "";
                    }

                    // Handling für '^'
                    bool isRightAssociative = keyChar == '^' || keyChar == '√';
                    int currentPrecedence = rechner.Precedence(keyChar);

                    while (operators.Count > 0 && operators.Peek() != '(' &&
                           (rechner.Precedence(operators.Peek()) > currentPrecedence ||
                           (!isRightAssociative && rechner.Precedence(operators.Peek()) >= currentPrecedence)))
                    {
                        // When processing operators
                        if (values.Count < 2)
                        {
                            Console.WriteLine("Error: Not enough values for operation.");
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

                    operators.Push(keyChar);
                    pastInput = keyChar;
                    continue;
                }


            } while (keyInfo.Key != ConsoleKey.Escape);
        }




        // Update user view
        private static void UpdateUserView(string userView, decimal stackResult, string temp)
        {
            decimal currentResult = stackResult;
            if (!string.IsNullOrEmpty(temp))
            {
                if (decimal.TryParse(temp, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tempValue))
                {
                    currentResult = tempValue;
                }
            }
            Console.WriteLine($"Result: {currentResult}\t({angleMode})");
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

                keyInfo = Console.ReadKey();
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
            Console.Clear();
            Console.WriteLine("Laengen");
            Console.WriteLine("Was wollen Sie umrechnen?");
            Console.WriteLine("1. Meter");
            Console.WriteLine("2. Kilometer");
            Console.WriteLine("3. Meilen");

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            Console.Clear();
            Console.WriteLine("Geben Sie einen Wert ein:");
            switch (keyInfo.Key)
            {
                case ConsoleKey.D1:
                    Console.Write("Meter:");
                    do
                    {
                        try
                        {
                            eingegebenelaenge = Convert.ToDecimal(Console.ReadLine());
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
                    Console.Write("Kilometer:");
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
                    Console.Write("Meilen:");
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
            Console.ReadKey();
            Console.Clear();
            

        }
        private static void Gewicht()
        {
            ConsoleKeyInfo keyInfo = new();
            do
            {
                decimal eingegebeneGewicht = 0;
                decimal difference = 2.205M;
                bool error = false;
                Console.Clear();
                Console.WriteLine("Gewicht");
                Console.WriteLine("Was wollen Sie umrechnen?");
                Console.WriteLine("1. Kilogramm");
                Console.WriteLine("2. Pfund");

                keyInfo = Console.ReadKey(true);
                Console.Clear();
                Console.WriteLine("Geben Sie einen Wert ein:");
                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
                        do
                        {
                            Console.Write("Kilogramm: ");
                            string input = Console.ReadLine();
                            if (decimal.TryParse(input.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out eingegebeneGewicht))
                            {
                                error = false;
                                Console.WriteLine($"Pfund: {eingegebeneGewicht * 2.20462m}");
                            }
                            else
                            {
                                Console.WriteLine("Ungültige Eingabe. Bitte versuchen Sie es erneut.");
                                error = true;
                            }
                        } while (error);
                        break;
                    case ConsoleKey.D2:
                        do
                        {
                            Console.Write("Pfund: ");
                            string input = Console.ReadLine();
                            if (decimal.TryParse(input.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out eingegebeneGewicht))
                            {
                                error = false;
                                Console.WriteLine($"Pfund: {eingegebeneGewicht / 2.20462m}");
                            }
                            else
                            {
                                Console.WriteLine("Ungültige Eingabe. Bitte versuchen Sie es erneut.");
                                error = true;
                            }
                        } while (error);
                        break;
                }
                Console.ReadKey();
            } while (keyInfo.Key != ConsoleKey.Escape);
            
        }

        private static void Temperatur()
        {
            decimal eingegebeneTemperatur = 0;
            bool error = false;
            Console.Clear();
            Console.WriteLine("Temperatur");
            Console.WriteLine("Was wollen Sie umrechnen?");
            Console.WriteLine("1. Celsius");
            Console.WriteLine("2. Fahrenheit");
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            Console.Clear();
            Console.WriteLine("Geben Sie einen Wert ein:");
            switch( keyInfo.Key)
            {
                case ConsoleKey.D1:
                    Console.Write(" Celsius: ");
                    do
                    {
                        Console.Write("Celsius: ");
                        string input = Console.ReadLine();
                        if (decimal.TryParse(input.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out eingegebeneTemperatur))
                        {
                            error = false;
                            Console.WriteLine($" Fahrenheit: {eingegebeneTemperatur * 9 / 5 + 32}");
                        }
                        else
                        {
                            Console.WriteLine("Ungültige Eingabe. Bitte versuchen Sie es erneut.");
                            error = true;
                        }
                    } while (error);
                    
                    break;
                case ConsoleKey.D2:
                    do
                    {
                        Console.Write(" Fahrenheit: ");
                        string input = Console.ReadLine();
                        if (decimal.TryParse(input.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out eingegebeneTemperatur))
                        {
                            error = false;
                            Console.WriteLine($" Celsius: {(eingegebeneTemperatur - 32) * 5 / 9}");
                        }
                        else
                        {
                            Console.WriteLine("Ungültige Eingabe. Bitte versuchen Sie es erneut.");
                            error = true;
                        }
                    } while (error);
                    
                    break;
            }
            Console.ReadKey();
        }
    }
}