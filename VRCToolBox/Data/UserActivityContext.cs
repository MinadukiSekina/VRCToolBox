using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VRCToolBox.Settings;

namespace VRCToolBox.Data
{
    internal class UserActivityContext : DbContext
    {
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<WorldVisit> WorldVisits { get; set; }

        private readonly string _connectionText;
        public UserActivityContext() : this(ProgramSettings.Settings.VRChatLogPath)
        {
        }
        public UserActivityContext(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException();
            if (!System.IO.Directory.Exists(System.IO.Directory.GetParent(connectionString)?.FullName))
                throw new System.IO.DirectoryNotFoundException();
            _connectionText = connectionString;
        }
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            UlidToStringConverter ulidToStringConverter = new UlidToStringConverter();
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserActivity>().Property(u => u.WorldVisitId).HasConversion(ulidToStringConverter);
            modelBuilder.Entity<UserActivity>().HasKey(u => new { u.UserName, u.ActivityTime, u.ActivityType, u.FileName });
            modelBuilder.Entity<WorldVisit>().Property(w => w.WorldVisitId).HasConversion(ulidToStringConverter);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!System.IO.Directory.Exists(ProgramSettings.Settings.UserActivityDBPath)) System.IO.Directory.CreateDirectory(ProgramSettings.Settings.UserActivityDBPath);
            optionsBuilder.UseSqlite($@"Data Source={ProgramSettings.Settings.UserActivityDBPath}\{ProgramConst.VRChatLogDBName}{ProgramConst.FileExtensionSQLite3}");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
