using System.Reflection;

namespace RoomHub.Api.Common.Cqrs;

public static class HandlerRegistrationExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var handlerTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false });

        foreach (var type in handlerTypes)
        {
            var interfaces = type.GetInterfaces();

            foreach (var iface in interfaces)
            {
                if (iface.IsGenericType &&
                    (iface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                     iface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                {
                    services.AddScoped(iface, type);
                }
            }
        }

        return services;
    }
}
