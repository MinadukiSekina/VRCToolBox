using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VRCToolBox.Data
{
    public class PhotoData
    {
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        [Key]
        [Required]
        public string PhotoName { get; set; }
        [Required]
        public string PhotoDirPath { get; set; }
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        [Column(TypeName = "TEXT")]
        public Ulid? AvatarId { get; set; }
        [Column(TypeName ="TEXT")]
        public Ulid? WorldId { get; set; }
        [Column(TypeName = "TEXT")]
        public Ulid? TweetId { get; set; }

        [NotMapped]
        public string FullName
        {
            get { return $@"{PhotoDirPath}\{PhotoName}"; }
            set
            {
                if (value is null) throw new ArgumentNullException(nameof(value));
                if (!System.IO.File.Exists(value)) throw new System.IO.FileNotFoundException(value);

                string? dirPath   = System.IO.Path.GetDirectoryName(value);
                string? photoName = System.IO.Path.GetFileName(value);

                if (dirPath is null) throw new ArgumentNullException($@"フォルダのパスを抽出できません。{dirPath}");
                if (!System.IO.Directory.Exists(dirPath)) throw new System.IO.DirectoryNotFoundException(dirPath);

                if (photoName is null) throw new ArgumentNullException($@"ファイル名を抽出できません。{dirPath}");

                PhotoDirPath = dirPath;
                PhotoName    = photoName;
            }
        }
        [NotMapped]
        public bool IsSaved { get; set; }

        public AvatarData? Avatar { get; set; }
        public WorldData? World { get; set; }
        public Tweet? Tweet { get; set; }
        public ICollection<PhotoTag>? Tags { get; set; }
    }
}
