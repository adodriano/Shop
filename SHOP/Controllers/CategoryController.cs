﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHOP.Data;
using SHOP.Models;

namespace SHOP.Controllers
{
    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext context)
        {
            var category = await context.Categories.AsNoTracking().ToListAsync();

            return Ok(category);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Category>> GetById(int id, [FromServices] DataContext context)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            
            return Ok(category);
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Category>> Post([FromBody] Category model, [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            };

            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possivel criar a categoria" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<Category>> Put(int id, [FromBody] Category model, [FromServices] DataContext context)
        {
            //verifica se o ID informado é o mesmo do model
            if (id != model.Id)
            {
                return NotFound(new { message = "Categoria não encontrada" });
            }

            //verifica se os dados são válidos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            };

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {

                return BadRequest(new { message = "Este registro já foi atualizado" });
            }
            catch (Exception)
            {

                return BadRequest(new { message = "Não foi possivel atualizar a categoria" });
            }

        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<List<Category>>> Delete(int id, [FromServices] DataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound(new { message = "Categoria não encontrada" });
            }

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new { message = "Categoria removida com sucesso" });
            }
            catch (Exception)
            {

                return BadRequest(new { message = "Não foi possivel remover categoria" });
            }
        }
    }
}
