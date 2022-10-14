using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using VRCToolBox.Settings;

namespace VRCToolBox.Data
{
    internal class PhotoContext : DbContext
    {
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        public DbSet<PhotoData> Photos { get; set; }
        public DbSet<PhotoTag> PhotoTags { get; set; }
        public DbSet<Tweet> Tweets { get; set; }
        public DbSet<AvatarData> Avatars { get; set; }
        public DbSet<WorldData> Worlds { get; set; }
        public DbSet<UserData> Users { get; set; }

        private readonly string _connectionText;

        public PhotoContext(): this($@"{ProgramSettings.Settings.PhotoDataDBPath}\{ProgramConst.VRChatPhotoDBName}{ProgramConst.FileExtensionSQLite3}")
        {
        }
        public PhotoContext(string datasourcePath)
        {
            if (string.IsNullOrWhiteSpace(datasourcePath)) throw new ArgumentNullException(nameof(datasourcePath), message: "DatasourcePath is null or whitespace.");
            string? parentDirectoryPath = System.IO.Directory.GetParent(datasourcePath)?.FullName;
            if (string.IsNullOrWhiteSpace(parentDirectoryPath)) throw new ArgumentNullException(nameof(parentDirectoryPath), message: "ParentDirectoryPath is null or whitespace.");
            if (!System.IO.Directory.Exists(parentDirectoryPath)) System.IO.Directory.CreateDirectory(parentDirectoryPath);

            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = datasourcePath;
            _connectionText = connectionStringBuilder.ToString();
        }
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            UlidToStringConverter ulidToStringConverter = new UlidToStringConverter();
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PhotoData>().Property(p => p.WorldId).HasConversion(ulidToStringConverter);
            modelBuilder.Entity<PhotoData>().Property(p => p.AvatarId).HasConversion(ulidToStringConverter);
            modelBuilder.Entity<PhotoData>().Property(p => p.TweetId).HasConversion(ulidToStringConverter);

            modelBuilder.Entity<PhotoTag>().Property(p => p.TagId).HasConversion(ulidToStringConverter);

            modelBuilder.Entity<Tweet>().Property(t => t.TweetId).HasConversion(ulidToStringConverter);

            modelBuilder.Entity<AvatarData>().Property(a => a.AvatarId).HasConversion(ulidToStringConverter);
            modelBuilder.Entity<AvatarData>().Property(a => a.AuthorId).HasConversion(ulidToStringConverter);

            modelBuilder.Entity<WorldData>().Property(w => w.WorldId).HasConversion(ulidToStringConverter);
            modelBuilder.Entity<WorldData>().Property(w => w.AuthorId).HasConversion(ulidToStringConverter);
            modelBuilder.Entity<UserData>().Property(u => u.UserId).HasConversion(ulidToStringConverter);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionText);
            optionsBuilder.LogTo(m => System.Diagnostics.Debug.WriteLine(m));
            base.OnConfiguring(optionsBuilder);
        }
    }
}
