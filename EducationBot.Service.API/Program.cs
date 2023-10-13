using EducationBot.EfData.Context;
using EducationBot.Telegram.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Configuration.AddJsonFile("privatesettings.json", optional: false, reloadOnChange: true);

builder.Services.AddTransient<TelegramService>();
builder.Services.AddTransient<LessonHelperService>();
builder.Services.AddTransient<UserChatService>();

//var path = Path.Combine(AppContext.BaseDirectory, builder.Configuration["Connections:SqLiteDbName"]);
//builder.Services.AddDbContext<DataBaseContext>(option => option.UseSqlite($"Filename={path}"));

//builder.Services.AddDbContext<DataBaseContext>(option => option.UseSqlServer(builder.Configuration["Connections:MsSqlConnect"]));

builder.Services.AddDbContext<DataBaseContext>(option
    => option.UseMySql(builder.Configuration["Connections:MySqlConnect"], new MySqlServerVersion(new Version(7, 4, 28))));

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

app.UseExceptionHandler("/error");
// app.UseDeveloperExceptionPage();

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
