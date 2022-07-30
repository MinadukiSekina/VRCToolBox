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
    [Index(nameof(VisitTime))]
    [Index(nameof(WorldName))]
    public class WorldVisit
    {
        [Key]
        [Required]
        [Column(TypeName = "TEXT")]
        public Ulid WorldVisitId { get; set; }
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        public string WorldName { get; set; }
        public string VisitTime { get; set; }
        public string FileName { get; set; }
        public List<UserActivity> UserActivities { get; set; }
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
    }
}
