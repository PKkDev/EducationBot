using Azure.Core;
using EducationBot.EfData.Context;
using EducationBot.Service.API.Middleware;
using EducationBot.Service.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Private.json", false, true);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

//builder.Configuration.AddJsonFile("privatesettings.json", optional: false, reloadOnChange: true);

builder.Services.AddTransient<TelegramService>();
builder.Services.AddTransient<LessonHelperService>();
builder.Services.AddTransient<UserChatService>();

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
            swagger.Servers = new List<OpenApiServer> {
            new() { Url = serverUrl } };
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

app.UseAuthorization();

app.MapControllers();

app.Run();
