### Implemented in This project
/Docs/DomainModels.md
/Domain/Entites/User.cs {Id = new Guid,FirstName,LastName,Email,Password} = null!;
Application/Common/Interfaces/Persistence/IUserRepository.cs
```cs
namespace BuberDinner.Application.Common.Intefaces.Persistance

public interface IUserRepository{
    User? GetUserByEmail(string Email);
    void Add(User user);
}
```
Inject to `AuthenticationService.cs`
```cs
private readonly IUserRepository _userRepository;

// constructor injection below

// Implement to Register method
// 1. Validate the user doesn't exists
if(_userRepository.GetUserByEmail(email) is not null){
    throw new Exception("User with given email already exists");
}
// 2. Create user (generate unique ID) & Persist to DB
var user = new User
{
    FirstName = firstName,
    LastName = lastName,
    Email = email,
    Password = password
}
_userRepository.Add(user)

// modify the user.Id below

// Login Method
if(_userRepository.GetUserByEmail(email) is not User user)
{
    // This is not recommended as this expection will be shown to the user/client
    // As they could guess the email until is right , and the password next
    throw new Exception("User with given email already exists");
}
// 2. Validate password
if(user.Password != password)
{
    throw new Exception("Invalid password");
}
// modify to use the user variable
```
`Infrastructure/Persistence/UserRepository.cs`
```cs
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
```
```cs
// Inject in Infrastructure services DependencyInject.cs IoC container
services.AddScoped<IUserRepository,UserRepository>();
```

### Refactoring the the user a bit
`/Application/Services/Authentication/AuthenticationResult.cs`
```cs
public record AuthenticationResult
(
    User user,
    string Token
)
```
`/Application/Services/Authentication/AuthenticationServices.cs`
```cs
return 
```