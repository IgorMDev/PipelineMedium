using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Medium.DependencyInjection;

/// <summary>
/// Provides extension methods for adding Medium services to the <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the core Medium services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The updated service collection.</returns>
    internal static IServiceCollection AddMedium(this IServiceCollection services)
    {
        services.TryAddSingleton(typeof(IComponentBinderFactory<>), typeof(ComponentBinderFactory<>));
        services.TryAddSingleton(typeof(IComponentBinderFactory<,>), typeof(ComponentBinderFactory<,>));
        services.TryAddSingleton(typeof(IMedium<>), typeof(Medium<>));
        services.TryAddSingleton(typeof(IMedium<,>), typeof(Medium<,>));
        return services;
    }

    /// <summary>
    /// Adds Medium services for a specific payload type to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The updated service collection.</returns>
    internal static IServiceCollection AddMediumServices<TPayload>(this IServiceCollection services)
    {
        services.TryAddSingleton<IComponentBinderFactory<TPayload>, ComponentBinderFactory<TPayload>>();
        services.TryAddSingleton<IMedium<TPayload>, Medium<TPayload>>();
        return services;
    }

    /// <summary>
    /// Adds Medium services for a specific payload and result type to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The updated service collection.</returns>
    internal static IServiceCollection AddMediumServices<TPayload, TResult>(this IServiceCollection services)
    {
        services.TryAddSingleton<IComponentBinderFactory<TPayload, TResult>, ComponentBinderFactory<TPayload, TResult>>();
        services.TryAddSingleton<IMedium<TPayload, TResult>, Medium<TPayload, TResult>>();
        return services;
    }

    /// <summary>
    /// Adds Medium services for a specific payload type to the <see cref="IServiceCollection"/> and returns a <see cref="MediumBuilder{TPayload}"/>.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="name">The name of the Medium.</param>
    /// <returns>A <see cref="MediumBuilder{TPayload}"/> for configuring the Medium.</returns>
    public static MediumBuilder<TPayload> AddMedium<TPayload>(this IServiceCollection services, string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        services.AddMediumServices<TPayload>();

        return new MediumBuilder<TPayload>(services, name);
    }

    /// <summary>
    /// Adds Medium services for a specific payload type to the <see cref="IServiceCollection"/> and returns a <see cref="MediumBuilder{TPayload}"/>.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>A <see cref="MediumBuilder{TPayload}"/> for configuring the Medium.</returns>
    public static MediumBuilder<TPayload> AddMedium<TPayload>(this IServiceCollection services)
    {
        services.AddMediumServices<TPayload>();

        return new MediumBuilder<TPayload>(services);
    }

    /// <summary>
    /// Adds Medium services for a specific payload and result type to the <see cref="IServiceCollection"/> and returns a <see cref="MediumBuilder{TPayload, TResult}"/>.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="name">The name of the Medium.</param>
    /// <returns>A <see cref="MediumBuilder{TPayload, TResult}"/> for configuring the Medium.</returns>
    public static MediumBuilder<TPayload, TResult> AddMedium<TPayload, TResult>(this IServiceCollection services, string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        services.AddMediumServices<TPayload, TResult>();

        return new MediumBuilder<TPayload, TResult>(services, name);
    }

    /// <summary>
    /// Adds Medium services for a specific payload and result type to the <see cref="IServiceCollection"/> and returns a <see cref="MediumBuilder{TPayload, TResult}"/>.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>A <see cref="MediumBuilder{TPayload, TResult}"/> for configuring the Medium.</returns>
    public static MediumBuilder<TPayload, TResult> AddMedium<TPayload, TResult>(this IServiceCollection services)
    {
        services.AddMediumServices<TPayload, TResult>();

        return new MediumBuilder<TPayload, TResult>(services);
    }
}