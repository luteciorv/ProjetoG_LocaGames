namespace ProjetoG_LocaGames.Entities
{
    public class RentedGame
    {
        public Guid Id { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public bool Returned { get; set; }
        public bool Reserved { get; set; }

        // Relacionamento
        public Guid GameId { get; set; }
        public Game Game { get; set; } = null!;

        // Relacionamento
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
