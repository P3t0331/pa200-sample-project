using BusinessLayer.DTOs.Author;
using BusinessLayer.DTOs.Book;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Author;
using BusinessLayer.Services;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// register DBContext:
var mssqlDbName = "Bookhubproject-2-DB-2";

var folder = Environment.SpecialFolder.LocalApplicationData;
var dbPath = Path.Join(Environment.GetFolderPath(folder), mssqlDbName);

var mssqlConnectionString = $"Server=(localdb)\\mssqllocaldb;Integrated Security=True;MultipleActiveResultSets=True;Database={mssqlDbName};Trusted_Connection=True;";

builder.Services.AddDbContextFactory<BookHubDBContext>(options =>
{
    options
        .UseSqlServer(
            mssqlConnectionString,
            x => x.MigrationsAssembly("DAL.MSSQL.Migrations")
        )
        ;

});

/* Register Services */
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

TypeAdapterConfig<Author, AuthorDTO>.NewConfig().Map(dest => dest.Books, src => src.AuthorBooks.Select(ab => ab.Book.Adapt<BookDTO>()));
TypeAdapterConfig<Book, BookDTO>.NewConfig().Map(dest => dest.Authors, src => src.AuthorBooks.Select(ab => ab.Author.Adapt<AuthorWithoutBooksDTO>()))
                                            .Map(dest => dest.Genres, src => src.GenreBooks.Select(gb => gb.Genre.Adapt<GenreDTO>()));
TypeAdapterConfig<Customer, CustomerDTO>.NewConfig().Map(dest => dest.Username, src => src.Username);


builder.Services.AddIdentity<LocalIdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<BookHubDBContext>()
    .AddDefaultTokenProviders();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
