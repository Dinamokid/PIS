﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PisMirShow;

namespace PisMirShow.Migrations
{
    [DbContext(typeof(PisDbContext))]
    partial class PisDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PisMirShow.Models.FileInSystem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("File")
                        .IsRequired();

                    b.Property<string>("Name");

                    b.Property<int?>("TaskId")
                        .IsRequired();

                    b.Property<string>("Type")
                        .IsRequired();

                    b.Property<bool>("Сonfirmed");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("PisMirShow.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("PisMirShow.Models.TaskComments", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreateDate");

                    b.Property<int?>("TaskId")
                        .IsRequired();

                    b.Property<string>("Text")
                        .IsRequired();

                    b.Property<int?>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.HasIndex("UserId");

                    b.ToTable("TaskComments");
                });

            modelBuilder.Entity("PisMirShow.Models.TaskInSystem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DeadLine");

                    b.Property<DateTime?>("EndDate");

                    b.Property<string>("FilesId");

                    b.Property<int?>("FromUserId");

                    b.Property<DateTime?>("StartDate");

                    b.Property<int>("Status");

                    b.Property<string>("Text");

                    b.Property<string>("Title");

                    b.Property<int?>("ToUserId");

                    b.HasKey("Id");

                    b.HasIndex("FromUserId");

                    b.HasIndex("ToUserId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("PisMirShow.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BirthdayDay");

                    b.Property<string>("Department");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<DateTime?>("LastEnter");

                    b.Property<string>("LastName");

                    b.Property<string>("Login");

                    b.Property<string>("OfficePost");

                    b.Property<string>("Password");

                    b.Property<string>("Phone")
                        .HasMaxLength(12);

                    b.Property<DateTime>("RegisterTime");

                    b.Property<int?>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PisMirShow.Models.WallPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Author");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Message");

                    b.HasKey("Id");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("PisMirShow.Models.FileInSystem", b =>
                {
                    b.HasOne("PisMirShow.Models.TaskInSystem", "Task")
                        .WithMany("Files")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PisMirShow.Models.TaskComments", b =>
                {
                    b.HasOne("PisMirShow.Models.TaskInSystem", "Task")
                        .WithMany("Comments")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PisMirShow.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PisMirShow.Models.TaskInSystem", b =>
                {
                    b.HasOne("PisMirShow.Models.User", "FromUser")
                        .WithMany()
                        .HasForeignKey("FromUserId");

                    b.HasOne("PisMirShow.Models.User", "ToUser")
                        .WithMany()
                        .HasForeignKey("ToUserId");
                });

            modelBuilder.Entity("PisMirShow.Models.User", b =>
                {
                    b.HasOne("PisMirShow.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");
                });
#pragma warning restore 612, 618
        }
    }
}
