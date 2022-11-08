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
        public Ulid Id { get; }

        /// <summary>
        /// The name of the inner data.
        /// </summary>
        public ReactivePropertySlim<string> Name { get; }
        public ReactivePropertySlim<string> AuthorName { get; }

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
    /// <summary>
    /// For data model class.
    /// </summary>
    /// <typeparam name="T">POCO class.</typeparam>
    public interface IDataModel<T> : IDataModel where T : class 
    {
        /// <summary>
        /// Update the inner data.
        /// </summary>
        public void UpdateFrom(T data);


        public Task<List<T>> GetList();

        public Task<List<T>> GetList(string name);
    }
}
