using BudgetPlaner.Api.Attributes;

namespace BudgetPlaner.Api.Extensions;

public static class ServiceExtensions
{
    public static void RegisterAllDependency(this IServiceCollection services)
    {
        var scopedRegistration = typeof(ScopedRegistrationAttribute);
        var singletonRegistration = typeof(SingletonRegistrationAttribute);
        var transientRegistration = typeof(TransientRegistrationAttribute); 

        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p =>  p.IsDefined(scopedRegistration, true) || p.IsDefined(transientRegistration, true) || p.IsDefined(singletonRegistration, true) && !p.IsInterface).Select(s => new
            {
                Service = s.GetInterface($"I{s.Name}"),
                Implementation = s 
            }).Where(x => x.Service != null);

        foreach (var type in types)
        {
            if (type.Implementation.IsDefined(scopedRegistration, false))
            {
                services.AddScoped(type.Service ?? throw new InvalidOperationException(), type.Implementation);
            }

            if (type.Implementation.IsDefined(transientRegistration, false))
            {
                services.AddTransient(type.Service ?? throw new InvalidOperationException(), type.Implementation);
            }

            if (type.Implementation.IsDefined(singletonRegistration, false))
            {
                services.AddSingleton(type.Service ?? throw new InvalidOperationException(), type.Implementation);
            }
        }
    }
}