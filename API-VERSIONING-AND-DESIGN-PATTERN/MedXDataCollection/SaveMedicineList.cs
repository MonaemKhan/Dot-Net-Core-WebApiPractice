using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MedXDataCollection
{
    public class SaveMedicineList
    {
        public async Task SaveMedicinne()
        {
            HttpClient _http = new HttpClient();

            HashSet<DataCollection> dataCollections = new HashSet<DataCollection>();

            string pattern = @"<a\s+href=""(?<url>[^""]+)"".*?<li\s+title=""(?<title>[^""]+)"".*?<img\s+src=""(?<img>[^""]+)""[^>]*>.*?<span>\s*(?<name>.*?)\s*<span[^>]*>(?<strength>[^<]+)</span>";

            for (char c = 'a'; c <= 'z'; c++)
            {
                string searchstr = c + "";
                for (char d = 'a'; d <= 'z'; d++)
                {
                    string searchstr2 = d + "";
                    if (searchstr != searchstr2)
                    {
                        searchstr2 = searchstr + searchstr2;
                    }
                    //Console.WriteLine(searchstr2);

                    var url = $"http://medex.com.bd/ajax/search?searchtype=search&searchkey={searchstr2}";

                    var html = await _http.GetStringAsync(url);

                    var data = html.Split("</a>");


                    foreach (var item in data)
                    {
                        var match = Regex.Match(item, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

                        if (match.Success)
                        {
                            var dataCollection = new DataCollection
                            {
                                URL = match.Groups["url"].Value,
                                TITLE = match.Groups["title"].Value,
                                IMAGE_URL = match.Groups["img"].Value,
                                NAME = match.Groups["name"].Value.Trim(),
                                STREANGTH = match.Groups["strength"].Value.Trim(),
                            };
                            dataCollection.FULLNAMESET();
                            dataCollections.Add(dataCollection);
                        }
                    }

                    for (char e = 'a'; e <= 'z'; e++)
                    {
                        string searchstr3 = e + "";
                        if (searchstr2 != searchstr3)
                        {
                            searchstr3 = searchstr2 + searchstr3;
                        }
                        if (searchstr3.Length < 3)
                            break;
                        url = "";
                        html = "";
                        data = null;

                        url = $"http://medex.com.bd/ajax/search?searchtype=search&searchkey={searchstr3}";

                        html = await _http.GetStringAsync(url);

                        data = html.Split("</a>");


                        foreach (var item in data)
                        {
                            var match = Regex.Match(item, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

                            if (match.Success)
                            {
                                var dataCollection = new DataCollection
                                {
                                    URL = match.Groups["url"].Value,
                                    TITLE = match.Groups["title"].Value,
                                    IMAGE_URL = match.Groups["img"].Value,
                                    NAME = match.Groups["name"].Value.Trim(),
                                    STREANGTH = match.Groups["strength"].Value.Trim(),
                                };
                                dataCollection.FULLNAMESET();
                                dataCollections.Add(dataCollection);
                            }
                        }

                    }
                    url = "";
                    html = "";
                    data = null;
                }
                Console.WriteLine(c + " Finished !!!!");
            }

            string filePath = @"D:\Github Repos\Dot-Net-Core-WebApiPractice\API-VERSIONING-AND-DESIGN-PATTERN\MedXDataCollection\jsonData\medicineTempList.json";

            // JSON options (pretty print)
            var options = new JsonSerializerOptions { WriteIndented = true };

            // Serialize list to JSON
            string json = JsonSerializer.Serialize(dataCollections, options);

            // Save JSON to file
            File.WriteAllText(filePath, json, Encoding.UTF8);

            Console.WriteLine($"JSON file saved at {filePath}");

            Console.WriteLine($"Total Medicines: {dataCollections.Count}");
        }
    }
}
