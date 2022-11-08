using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;
using VRCToolBox.Maintenance.Interface;

namespace VRCToolBox.Maintenance.Shared
{
    public class DataAccessor<T, U> : ModelBase, IDataAccessor<T> where T : class, IDataModel<U>, IDisposable, new() where U : class, new()
    {
        private readonly Dictionary<Type, string> _types = new Dictionary<Type, string>()
        {
            {typeof(Data.AvatarData), "アバター" },
            {typeof(Data.WorldData) , "ワールド" },
            {typeof(Data.PhotoTag)  , "タグ"     },
            {typeof(Data.UserData)  , "ユーザー" }
        };

        public ReactivePropertySlim<T> Value { get; } = new ReactivePropertySlim<T>();

        public ObservableCollectionEX<T> Collection { get; } = new ObservableCollectionEX<T>();

        public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> AuthorName { get; } = new ReactiveProperty<string>();

        public string TypeName { get; private set; }

        public DataAccessor() : this(new T()) { }
        public DataAccessor(T data)
        {
            Value.Value= data;
            TypeName   = _types[typeof(U)];
            Name       = Value.Value.Name.ToReactivePropertyAsSynchronized(n => n.Value).AddTo(_compositeDisposable);
            AuthorName = Value.Value.AuthorName.ToReactivePropertyAsSynchronized(n => n.Value).AddTo(_compositeDisposable);
            Value.AddTo(_compositeDisposable);
            _ = SearchCollectionAsync();
        }
        public async Task DeleteDataAsync(int index)
        {
            try
            {
                if (Value.Value.Id == Ulid.Empty || index < 0 || Collection.Count <= index) return;
                var message = new MessageContent()
                {
                    Button = MessageButton.OKCancel,
                    DefaultResult = MessageResult.Cancel,
                    Icon = MessageIcon.Question,
                    Text = $@"{TypeName}データを削除します。{Environment.NewLine}よろしいですか？"
                };
                var result = message.ShowDialog();
                if (result != MessageResult.OK) return;

                await Value.Value.DeleteAsync();
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
            Value.Value.UpdateFrom();
        }

        public async Task SaveDataAsync()
        {
            try
            {
                await Value.Value.SaveAsync();
                if (!Collection.Any(d => d.Id == Value.Value.Id))
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
                List<U> list = await Value.Value.GetList().ConfigureAwait(false);
                Collection.AddRange(list.Select(u => Activator.CreateInstance(typeof(T), u) as T ?? new T()));
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
                List<U> list = await Value.Value.GetList(name).ConfigureAwait(false);
                Collection.AddRange(list.Select(u => Activator.CreateInstance(typeof(T), u) as T ?? new T()));
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
        }

        public void SelectDataFromCollection(int index)
        {
            if (index < 0 || Collection.Count <= index) return;
            Value.Dispose();
            Value.Value = Collection[index];
        }
    }
}
