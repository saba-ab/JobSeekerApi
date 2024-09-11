using JobSeekerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JobSeekerApi.Data;

public class DbSeeder
{
    public static void SeedCategories(JobContext context)
    {
        try
        {
            if (!context.Categories.Any())
            {
                
                var categories = new List<Category>()
                {
                    new() { Name = "ადმინისტრაცია/მენეჯმენტი" },
                    new() { Name = "გაყიდვები" },
                    new() { Name = "ფინანსები/სტატისტიკა" },
                    new() { Name = "PR/მარკეტინგი" },
                    new() { Name = "ლოჯისტიკა/ტრანსპორტი/დისტრიბუცია" },
                    new() { Name = "IT/პროგრამირება" },
                    new() { Name = "სამართალი" },
                    new() { Name = "მედიცინა/ფარმაცია" },
                    new() { Name = "სხვა" },
                    new() { Name = "კვება" },
                    new() { Name = "მშენებლობა/რემონტი" },
                    new() { Name = "განათლება" },
                    new() { Name = "მედია/გამომცემლობა" },
                    new() { Name = "სილამაზე/მოდა" },
                    new() { Name = "Delete Me"},
                    new() { Name = "დასუფთავება" },
                    new() { Name = "დაცვა/უსაფრთხოება" },
                    new() { Name = "ზოგადი ტექნიკური პერსონალი" }
                };
            
                context.Categories.AddRange(categories);
                context.SaveChanges();
                
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
}