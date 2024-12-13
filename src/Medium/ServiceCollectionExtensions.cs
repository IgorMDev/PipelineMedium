using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Medium.DependencyInjection;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddMedium(this IServiceCollection services)
    {
        services.TryAddSingleton(typeof(IComponentBinderFactory<>), typeof(ComponentBinderFactory<>));
        services.TryAddSingleton(typeof(IComponentBinderFactory<,>), typeof(ComponentBinderFactory<,>));
        services.TryAddSingleton(typeof(IMedium<>), typeof(Medium<>));
        services.TryAddSingleton(typeof(IMedium<,>), typeof(Medium<,>));
        return services;
    }

    internal static IServiceCollection AddMediumServices<TPayload>(this IServiceCollection services)
    {
        services.TryAddSingleton<IComponentBinderFactory<TPayload>, ComponentBinderFactory<TPayload>>();
        services.TryAddSingleton<IMedium<TPayload>, Medium<TPayload>>();
        return services;
    }

    internal static IServiceCollection AddMediumServices<TPayload, TResult>(this IServiceCollection services)
    {
        services.TryAddSingleton<IComponentBinderFactory<TPayload, TResult>, ComponentBinderFactory<TPayload, TResult>>();
        services.TryAddSingleton<IMedium<TPayload, TResult>, Medium<TPayload, TResult>>();
        return services;
    }

    public static MediumBuilder<TPayload> AddMedium<TPayload>(this IServiceCollection services, string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        services.AddMediumServices<TPayload>();

        return new MediumBuilder<TPayload>(services, name);
    }
    public static MediumBuilder<TPayload> AddMedium<TPayload>(this IServiceCollection services)
    {
        services.AddMediumServices<TPayload>();

        return new MediumBuilder<TPayload>(services);
    }

    public static MediumBuilder<TPayload, TResult> AddMedium<TPayload, TResult>(this IServiceCollection services, string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        services.AddMediumServices<TPayload, TResult>();

        return new MediumBuilder<TPayload, TResult>(services, name);
    }
    public static MediumBuilder<TPayload, TResult> AddMedium<TPayload, TResult>(this IServiceCollection services)
    {
        services.AddMediumServices<TPayload, TResult>();

        return new MediumBuilder<TPayload, TResult>(services);
    }
}