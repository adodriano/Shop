using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHOP.Data;
using SHOP.Models;

namespace SHOP.Controllers
{
    [Route("V1/products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            var products = await context.Products.Include(c => c.Category).AsNoTracking().ToListAsync();

            return Ok(products);
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(int id, [FromServices] DataContext context)
        {
            var products = await context.Products.Include(c => c.Category).AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            return Ok(products);
        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetByCategory(int id, [FromServices] DataContext context)
        {
            var products = await context.Products.Include(c => c.Category).AsNoTracking().Where(c => c.CategoryId == id).ToListAsync();

            return Ok(products);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Post([FromBody] Product model, [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            };

            try
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possivel criar o Produto" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<Product>> Put(int id, [FromBody] Product model, [FromServices] DataContext context)
        {
            //verifica se o ID informado é o mesmo do model
            if (id != model.Id)
            {
                return NotFound(new { message = "Produto não encontrado" });
            }

            //verifica se os dados são válidos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            };

            try
            {
                context.Entry<Product>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {

                return BadRequest(new { message = "Este Produto já foi atualizado" });
            }
            catch (Exception)
            {

                return BadRequest(new { message = "Não foi possivel atualizar o produto" });
            }

        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<List<Product>>> Delete(int id, [FromServices] DataContext context)
        {
            var product = await context.Products.FirstOrDefaultAsync(c => c.Id == id);
            if (product == null)
            {
                return NotFound(new { message = "Produto não encontrado" });
            }

            try
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return Ok(new { message = "Produto removido com sucesso" });
            }
            catch (Exception)
            {

                return BadRequest(new { message = "Não foi possivel remover o produto" });
            }
        }
    }
}
