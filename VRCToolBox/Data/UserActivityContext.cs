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
    internal class UserActivityContext : DbContext
    {
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<WorldVisit> WorldVisits { get; set; }

        private readonly string _connectionText;
        public UserActivityContext() : this($@"{ProgramSettings.Settings.UserActivityDBPath}\{ProgramConst.VRChatLogDBName}{ProgramConst.FileExtensionSQLite3}")
        {
        }
        public UserActivityContext(string datasourcePath)
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
            modelBuilder.Entity<UserActivity>().Property(u => u.WorldVisitId).HasConversion(ulidToStringConverter);
            modelBuilder.Entity<UserActivity>().HasKey(u => new { u.UserName, u.ActivityTime, u.ActivityType, u.FileName, u.FileRowIndex });
            modelBuilder.Entity<WorldVisit>().Property(w => w.WorldVisitId).HasConversion(ulidToStringConverter);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionText);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
