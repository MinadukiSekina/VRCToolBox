using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Maintenance.Interface
{
    public interface IDataModel
    {
        /// <summary>
        /// The id of the data.
        /// </summary>
        public Ulid Id { get; set; }

        /// <summary>
        /// The name of the inner data.
        /// </summary>
        public ReactivePropertySlim<string> Name { get; }

        /// <summary>
        /// Save the data, include update, in DB.
        /// </summary>
        /// <returns></returns>
        public Task SaveAsync();

        /// <summary>
        /// Delete the data in DB.
        /// </summary>
        /// <returns></returns>
        public Task DeleteAsync();

    }
    public interface IDataModelWithAuthor : IDataModel
    {
        public Ulid? AuthorId { get; set; }
        public ReactivePropertySlim<string?> AuthorName { get; }
    }
    public interface IDataUser: IDataModel
    {
        public ReactivePropertySlim<string?> TwitterId { get; }
        public ReactivePropertySlim<string?> TwitterName { get; }
    }

    public static class DataModelExtensions
    {
        public static T UpdateFrom<T, U>(this T self, U other) where T : IDataModel
        {
            if (self is IDataModel model) 
            {
                switch (other)
                {
                    case IDataModel model2:
                        model.Id         = model2.Id;
                        model.Name.Value = model2.Name.Value;
                        break;

                    case Data.AvatarData avatar:
                        model.Id         = avatar.AvatarId;
                        model.Name.Value = avatar.AvatarName;
                        break;

                    case Data.WorldData world:
                        model.Id         = world.WorldId;
                        model.Name.Value = world.WorldName;
                        break;

                    case Data.UserData userData:
                        model.Id         = userData.UserId;
                        model.Name.Value = userData.VRChatName ?? string.Empty;
                        break;

                    default:
                        break;
                }
            }
            if (self is IDataModelWithAuthor model3) 
            {
                switch (other)
                {
                    case IDataModelWithAuthor model4:
                        model3.AuthorId         = model4.AuthorId;
                        model3.AuthorName.Value = model4.AuthorName.Value;
                        break;

                    case Data.AvatarData avatar:
                        model3.AuthorId         = avatar.AuthorId;
                        model3.AuthorName.Value = avatar.Author?.Name;
                        break;

                    case Data.WorldData world:
                        model3.AuthorId         = world.AuthorId;
                        model3.AuthorName.Value = world.Author?.Name;
                        break;

                    default:
                        break;
                }
            }
            if (self is IDataUser model5) 
            {
                switch (other)
                {
                    case IDataUser model6:
                        model5.TwitterId.Value   = model6.TwitterId.Value;
                        model5.TwitterName.Value = model6.TwitterName.Value;
                        break;

                    case Data.UserData user:
                        model5.TwitterId.Value   = user.TwitterId;
                        model5.TwitterName.Value = user.TwitterName;
                        break;
                }
            }
            return self;
        }
    }
}
