using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuraiNinja
{
    class Menu
    {

        private bool thorough = true;

        public Menu()
        {

        }

        public void showMenu()
        {
            Console.WriteLine("Welcome to the 3DS Title Database Creator!\n\n");
            Console.WriteLine("[1]        Thorough mode              [ ]");
            Console.WriteLine("[Enter]    Create Database\n");
            Console.Write("Press key to continue: ");
            
            while(true)
            {
                ConsoleKeyInfo cki = Console.ReadKey();
                string key = cki.Key.ToString();

                if(key.Equals("D1") || key.Equals("NumPad1"))
                {
                    if (thorough)
                    {
                        Console.SetCursorPosition(39,3);
                        Console.Write("x]");
                        thorough = false;
                    }
                    else
                    {
                        Console.SetCursorPosition(39, 3);
                        Console.Write(" ]");
                        thorough = true;
                    }
                        
                }

                if(cki.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    break;
                }

                Console.SetCursorPosition(23, 6);
            }

        }

        public bool GetThorough()
        {
            return !thorough;
        }

    }
}
