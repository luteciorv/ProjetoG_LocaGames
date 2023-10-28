using System.ComponentModel.DataAnnotations;

namespace ProjetoG_LocaGames.Entities;

public class Game
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Genre { get; set; }
    public string Description { get; set; }
    public DateTime ReleaseDate { get; set; }
    public decimal PricePerHour { get; set; }
    public string GameDistributor { get; set; }
    public string Platform { get; set; }

    // Relacionamento
    public RentedGame? RentedGame { get; set; }
}
