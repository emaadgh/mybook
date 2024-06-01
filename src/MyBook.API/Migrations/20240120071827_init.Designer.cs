﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyBook.DbContexts;

#nullable disable

namespace MyBook.API.Migrations
{
    [DbContext(typeof(MyBookDbContext))]
    [Migration("20240120071827_init")]
    partial class init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MyBook.Entities.Author", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("DateOfBirth")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Authors");

                    b.HasData(
                        new
                        {
                            Id = new Guid("a4848a8c-49ed-45ec-9ef8-ea3761793db4"),
                            DateOfBirth = new DateTimeOffset(new DateTime(1965, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 30, 0, 0)),
                            Name = "J.K. Rowling"
                        },
                        new
                        {
                            Id = new Guid("ad3d0a3d-006d-4a2d-817b-114cf7e22904"),
                            DateOfBirth = new DateTimeOffset(new DateTime(1947, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 30, 0, 0)),
                            Name = "Stephen King"
                        },
                        new
                        {
                            Id = new Guid("d3b05403-79b9-460a-9d5e-a3641fd5a1b2"),
                            DateOfBirth = new DateTimeOffset(new DateTime(1952, 3, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 30, 0, 0)),
                            Name = "Douglas Adams"
                        });
                });

            modelBuilder.Entity("MyBook.Entities.Book", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasMaxLength(1500)
                        .HasColumnType("nvarchar(1500)");

                    b.Property<string>("ISBN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("PublicationDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Publisher")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Books");

                    b.HasData(
                        new
                        {
                            Id = new Guid("0d35c02d-11f2-4fad-9b76-e2562538c6fa"),
                            AuthorId = new Guid("a4848a8c-49ed-45ec-9ef8-ea3761793db4"),
                            ISBN = "439785960",
                            PublicationDate = new DateTimeOffset(new DateTime(2006, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 30, 0, 0)),
                            Publisher = "Scholastic Inc.",
                            Title = "Harry Potter and the Half-Blood Prince (Harry Potter  #6)"
                        },
                        new
                        {
                            Id = new Guid("1a177495-2212-4a00-94cf-8d1e91086f41"),
                            AuthorId = new Guid("a4848a8c-49ed-45ec-9ef8-ea3761793db4"),
                            ISBN = "439358078",
                            PublicationDate = new DateTimeOffset(new DateTime(2004, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 30, 0, 0)),
                            Publisher = "Scholastic Inc.",
                            Title = "Harry Potter and the Order of the Phoenix (Harry Potter  #5)"
                        });
                });

            modelBuilder.Entity("MyBook.Entities.Book", b =>
                {
                    b.HasOne("MyBook.Entities.Author", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });
#pragma warning restore 612, 618
        }
    }
}