# SamuraiNinja
Tool to get current 3DS eShop Titles from Nintendo

Currently incompleted.

#### Important Classes

##### Class DatabaseCreator.cs
**Functions**
- void SearchTitleIDs()
- void SetRegions()

##### Class NinjaSamurai.cs
**Functions**
- string GetNSUID(string tid)
 * Get NSUID from TitleID as String
- Tuple<string, string> GetSeedAndSize(string tid)
 * Get Seed and Size from a specific TitleID as Tuple (string seed, string size)
- void SetMetadata(Title oldTitle, out Title newTitle)
 * Set (Demo)Title, Publisher, Serial of a Title

##### Class IconRetriever.cs
**Functions**
- string GetRegion(string TitleID)
 * Returns Region as string (JP, US, GB, HK, KR, TW, ALL)

##### Class Title.cs
**Variables**
- string Name
- string Publisher
- string Region
- string Type
- string TitleID
- string NSUID
- string Seed
- string Size
- bool? Available
