using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Access configuration
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("QuoteAppSettings"));
var appSettings = new AppSettings();
builder.Configuration.GetSection("QuoteAppSettings").Bind(appSettings);

// Force HTTPS in production
// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenAnyIP(5010);
//     options.ListenAnyIP(5011, listenOptions => listenOptions.UseHttps());
// });
// Configure Authentication with Google SSO
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = appSettings.JWToken.Authority;
    options.Audience = appSettings.JWToken.Audience;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = appSettings.GoogleOAuth.ClientId;
    options.ClientSecret = appSettings.GoogleOAuth.ClientSecret;
    options.CallbackPath = appSettings.GoogleOAuth.CallbackPath; // Ensure this matches in Google OAuth config
});

// Configure Authorization (RBAC)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOnly", policy => policy.RequireAuthenticatedUser());
});

// Configure Swagger with OAuth2 Login
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = appSettings.ApplicationName, Version = appSettings.Version });

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/auth"),
                TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "Authenticate using Google SSO" },
                    { "profile", "Access user profile information" },
                    { "email", "Access email address" }
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            new List<string> { "openid", "profile", "email" }
        }
    });
});

builder.Services.AddControllers();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", appSettings.ApplicationName);
    c.OAuthClientId(appSettings.GoogleOAuth.ClientId);
    c.OAuthClientSecret(appSettings.GoogleOAuth.ClientSecret);
    c.OAuthUsePkce();
});

app.MapControllers();
app.Run();