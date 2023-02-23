namespace WebApi;

public static class WebApplicationExtensions
{
    public static void MapApiRoutes(this WebApplication app)
    {
        var group = app.MapGroup("Api");

        var type = typeof(IApiRoute);
        var types = type.Assembly.GetTypes().Where(p => p.IsClass && p.IsAssignableTo(type));

        foreach (var routeType in types)
        {
            var route = (IApiRoute)Activator.CreateInstance(routeType);
            route.Map(group);
        }
    }
}