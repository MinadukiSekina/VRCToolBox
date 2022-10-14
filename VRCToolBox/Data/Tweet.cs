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
    public class Tweet
    {
        [Key]
        [Required]
        [Column(TypeName ="TEXT")]
        public Ulid TweetId { get; set; }
        public string? Content { get; set; }
        public bool IsTweeted { get; set; }

        [NotMapped]
        public bool IsSaved { get; set; }

#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        public List<PhotoData> Photos { get; set; }
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        public ICollection<UserData>? Users { get; set; }
    }
}
