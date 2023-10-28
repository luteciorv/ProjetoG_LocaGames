using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoG_LocaGames.Data;
using ProjetoG_LocaGames.Entities;

namespace ProjetoG_LocaGames.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private DataContext Contexto;

        public GamesController(DataContext context)
        {
            Contexto = context;
        }

        // GET: api/Games
        [HttpGet]
        public ActionResult<IEnumerable<Game>> GetGames()
        {
            if (Contexto.Games == null)
            {
                return NotFound();
            }

            return Contexto.Games.ToList();
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public ActionResult<Game> GetGame(Guid id)
        {
            if (Contexto.Games == null)
            {
                return NotFound();
            }
            var game = Contexto.Games.Find(id);
            if (game == null)
            {
                return NotFound();
            }

            return game;
        }

        // PUT: api/Games/5
        [HttpPut]
        [Route("{id}")]
        public IActionResult PutGame(Guid id, Game game)
        {
            if (id != game.Id)
            {
                return BadRequest();
            }

            Contexto.Entry(game).State = EntityState.Modified;

            try
            {
                Contexto.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JogoExiste(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<Game> PostGame(string name, string genre, string description, string releaseDate, decimal pricePerHour, string gameDistributor, string platform)
        {
            var novoJogo = new Game
            {
                Name = name,
                Genre = genre,
                Description = description,
                ReleaseDate = Convert.ToDateTime(releaseDate),
                PricePerHour = pricePerHour,
                GameDistributor = gameDistributor,
                Platform = platform
            };

            Contexto.Games.Add(novoJogo);
            Contexto.SaveChanges();

            return CreatedAtAction("GetGame", new { id = novoJogo.Id }, novoJogo);
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public IActionResult DeleteGame(Guid id)
        {
            if (Contexto.Games == null)
            {
                return NotFound();
            }

            var jogo = Contexto.Games.Find(id);
            if (jogo == null)
            {
                return NotFound();
            }

            Contexto.Games.Remove(jogo);
            Contexto.SaveChanges();

            return NoContent();
        }

        private bool JogoExiste(Guid id)
        {
            return (Contexto.Games?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
