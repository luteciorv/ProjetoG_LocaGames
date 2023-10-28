using Microsoft.EntityFrameworkCore;
using ProjetoG_LocaGames.Data;
using ProjetoG_LocaGames.Entities;

namespace ProjetoG_LocaGames.Services
{
    public class UserService
    {
        private readonly DataContext _dataContext;
        public UserService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public List<User> GetAll()
        {
            return _dataContext.Users.ToList();
        }

        public User? GetById(Guid id)
        {
            return _dataContext.Users
                                .Where(u => u.Id == id)
                                .Include(u => u.RentedGames)
                                .FirstOrDefault();
        }

        public void AddUser(User user)
        {
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();
        }
    }
}
