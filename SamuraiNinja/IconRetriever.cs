using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SamuraiNinja
{
    class IconRetriever
    {
        private byte[] IV;
        private byte[][] DecryptionKeys;
        List<bool> regions;
        private FailCallback callfail;

        public IconRetriever(FailCallback callfail)
        {
            IV = new byte[16]{164,105,135,174,71,216,43,180,250,138,188,4,80,40,95,164};
            DecryptionKeys = new byte[4][]
            {
                new byte[16]{74,185,164,14,20,105,117,168,75,177,180,243,236,239,196,123},
                new byte[16]{144,160,187,30,14,134,74,232,125,19,166,160,61,40,201,184},
                new byte[16]{byte.MaxValue,187,87,193,78,152,236,105,117,179,132,252,244,7,134,181},
                new byte[16]{128,146,55,153,180,31,54,166,167,95,184,180,140,149,246,111}
            };
            regions = new List<bool>();
            this.callfail = callfail;
        }

        public string GetRegion(String TitleID)
        {
            string address = string.Format("https://idbe-ctr.cdn.nintendo.net/icondata/10/{0}.idbe", TitleID);
            byte[] data;
            regions.Clear();

            try
            {
                using (WebClient webClient = new WebClient())
                data = webClient.DownloadData(address);
            }
            catch (WebException)
            {
                //Console.WriteLine("[Icon]: Unable to find requested TitleID.");
                callfail();
                return null;
            }

            byte[] Encrypted = new byte[data.Length - 2];
            Array.Copy((Array)data, 2, (Array)Encrypted, 0, Encrypted.Length);
            byte[] Decrypted = this.AESDecrypt(Encrypted, this.DecryptionKeys[(int)data[1]], this.IV);

            uint num1 = BitConverter.ToUInt32(Decrypted, 48);
            for (int i = 0; i < 7; i++)
                regions.Add(((int)(num1 >> i) & 1) == 1);

            for (int i = 0; i < regions.Count; i++)
            {
                if (regions[i])
                {
                    switch (i)
                    {
                        case 0:
                            return "JP";
                        case 1:
                            return "US";
                        case 2:
                            return "GB";
                        case 3:
                            return "GB";
                        case 4:
                            return "HK";
                        case 5:
                            return "KR";
                        case 6:
                            return "TW";
                        default:
                            return "ALL";
                    }
                }
            }
            return null;
        }

        private byte[] AESDecrypt(byte[] Encrypted, byte[] key, byte[] iv)
        {
            byte[] numArray = new byte[Encrypted.Length];
            using (AesManaged aesManaged = new AesManaged())
            {
                aesManaged.Key = key;
                aesManaged.IV = iv;
                aesManaged.Padding = PaddingMode.None;
                aesManaged.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = aesManaged.CreateDecryptor();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Write))
                        cryptoStream.Write(Encrypted, 0, Encrypted.Length);
                    numArray = memoryStream.ToArray();
                }
            }
            return numArray;
        }
    }
}
