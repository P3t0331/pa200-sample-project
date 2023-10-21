﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccessLayer.Migrations
{
    [DbContext(typeof(BookHubDBContext))]
    [Migration("20231021112254_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.12");

            modelBuilder.Entity("DataAccessLayer.Models.Author", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Authors");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "George Orwell"
                        },
                        new
                        {
                            Id = 2,
                            Name = "J.K. Rowling"
                        },
                        new
                        {
                            Id = 3,
                            Name = "J.R.R. Tolkien"
                        });
                });

            modelBuilder.Entity("DataAccessLayer.Models.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuthorId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GenreId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.Property<int>("PublisherId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("PublisherId");

                    b.ToTable("Books");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AuthorId = 1,
                            GenreId = 1,
                            Price = 15.99m,
                            PublisherId = 1,
                            Title = "1984"
                        },
                        new
                        {
                            Id = 2,
                            AuthorId = 2,
                            GenreId = 2,
                            Price = 20.99m,
                            PublisherId = 2,
                            Title = "Harry Potter and the Philosopher's Stone"
                        },
                        new
                        {
                            Id = 3,
                            AuthorId = 3,
                            GenreId = 3,
                            Price = 25.99m,
                            PublisherId = 3,
                            Title = "The Hobbit"
                        });
                });

            modelBuilder.Entity("DataAccessLayer.Models.Publisher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Publishers");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Penguin Books"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Bloomsbury"
                        },
                        new
                        {
                            Id = 3,
                            Name = "HarperCollins"
                        });
                });

            modelBuilder.Entity("DataAccessLayer.Models.Book", b =>
                {
                    b.HasOne("DataAccessLayer.Models.Author", "Author")
                        .WithMany("Books")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Models.Publisher", "Publisher")
                        .WithMany("Books")
                        .HasForeignKey("PublisherId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Author", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Publisher", b =>
                {
                    b.Navigation("Books");
                });
#pragma warning restore 612, 618
        }
    }
}