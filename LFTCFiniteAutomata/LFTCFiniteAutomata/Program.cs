using LFTCFiniteAutomata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFTCFiniteAutomata
{    
    class Program
    {
        static void PrintMenu()
        {
            Console.WriteLine("Choose element to display:");
            Console.WriteLine("1.List of states");
            Console.WriteLine("2.Alphabet");
            Console.WriteLine("3.Transitions");
            Console.WriteLine("4.Initial state");
            Console.WriteLine("5.Final states");
        }

        static void Main(string[] args)
        {
            var fa = new FSA("D:/Projects/personal/College/LFTCFiniteAutomata/LFTCFiniteAutomata/FA.txt");
            fa.Accepts("1014");
            bool done = false;
            while (!done)
            {
                PrintMenu();
                var cmd = Console.ReadKey().KeyChar;
                switch (cmd)
                {
                    case '1':
                        Console.WriteLine();
                        fa.PrintQ();
                        Console.WriteLine("----------------------------------------------------------------");
                        break;
                    case '2':
                        Console.WriteLine();
                        fa.PrintSigma();
                        Console.WriteLine("----------------------------------------------------------------");
                        break;
                    case '3':
                        Console.WriteLine();
                        fa.PrintDelta();
                        Console.WriteLine("----------------------------------------------------------------");
                        break;
                    case '4':
                        Console.WriteLine();
                        fa.PrintQ0();
                        Console.WriteLine("----------------------------------------------------------------");
                        break;
                    case '5':
                        Console.WriteLine();
                        fa.PrintF();
                        Console.WriteLine("----------------------------------------------------------------");
                        break;
                    case '0':
                        done = true;
                        break;
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            }
        }
    }
}
