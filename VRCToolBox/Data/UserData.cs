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
    [Index(nameof(VRChatName))]
    public class UserData
    {
        [Key]
        [Required]
        [Column(TypeName = "TEXT")]
        public Ulid UserId { get; set; }
        public string? VRChatName { get; set; }
        public string? TwitterId { get; set; }
        public string? TwitterName { get; set; }

        [NotMapped]
        public string Name
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(VRChatName) ) return VRChatName;
                if (!string.IsNullOrWhiteSpace(TwitterName)) return TwitterName;
                if (!string.IsNullOrWhiteSpace(TwitterId)  ) return TwitterId;
                return "名前なし";
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
                if (value[0] == '@') 
                {
                    TwitterId = value;
                }
                else
                {
                    VRChatName = value;
                }
            }
        }
        public ICollection<Tweet>? Tweets { get; set; }
        public List<AvatarData>? Avatars { get; set; }
        public List<WorldData>? Worlds { get; set; }
    }
}
