using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Maintenance.Interface
{
    public interface IDataAccessor : IDisposable
    {
        public string TypeName { get; }

        public ReactivePropertySlim<int> SelectedIndex { get; }

        /// <summary>
        /// Search collection.
        /// </summary>
        /// <returns></returns>
        public Task SearchCollectionAsync();

        /// <summary>
        /// Search collection by name.
        /// </summary>
        /// <param name="name">World name.</param>
        /// <returns></returns>
        public Task SearchCollectionAsync(string name);

        /// <summary>
        /// Select the data from collection by index.
        /// </summary>
        /// <param name="index"></param>
        public void SelectDataFromCollection(int index);

        /// <summary>
        /// Renew the data.
        /// </summary>
        public void RenewData();

        /// <summary>
        /// Save the selected data in DB.
        /// </summary>
        /// <returns></returns>
        public Task SaveDataAsync();

        /// <summary>
        /// Delete the selected data in DB.
        /// </summary>
        /// <param name="index">The index of the selected data in the collection.</param>
        /// <returns></returns>
        public Task DeleteDataAsync(int index);
    }
    /// <summary>
    /// For data access.
    /// </summary>
    /// <typeparam name="T">Data model.</typeparam>
    public interface IDataAccessor<T> : IDataAccessor where T : class, IDataModel
    {
        /// <summary>
        /// The selected data.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// The collection of data.
        /// </summary>
        public ObservableCollectionEX<T> Collection { get; }

    }

    public interface IDataAccessorOneRelation<T, U> : IDataAccessor<T> where T : class, IDataModel  where U : class, IDataModel
    {
        public ObservableCollectionEX<U> SubCollection_0 { get; }
    }
    public interface IDataAccessorTwoRelation<T, U, V> : IDataAccessorOneRelation<T, U> where T : class, IDataModel where U : class, IDataModel where V : class, IDataModel
    {
        public ObservableCollectionEX<V> SubCollection_1 { get; }
    }
}
