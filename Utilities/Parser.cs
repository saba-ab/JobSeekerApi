using System.Globalization;
using HtmlAgilityPack;

namespace JobSeekerApi.Utilities;

public class Parser(HtmlNode node, string baseUrl)
{
    private HtmlNode Node { get; set; } = node;
    private string BaseUrl { get; set; } = baseUrl;


    public string GetTitle()
    {
        var titleNode = Node.SelectSingleNode(".//a");
        var title = titleNode.InnerText.Trim();
        return title;
    }

    public string GetUrl()
    {
        var relativeUrl = Node.SelectSingleNode(".//a").Attributes["href"].Value;
        var url =  new Uri(new Uri(BaseUrl), relativeUrl).ToString();
        return url;
    }

    public string GetCompany()
    {
        var companyNode = Node.SelectNodes(".//td")[3];
        var company = companyNode.InnerText.Trim();
        return company;
    }

    public DateTime GetPostedDate()
    {
        var postedDateNode = Node.SelectNodes(".//td")[4];
        var postedDateText = postedDateNode?.InnerText.Trim();
        DateTime.TryParseExact(postedDateText, "dd MMMM", new CultureInfo("ka-GE"), DateTimeStyles.None, out DateTime postedAt);

        postedAt = postedAt.AddYears(DateTime.Now.Year - postedAt.Year);
       
        return postedAt;
    }

    public DateTime GetValidUntilDate()
    {
        var validUntilNode = Node.SelectNodes(".//td")[5];
        var validUntilText = validUntilNode?.InnerText.Trim();
        DateTime.TryParseExact(validUntilText, "dd MMMM", new CultureInfo("ka-GE"), DateTimeStyles.None, out DateTime validUntil);
            
        validUntil = validUntil.AddYears(DateTime.Now.Year - validUntil.Year);
        
        return validUntil;
    }

    public int GetJobId()
    {
        var jobIdString = Node.SelectNodes(".//td")[0].FirstChild.Attributes["id"]?.Value;
        if (string.IsNullOrEmpty(jobIdString)) return 0;
        var jobId = int.Parse(jobIdString);
        
        return jobId;
    }

    public string? GetLocation()
    {
        var titleNode = Node.SelectNodes(".//td")[1];
        var locationNode = titleNode.SelectSingleNode(".//i")?.InnerText.Trim();
        var location = locationNode?[1..]?.Trim();

        return location;
    }
}