using BusinessLayer.DTOs.Author;
using BusinessLayer.DTOs.Book;
using BusinessLayer.DTOs;
using BusinessLayer.Services;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Middleware;
using BusinessLayer.DTOs.Genre;
using WebMVC.Binders;
using System.Globalization;
using BusinessLayer.DTOs.PurchaseHistory;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// register DBContext:
var postgresConnectionString = "Host=pa200-postgres-hw2.postgres.database.azure.com;Port=5432;Database=postgres;Username=pa200-bookhub;SslMode=Require;TrustServerCertificate=true;";
var defaultCredential = new DefaultAzureCredential();
var tokenRequestContext = new Azure.Core.TokenRequestContext(new[] { "https://ossrdbms-aad.database.windows.net/.default" });

try
{
    var accessToken = (await defaultCredential.GetTokenAsync(tokenRequestContext)).Token;
    var connString = new NpgsqlConnectionStringBuilder(postgresConnectionString)
    {
        Password = accessToken,
        SslMode = SslMode.Require,
        TrustServerCertificate = true
    }.ConnectionString;

    builder.Services.AddDbContextFactory<BookHubDBContext>(options =>
    {
        options.UseNpgsql(
            connString,
            x => x.MigrationsAssembly("DAL.MSSQL.Migrations")
        );
    });
}
catch (Exception ex)
{
    throw new ApplicationException("Database connection failed", ex);
}

/* Register Services */
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IPurchaseHistoryService, PurchaseHistoryService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

TypeAdapterConfig<Author, AuthorDTO>.NewConfig().Map(dest => dest.Books, src => src.AuthorBooks.Select(ab => ab.Book.Adapt<BookDTO>()));
TypeAdapterConfig<Book, BookDTO>.NewConfig().Map(dest => dest.Authors, src => src.AuthorBooks.Select(ab => ab.Author.Adapt<AuthorWithoutBooksDTO>()))
                                            .Map(dest => dest.Genres, src => src.GenreBooks.Select(gb => gb.Genre.Adapt<GenreDTO>()));
TypeAdapterConfig<Customer, CustomerDTO>.NewConfig().Map(dest => dest.Username, src => src.Username);


builder.Services.AddIdentity<LocalIdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<BookHubDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Sets the path for the login page.
    // If a user attempts to access a resource that requires authentication and they are not authenticated,
    // they will be redirected to this path.
    options.LoginPath = "/Account/Login";
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    await SeedRoles(scope.ServiceProvider);
}

app.Run();

async Task SeedRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }
}
