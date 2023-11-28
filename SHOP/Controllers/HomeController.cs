using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SHOP.Data;
using SHOP.Models;

namespace SHOP.Controllers
{
    [Route("v1")]
    [ApiController]
    public class HomeController : Controller
    {
        public async Task<ActionResult<dynamic>> Get([FromServices] DataContext context )
        {
            var employee = new User {Id = 1, UserName = "pedro", Password = "pedro", Role = "employee"};
            var manager = new User {Id = 2, UserName = "maria", Password = "maria", Role = "employee"};
            var category = new Category {Id = 1, Title = "Informática"};
            var product = new Product {Id = 1, Category = category, Title = "Mouse", Price = 299, Description = "Mouse sem fio"};
            context.Users.Add(employee);
            context.Users.Add(manager); 
            context.Categories.Add(category);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Dados configurados"
            });
        }

    }
}
