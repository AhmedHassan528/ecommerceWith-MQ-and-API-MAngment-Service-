using Authentication_With_JWT.Helper;
using Authentication_With_JWT.Setting;
using Azure.AI.OpenAI;
using ecommerceWith_MQ_and_API_MAngment_Service_.Errors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MultiTenancy.Services.OrderService;
using MultiTenancy.Services.paymobServices;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// =======================
// WebRootPath
// =======================
builder.Environment.WebRootPath ??= Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

// =======================
// 1. CORS
// =======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(
                  "http://localhost:4200",  // Your Angular dev
                  "https://tiffaney-adequate-julietta.ngrok-free.dev",
                  "http://localhost:55688",
                  "https://api-mang-test.azure-api.net" 
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowedToAllowWildcardSubdomains()
              .WithExposedHeaders("Set-Cookie"));
});

// =======================
// 2. Identity
// =======================
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// =======================
// 3. Authorization
// =======================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

// =======================
// 4. JWT Authentication
// =======================
builder.Services.AddAuthentication(op =>
{
    op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(op =>
{
    op.RequireHttpsMetadata = true;
    op.SaveToken = true;
    op.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(configuration["JWT:Key"])
        ),
        ValidateIssuer = true,
        ValidIssuer = "https://api-mang-test.azure-api.net",
        ValidateAudience = true,
        ValidAudience = "https://api-mang-test.azure-api.net",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    op.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Check cookie first
            if (context.Request.Cookies.ContainsKey("jwtToken"))
            {
                context.Token = context.Request.Cookies["jwtToken"];
            }
            else if (string.IsNullOrEmpty(context.Token))
            {
                string authorization = context.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                {
                    context.Token = authorization.Substring("Bearer ".Length).Trim();
                }
            }
            return Task.CompletedTask;
        }
    };
});


// =======================
// 5. DB Context
// =======================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
});

// =======================
// 6. Services Injection
// =======================
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoriesServices, CategoriesServices>();
builder.Services.AddScoped<IBrandServices, BrandServices>();
builder.Services.AddScoped<IWishListServices, WishListServices>();
builder.Services.AddScoped<IAddressServices, AddressServices>();
builder.Services.AddScoped<ICartServices, CartServices>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISendMail, SendMail>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymobService, PaymobService>();

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<JWT>(configuration.GetSection("JWT"));
builder.Services.Configure<MailSettings>(configuration.GetSection("MailSettings"));

// =======================
// 7. OpenAI Client / azure
// =======================
var openAiConfig = builder.Configuration.GetSection("OpenAI");
builder.Services.AddSingleton(sp =>
    new OpenAIClient(
        new Uri(openAiConfig["Endpoint"]),
        new Azure.AzureKeyCredential(openAiConfig["ApiKey"])
    )
);



// =======================
// 8. Controllers
// =======================
builder.Services.AddControllers();

// =======================
// 9. Global Exception Handler
// =======================
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// =======================
// 10. Swagger
// =======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "E-Commerce API",
        Description = "E-Commerce API"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


// =======================
// Build App
// =======================
var app = builder.Build();

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductCoverImages")),
    RequestPath = "/ProductCoverImages"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "BrandImages")),
    RequestPath = "/BrandImages"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CategoryImages")),
    RequestPath = "/CategoryImages"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImages")),
    RequestPath = "/ProductImages"
});



// =======================
// Middlewares
// =======================
app.UseCors(builder =>
    builder.WithOrigins(
            "http://localhost:4200",
            "https://tiffaney-adequate-julietta.ngrok-free.dev", "https://api-mang-test.azure-api.net")
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseExceptionHandler();

app.UseCors("AllowFrontend");
app.UseCors("AllowLocalhost");
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


//// 1️⃣ Static Files لازم أول شيء
//app.UseStaticFiles();

//// 2️⃣ Exception Handler
//app.UseExceptionHandler();

//// 3️⃣ CORS — مرّة واحدة فقط
//app.UseCors("AllowFrontend");

//// 4️⃣ HTTPS Redirection (بعد ستاتيك فايلز)
//app.UseHttpsRedirection();

//// 5️⃣ Auth
//app.UseAuthentication();
//app.UseAuthorization();


app.Run();







//var builder = WebApplication.CreateBuilder(args);



//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();




