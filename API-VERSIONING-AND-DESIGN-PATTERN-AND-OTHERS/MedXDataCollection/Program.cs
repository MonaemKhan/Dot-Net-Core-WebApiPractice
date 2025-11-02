using MedXDataCollection;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

class Program
{
    static int count = 0;
    static List<htmlData> htmlDatas = new List<htmlData>();
    static async Task Main()
    {
        string filePath = @"D:\Github Repos\Dot-Net-Core-WebApiPractice\API-VERSIONING-AND-DESIGN-PATTERN\MedXDataCollection\jsonData\medicineTempList.json";
        string jsonString = File.ReadAllText(filePath);

        List<DataCollection>? dataCollections = JsonSerializer.Deserialize<List<DataCollection>>(jsonString);
        count = dataCollections.Count;
        // Use thread-safe collection instead of List<>
        

        // Limit concurrency
        var semaphore = new SemaphoreSlim(1000);

        var tasks = new List<Task>();
        int index = 0;
        foreach (var medlist in dataCollections.Take(1))
        {
            var result = await new HTMLConverter()
                                 .URL(medlist.URL)
                                 .SelectTagName("section")
                                 .StartProcess();
            var data = await result.GetTagListData();
            htmlDatas.Add(new htmlData
            {
                MedicineName = medlist.FULLNAME,
                BnOrEn = "en",
                HTMLTagDetails = data
            });
            CompleteTask();
        }

        filePath = @"D:\Github Repos\Dot-Net-Core-WebApiPractice\API-VERSIONING-AND-DESIGN-PATTERN\MedXDataCollection\jsonData\medicineHTMLTempList.json";

        // JSON options (pretty print)
        var options = new JsonSerializerOptions { WriteIndented = true };

        // Serialize list to JSON
        string json = JsonSerializer.Serialize(htmlDatas, options);

        // Save JSON to file
        File.WriteAllText(filePath, json, Encoding.UTF8);

        Console.WriteLine($"Processed {htmlDatas.Count} entries.");
    }

    static void CompleteTask()
    {
        count--;
        Console.WriteLine($"Remaining: {count}");
    }

    static async Task ProcessUrl(string url, string medName, 
        string lang, SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();
        try
        {
            
            //Console.WriteLine($"Process Complete for {medName} for Language {lang}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing {url}: {ex.Message}");
        }
        finally
        {
            semaphore.Release();
        }
    }
}
