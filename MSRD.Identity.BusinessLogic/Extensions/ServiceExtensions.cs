using Microsoft.Extensions.DependencyInjection;
using MSRD.Identity.BusinessLogic.Services;
using MSRD.Identity.BusinessLogic.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.BusinessLogic.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMsrdIdentityBusinessLogic(this IServiceCollection services)
        {
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IAccountService, AccountService>(); 
            services.AddTransient<IEmailSendingService, EmailSendingService>(); 

            return services;
        }
    }
}
