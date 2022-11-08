using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MSRD.Identity.Core.Auth.Models;

namespace MSRD.Identity.PersistentStorage
{
    public sealed class MsrdIdentityContext : IdentityDbContext<User, Role, string,
        IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public MsrdIdentityContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }


        public void Migrate()
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            //https://github.com/openiddict/openiddict-core/issues/401
            builder.UseOpenIddict();

            builder.Entity<User>(b =>
            {
                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<Role>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });
        }
    }
    internal class RepositoryContextFactory : IDesignTimeDbContextFactory<MsrdIdentityContext>
    {
        public MsrdIdentityContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory() + @"\..\MSRD.Identity\")
                .AddJsonFile("appSettings.Development.json")
                .Build();
            return new MsrdIdentityContext(new DbContextOptionsBuilder<MsrdIdentityContext>().UseSqlServer(configuration.GetConnectionString("Repository")).Options);
        }
    }
}
