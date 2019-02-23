﻿// <auto-generated />
using System;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ChatApp.Migrations
{
    [DbContext(typeof(ChatDbContext))]
    [Migration("20190223070534_AddUserAvatar")]
    partial class AddUserAvatar
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("ChatApp.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnName("text")
                        .HasMaxLength(1000);

                    b.Property<int>("Timecreated")
                        .HasColumnName("timecreated");

                    b.Property<int>("UserId")
                        .HasColumnName("userid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("messages");
                });

            modelBuilder.Entity("ChatApp.Models.Token", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int>("Timecreated")
                        .HasColumnName("timecreated");

                    b.Property<int>("UserId")
                        .HasColumnName("userid");

                    b.Property<string>("Val")
                        .IsRequired()
                        .HasColumnName("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("tokens");
                });

            modelBuilder.Entity("ChatApp.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<byte[]>("Image")
                        .HasColumnName("image");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnName("login");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnName("password");

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("ChatApp.Models.Message", b =>
                {
                    b.HasOne("ChatApp.Models.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ChatApp.Models.Token", b =>
                {
                    b.HasOne("ChatApp.Models.User", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
