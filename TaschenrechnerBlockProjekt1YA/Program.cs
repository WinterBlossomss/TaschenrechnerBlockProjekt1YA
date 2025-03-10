using System.ComponentModel;
using Mathe;
namespace TaschenrechnerBlockProjekt1YA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool comma = false;
            char pastinput = 's';
            decimal temp = '0';
            decimal sum = 0 ;
            Stack<char> stack = new Stack<char>();
            string userview = "";
            Taschenrechner rechner = new Taschenrechner();
            ConsoleKeyInfo keyInfo;
            do
            {
                decimal currentinput = 0;
                keyInfo = Console.ReadKey();
                userview = userview + keyInfo.KeyChar.ToString();
                if(comma)
                {
                    string tempcomma = pastinput.ToString() + keyInfo.KeyChar;
                    while(!rechner.IsOperator(keyInfo.KeyChar))
                    {
                        keyInfo = Console.ReadKey();
                        stack.Push(keyInfo.KeyChar);
                        tempcomma = tempcomma + keyInfo.KeyChar;
                    }
                    currentinput = Convert.ToDecimal(tempcomma);
                    comma = false;
                }
                
                rechner.Validate(pastinput, keyInfo.KeyChar);
                if (rechner.IsNumber(keyInfo.KeyChar) && pastinput == 's') //First input
                {
                    currentinput = Convert.ToDecimal(keyInfo.KeyChar.ToString());
                    temp = currentinput; //Save the number
                    pastinput = Convert.ToChar(currentinput.ToString());
                }
                else if(rechner.IsNumber(keyInfo.KeyChar) && rechner.IsNumber(pastinput)) // Current Input is Number && Past input is Number
                {
                    decimal doubledigits = Convert.ToDecimal(keyInfo.KeyChar.ToString());
                    pastinput = (char)Convert.ToDecimal(pastinput.ToString());
                    currentinput = pastinput * 10 + doubledigits;

                    (sum, pastinput, temp) = Aussrechnen(sum, pastinput, temp, currentinput, userview, rechner);
                }
                else if (rechner.IsNumber(keyInfo.KeyChar) && rechner.IsOperator(pastinput)) // Current Input is Number && Past input Operator
                {
                    currentinput = Convert.ToDecimal(keyInfo.KeyChar.ToString());

                    (sum,pastinput,temp) = Aussrechnen(sum, pastinput, temp, currentinput, userview, rechner);
                }
                else if (rechner.IsOperator(keyInfo.KeyChar) && rechner.IsNumber(pastinput)) //Current Input Operator && Past input is Number
                {
                    stack.Push(pastinput);
                    pastinput = keyInfo.KeyChar;
                    stack.Push(keyInfo.KeyChar);
                }
                else if(Char.IsPunctuation(keyInfo.KeyChar) && rechner.IsNumber(pastinput)) //Current Input Comma && Past input number
                {
                    //string tempcomma = pastinput.ToString() + keyInfo.KeyChar;
                    //while(!rechner.IsOperator(keyInfo.KeyChar))
                    //{
                    //    keyInfo = Console.ReadKey();
                    //    stack.Push(keyInfo.KeyChar);
                    //    tempcomma = tempcomma + keyInfo.KeyChar;
                    //}
                    //currentinput = Convert.ToDecimal(tempcomma);
                    comma = true;
                }
                else if (keyInfo.KeyChar == 'c') //Clear
                {
                    pastinput = 's';
                    Console.Clear();
                    stack.Clear();
                    Console.WriteLine("Cleared");
                }
                else if (keyInfo.Key == ConsoleKey.Backspace) //Erase
                {
                    userview = userview.Remove(userview.Length - 2);
                    pastinput = userview.Last();
                    Console.Clear();
                    Console.Write(userview);
                }
                else if (keyInfo.KeyChar == '=')
                {
                    Console.Clear();
                    Console.WriteLine(sum);
                    userview = sum.ToString();
                    pastinput = 's';
                }

            } while(keyInfo.Key != ConsoleKey.Escape);
        }

        public static (decimal sum, char pastinput, decimal temp) Aussrechnen(decimal sum, char pastinput,decimal temp,decimal currentinput, string userview,Taschenrechner rechner)
        {
            sum = rechner.CheckOperator(pastinput, temp, currentinput);
            Console.Clear();
            Console.Write(sum + "\n" + userview);

            pastinput = Convert.ToChar(currentinput.ToString());
            temp = sum;

            return (sum, pastinput, temp);
        }
    }
}
