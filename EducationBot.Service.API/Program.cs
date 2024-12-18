using EducationBot.EfData.Context;
using EducationBot.Service.API.AuthRules;
using EducationBot.Service.API.BackJobs;
using EducationBot.Service.API.Middleware;
using EducationBot.Service.API.Model;
using EducationBot.Service.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NLog.Web;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddJsonFile("appsettings.Private.json", false, true);

    builder.Services.Configure<TelegramSettings>(builder.Configuration.GetSection("TelegramSettings"));

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
    builder.Host.UseNLog();

    builder.Services.AddScoped<TelegramService>();
    builder.Services.AddScoped<LessonHelperService>();
    builder.Services.AddScoped<UserChatService>();

    builder.Services.AddScoped<CheckLessonWorker>();
    builder.Services.AddHostedService<CheckLessonHostedService>();

    #region Auth

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer();

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("ApiKeyPolicy", policy =>
        {
            policy.AddAuthenticationSchemes(new[] { JwtBearerDefaults.AuthenticationScheme });
            policy.Requirements.Add(new ApiKeyRequirement());
        });
    });

    builder.Services.AddScoped<IAuthorizationHandler, ApiKeyHandler>();

    #endregion Auth

    #region Sqlite

    //var source = Path.Combine(AppContext.BaseDirectory, "educationBot.db");
    //var connection = $"Data Source={source}";

    //builder.Services.AddDbContext<DataBaseContext>(option => option.UseSqlite(connection));

    #endregion Sqlite

    #region Postgre

    var connection = builder.Configuration.GetConnectionString("postgreSql");
    builder.Services.AddDbContext<DataBaseContext>(option => option.UseNpgsql(connection));

    #endregion Postgre

    //using ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
    //var context = serviceProvider.GetRequiredService<DataBaseContext>();
    //DataBaseContext.SeedInitilData(context);

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "EducationBot.Telegram",
            Version = "v1"
        });

        c.OperationFilter<AddRequiredHeaderParameter>();
    });

    builder.Services.AddHttpClient();

    #region Register CORS

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowAnyOrigin();
        });
    });

    #endregion Register CORS

    var app = builder.Build();

    app.UseMiddleware<ErrorHandler>();

    app.UseSwagger(opt =>
    {
        if (builder.Environment.IsProduction())
        {
            opt.PreSerializeFilters.Add((swagger, httpReq) =>
            {
                //var serverUrl = $"{httpReq.Scheme}://{httpReq.Host}/education-bot/";
                var serverUrl = $"https://{httpReq.Host}/education-bot/";
                swagger.Servers = new List<OpenApiServer> { new() { Url = serverUrl } };
            });
        }
    });
    app.UseSwaggerUI(opt =>
    {
        if (builder.Environment.IsDevelopment())
            opt.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        else
            opt.SwaggerEndpoint("/education-bot/swagger/v1/swagger.json", "v1");
    });


    app.UseHttpsRedirection();

    //app.UseStaticFiles();
    //app.UseStaticFiles(new StaticFileOptions()
    //{
    //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources\Images")),
    //    RequestPath = new PathString("/Resources/Images")
    //});

    #region Use CORS

    app.UseCors();

    #endregion Use CORS

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}