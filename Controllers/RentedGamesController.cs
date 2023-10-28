using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoG_LocaGames.Data;
using ProjetoG_LocaGames.Entities;
using ProjetoG_LocaGames.Services;

namespace ProjetoG_LocaGames.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentedGamesController : ControllerBase
    {
        private DataContext dataContext;
        public UserService Service;

        public RentedGamesController(DataContext context)
        {
            dataContext = context;
            Service = new UserService(context);
        }

        // Recuperar informações do aluguel
        [HttpGet]
        [Route("{rentGameId:guid}")]
        public IActionResult Get(Guid rentGameId)
        {
            var rentGame = dataContext.RentedGames
                                .Where(r => r.Id == rentGameId)
                                .Include(r => r.Game)
                                .Include(r => r.User)
                                .FirstOrDefault();

            return Ok(rentGame);
        }

        // Alugar o jogo
        [HttpPost]
        public IActionResult RentGame(Guid userId, Guid gameId, string checkOutDate)
        {
            // Validações
            DateTime checkOut = Convert.ToDateTime(checkOutDate);
            if (checkOut <= DateTime.Now) return BadRequest("A data de checkout não pode ser menor ou igual a data de checkin");

            var user = Service.GetById(userId);
            if (user == null) return NotFound("O usuário com o id informado não foi encontrado");

            var game = dataContext.Games.Find(gameId);
            if (game == null) return NotFound("O jogo com o id informado não foi encontrado");

            var jaFoiAlugado = dataContext.RentedGames.Where(r => r.User == user && r.GameId == gameId && (!r.Returned || r.Reserved)).FirstOrDefault();
            if(jaFoiAlugado != null)
            {
                return BadRequest($"O jogo já foi alugado/reservado. Id da transação: {jaFoiAlugado.Id}");
            }

            var rentGame = new RentedGame
            {
                Id = Guid.NewGuid(),
                GameId = gameId,
                UserId = userId,
                CheckIn = DateTime.Now,
                CheckOut = checkOut,
                Returned = false
            };

            dataContext.RentedGames.Add(rentGame);
            dataContext.SaveChanges();

            return Ok($"Jogo alugado com sucesso. Id {rentGame.Id}");
        }

        // Devolver jogo
        [HttpPut]
        public IActionResult ReturnGame(Guid rentGameId)
        {
            var rentGame = dataContext.RentedGames
                                 .Where(r => r.Id == rentGameId)
                                 .Include(r => r.Game)
                                 .Include(r => r.User)
                                 .FirstOrDefault();

            if (rentGame == null) return NotFound();

            if (rentGame.Returned) return Ok("O jogo já foi devolvido");

            // Pagar multa
            if(DateTime.Now > rentGame.CheckOut)
            {
                var hoursExceed = (DateTime.Now - rentGame.CheckOut).TotalHours;
                var totalPrice = rentGame.Game.PricePerHour * (decimal) hoursExceed;

                return BadRequest($"Horas atrasadas: {hoursExceed} \nValor total da multa: {totalPrice}");
            }

            rentGame.Returned = true;
            rentGame.Reserved = false;

            dataContext.Entry(rentGame).State = EntityState.Modified;

            try
            {
                dataContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if ((dataContext.RentedGames?.Any(e => e.Id == rentGameId)).GetValueOrDefault())
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Jogo devolvido com sucesso");
        }

        [HttpPut]
        [Route("multa")]
        public IActionResult PayFine(Guid rentGameId, decimal payment)
        {
            var rentGame = dataContext.RentedGames
                                 .Where(r => r.Id == rentGameId)
                                 .Include(r => r.Game)
                                 .Include(r => r.User)
                                 .FirstOrDefault();

            if (rentGame is null) return NotFound();
            if (rentGame.Returned) return Ok("O jogo já foi devolvido");

            // Pagar multa
            // O cálculo da multa é dado pela quantidade em horas passadas desde o checkout
            // Multiplicado pelo valor hora do jogo
            if (DateTime.Now > rentGame.CheckOut)
            {
                var hoursExceed = (DateTime.Now - rentGame.CheckOut).TotalHours;
                var totalPrice = rentGame.Game.PricePerHour * (decimal)hoursExceed;

                if(payment > totalPrice)
                {
                    rentGame.Returned = true;
                    rentGame.Reserved = false;

                    dataContext.Entry(rentGame).State = EntityState.Modified;

                    try
                    {
                        dataContext.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if ((dataContext.RentedGames?.Any(e => e.Id == rentGameId)).GetValueOrDefault())
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return Ok($"Multa paga e jogo devolvido. Troco: R$ {payment - totalPrice}");
                }
                else if (payment == totalPrice)
                {
                    rentGame.Returned = true;
                    rentGame.Reserved = false;

                    dataContext.Entry(rentGame).State = EntityState.Modified;

                    try
                    {
                        dataContext.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if ((dataContext.RentedGames?.Any(e => e.Id == rentGameId)).GetValueOrDefault())
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return Ok($"Multa paga e jogo devolvido");
                }
                else
                {
                    return BadRequest("Valor do pagamento incorreto");
                }
            }

            return BadRequest("O jogo não está com multa");
        }

        [HttpPost]
        [Route("reservar")]
        public IActionResult ReserveGame(Guid userId, Guid gameId, string checkIn, string checkOut)
        {
            DateTime checkin = Convert.ToDateTime(checkIn);
            DateTime checkout = Convert.ToDateTime(checkOut);

            var user = new UserService(dataContext).GetById(userId);
            if (user == null) return NotFound("Usuário não encontrado");

            var game = dataContext.Games.Where(g => g.Id == gameId).FirstOrDefault();
            if (game == null) return NotFound("Jogo não encontrado");

            var alreadReserved = dataContext.RentedGames.Where(r => r.UserId == userId && r.GameId == gameId && !r.Returned).FirstOrDefault();
            if(alreadReserved != null)
            {
                return BadRequest("O usuário já alugou o jogo");
            }

            var rentedGame = new RentedGame
            {
                Id = new Guid(),
                UserId = userId,
                GameId = gameId,
                CheckIn = checkin,
                CheckOut = checkout,
                Reserved = true,
                Returned = false
            };

            dataContext.RentedGames.Add(rentedGame);
            dataContext.SaveChanges();

            return Ok($"O usuário reservou o jogo. Id {rentedGame.Id}");
        }
    }
}
