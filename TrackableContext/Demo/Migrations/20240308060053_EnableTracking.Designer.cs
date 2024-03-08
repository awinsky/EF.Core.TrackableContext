﻿// <auto-generated />
using Demo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Demo.Migrations
{
    [DbContext(typeof(DemoContext))]
    [Migration("20240308060053_EnableTracking")]
    partial class EnableTracking
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Demo.Model.Movie", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.ToTable("Movies", (string)null);
                });

            modelBuilder.Entity("Demo.Model.MovieVersioned", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<long>("EntityVersion")
                        .HasColumnType("bigint")
                        .HasColumnName("SYS_CHANGE_VERSION");

                    b.Property<string>("Operation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("SYS_CHANGE_OPERATION");

                    b.HasKey("Id");

                    b.ToTable((string)null);

                    b.ToSqlQuery("SELECT ch.[Id], ch.SYS_CHANGE_OPERATION, ch.SYS_CHANGE_VERSION  FROM CHANGETABLE(CHANGES [dbo].[Movies], @last_synchronization_version) AS ch");
                });

            modelBuilder.Entity("Demo.Model.Movie", b =>
                {
                    b.HasOne("Demo.Model.MovieVersioned", null)
                        .WithOne("Entity")
                        .HasForeignKey("Demo.Model.Movie", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Demo.Model.MovieVersioned", b =>
                {
                    b.Navigation("Entity")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
