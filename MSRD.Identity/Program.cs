using FluentResults;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using MSRD.Identity;
using MSRD.Identity.BusinessLogic.Extensions;
using MSRD.Identity.Core;
using MSRD.Identity.Core.Query;
using MSRD.Identity.Extensions;
using MSRD.Identity.Models.UserManagement.Responses;
using MSRD.Identity.PersistentStorage.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection(nameof(SmtpSettings)));
builder.Services.Configure<Templates>(builder.Configuration.GetSection(nameof(Templates)));
builder.Services.Configure<AdminUser>(builder.Configuration.GetSection(nameof(AdminUser)));
builder.Services.AddSingleton<ResultLogger>();

// Add services to the container.

builder.Services.AddControllers();


builder.Services
    .AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddAutoMapper(x => { 
    x.CreateMap<MSRD.Identity.Core.Auth.Models.UserInfoView, UserResponseModel>(); 
    x.CreateMap<QueryResponse<MSRD.Identity.Core.Auth.Models.UserInfoView>, QueryResponse<UserResponseModel>>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});


builder.Services.AddMsrdIdentityPersistentStorage(builder.Configuration.GetConnectionString("Repository"));
builder.Services.ConfigureIdentity();
builder.Services.AddMsrdIdentityBusinessLogic();

builder.Services.Configure<HttpLoggingOptions>(options =>
{
    options.RequestHeaders.Add("x-envoy-original-path");
    options.RequestHeaders.Add("x-envoy-path-base");
});

var app = builder.Build();

app.ValidateSettings<AppSettingsValidator, AppSettings>();

app.UsePathBaseFromHeader("x-envoy-path-base");
app.UseRouting();
app.UseCors(opts => {
    opts.AllowAnyMethod();
    opts.AllowAnyHeader();
    opts.AllowAnyOrigin();
    });

app.UseHttpLogging();

app.RunMsrdIdentityPersistentStorageMigration();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ResultLogger>();
    Result.Setup(x => x.Logger = logger);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
