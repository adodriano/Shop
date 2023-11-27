using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHOP.Data;
using SHOP.Models;
using SHOP.Services;
using System.Data;

namespace SHOP.Controllers
{
    [Route("users")]
    public class UserController : Controller
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            var users = await context.Users.AsNoTracking().ToListAsync();
            return users;
        }


        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        // [Authorize(Roles = "manager")] (se nao tiver user no banco nao vai conseguir nem criar o manager)
        public async Task<ActionResult<User>> Post([FromServices] DataContext context, [FromBody] User model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // força o user a ser sempre "func"
                model.Role = "employee";

                context.Users.Add(model);
                await context.SaveChangesAsync();

                //esconde a senha
                model.Password = "";

                return Ok(model);
            }
            catch (Exception)
            {

                return BadRequest(new { message = "Não foi possivel criar o usuário" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put([FromServices] DataContext context, int id, [FromBody] User model)
        {
            //verifica se os dados são válidos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            };

            //verifica se o ID informado é o mesmo do model
            if (id != model.Id)
            {
                return NotFound(new { message = "Usuário não encontrada" });
            }
            try
            {
                context.Entry<User>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {

                return BadRequest(new { message = "Este Usuário já foi atualizado" });
            }
            catch (Exception)
            {

                return BadRequest(new { message = "Não foi possivel atualizar o Usuário" });
            }


        }


        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromServices] DataContext context, [FromBody] User model)
        {
            var user = await context.Users.AsNoTracking().Where(x => x.UserName == model.UserName && x.Password == model.Password).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "Usuário ou senha inválidos" });
            }

            var token = TokenService.GenerateToken(user);

            //esconde a senha
            user.Password = "";
            return new
            {
                user = user,
                token = token
            };
        }
    }
}
