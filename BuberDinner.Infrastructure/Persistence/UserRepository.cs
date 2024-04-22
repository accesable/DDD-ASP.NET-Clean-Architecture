using BuberDinner.Application.Common.Intefaces.Persistance;
using BuberDinner.Domain.Entities;

namespace BuberDinner.Infrastructure.Persistence;
public class UserRepository : IUserRepository
{
    private static readonly List<User> _user = new ();
    public void Add(User user)
    {
        _user.Add(user);
    }
    public User? GetUserByEmail(string email)
    {
        return _user.SingleOrDefault(u => u.Email == email);
    }
}