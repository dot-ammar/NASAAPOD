using Flurl.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;

var nasaJSON = string.Empty;
DateTime d;
bool chValidity;
string date;
dynamic DynamicData = string.Empty;
string userName = Environment.UserName;
long FILEasize;
bool todayASK;
string ext = string.Empty;

Console.WriteLine("Welome to the NASA Astronomy Photo Of The Day (APOD) download center.\nWould you like to download the photo for today or for another date?\n*Sometimes can be a video...");

while (true)
{
    Console.WriteLine("Type 'Today' or a date in the format of 'YYYY-MM-DD' which must be after 1995-06-16");
    date = Console.ReadLine();
    if (date == "Today" || date == "today")
    {

        Console.WriteLine($"\nLoading APOD for today...");
        nasaJSON = await "https://api.nasa.gov/planetary/apod?api_key=DEMO_KEY"  //retrieving the photo
            .GetStringAsync();
        DynamicData = JsonConvert.DeserializeObject(nasaJSON); //parsing the JSON
        Console.WriteLine($"\nTitle: '{DynamicData.title}'.");
        Console.WriteLine($"Description: {DynamicData.explanation}");
        date = "today";
        todayASK = true;
        break;
    }

    else
    {
        //checking for whether correct format of date was entered
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
            Console.WriteLine($"\nLoading APOD for {date}...");
            nasaJSON = await $"https://api.nasa.gov/planetary/apod?api_key=DEMO_KEY&date={date}" //retrieving the photo
                .GetStringAsync();
        }
        catch (Flurl.Http.FlurlHttpException) //means that api rejected that date
        {
            Console.WriteLine($"Media for {date} does not exist.\nExiting...");
            Console.ReadLine();
            return;
        }
        DynamicData = JsonConvert.DeserializeObject(nasaJSON); //get the parts of the JSON and save them as dynamic data
        Console.WriteLine($"\nTitle: '{DynamicData.title}'.");
        Console.WriteLine($"Description: {DynamicData.explanation}");
        todayASK = false;
        break;
    }

    else
    {
        Console.WriteLine("That is not valid input, please try again.");
    }
}

if (DynamicData.media_type == "video")
{
    if (todayASK == true)
    {
        Console.WriteLine($"Today's media is a video.");
    }
    else
    {
        Console.WriteLine($"{DynamicData.date}'s media is a video.");
    }

    Console.WriteLine($"Would you like to open the video in the browser? (Y/N)");
    {
        string read1 = Console.ReadLine();
        while (true)
        {
            if (read1 == "Y" || read1 == "y")
            {
                Console.WriteLine("Opening...");

                Process myProcess = new Process();

                try
                {
                    // true is the default, but it is important not to set it to false.
                    // opens the link in default browser.
                    myProcess.StartInfo.UseShellExecute = true;
                    myProcess.StartInfo.FileName = $"{youtubeEmbedtoNormal(DynamicData.url)}";
                    myProcess.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
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
        }
    }
    Console.ReadLine();
    return;
}

ext = GetFileExtensionFromUrl(DynamicData.hdurl);

    if (File.Exists(@$"C:\Users\{userName}\Pictures\NASAAPODs\{DynamicData.title} {DynamicData.date}.{ext}")) //doesn't download a new one anyways but just so the user knows
{
    Console.WriteLine("\nThe selected NASA astronomy photo of the day already exists in your Pictures folder. Would you like to open the directory? (Y/N)");
  

    while (true)
    {
        string read1 = Console.ReadLine();

        if (read1 == "Y" || read1 == "y")
        {
            Console.WriteLine("Opening folder...");
            Process.Start("explorer.exe", @$"C:\Users\{userName}\Pictures\NASAAPODs"); //opens the folder
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
            .DownloadFileAsync(@$"C:\Users\{userName}\Pictures\NASAAPODs", $"{DynamicData.title} {DynamicData.date}.{ext}"); //downloads file
        FileInfo fi = new FileInfo(@$"C:\Users\{userName}\Pictures\NASAAPODs\{DynamicData.title} {DynamicData.date}.{ext}"); //gets it's info/properties.
        FILEasize = (fi.Length)/1000;
        Console.WriteLine("File Size in KiloBytes: {0}", FILEasize);
        Console.WriteLine("Download succesful!");
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
    }
    Console.ReadLine();
}

//functional methods

static string GetFileExtensionFromUrl(string url) //seperates the file extension of a html file.
{
    url = url.Split('?')[0];
    url = url.Split('/').Last();
    return url.Contains('.') ? url.Substring(url.LastIndexOf('.')) : "";
}

static string youtubeEmbedtoNormal(string url) //makes the embed version of a link to a normal link so the program can properly parse it. For example: https://www.youtube.com/embed/s6IpsM_HNcU > https://www.youtube.com/watch?v=s6IpsM_HNcU&ab_channel=APODVideos
{
    string input = url;
    string pattern = "embed/";
    string replace = "watch?v=";
    string result = Regex.Replace(input, pattern, replace);
    return result;
}