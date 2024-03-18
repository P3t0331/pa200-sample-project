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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// register DBContext:
var sqliteDbName = "bookhubproject2.db";
var dbPath = Path.Combine(Environment.CurrentDirectory, sqliteDbName);
var sqliteConnectionString = $"Data Source={dbPath}";
var postgresqlConnectionString = $"Host=10.0.2.4;Port=5432;Database={sqliteDbName};Username=postgres;Password=password";

builder.Services.AddDbContextFactory<BookHubDBContext>(options =>
{
    options
        .UseNpgsql(
            postgresqlConnectionString, // Make sure this is your PostgreSQL connection string
            x => x.MigrationsAssembly("DAL.SQLite.Migrations") // Adjust the MigrationsAssembly if needed
        );
});

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
