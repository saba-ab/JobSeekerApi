using System.Globalization;
using HtmlAgilityPack;
using JobSeekerApi.Data;
using JobSeekerApi.Models;
using JobSeekerApi.Utilities;

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
            // initialize parser object 
            
            var parser = new Parser(node, baseUrl);
       
            var title = parser.GetTitle();
            
            var url = parser.GetUrl();

            var company = parser.GetCompany();

            var postedAt = parser.GetPostedDate();

            var validUntil = parser.GetValidUntilDate();

            // maybe ignore location before finding logic on jobs.ge
            var location = parser.GetLocation();

            // place in enums
            var platform = "jobs.ge";

            // temp
            var categoryNode = node.SelectNodes(".//td")[1];
            var category = categoryNode?.InnerText.Trim();

            var jobId = parser.GetJobId();
            if (jobId == 0)
            {
                continue;
            }
            
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