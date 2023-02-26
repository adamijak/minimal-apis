using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using WebApi.Helpers;
using WebApi.Objects;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi.Api;

public class UsersApiRoute : IApiRoute
{
    private readonly User[] users = new User[] {
        new User{Id = "1" , FirstName = "Pavel", LastName = "Vokoun"},
        new User{Id = "2" , FirstName = "Petr", LastName = "Vorel"},
        new User{Id = "3" , FirstName = "Andy", LastName = "Dandy"},
    };

    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("users", OnGet);
    }

    public IEnumerable<User> OnGet() => users;

  
}

