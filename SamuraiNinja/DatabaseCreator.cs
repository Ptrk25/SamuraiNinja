using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SamuraiNinja
{
    class DatabaseCreator
    {
        private List<Title> titles = new List<Title>();
        private List<Title> failed_titles = new List<Title>();
        private List<Tuple<string, string>> types = new List<Tuple<string, string>>();

        private uint title_num = 0x30000;
        private ushort num_threads = 0;
        private uint curr_num = 0;
        private uint failed = 0;
        private readonly ushort MAX_TITLES = 30000;
        private readonly ushort MAX_THREADS = 15;
        private uint listsize = 0;

        public DatabaseCreator()
        {
            types.Add(Tuple.Create("eShopApp", "00040000"));
            types.Add(Tuple.Create("Demo", "00040002"));
            types.Add(Tuple.Create("UpdatePatch", "0004000E"));
            num_threads = MAX_THREADS;
        }

        //Search for TitleIDs
        public void SearchTitleIDs()
        {
            updateMenu();

            Console.WriteLine(string.Format("Searching for TitleIDs via Ninja... ({0} TitleIDs)\n", (MAX_TITLES*3)));
            foreach (Tuple<string, string> type in types)
            {
                if (type.Item1.Equals("Demo"))
                    title_num = 0x30001;
                else if (type.Item1.Equals("UpdatePatch"))
                    title_num = 0x20000;
                else
                    title_num = 0x30000;

                for(ushort i = 0; i < MAX_THREADS/3; i++)
                {
                    //Theads in Liste!!
                    NinjaWorker ninjaw = new NinjaWorker(title_num, (uint)(MAX_TITLES/(MAX_THREADS/3)), type.Item1, new TitlelistCallback(ProcessThreadTitles), new CurrentStatusCallback(ProcessThreadCounter), new FailCallback(ProcessFailCounter));

                    Thread t = new Thread(new ThreadStart(ninjaw.GetNSUIDs));
                    t.Start();
                    title_num += (uint) (MAX_TITLES / (MAX_THREADS / 3))*0x100 + 0x100;

                    Console.WriteLine(string.Format("Thread started! Type: {0} {1}",type.Item1, i));
                }
                
            }
            Console.WriteLine();
            while (num_threads > 0) { Thread.Sleep(1000); }
            Console.WriteLine(string.Format("\n\nFound {0} TitleIDs on Ninja!\n", titles.Count));
            Thread.Sleep(1000);
        }

        //Get Regions for TitleIDs
        public void SetRegions()
        {
            failed = 0;
            updateMenu();

            Console.WriteLine(string.Format("Setting Regions for {0} TitleIDs via IDBE...\n", titles.Count));

            var lltitles = new List<List<Title>>();
            lltitles = SplitList<Title>(titles, 400);
            num_threads = (ushort) lltitles.Count;
            int i = 0;
            listsize = (uint) titles.Count;

            titles.Clear();
            curr_num = 0;

            foreach(List<Title> partlist in lltitles)
            {
                IDBEWorker idbew = new IDBEWorker(partlist, new TitlelistCallback(ProcessThreadTitles), new CurrentStatusCallback(ProcessIDBECounter), new FailCallback(ProcessFailCounter));
                Thread t = new Thread(new ThreadStart(idbew.GetRegions));
                t.Start();
                Console.WriteLine(string.Format("Thread started! Count: {0}", i));
                i++;
            }
            Console.WriteLine();
            while (num_threads > 0) { Thread.Sleep(1000); }
            Console.WriteLine("\n\nRegions setted!\n");
            Thread.Sleep(1000);
           
        }

        //Sets Metadata for each Title
        public void SetMetadata()
        {
            failed = 0;
            updateMenu();

            Console.WriteLine(string.Format("Setting Metadata for {0} TitleIDs via Samurai...\n", titles.Count));
            var lltitles = new List<List<Title>>();
            lltitles = SplitList<Title>(titles, 400);
            num_threads = (ushort) lltitles.Count;
            int i = 0;
            listsize = (uint)titles.Count;

            titles.Clear();
            curr_num = 0;

            foreach(List<Title> partlist in lltitles)
            {
                SamuraiWorker samuw = new SamuraiWorker(partlist, new TitlelistCallback(ProcessThreadTitles), new CurrentStatusCallback(ProcessIDBECounter), new FailCallback(ProcessFailCounter));
                Thread t = new Thread(new ThreadStart(samuw.SetMetadata));
                t.Start();
                Console.WriteLine(string.Format("Thread started! Count: {0}", i));
                i++;
            }
            Console.WriteLine();
            while (num_threads > 0) { Thread.Sleep(1000); }
            Console.WriteLine("\n\nMetadata setted!\n");
            Thread.Sleep(1000);
        }

        //Gets Seed and Size of an Title
        public void SetSeedAndSize()
        {
            failed = 0;
            updateMenu();

            Console.WriteLine(string.Format("Setting Seed and Size for {0} TitleIDs via Ninja...\n", titles.Count));
            var lltitles = new List<List<Title>>();
            lltitles = SplitList<Title>(titles, 400);
            num_threads = (ushort)lltitles.Count;
            int i = 0;
            listsize = (uint)titles.Count;

            titles.Clear();
            curr_num = 0;

            foreach (List<Title> partlist in lltitles)
            {
                SamuraiWorker ninjw = new SamuraiWorker(partlist, new TitlelistCallback(ProcessThreadTitles), new CurrentStatusCallback(ProcessIDBECounter), new FailCallback(ProcessFailCounter));
                Thread t = new Thread(new ThreadStart(ninjw.SetSeedAndSize));
                t.Start();
                Console.WriteLine(string.Format("Thread started! Count: {0}", i));
                i++;
            }
            Console.WriteLine();
            while (num_threads > 0) { Thread.Sleep(1000); }
            Console.WriteLine("\n\nSeed and Size setted!\n");
            Thread.Sleep(1000);
            updateMenu();
        }

        //Return function
        public List<Title> GetTitles()
        {
            return titles;
        }

        public List<Title> GetFailedTitles()
        {
            return failed_titles;
        }

        //Menu
        private void updateMenu()
        {
            Console.Clear();
            Console.WriteLine("3DS Title Database Creator\n");
            Console.WriteLine(" --------------------------------");
            Console.WriteLine(string.Format(" | Titles:  {0}", titles.Count));
            Console.WriteLine(string.Format(" | Failed:  {0}", failed_titles.Count));
            Console.SetCursorPosition(32,3);
            Console.Write("|");
            Console.SetCursorPosition(32, 4);
            Console.Write("|");
            Console.SetCursorPosition(0, 5);
            Console.WriteLine(" --------------------------------\n");
        }

        //Threadfunctions
        private void ProcessThreadTitles(List<Title> titles)
        {
            this.titles.AddRange(titles);
            num_threads--;
        }

        private void ProcessThreadCounter()
        {
            curr_num++;
            Console.Write(string.Format("\r{0} of {1} completed ({2} Titles found) ({3:0.00} %)", curr_num, MAX_TITLES*3, curr_num-failed, (float)curr_num/((float)MAX_TITLES*3)*100));
        }

        private void ProcessIDBECounter()
        {
            curr_num++;
            Console.Write(string.Format("\r{0} of {1} completed ({2} failed) ({3:0.00} %)", curr_num, listsize, failed, (float)curr_num / (float) listsize * 100));
        }

        private void ProcessFailCounter(Title title)
        {
            if(title != null)
            {
                failed_titles.Add(title);
            }
            failed++;
        }

        //Listsplitter
        private List<List<T>> SplitList<T>(List<T> tilist, int size)
        {
            var chunks = new List<List<T>>();
            var chunkCount = tilist.Count() / size;

            if (tilist.Count % size > 0)
                chunkCount++;

            for (var i = 0; i < chunkCount; i++)
                chunks.Add(tilist.Skip(i*size).Take(size).ToList());

            return chunks;
        }

    }
}
