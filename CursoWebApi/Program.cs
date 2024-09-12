using CursoWebApi.Services;
using CursoWebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Leer la configuración de JWT desde appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");

// Agregar servicios al contenedor.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Configura la versión y el título de la API en la documentación de Swagger
    c.SwaggerDoc("v1", new() { Title = "Your API", Version = "v1" });

    // Configura la autenticación JWT en Swagger para proteger los endpoints
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",  // Nombre del encabezado de autenticación
        Type = SecuritySchemeType.Http,  // Tipo de esquema de seguridad
        Scheme = "bearer",  // Esquema de seguridad: Bearer (para tokens JWT)
        BearerFormat = "JWT",  // Indica que el formato es JWT
        In = ParameterLocation.Header,  // Ubicación del token: encabezado HTTP
        Description = "Ingrese 'Bearer' [espacio] seguido de su token JWT"  // Descripción que aparece en Swagger
    });

    // Establece los requisitos de seguridad: Swagger requiere el esquema de autenticación "Bearer"
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,  // Referencia al esquema de seguridad
                    Id = "Bearer"  // Nombre del esquema definido anteriormente
                }
            },
            new string[] {}  // No se especifican roles o scopes, aplicable para cualquier endpoint seguro
        }
    });
});

// Configuración del esquema de autenticación JWT
builder.Services.AddAuthentication(options =>
{
    // Esquema predeterminado para autenticar las solicitudes
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // Esquema predeterminado para manejar los desafíos de autenticación
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Configuración adicional para el esquema JWT
.AddJwtBearer(options =>
{
    // Parámetros de validación del token JWT
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,  // Valida que el emisor (Issuer) del token sea correcto
        ValidateAudience = true,  // Valida que la audiencia (Audience) del token sea correcta
        ValidateLifetime = true,  // Valida que el token no esté expirado
        ValidateIssuerSigningKey = true,  // Valida la clave de firma del token

        // Emisor y audiencia válidos, que se obtienen de la configuración (jwtSettings)
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        // Clave de firma del token, convertida a bytes
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});


// Conexión a la base de datos
builder.Services.AddSqlServer<TareasContext>(
    "Data Source=localhost;Database=TareasDb;User Id=sa;Password=123456789;TrustServerCertificate=True"
);

// Servicios de dependencia
builder.Services.AddScoped<IHelloWordService>(p => new HelloWordService());
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<ITareasService, TareasService>();

var app = builder.Build();

// Configurar el pipeline HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication(); // Añadir autenticación
app.UseAuthorization();

app.MapControllers();

app.Run();
