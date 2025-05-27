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
    /// Adds Medium services for a specific request type to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The updated service collection.</returns>
    internal static IServiceCollection AddMediumServices<TRequest>(this IServiceCollection services)
    {
        services.TryAddSingleton<IComponentBinderFactory<TRequest>, ComponentBinderFactory<TRequest>>();
        services.TryAddSingleton<IMedium<TRequest>, Medium<TRequest>>();
        return services;
    }

    /// <summary>
    /// Adds Medium services for a specific request and result type to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The updated service collection.</returns>
    internal static IServiceCollection AddMediumServices<TRequest, TResult>(this IServiceCollection services)
    {
        services.TryAddSingleton<IComponentBinderFactory<TRequest, TResult>, ComponentBinderFactory<TRequest, TResult>>();
        services.TryAddSingleton<IMedium<TRequest, TResult>, Medium<TRequest, TResult>>();
        return services;
    }

    /// <summary>
    /// Adds Medium services for a specific request type to the <see cref="IServiceCollection"/> and returns a <see cref="MediumBuilder{TRequest}"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="name">The name of the Medium.</param>
    /// <returns>A <see cref="MediumBuilder{TRequest}"/> for configuring the Medium.</returns>
    public static MediumBuilder<TRequest> AddMedium<TRequest>(this IServiceCollection services, string name)
    {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(name);
#else
        if (name is null) {
            throw new ArgumentNullException(nameof(name));
        }
#endif  

        services.AddMediumServices<TRequest>();

        return new MediumBuilder<TRequest>(services, name);
    }

    /// <summary>
    /// Adds Medium services for a specific request type to the <see cref="IServiceCollection"/> and returns a <see cref="MediumBuilder{TRequest}"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>A <see cref="MediumBuilder{TRequest}"/> for configuring the Medium.</returns>
    public static MediumBuilder<TRequest> AddMedium<TRequest>(this IServiceCollection services)
    {
        services.AddMediumServices<TRequest>();

        return new MediumBuilder<TRequest>(services);
    }

    /// <summary>
    /// Adds Medium services for a specific request and result type to the <see cref="IServiceCollection"/> and returns a <see cref="MediumBuilder{TRequest, TResult}"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="name">The name of the Medium.</param>
    /// <returns>A <see cref="MediumBuilder{TRequest, TResult}"/> for configuring the Medium.</returns>
    public static MediumBuilder<TRequest, TResult> AddMedium<TRequest, TResult>(this IServiceCollection services, string name)
    {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(name);
#else
        if (name is null) {
            throw new ArgumentNullException(nameof(name));
        }
#endif  

        services.AddMediumServices<TRequest, TResult>();

        return new MediumBuilder<TRequest, TResult>(services, name);
    }

    /// <summary>
    /// Adds Medium services for a specific request and result type to the <see cref="IServiceCollection"/> and returns a <see cref="MediumBuilder{TRequest, TResult}"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>A <see cref="MediumBuilder{TRequest, TResult}"/> for configuring the Medium.</returns>
    public static MediumBuilder<TRequest, TResult> AddMedium<TRequest, TResult>(this IServiceCollection services)
    {
        services.AddMediumServices<TRequest, TResult>();

        return new MediumBuilder<TRequest, TResult>(services);
    }
}