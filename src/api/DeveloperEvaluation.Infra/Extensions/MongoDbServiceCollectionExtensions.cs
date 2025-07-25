using DeveloperEvaluation.Core.Domain.Entities;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Infra.Configuration;
using DeveloperEvaluation.Infra.Repositories;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class MongoDbServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

        services.RegisterMongoDbCollections(builder =>
        {
            builder
                .RegisterCollection<Sale>("Sales", cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                })
                .RegisterCollection<SaleItem>("SaleItems", cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                })
                .RegisterCollection<User>("Users", cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                }).
                RegisterCollection<Product>("Products", cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                }).
                RegisterCollection<Cart>("Carts", cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
        });

        services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));

        return services;
    }

    public static IServiceCollection RegisterMongoDbCollections(this IServiceCollection services, Action<MongoCollectionBuilder> configure)
    {
        var builder = new MongoCollectionBuilder();
        configure(builder);

        builder.ApplyClassMaps();

        services.AddSingleton(builder);

        return services;
    }
}
