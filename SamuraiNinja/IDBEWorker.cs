using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuraiNinja
{
    class IDBEWorker
    {
        private List<Title> titles;
        private TitlelistCallback calllist;
        private CurrentStatusCallback callint;
        private FailCallback callfail;

        public IDBEWorker(List<Title> titles, TitlelistCallback calllist, CurrentStatusCallback callint, FailCallback callfail)
        {
            this.titles = titles;
            this.calllist = calllist;
            this.callint = callint;
            this.callfail = callfail;
        }

        public void GetRegions()
        {
            IconRetriever icon = new IconRetriever(callfail);

            for (ushort i = 0; i < titles.Count; i++)
            {
                titles[i].Region = icon.GetRegion(titles[i].TitleID);
                
                if (titles[i].Region == null)
                    titles[i].Available = false;

                callint();
            }
            calllist(titles);
        }
    }
}
