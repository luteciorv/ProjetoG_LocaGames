namespace ProjetoG_LocaGames.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // Relacionamento
        public ICollection<RentedGame> RentedGames { get; set; } = new List<RentedGame>();
    }
}
