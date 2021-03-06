// <auto-generated />
using System;
using BlogSample;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BlogSample.Migrations
{
    [DbContext(typeof(BlogDbContext))]
    [Migration("20210821151331_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0-preview.7.21378.4");

            modelBuilder.Entity("BlogSample.Blog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Blogs");
                });

            modelBuilder.Entity("BlogSample.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AuthorName")
                        .HasColumnType("TEXT");

                    b.Property<int>("BlogId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("ReadTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ThumbnailUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BlogId", "Slug")
                        .IsUnique();

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("BlogSample.Post", b =>
                {
                    b.HasOne("BlogSample.Blog", null)
                        .WithMany("MyProperty")
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BlogSample.Blog", b =>
                {
                    b.Navigation("MyProperty");
                });
#pragma warning restore 612, 618
        }
    }
}
