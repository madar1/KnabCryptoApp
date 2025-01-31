var builder = WebApplication.CreateBuilder(args);

// Access configuration
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("QuoteAppSettings"));
builder.Services.Configure<CoinMarketCap>(builder.Configuration.GetSection("QuoteAppSettings:CoinMarketCap"));


// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();
