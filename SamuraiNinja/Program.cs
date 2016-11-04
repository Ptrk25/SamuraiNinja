using System;
using System.Collections.Generic;
using System.IO;
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

            if (File.Exists("ctr-common.pfx"))
            {
                Menu menu = new Menu();
                menu.showMenu();

                DatabaseCreator dbc = new DatabaseCreator();
                dbc.SearchTitleIDs();
                dbc.SetRegions();
                dbc.SetMetadata();
                dbc.SetSeedAndSize();

                List<Title> titles = dbc.GetTitles();
                List<Title> failed_titles = dbc.GetFailedTitles();

                FileCreator filec = new FileCreator(titles, failed_titles);
                filec.sortTitles();
                filec.createXMLFile();
            }
            else
            {
                Console.WriteLine("ctr-common.pfx not found!\n\nPress any key to exit...");
            }
            

            Console.ReadKey();
        }
    }
}
