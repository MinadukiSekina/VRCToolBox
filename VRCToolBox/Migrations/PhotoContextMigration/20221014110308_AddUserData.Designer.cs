﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VRCToolBox.Data;

#nullable disable

namespace VRCToolBox.Migrations.PhotoContextMigration
{
    [DbContext(typeof(PhotoContext))]
    [Migration("20221014110308_AddUserData")]
    partial class AddUserData
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.7");

            modelBuilder.Entity("PhotoDataPhotoTag", b =>
                {
                    b.Property<string>("PhotosPhotoName")
                        .HasColumnType("TEXT");

                    b.Property<string>("TagsTagId")
                        .HasColumnType("TEXT");

                    b.HasKey("PhotosPhotoName", "TagsTagId");

                    b.HasIndex("TagsTagId");

                    b.ToTable("PhotoDataPhotoTag");
                });

            modelBuilder.Entity("VRCToolBox.Data.AvatarData", b =>
                {
                    b.Property<string>("AvatarId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AuthorId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("AvatarName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("AvatarId");

                    b.HasIndex("AuthorId");

                    b.ToTable("Avatars");
                });

            modelBuilder.Entity("VRCToolBox.Data.PhotoData", b =>
                {
                    b.Property<string>("PhotoName")
                        .HasColumnType("TEXT");

                    b.Property<string>("AvatarId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PhotoDirPath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TweetId")
                        .HasColumnType("TEXT");

                    b.Property<string>("WorldId")
                        .HasColumnType("TEXT");

                    b.HasKey("PhotoName");

                    b.HasIndex("AvatarId");

                    b.HasIndex("TweetId");

                    b.HasIndex("WorldId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("VRCToolBox.Data.PhotoTag", b =>
                {
                    b.Property<string>("TagId")
                        .HasColumnType("TEXT");

                    b.Property<string>("TagName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("TagId");

                    b.HasIndex("TagName");

                    b.ToTable("PhotoTags");
                });

            modelBuilder.Entity("VRCToolBox.Data.Tweet", b =>
                {
                    b.Property<string>("TweetId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsTweeted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserDataUserId")
                        .HasColumnType("TEXT");

                    b.HasKey("TweetId");

                    b.HasIndex("UserDataUserId");

                    b.ToTable("Tweets");
                });

            modelBuilder.Entity("VRCToolBox.Data.UserData", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("TwitterId")
                        .HasColumnType("TEXT");

                    b.Property<string>("TwitterName")
                        .HasColumnType("TEXT");

                    b.Property<string>("VRChatName")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.HasIndex("VRChatName");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("VRCToolBox.Data.WorldData", b =>
                {
                    b.Property<string>("WorldId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AuthorId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("WorldName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("WorldId");

                    b.HasIndex("AuthorId");

                    b.ToTable("Worlds");
                });

            modelBuilder.Entity("PhotoDataPhotoTag", b =>
                {
                    b.HasOne("VRCToolBox.Data.PhotoData", null)
                        .WithMany()
                        .HasForeignKey("PhotosPhotoName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VRCToolBox.Data.PhotoTag", null)
                        .WithMany()
                        .HasForeignKey("TagsTagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VRCToolBox.Data.AvatarData", b =>
                {
                    b.HasOne("VRCToolBox.Data.UserData", "Author")
                        .WithMany("Avatars")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("VRCToolBox.Data.PhotoData", b =>
                {
                    b.HasOne("VRCToolBox.Data.AvatarData", "Avatar")
                        .WithMany()
                        .HasForeignKey("AvatarId");

                    b.HasOne("VRCToolBox.Data.Tweet", "Tweet")
                        .WithMany("Photos")
                        .HasForeignKey("TweetId");

                    b.HasOne("VRCToolBox.Data.WorldData", "World")
                        .WithMany()
                        .HasForeignKey("WorldId");

                    b.Navigation("Avatar");

                    b.Navigation("Tweet");

                    b.Navigation("World");
                });

            modelBuilder.Entity("VRCToolBox.Data.Tweet", b =>
                {
                    b.HasOne("VRCToolBox.Data.UserData", null)
                        .WithMany("Tweets")
                        .HasForeignKey("UserDataUserId");
                });

            modelBuilder.Entity("VRCToolBox.Data.WorldData", b =>
                {
                    b.HasOne("VRCToolBox.Data.UserData", "Author")
                        .WithMany("Worlds")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("VRCToolBox.Data.Tweet", b =>
                {
                    b.Navigation("Photos");
                });

            modelBuilder.Entity("VRCToolBox.Data.UserData", b =>
                {
                    b.Navigation("Avatars");

                    b.Navigation("Tweets");

                    b.Navigation("Worlds");
                });
#pragma warning restore 612, 618
        }
    }
}
