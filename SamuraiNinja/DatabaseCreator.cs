using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SamuraiNinja
{
    class DatabaseCreator
    {
        private List<Title> titles = new List<Title>();
        private List<Tuple<string, string>> types = new List<Tuple<string, string>>();

        private uint title_num = 0x30000;
        private ushort num_threads = 0;
        private uint curr_num = 0;
        private uint failed = 0;
        private readonly ushort MAX_TITLES = 15000;
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
            failed = 0;

            Console.WriteLine("Searching for TitleIDs via Ninja... (45000 TitleIDs)\n");
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
            Console.WriteLine(string.Format("\nFound {0} TitleIDs on Ninja!\n", titles.Count));
        }

        //Get Regions for TitleIDs
        public void SetRegions()
        {
            failed = 0;

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
            Console.WriteLine("\nRegions setted!");
           
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

        private void ProcessFailCounter()
        {
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
