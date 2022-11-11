using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;
using VRCToolBox.Maintenance.Interface;

namespace VRCToolBox.Maintenance.Shared
{
    public class DataAccessor<T> : ModelBase, IDataAccessor<T> where T : class, IDataModel, IDisposable, new()
    {
        private readonly Dictionary<Type, string> _types = new Dictionary<Type, string>()
        {
            {typeof(Avatars.M_Avatar), "アバター" },
            {typeof(Worlds.M_World) , "ワールド" },
            {typeof(PhotoTags.M_PhotoTag)  , "タグ"     },
            {typeof(Data.UserData)  , "ユーザー" }
        };
        protected IDBOperator _operator;
        public T Value { get; protected set; }

        public ObservableCollectionEX<T> Collection { get; } = new ObservableCollectionEX<T>();

        public string TypeName { get; private set; }

        //public DataAccessor() : this(new T()) { }
        public DataAccessor(IDBOperator dBOperator) : this(new T(), dBOperator) { }
        public DataAccessor(T data, IDBOperator dBOperator)
        {
            _operator = dBOperator;
            Value     = data;
            TypeName  = _types[typeof(T)];
            Value.AddTo(_compositeDisposable);
            _ = SearchCollectionAsync().ConfigureAwait(false);
        }
        public async Task DeleteDataAsync(int index)
        {
            try
            {
                if (Value.Id == Ulid.Empty || index < 0 || Collection.Count <= index) return;
                var message = new MessageContent()
                {
                    Button = MessageButton.OKCancel,
                    DefaultResult = MessageResult.Cancel,
                    Icon = MessageIcon.Question,
                    Text = $@"{TypeName}データを削除します。{Environment.NewLine}よろしいですか？"
                };
                var result = message.ShowDialog();
                if (result != MessageResult.OK) return;

                await Value.DeleteAsync().ConfigureAwait(false);
                Collection.Remove(Collection[index]);
                RenewData();

                message = new MessageContent()
                {
                    Button = MessageButton.OK,
                    DefaultResult = MessageResult.OK,
                    Icon = MessageIcon.Information,
                    Text = $@"{TypeName}データを削除しました。"
                };
                message.ShowMessage();
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Button = MessageButton.OK,
                    DefaultResult = MessageResult.OK,
                    Icon = MessageIcon.Exclamation,
                    Text = $@"申し訳ありません。{TypeName}データの削除中にエラーが発生しました。{Environment.NewLine}{ex.Message}"
                };
                message.ShowMessage();
            }
        }

        public void RenewData()
        {
            Value.UpdateFrom(new T());
        }

        public virtual async Task SaveDataAsync()
        {
            try
            {
                await Value.SaveAsync().ConfigureAwait(false);
                if (!Collection.Any(d => d.Id == Value.Id))
                {
                    var newTag = Activator.CreateInstance(typeof(T), Value) as T;
                    if (newTag is not null) Collection.Add(newTag);
                }
                RenewData();
                var message = new MessageContent()
                {
                    Button = MessageButton.OK,
                    DefaultResult = MessageResult.OK,
                    Icon = MessageIcon.Information,
                    Text = $@"{TypeName}データを保存しました。"
                };
                message.ShowMessage();
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Button = MessageButton.OK,
                    DefaultResult = MessageResult.OK,
                    Icon = MessageIcon.Exclamation,
                    Text = $@"申し訳ありません。{TypeName}データの保存中にエラーが発生しました。{Environment.NewLine}{ex.Message}"
                };
                message.ShowMessage();
            }
        }

        public async Task SearchCollectionAsync()
        {
            try
            {
                Collection.Clear();
                Collection.AddRange(await _operator.GetCollectionAsync<T>().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
        }

        public async Task SearchCollectionAsync(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    await SearchCollectionAsync().ConfigureAwait(false);
                    return;
                }
                Collection.Clear();
                Collection.AddRange(await _operator.GetCollectionAsync<T>(name).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
        }

        public void SelectDataFromCollection(int index)
        {
            if (index < 0 || Collection.Count <= index) return;
            var selectedData = Activator.CreateInstance(typeof(T), new object[] { _operator, Collection[index] }) as T ?? new T();
            Value.UpdateFrom(selectedData);
        }
    }

    //public class DataAccessorWithAuthor<T> : DataAccessor<T>, IDataAccessorWithAuthor<T> where T : class, IDataModelWithAuthor, IDisposable, new()
    //{
    //    public DataAccessorWithAuthor(IDBOperator dBOperator) : this(new T(), dBOperator) { }
    //    public DataAccessorWithAuthor(T data, IDBOperator dBOperator) : base(data, dBOperator) { }
    //}
}
