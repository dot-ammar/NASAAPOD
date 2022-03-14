using Flurl.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

var nasaJSON = string.Empty;
DateTime d;
bool chValidity;
string date;
dynamic DynamicData = string.Empty;
string userName = Environment.UserName;
long FILEasize;
Console.WriteLine("Welome to the NASA APOD download center.\nWould you like to download the photo for today or for another date?");

while (true)
{
    Console.WriteLine("Type 'Today' or a date in the format of 'YYYY-MM-DD' which must be after 1995-06-16");
    date = Console.ReadLine();
    if (date == "Today" || date == "today")
    {

        Console.WriteLine($"\nLoading astronomy photo of the date by NASA for today...");
        nasaJSON = await "https://api.nasa.gov/planetary/apod?api_key=DEMO_KEY"
            .GetStringAsync();
        DynamicData = JsonConvert.DeserializeObject(nasaJSON);
        Console.WriteLine($"\nTitle: '{DynamicData.title}'.");
        Console.WriteLine($"Description: {DynamicData.explanation}");
        date = "today";
        break;
    }

    else
    {

        chValidity = DateTime.TryParseExact(
        $"{date}",
        "yyyy-mm-dd",
        CultureInfo.InvariantCulture,
        DateTimeStyles.None,
        out d);

    }

    if (chValidity == true)
    {
        try
        {
            Console.WriteLine($"\nLoading Astronomy Photo Of The Day by NASA for {date}...");
            nasaJSON = await $"https://api.nasa.gov/planetary/apod?api_key=DEMO_KEY&date={date}"
                .GetStringAsync();
        }
        catch (Flurl.Http.FlurlHttpException)
        {
            Console.WriteLine($"Image for {date} does not exist.\nExiting...");
            Console.ReadLine();
            return;
        }
        DynamicData = JsonConvert.DeserializeObject(nasaJSON);
        Console.WriteLine($"\nTitle: '{DynamicData.title}'.");
        Console.WriteLine($"Description: {DynamicData.explanation}");
        break;
    }

    else
    {
        Console.WriteLine("That is not valid input, please try again.");
    }
}

string ext = GetFileExtensionFromUrl(DynamicData.hdurl);

if (File.Exists(@$"C:\Users\{userName}\Pictures\NASAAPODs\{DynamicData.title} {DynamicData.date}.{ext}"))
{
    Console.WriteLine("\nThe selected NASA astronomy photo of the day already exists in your Pictures folder. Would you like to open the directory? (Y/N)");
  

    while (true)
    {
        string read1 = Console.ReadLine();

        if (read1 == "Y" || read1 == "y")
        {
            Console.WriteLine("Opening folder...");
            Process.Start("explorer.exe", @$"C:\Users\{userName}\Pictures\NASAAPODs");
            break;
        }

        if (read1 == "N" || read1 == "n")
        {
            Console.WriteLine("Okay, exiting...");
            break;
        }

        else
        {
            Console.WriteLine("That is not a valid input please try again.");
            Console.ReadLine();
        }
    }
    Console.ReadLine();
    return;
}

Console.WriteLine($"\nWould you like to download it? (Y/N)");
while (true)
{
    string read1 = Console.ReadLine();

    if (read1 == "Y" || read1 == "y")
    {
        Console.WriteLine("\nDownloading to user's pictures folder...");
        var path = await @$"{DynamicData.hdurl}"
            .DownloadFileAsync(@$"C:\Users\{userName}\Pictures\NASAAPODs", $"{DynamicData.title} {DynamicData.date}.{ext}");
        FileInfo fi = new FileInfo(@$"C:\Users\{userName}\Pictures\NASAAPODs\{DynamicData.title} {DynamicData.date}.{ext}");
        FILEasize = (fi.Length)/1000;
        Console.WriteLine("File Size in KiloBytes: {0}", FILEasize);
        Console.WriteLine("Download succesful!");
        break;
    }

    if (read1 == "N" || read1 == "n")
    {
        Console.WriteLine("Okay, exiting...");
        break;
    }

    else
    {
        Console.WriteLine("That is not a valid input please try again.");
    }

    Console.WriteLine("Opening folder...");
    Process.Start("explorer.exe", @$"C:\Users\{userName}\Pictures\NASAAPODs");
    Console.ReadLine();
}

//functional methods

static string GetFileExtensionFromUrl(string url)
{
    url = url.Split('?')[0];
    url = url.Split('/').Last();
    return url.Contains('.') ? url.Substring(url.LastIndexOf('.')) : "";
}

