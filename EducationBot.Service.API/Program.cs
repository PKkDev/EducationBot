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

var app = builder.Build();

app.UseMiddleware<ErrorHandler>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    if (builder.Environment.IsDevelopment())
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EducationBot.Telegram v1");
    else
        c.SwaggerEndpoint("/EducationBot/swagger/v1/swagger.json", "EducationBot.Telegram v1");
});

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources\Images")),
    RequestPath = new PathString("/Resources/Images")
});

app.UseAuthorization();

app.MapControllers();

app.Run();
