using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SamuraiNinja
{
    class FileCreator
    {

        private string path;
        List<Title> titles;
        List<Title> failed_titles;

        public FileCreator(List<Title> titles, List<Title> failed_titles)
        {
            path = Directory.GetCurrentDirectory();
            this.titles = titles;
            this.failed_titles = failed_titles;

        }

        public void sortTitles()
        {
            foreach(Title title in failed_titles)
            {
                if(title.Name != null && title.Region != null)
                {
                    if(title.Name.Length > 1)
                    {
                        title.Seed = "none";
                        title.Size = "unkown";
                        titles.Add(title);
                    }
                }
            }

            titles.OrderBy(o=>o.Type);
        }

        public void createXMLFile()
        {
            Console.WriteLine("Creating file...\n");

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.OmitXmlDeclaration = true;
            settings.NewLineOnAttributes = true;

            string type = "";

            int i = 0;

            using (XmlWriter writer = XmlWriter.Create("database.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("database");

                foreach (Title title in titles)
                {
                    if (title.Region.Equals("GB"))
                        title.Region = "EUR";
                    else if (title.Region.Equals("JP"))
                        title.Region = "JPN";
                    else if (title.Region.Equals("US"))
                        title.Region = "USA";
                    else if (title.Region.Equals("KR"))
                        title.Region = "KOR";
                    else if (title.Region.Equals("TW"))
                        title.Region = "TWN";

                    if (!title.Type.Equals(type))
                    {
                        type = title.Type;
                        writer.WriteComment(type);
                    }

                    writer.WriteStartElement("Ticket");

                    writer.WriteElementString("name", title.Name);
                    writer.WriteElementString("publisher", title.Publisher);
                    writer.WriteElementString("region", title.Region);
                    writer.WriteElementString("serial", title.Serial);
                    writer.WriteElementString("titleid", title.TitleID);
                    writer.WriteElementString("nsuid", title.NSUID);
                    writer.WriteElementString("seed", title.Seed);
                    writer.WriteElementString("size", title.Size);

                    i++;
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }


            Console.WriteLine("File created! ("+ i +" Titles)\n\n\nPress any key to exit...");
        }

    }
}
