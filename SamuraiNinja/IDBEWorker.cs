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
        private List<Title> new_titles = new List<Title>();
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
                Title newTitle;
                icon.SetRegion(titles[i], out newTitle);

                if (newTitle != null)
                {
                    new_titles.Add(newTitle);
                }
                callint();
            }
            calllist(new_titles);
        }
    }
}
