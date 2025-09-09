using System.Text.RegularExpressions;

HttpClient _http = new HttpClient();

var url = $"http://medex.com.bd/ajax/search?searchtype=search&searchkey={"xyzv"}";
var html = await _http.GetStringAsync(url);

var data = html.Split("</a>");

string pattern = @"<a\s+href=""(?<url>[^""]+)"".*?<li\s+title=""(?<title>[^""]+)"".*?<img\s+src=""(?<img>[^""]+)""[^>]*>.*?<span>\s*(?<name>.*?)\s*<span[^>]*>(?<strength>[^<]+)</span>";


foreach (var item in data)
{
    var match = Regex.Match(item, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

    if (match.Success)
    {
        string url1 = match.Groups["url"].Value;
        string title = match.Groups["title"].Value;
        string img = match.Groups["img"].Value;
        string name = match.Groups["name"].Value.Trim();
        string strength = match.Groups["strength"].Value.Trim();
        string fullName = $"{name} {strength}";

        Console.WriteLine($"URL: {url1}");
        Console.WriteLine($"Title: {title}");
        Console.WriteLine($"Image URL: {img}");
        Console.WriteLine($"Medicine Name: {fullName}");
    }
}