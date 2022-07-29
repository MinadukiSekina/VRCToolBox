﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VRCToolBox.Data;

#nullable disable

namespace VRCToolBox.Migrations.UserActivityContextMigrations
{
    [DbContext(typeof(UserActivityContext))]
    [Migration("20220729114956_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.7");

            modelBuilder.Entity("VRCToolBox.Data.UserActivity", b =>
                {
                    b.Property<string>("UserName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ActivityTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("ActivityType")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .HasColumnType("TEXT");

                    b.Property<string>("WorldVisitId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserName", "ActivityTime", "ActivityType", "FileName");

                    b.HasIndex("UserName");

                    b.HasIndex("WorldVisitId");

                    b.ToTable("UserActivities");
                });

            modelBuilder.Entity("VRCToolBox.Data.WorldVisit", b =>
                {
                    b.Property<Ulid>("WorldVisitId")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("VisitTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("WorldName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("WorldVisitId");

                    b.HasIndex("VisitTime");

                    b.HasIndex("WorldName");

                    b.ToTable("WorldVisits");
                });

            modelBuilder.Entity("VRCToolBox.Data.UserActivity", b =>
                {
                    b.HasOne("VRCToolBox.Data.WorldVisit", "world")
                        .WithMany("UserActivities")
                        .HasForeignKey("WorldVisitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("world");
                });

            modelBuilder.Entity("VRCToolBox.Data.WorldVisit", b =>
                {
                    b.Navigation("UserActivities");
                });
#pragma warning restore 612, 618
        }
    }
}
