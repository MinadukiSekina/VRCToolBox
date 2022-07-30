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
    [Index(nameof(WorldVisitId))]
    [Index(nameof(UserName))]
    public class UserActivity
    {
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        public string ActivityTime { get; set; }
        public string ActivityType { get; set; }
        public string UserName { get; set; }
        public string FileName { get; set; }

        [Column(TypeName = "TEXT")]
        public Ulid WorldVisitId { get; set; }
        public WorldVisit world { get; set; }
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
    }
}
