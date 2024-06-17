using summary.api.Clients.Files;
using summary.api.Clients.GPT;
using summary.api.Repositorys;
using summary.api.Services;

var builder = WebApplication.CreateBuilder(args);

string CorsPolicy = "_corsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy,
      builder => builder
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependecy Injection
builder.Services.AddScoped<ISummaryService, SummaryService>();
builder.Services.AddScoped<ISummaryRepository, SummaryRepository>();
builder.Services.AddScoped<IFileReader, FileReader>();
builder.Services.AddScoped<IGptClient, GptClient>();

var app = builder.Build();
app.UseCors(CorsPolicy);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
