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
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        [Column(TypeName = "TEXT")]
        public Ulid? AvatarId { get; set; }
        [Column(TypeName ="TEXT")]
        public Ulid? WorldId { get; set; }
        [Required]
        [Column(TypeName = "TEXT")]
        public Ulid TweetId { get; set; }

        public AvatarData? AvatarData { get; set; }
        public WorldData? WorldData { get; set; }
        public ICollection<PhotoTag>? Tags { get; set; }
    }
}
