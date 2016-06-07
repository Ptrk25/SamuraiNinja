
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Threading;

namespace SamuraiNinja
{
    public class NinjaSamurai
    {
        private readonly ushort MAX_TRIES = 10;

        public NinjaSamurai()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        public string GetNSUID(string tid)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2 certificate = store.Certificates[0];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://ninja.ctr.shop.nintendo.net/ninja/ws/titles/id_pair?title_id[]=" + tid);
            req.ClientCertificates.Add(certificate);

            req.UserAgent = "API Client";
            req.Accept = "application/xml";
            req.Method = WebRequestMethods.Http.Get;

            string result = "";

            for (int i = 0; i < MAX_TRIES; i++)
            {
                try
                {
                    using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                    {
                        StreamReader reader = new StreamReader(resp.GetResponseStream());
                        result = reader.ReadToEnd();
                    }
                    break;
                }
                catch (WebException)
                {
                    Thread.Sleep(1000);
                }
            }

            if (result.Equals(""))
                return "Error";
            
            XDocument doc = XDocument.Parse(result);
            var nsuids = doc.Descendants("ns_uid");
            foreach (var nsuid in nsuids)
            {
                result = nsuid.Value;
            }
            return result;
        }

        public Tuple<string,string> GetSeedAndSize(string tid)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2 certificate = store.Certificates[0];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://ninja.ctr.shop.nintendo.net/ninja/ws/DE/title/" + tid + "/ec_info");
            req.ClientCertificates.Add(certificate);

            req.UserAgent = "API Client";
            req.Accept = "application/xml";
            req.Method = WebRequestMethods.Http.Get;

            string result = "";

            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                StreamReader reader = new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
            }

            XDocument doc = XDocument.Parse(result);
            var dSize = doc.Descendants("content_size");
            var dSeed = doc.Descendants("external_seed");
            string Size = null;
            string Seed = null;
            foreach (var sSize in dSize)
            {
                Size = sSize.Value;
            }
            foreach (var sSeed in dSeed)
            {
                Seed = sSeed.Value;
            }
            return Tuple.Create(Seed,Size);
        }

        public void SetMetadata(Title oldTitle, out Title newTitle)
        {
            newTitle = new Title(oldTitle.TitleID, oldTitle.NSUID, oldTitle.Type);
            newTitle.Region = oldTitle.Region;
            newTitle.Size = oldTitle.Size;
            newTitle.Seed = oldTitle.Seed;

            WebClient client = new WebClient();
            string xml = client.DownloadString(string.Format("https://samurai.ctr.shop.nintendo.net/samurai/ws/{0}/title/{1}", oldTitle.Region, newTitle.NSUID));

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XPathNavigator navigator = doc.CreateNavigator();

            if (oldTitle.Type.Equals("Demo"))
            {
                byte[] bytes = Encoding.Default.GetBytes(GetStringValues(navigator, "//eshop/title/demo_titles/demo_title/name"));
                newTitle.Name = Encoding.UTF8.GetString(bytes);
                bytes = Encoding.Default.GetBytes(GetStringValues(navigator, "//eshop/title/publisher/name"));
                newTitle.Publisher = Encoding.UTF8.GetString(bytes);
                newTitle.Serial = GetStringValues(navigator, "//eshop/title/product_code");
            }
            else
            {
                byte[] bytes = Encoding.Default.GetBytes(GetStringValues(navigator, "//eshop/title/name"));
                newTitle.Name = Encoding.UTF8.GetString(bytes);
                bytes = Encoding.Default.GetBytes(GetStringValues(navigator, "//eshop/title/publisher/name"));
                newTitle.Publisher = Encoding.UTF8.GetString(bytes);
                newTitle.Serial = GetStringValues(navigator, "//eshop/title/product_code");
            }
            return;
        }

        private string GetStringValues(XPathNavigator navigator, string xpath)
        {
            StringBuilder sb = new StringBuilder();
            XPathNodeIterator bookNodesIterator = navigator.Select(xpath);
            while (bookNodesIterator.MoveNext())
                sb.Append(string.Format("{0}", bookNodesIterator.Current.Value));
            return sb.ToString();
        }
    }
}
