using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuraiNinja
{
    class SamuraiWorker
    {

        private List<Title> titles;
        private List<Title> new_titles = new List<Title>();
        private TitlelistCallback calllist;
        private CurrentStatusCallback callint;
        private FailCallback callfail;

        public SamuraiWorker(List<Title> titles, TitlelistCallback calllist, CurrentStatusCallback callint, FailCallback callfail)
        {
            this.titles = titles;
            this.calllist = calllist;
            this.callint = callint;
            this.callfail = callfail;
        }

        public void SetMetadata()
        {
            NinjaSamurai samurai = new NinjaSamurai();

            for (ushort i = 0; i < titles.Count; i++)
            {
                Title curr_title = titles[i];
                samurai.SetMetadata(curr_title, out curr_title);

                if(curr_title.Name.Length < 1)
                {
                    callint();
                    callfail(curr_title);
                    continue;
                }

                new_titles.Add(curr_title);
                callint();
            }
            calllist(new_titles);
        }

        public void SetSeedAndSize()
        {
            NinjaSamurai samurai = new NinjaSamurai();

            for (ushort i = 0; i < titles.Count; i++)
            {
                Title curr_title = titles[i];
                samurai.SetSeedAndSize(curr_title, out curr_title);

                if(curr_title.Size == null)
                {
                    callint();
                    callfail(curr_title);
                    continue;
                }

                new_titles.Add(curr_title);
                callint(); ;
            }
            calllist(new_titles);
        }
        
    }
}
