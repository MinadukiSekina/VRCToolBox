using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;

namespace VRCToolBox.Maintenance.Avatars
{
    public class M_DataAccessorAvatar : ModelBase
    {
        public ObservableCollectionEX<M_Avatar> Avatars { get; } = new ObservableCollectionEX<M_Avatar>();

        public M_DataAccessorAvatar()
        {
            _ = SearchAvatarsAsync();
        }
        public async Task SearchAvatarsAsync()
        {
            try
            {
                Avatars.Clear();
                using(var context = new PhotoContext())
                {
                    List<AvatarData> avatars = await context.Avatars.Include(a => a.Author).ToListAsync();
                    Avatars.AddRange(avatars.Select(a => new M_Avatar(a)));
                }
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
        }
    }
}
