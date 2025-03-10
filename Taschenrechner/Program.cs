using System.Numerics;
using Mathe;
namespace Taschenrechner1YA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleKeyInfo keyInfo = new();
            Taschenrechner rechner = new();
            decimal tempdec = 0;
            //char tempchar = 's';
            char op = '+';
            char lastinput = 'a';
            char currentinput = 'w';
            string userview = "";
            Stack<char> calc = new();
            do
            {
                keyInfo = Console.ReadKey();
                if (!rechner.Validate(lastinput, keyInfo.KeyChar))
                    Console.WriteLine("\b");
                currentinput = keyInfo.KeyChar;
                userview = userview + currentinput.ToString();
                if (keyInfo.Key == ConsoleKey.Delete)
                {
                    calc.Pop();
                    userview.Substring(0, userview.Length - 1);
                    Console.WriteLine("\b");
                }
                else if(rechner.IsNumber(currentinput))
                {
                    if (rechner.IsNumber(lastinput))
                    {
                        tempdec = Convert.ToDecimal(tempdec.ToString() + currentinput.ToString());
                    }
                    else
                    {
                        calc.Push(Convert.ToChar(tempdec));
                    }
                    switch (op)
                    {
                        case '+':
                            {
                                tempdec = rechner.Addition(tempdec, currentinput); break;
                            }
                        case '-':
                            {
                                tempdec = rechner.Addition(tempdec, currentinput); break;
                            }
                        case '*':
                            {
                                tempdec = rechner.Addition(tempdec, currentinput); break;
                            }
                        case '/':
                            {
                                tempdec = rechner.Addition(tempdec, currentinput); break;
                            }
                    }
                }
                else
                {
                    currentinput = op;
                }

            } while (keyInfo.Key != ConsoleKey.Escape);

        }
    }
}
