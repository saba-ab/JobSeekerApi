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
    private const string BaseUrl = "https://jobs.ge/?page={0}&q=&cid={1}&lid=0&jid=0&in_title=0&has_salary=0&is_ge=0&for_scroll=yes";
    private readonly List<int> _categories = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 16, 17, 18 };

    public JobParserService(JobContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    public async Task ParseJobsAsync()
    {
        var today = DateTime.Now.Day;
        
        foreach (var categoryId in _categories)
        {
            int page = 1;
            bool shouldStop = false;

            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (category == null)
            {
                continue;
            }

            while (!shouldStop)
            {
                string url = string.Format(BaseUrl, page, categoryId);
                var response = await _httpClient.GetStringAsync(url);
                var document = new HtmlDocument();
                document.LoadHtml(response);

                var jobNodes = document.DocumentNode.SelectNodes("//tr[td/a]");

                if (jobNodes == null || jobNodes.Count == 0)
                {
                    break; 
                }

                foreach (var node in jobNodes)
                {
                    var parser = new Parser(node, BaseUrl);

                    var postedAt = parser.GetPostedDate();
                    
                    if (postedAt.Day != today)
                    {
                        shouldStop = true;
                        break;
                    }

                    var jobId = parser.GetJobId();
                    if (_context.Jobs.Any(j => j.JobId == jobId)) continue;

                    var job = new Job
                    {
                        Url = parser.GetUrl(),
                        JobId = jobId,
                        Title = parser.GetTitle(),
                        Company = parser.GetCompany(),
                        Location = parser.GetLocation(),
                        Platform = "jobs.ge",
                        CategoryId = category.Id, 
                        PostedAt = postedAt,
                        ValidUntil = parser.GetValidUntilDate()
                    };

                    _context.Jobs.Add(job);
                }

                if (!shouldStop)
                {
                    page++; // Move to the next page if necessary
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    private string GetCategoryById(int categoryId)
    {
        return categoryId switch
        {
            1 => "ადმინისტრაცია/მენეჯმენტი",
            2 => "გაყიდვები",
            3 => "ფინანსები/სტატისტიკა",
            4 => "PR/მარკეტინგი",
            5 => "ლოჯისტიკა/ტრანსპორტი/დისტრიბუცია",
            6 => "IT/პროგრამირება",
            7 => "სამართალი",
            8 => "მედიცინა/ფარმაცია",
            9 => "სხვა",
            10 => "კვება",
            11 => "მშენებლობა/რემონტი",
            12 => "განათლება",
            13 => "მედია/გამომცემლობა",
            14 => "სილამაზე/მოდა",
            16 => "დასუფთავება",
            17 => "დაცვა/უსაფრთხოება",
            18 => "ზოგადი ტექნიკური პერსონალი",
            _ => "Unknown"
        };
    }
}