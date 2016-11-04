namespace SamuraiNinja
{
    public class Title
    {
        public Title(string TitleID, string NSUID, string Type)
        {
            this.TitleID = TitleID;
            this.NSUID = NSUID;
            this.Type = Type;
        }

        public string Name { get; set; }
        public string Publisher { get; set; }
        public string Region { get; set; }
        public string Serial { get; set; }
        public string Type { get; set; }
        public string TitleID { get; set; }
        public string NSUID { get; set; }
        public string Seed { get; set; }
        public string Size { get; set; }
    }
}
