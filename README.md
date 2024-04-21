# Clean Architecture & Domain-Driven Design in ASP.NET REST API

![DDD](Docs/images/ddd_layers.png)
1. DDD emphasizes placing the domain model at the center of the design. This means that the core business logic and rules are expressed directly within the domain model.
2. Users of the system interact primarily with the domain model, which encapsulates the business concepts and processes. They don't need to concern themselves with the underlying technologies used to implement these functionalities.
3. DDD allows for a clear separation between the domain logic and the infrastructure or technical details. This means that changes to the infrastructure or technology stack can be made without affecting the core business logic, and vice versa.
4. DDD also enables the creation of a ubiquitous language shared by domain experts and developers, ensuring that the domain model accurately reflects the language and concepts of the business domain.
> "In Domain-Driven Design (DDD), the focus is on placing the domain logic at the core of the architecture, where the domain model defines the business rules and processes. Users interact primarily with this domain model, abstracted from the underlying technologies. DDD enables a separation of concerns, allowing for changes in infrastructure or technology without impacting the core domain logic, and vice versa. Additionally, it promotes the use of a ubiquitous language shared by domain experts and developers to accurately represent the business domain."

---

> Domain-Driven Design (DDD) serves as a bridge between software architecture and business solutions. It provides a set of principles, patterns, and practices that help developers design software systems that accurately reflect the complexities of the business domain they are working with.

> By focusing on modeling the domain and its concepts within the software, DDD enables developers to create systems that are more aligned with the needs and requirements of the business. This alignment not only enhances communication between technical and non-technical stakeholders but also leads to more maintainable and adaptable software over time.

> So, DDD sits at the intersection of software architecture and business solutions, leveraging architectural concepts to solve business problems effectively.

---
❓  Asking Question : so it like a the middle between software architecture and business solutions

Exactly! Domain-Driven Design (DDD) serves as a bridge between software architecture and business solutions. It provides a set of principles, patterns, and practices that help developers design software systems that accurately reflect the complexities of the business domain they are working with.

By focusing on modeling the domain and its concepts within the software, DDD enables developers to create systems that are more aligned with the needs and requirements of the business. This alignment not only enhances communication between technical and non-technical stakeholders but also leads to more maintainable and adaptable software over time.

So, DDD sits at the intersection of software architecture and business solutions, leveraging architectural concepts to solve business problems effectively.

![Clean Architecture](Docs/images/Clean%20Architecture%20Template.png)

## JWT Generator Implimetations
At the **presentation layer** (`BuberDinner.Api`) services will be registered by `program.cs`
```cs
builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);
```
The Authentication Logic will be implmented in **Application Layer** via the `DependencyInjection.cs`
```cs
using BuberDinner.Application.Services.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace BuberDinner.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService,AuthenticationService>();  

        return services; 
    }
}
```
The `AuthenticationService` require an services from the **infastructure layer** which is `IJwtTokenGenerator` . Let See how the `IJwtTokenGenerator` is implemented and injected in the `BuberDinner.Infrastructure` \
Notice that the `IJwtTokenGenerator` belong to the Application Layer.

```cs
using BuberDinner.Application.Common.Interfaces.Authentication;
namespace BuberDinner.Application.Services.Authentication;


public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthenticationService(IJwtTokenGenerator jwtTokenGenerator)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
    }
    // Overried Method
    public AuthenticationResult Register(string firstName, string lastName, string email, string password)
    {
        // TODO : Check if user exists

        // create User (generate unique ID)
        Guid userId = Guid.NewGuid();
        var token = _jwtTokenGenerator.GenerateToken(userId, firstName, lastName);
 

        return new AuthenticationResult(
            userId,
            firstName,
            lastName,
            email,
            token
        );
    }
    // Overried Method
    public AuthenticationResult Login(string email, string password)
    {
        return new AuthenticationResult(
            Guid.NewGuid(),
            "firstName",
            "lastName",
            email,
            "password Token"
        );
    }
}
```
In this Layer all the Interfaces from `BuberDinner.Application` is implemented by the Infrastructure `DependencyInjection.cs`. 
Also the use the `Options Pattern` to retrieve the values from the `appsettings*.json` from `BuberDinner.Api`
```cs
// Infrastructure Layer
using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Application.Common.Interfaces.Services;
using BuberDinner.Infrastructure.Authentication;
using BuberDinner.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace BuberDinner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,ConfigurationManager configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IJwtTokenGenerator,JwtTokenGenerator>();
        services.AddSingleton<IDateTimeProvider,DateTimeProvider>();    
        return services;
    }
}
```
```cs
// JWT Implemation 
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BuberDinner.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly JwtSettings _jwtSettings;
    public JwtTokenGenerator(IDateTimeProvider dateTimeProvider,IOptions<JwtSettings> jwtOptions){
        _jwtSettings=jwtOptions.Value;
        _dateTimeProvider = dateTimeProvider;
    }
    public string GenerateToken(Guid userId, string firstName, string lastName)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Secret)
            ),SecurityAlgorithms.HmacSha256
        );
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, firstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, lastName),
        };

        var securityToken = new JwtSecurityToken(
            issuer:_jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims:claims,
            signingCredentials:signingCredentials,
            expires:_dateTimeProvider.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}
```

> The presentation layer utilizes the logic provided by the application layer. However, it needs to specify the infrastructure layer options and implementations to configure the behavior of the application interfaces

❓ Application is the algorithm and infrastructure is like a input for that algorithm ? 
> In the context of software architecture, the application layer typically contains the core logic and algorithms that define how the system operates. This includes business rules, data processing, and other essential functionalities that make the software function according to its intended purpose.

> On the other hand, the infrastructure layer provides the necessary environment and support for the application to run. This can include things like databases, network connections, external services, configuration settings, and more.

> So, you can think of the application layer as the algorithm or the "brains" of the system, while the infrastructure layer provides the necessary inputs, resources, and environment for the application layer to execute its logic effectively.

## `dotnet user-secrets`
It is not a good idea to store a secret key directly in the `appsettings*.json` so instead we dotnet feature to store it 

```bash
# this will create a UserSecretsId field in the project.csproj file
dotnet user-secrets init --project .\BuberDinner.Api\

# Set the Serect in the appsettings file 
dotnet user-secrets set --project .\BuberDinner.Api\ "JwtSettings:Secret" "tran-nhut-anh-123-@#!"

# List all the settings
dotnet user-secrets list --project .\BuberDinner.Api\
```




