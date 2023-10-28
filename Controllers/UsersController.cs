using Microsoft.AspNetCore.Mvc;
using ProjetoG_LocaGames.Data;
using ProjetoG_LocaGames.Entities;
using ProjetoG_LocaGames.Services;
using ProjetoG_LocaGames.Utils;

namespace ProjetoG_LocaGames.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public DataContext DataContext;
        private UserService service;

        public UsersController(DataContext context)
        {
            DataContext = context;
            service = new UserService(context);
        }

        // GET: api/Users
        [HttpGet]
        public ActionResult<List<User>> Get()
        {
            return service.GetAll();
        }

        // GET: api/Users/5
        [HttpGet]
        [Route("{id}")]
        public ActionResult<User> Get(Guid id)
        {
            var user = service.GetById(id);
            if (user is null) return NotFound();

            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        public ActionResult<User> PostUser(string name, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) 
                return BadRequest("Invalid Request");

            if (!StringUtils.IsPasswordStrong(password)) return BadRequest("A senha informada é muito fraca");

            var novoUsuario = new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                Password = password,
            };

            service.AddUser(novoUsuario);

            return CreatedAtAction(nameof(Get), new { id = novoUsuario.Id }, novoUsuario);
        }

        [HttpPut]
        [Route("limpar-database")]
        public ActionResult CleanDatabase()
        {
            DataContext.LimparTabela<User>();
            DataContext.LimparTabela<RentedGame>();
            DataContext.LimparTabela<Game>();

            return Ok("Banco de dados limpo");
        }
    }
}
