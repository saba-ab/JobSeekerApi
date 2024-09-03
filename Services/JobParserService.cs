using System.Globalization;
using HtmlAgilityPack;
using JobSeekerApi.Data;
using JobSeekerApi.Models;

namespace JobSeekerApi.Services;

public class JobParserService
{
    private readonly JobContext _context;
    private readonly HttpClient _httpClient;
    private readonly string baseUrl = "https://jobs.ge/";

    public JobParserService(JobContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    public async Task ParseJobsAsync()
    {
        var response = await _httpClient.GetStringAsync(baseUrl);
        var document = new HtmlDocument();
        document.LoadHtml(response);

        var jobNodes = document.DocumentNode.SelectNodes("//tr[td/a]");

        if (jobNodes == null)
        {
            return;
        }

        foreach (var node in jobNodes)
        {
            // Parse Job Title and URL (assuming <a> tag contains the title and link)
            var titleNode = node.SelectSingleNode(".//a");
            var title = titleNode?.InnerText.Trim();
            
            // Parse url from a tag attribute href
            var relativeUrl = titleNode?.Attributes["href"]?.Value;
            var url = !string.IsNullOrEmpty(relativeUrl) ? new Uri(new Uri(baseUrl), relativeUrl).ToString() : null;

            // Parse Company Name (assuming it's in the 4th <td>)
            var companyNode = node.SelectNodes(".//td")[3];
            var company = companyNode?.InnerText.Trim();

            // Parse Location (assuming it's in the 3rd <td>)
            var locationNode = node.SelectNodes(".//td")[2];
            var location = locationNode?.InnerText.Trim();

            // Platform (assuming this is constant for jobs.ge)
            var platform = "jobs.ge";

            // Parse Category (assuming it's in the 2nd <td>, adjust as needed)
            var categoryNode = node.SelectNodes(".//td")[1];
            var category = categoryNode?.InnerText.Trim();

            // Parse Posting Date (assuming it's in the 5th <td>)
            var postedDateNode = node.SelectNodes(".//td")[4];
            var postedDateText = postedDateNode?.InnerText.Trim();
            DateTime.TryParseExact(postedDateText, "dd MMMM", new CultureInfo("ka-GE"), DateTimeStyles.None, out DateTime postedAt);

            // Adjust the year for postedAt
            postedAt = postedAt.AddYears(DateTime.Now.Year - postedAt.Year);

            // Parse Valid Until (assuming it's based on the posting date, adjust if there's an actual field)
            var validUntil = postedAt.AddMonths(1);

            // Generate a Job ID (from star icon extracting id)
            var jobIdString = node.SelectNodes(".//td")[0].FirstChild.Attributes["id"]?.Value;
            if (string.IsNullOrEmpty(jobIdString))
            {
                continue;
            }
            var jobId = int.Parse(jobIdString);

            // Avoid adding duplicate entries
            if (_context.Jobs.Any(j => j.JobId == jobId)) continue;
            var job = new Job
            {
                Url = url,
                JobId = jobId,
                Title = title,
                Company = company,
                Location = location,
                Platform = platform,
                Category = category,
                PostedAt = postedAt,
                ValidUntil = validUntil
            };
            Console.WriteLine($"Adding job {job.PostedAt} to database.");
            _context.Jobs.Add(job);
        }
        Console.WriteLine($"{jobNodes.Count} Jobs have been parsed.");
        var date = DateTime.Now.Day;
        Console.WriteLine($"{date} day");

        await _context.SaveChangesAsync();

    }
}