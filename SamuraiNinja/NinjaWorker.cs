using System.Collections.Generic;

namespace SamuraiNinja
{
    class NinjaWorker
    {
        private uint start;
        private uint count;
        private string type;
        private TitlelistCallback callback;
        private CurrentStatusCallback callint;
        private FailCallback callfail;

        public NinjaWorker(uint start, uint count, string type, TitlelistCallback callback, CurrentStatusCallback callint, FailCallback callfail)
        {
            this.start = start;
            this.count = count;
            this.type = type;
            this.callback = callback;
            this.callint = callint;
            this.callfail = callfail;
        }

        public void GetNSUIDs()
        {
            NinjaSamurai ninja = new NinjaSamurai();
            List<Title> titles = new List<Title>();
            string title_base = null;

            if (type.Equals("eShopApp"))
                title_base = "00040000";
            else if (type.Equals("Demo"))
                title_base = "00040002";
            else if (type.Equals("UpdatePatch"))
                title_base = "0004000E";

            for(uint i = start; i < start+count*0x100; i += 0x100)
            {
                string title_id = title_base + i.ToString("X8");
                string ns_id = ninja.GetNSUID(title_id);
                if (ns_id.Length == 14)
                {
                    callint();
                    titles.Add(new Title(title_id, ns_id, type));
                    //Console.WriteLine(string.Format("TID: {0}   NSIUD: {1}  Type: {2}", title_id, ns_id, type));
                }
                else
                {
                    callint();
                    callfail(null);
                    //Console.WriteLine(string.Format("TID: {0}   NSIUD: None            Type: {1}", title_id, type));
                }
                    
            }

            callback(titles);
        }
    }
}

public delegate void TitlelistCallback(List<SamuraiNinja.Title> titles);

public delegate void CurrentStatusCallback();

public delegate void FailCallback(SamuraiNinja.Title title);