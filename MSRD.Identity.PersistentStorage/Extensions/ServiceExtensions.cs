using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSRD.Identity.PersistentStorage.Repositories;
using MSRD.Identity.PersistentStorage.Repositories.Interfaces;

namespace MSRD.Identity.PersistentStorage.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMsrdIdentityPersistentStorage(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<MsrdIdentityContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(180));
                options.UseOpenIddict();
            });

            services.AddTransient<IIdentityRepository, IdentityRepository>();

            return services;
        }

        public static IHost RunMsrdIdentityPersistentStorageMigration(this IHost app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<MsrdIdentityContext>();
                context.Migrate();

                var identityRepository = services.GetRequiredService<IIdentityRepository>();
                identityRepository.SeedUsersAndRoles().Wait();
            }

            return app;
        }
    }
}
