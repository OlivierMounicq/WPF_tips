// Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration de l'authentification JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Configuration des politiques d'autorisation basées sur les Claims
builder.Services.AddAuthorization(options =>
{
    // Policy basée sur un claim simple
    options.AddPolicy("RequireAdminDepartment", policy =>
        policy.RequireClaim("Department", "Admin", "IT"));

    // Policy basée sur plusieurs claims
    options.AddPolicy("RequireSeniorEmployee", policy =>
        policy.RequireClaim("EmployeeLevel", "Senior", "Lead", "Manager"));

    // Policy avec condition personnalisée
    options.AddPolicy("RequireMinimumAge", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "Age") &&
            int.Parse(context.User.FindFirst("Age").Value) >= 18));

    // Policy combinant plusieurs conditions
    options.AddPolicy("RequireAdminAccess", policy =>
        policy.RequireClaim("Department", "Admin")
              .RequireClaim("AccessLevel", "Full"));

    // Policy pour vérifier l'email vérifié
    options.AddPolicy("RequireVerifiedEmail", policy =>
        policy.RequireClaim("EmailVerified", "true"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// ========================================
// AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Validation basique (à remplacer par une vraie validation)
        if (request.Username == "admin" && request.Password == "password123")
        {
            var token = GenerateJwtToken(request.Username, isAdmin: true);
            return Ok(new { token });
        }
        else if (request.Username == "user" && request.Password == "password123")
        {
            var token = GenerateJwtToken(request.Username, isAdmin: false);
            return Ok(new { token });
        }

        return Unauthorized(new { message = "Identifiants invalides" });
    }

    private string GenerateJwtToken(string username, bool isAdmin)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Création des claims pour l'utilisateur
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("Username", username),
            new Claim(ClaimTypes.Email, $"{username}@example.com"),
            new Claim("EmailVerified", "true"),
            new Claim("Age", isAdmin ? "30" : "25")
        };

        // Claims spécifiques selon le type d'utilisateur
        if (isAdmin)
        {
            claims.Add(new Claim("Department", "Admin"));
            claims.Add(new Claim("AccessLevel", "Full"));
            claims.Add(new Claim("EmployeeLevel", "Manager"));
        }
        else
        {
            claims.Add(new Claim("Department", "Sales"));
            claims.Add(new Claim("AccessLevel", "Limited"));
            claims.Add(new Claim("EmployeeLevel", "Junior"));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record LoginRequest(string Username, string Password);

// ========================================
// ProductsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // Route accessible sans authentification
    [HttpGet("public")]
    public IActionResult GetPublicProducts()
    {
        return Ok(new { message = "Liste des produits publics" });
    }

    // Route accessible uniquement aux utilisateurs authentifiés
    [Authorize]
    [HttpGet("all")]
    public IActionResult GetAllProducts()
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        return Ok(new { 
            message = $"Liste complète des produits pour {username}",
            products = new[] { "Produit 1", "Produit 2", "Produit 3" }
        });
    }

    // Route accessible uniquement aux départements Admin ou IT
    [Authorize(Policy = "RequireAdminDepartment")]
    [HttpGet("admin")]
    public IActionResult GetAdminProducts()
    {
        var department = User.FindFirst("Department")?.Value;
        return Ok(new { 
            message = $"Produits admin pour le département {department}",
            products = new[] { "Produit Admin 1", "Produit Admin 2" }
        });
    }

    // Route pour les employés seniors uniquement
    [Authorize(Policy = "RequireSeniorEmployee")]
    [HttpGet("premium")]
    public IActionResult GetPremiumProducts()
    {
        var level = User.FindFirst("EmployeeLevel")?.Value;
        return Ok(new { 
            message = $"Produits premium pour niveau {level}",
            products = new[] { "Premium 1", "Premium 2" }
        });
    }

    // Route nécessitant plusieurs conditions (Admin + Full Access)
    [Authorize(Policy = "RequireAdminAccess")]
    [HttpPost]
    public IActionResult CreateProduct([FromBody] ProductRequest product)
    {
        return Ok(new { 
            message = "Produit créé avec succès",
            product = product.Name
        });
    }

    // Route vérifiant manuellement les claims
    [Authorize]
    [HttpGet("custom-check")]
    public IActionResult CustomClaimCheck()
    {
        // Vérification manuelle des claims
        var hasDepartmentClaim = User.HasClaim(c => c.Type == "Department");
        var departmentValue = User.FindFirst("Department")?.Value;
        var accessLevel = User.FindFirst("AccessLevel")?.Value;

        if (accessLevel == "Full")
        {
            return Ok(new { 
                message = "Accès complet autorisé",
                department = departmentValue,
                allClaims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }

        return Forbid();
    }

    // Route avec vérification d'âge minimum
    [Authorize(Policy = "RequireMinimumAge")]
    [HttpGet("adult-only")]
    public IActionResult GetAdultOnlyProducts()
    {
        var age = User.FindFirst("Age")?.Value;
        return Ok(new { 
            message = $"Produits réservés aux adultes (votre âge: {age})",
            products = new[] { "Produit 18+", "Produit Mature" }
        });
    }
}

public record ProductRequest(string Name, decimal Price);

// ========================================
// appsettings.json
/*
{
  "Jwt": {
    "Key": "VotreCléSecrèteTrèsLongueEtSécurisée123456789",
    "Issuer": "MonAPI",
    "Audience": "MonAPIClient"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
*/
