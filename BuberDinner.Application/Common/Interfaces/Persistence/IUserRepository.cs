using BuberDinner.Domain.Entities;

namespace BuberDinner.Application.Common.Intefaces.Persistance;

public interface IUserRepository{
    User? GetUserByEmail(string Email);
    void Add(User user);
}