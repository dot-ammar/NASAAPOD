using Flurl.Http;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;

var nasaJSON = string.Empty;
DateTime d;
bool chValidity;
string date;
dynamic DynamicData = string.Empty;
string userName = Environment.UserName;

while (true)
{
    Console.WriteLine("Welome to the NASA APOD download center.\nWould you like to download the photo for today or for another date?\nType 'Today' or a date in the format of 'YYYY-MM-DD' which must be after 1995-06-16");
    date = Console.ReadLine();

    if (date == "Today")
    {
        Console.WriteLine($"\nLoading astronomy photo of the date by NASA for today...");
        nasaJSON = await "https://api.nasa.gov/planetary/apod?api_key=DEMO_KEY"
            .GetStringAsync();
        DynamicData = JsonConvert.DeserializeObject(nasaJSON);
        Console.WriteLine($"Today's nasa photo of the day is '{ DynamicData.title}'.");
        Console.WriteLine($"Description: {DynamicData.explanation}");
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
        Console.WriteLine($"\nLoading Astronomy Photo Of The Day by NASA for {date}...");
        nasaJSON = await $"https://api.nasa.gov/planetary/apod?api_key=DEMO_KEY&date={date}"
            .GetStringAsync();
        DynamicData = JsonConvert.DeserializeObject(nasaJSON);
        Console.WriteLine($"{date}'s nasa photo of the day is '{ DynamicData.title}'.");
        Console.WriteLine($"Description: {DynamicData.explanation}");
        break;
    }

    else
    {
        Console.WriteLine("That is not valid input, please try again.");
    }
}


if (File.Exists(@$"C:\Users\{userName}\Pictures\{DynamicData.title}.jpg"))
{
    Console.WriteLine("\nThe selected NASA astronomy photo of the day already exists in your Pictures folder.");
    return;
}

Console.WriteLine($"Would you like to download it? (Y/N)");
while (true)
{
    string read1 = Console.ReadLine();

    if (read1 == "Y")
    {
        Console.WriteLine("\nDownloading to user's pictures folder...");
        var path = await @$"{DynamicData.url}"
            .DownloadFileAsync(@$"C:\Users\{userName}\Pictures", $"{DynamicData.title}.jpg");
        Console.WriteLine("Download succesful!");
        break;
    }

    if (read1 == "N")
    {
        Console.WriteLine("Okay, exiting...");
        break;
    }

    else
    {
        Console.WriteLine("That is not a valid input please try again.");
    }

}





