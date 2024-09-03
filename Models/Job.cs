namespace JobSeekerApi.Models;

public class Job
{
    public int Id { get; set; }
    public int JobId { get; set; }
    
    public string Url { get; set; }
    public string Title { get; set; }
    public string Company { get; set; }
    public string Location { get; set; }
    public string Platform { get; set; }
    public string Category { get; set; }    
    public DateTime PostedAt { get; set; }
    public DateTime ValidUntil { get; set; }
    
}