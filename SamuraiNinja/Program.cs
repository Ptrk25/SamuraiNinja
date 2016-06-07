using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SamuraiNinja
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the 3DS Title Database Creator!\n\n");

            DatabaseCreator dbc = new DatabaseCreator();
            dbc.SearchTitleIDs();
            dbc.SetRegions();

            Console.ReadKey();
        }
    }
}
