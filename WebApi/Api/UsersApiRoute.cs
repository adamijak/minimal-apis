using System.Diagnostics;
using System.Linq.Expressions;
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
        group.MapGet("users1", OnGet1);
        group.MapGet("users2", OnGet2);
    }

    public async Task<IEnumerable<User>> OnGet1(string[] select)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        try
        {
            return users.Select(CreateNewStatement<User>(select));
        }
        finally
        {
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedTicks);
        }
    }

    public async Task<IEnumerable<User>> OnGet2(string[] select)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        try
        {
            return users.Select(u => new User { Id = u.Id, FirstName = u.FirstName });
        }
        finally
        {
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedTicks);
        }
    }

    public Func<T, T> CreateNewStatement<T>(params string[] propertyNames)
    {
        var type = typeof(T);
        // input parameter "o"
        var xParameter = Expression.Parameter(type, "o");

        // new statement "new Data()"
        var xNew = Expression.New(type);

        // create initializers
        var bindings = propertyNames.Select(p =>
        {
            var property = type.GetProperty(p);
            // original value "o.Field1"
            var xOriginal = Expression.Property(xParameter, property);

            // set value "Field1 = o.Field1"
            return Expression.Bind(property, xOriginal);
        });
        // initialization "new Data { Field1 = o.Field1, Field2 = o.Field2 }"
        var xInit = Expression.MemberInit(xNew, bindings);

        // expression "o => new Data { Field1 = o.Field1, Field2 = o.Field2 }"
        var lambda = Expression.Lambda<Func<T, T>>(xInit, xParameter);

        // compile to Func<Data, Data>
        return lambda.Compile();
    }
}

