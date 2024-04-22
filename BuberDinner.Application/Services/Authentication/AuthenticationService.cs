using BuberDinner.Application.Common.Intefaces.Persistance;
using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Domain.Entities;

namespace BuberDinner.Application.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IJwtTokenGenerator jwtTokenGenerator,IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }
    // Overried Method
    public AuthenticationResult Register(string firstName, string lastName, string email, string password)
    {
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
        };
        _userRepository.Add(user);

        var token = _jwtTokenGenerator.GenerateToken(user);
 

        return new AuthenticationResult(
            user,
            token
        );
    }
    // Overried Method
    public AuthenticationResult Login(string email, string password)
    {
        if(_userRepository.GetUserByEmail(email) is not User user)
        {
            // This is not recommended as this expection will be shown to the user/client
            // As they could guess the email until is right , and the password next
            throw new Exception("User with given email not founded");
        }
        // 2. Validate password
        if(user.Password != password)
        {
            throw new Exception("Invalid password");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthenticationResult(
            user,
            token
        );
    }
}