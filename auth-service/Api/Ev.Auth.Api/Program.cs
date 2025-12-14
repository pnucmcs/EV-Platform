using Ev.Auth.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Ev.Auth.Application.Mapping.AuthProfile).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<Ev.Auth.Application.Validation.RegisterRequestValidator>();
builder.Services.AddMediatR(typeof(Ev.Auth.Application.Services.IAuthService).Assembly);

var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection.GetValue<string>("Issuer") ?? "ev-platform";
var audience = jwtSection.GetValue<string>("Audience") ?? "ev-clients";
var signingKey = jwtSection.GetValue<string>("SigningKey") ?? "dev-signing-key-please-change";
var tokenMinutes = jwtSection.GetValue<int?>("TokenMinutes") ?? 60;

builder.Services.AddAuthInfrastructure(issuer, audience, signingKey, TimeSpan.FromMinutes(tokenMinutes));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
