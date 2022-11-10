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

        /// <summary>
        /// Update the inner data.
        /// </summary>
        public void UpdateFrom();

    }
    public interface IDataModelWithAuthor : IDataModel
    {
        public Ulid? AuthorId { get; set; }
        public ReactivePropertySlim<string?> AuthorName { get; }
    }
    public static class DataModelExtensions
    {
        public static T UpdateFrom<T>(this T self, T other)
        {
            if (self is IDataModel model && other is IDataModel model2) 
            {
                model.Id         = model2.Id;
                model.Name.Value = model2.Name.Value;
            }
            if(self is IDataModelWithAuthor model3 && other is IDataModelWithAuthor model4)
            {
                model3.AuthorId         = model4.AuthorId;
                model3.AuthorName.Value = model4.AuthorName.Value;
            }
            return self;
        }
    }
}
