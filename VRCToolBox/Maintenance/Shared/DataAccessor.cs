using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;
using VRCToolBox.Maintenance.Interface;

namespace VRCToolBox.Maintenance.Shared
{
    public class DataAccessor<T> : ModelBase, IDataAccessor<T> where T : class, IDataModel, IDisposable
    {
        private readonly Dictionary<Type, string> _types = new Dictionary<Type, string>()
        {
            {typeof(DataModelWithAuthor<AvatarData>), "アバター" },
            {typeof(DataModelWithAuthor<WorldData>) , "ワールド" },
            {typeof(DataModelBase<PhotoTag>)        , "タグ"     },
            {typeof(DataModelUser)                  , "ユーザー" }
        };
        protected IDBOperator _operator;
        public T Value { get; protected set; }

        public ReactivePropertySlim<int> SelectedIndex { get; } = new ReactivePropertySlim<int>();
        public ObservableCollectionEX<T> Collection { get; } = new ObservableCollectionEX<T>();

        protected List<string> Names { get; set; } = new List<string>();
        public ObservableCollectionEX<string> SuggestItems { get; protected set; } = new ObservableCollectionEX<string>();
        public string TypeName { get; private set; }

        //public DataAccessor() : this(new T()) { }
        public DataAccessor(IDBOperator dBOperator) : this(InstanceFactory.CreateInstance<T>(new object[] {dBOperator}), dBOperator) { }
        public DataAccessor(T data, IDBOperator dBOperator)
        {
            _operator = dBOperator;
            Value     = data;
            TypeName  = _types[typeof(T)];
            Value.AddTo(_compositeDisposable);
            SelectedIndex.AddTo(_compositeDisposable);
            SelectedIndex.Value = -1;
            SelectedIndex.Subscribe(i => SelectDataFromCollection(i));
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
            ItemRenewed();
        }
        protected virtual void ItemRenewed()
        {
            SelectedIndex.Value = -1;
        }
        public virtual async Task SaveDataAsync()
        {
            try
            {
                await Value.SaveAsync().ConfigureAwait(false);
                var newData = InstanceFactory.CreateInstance<T>(new object[] { _operator, Value });

                if (Collection.FirstOrDefault(d => d.Id == Value.Id) is T item)
                {
                    var name = Names.FirstOrDefault(n => n == item.Name.Value);
                    if (!string.IsNullOrEmpty(name)) name = newData.Name.Value;
                    item.UpdateFrom(newData);                    
                }
                else
                {
                    Names.Add(newData.Name.Value);
                    Collection.Add(newData);
                }

                if (SelectedIndex.Value < 0) SelectedIndex.Value = Collection.Count - 1;

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
                
                Names.Clear();
                Names.AddRange(Collection.Select(c => c.Name.Value));

                SuggestItems.Clear();
                //SuggestItems.AddRange(Names);
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
                SuggestItems.Clear();
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
        }

        public void SelectDataFromCollection(int index)
        {
            if (index < 0 || Collection.Count <= index) 
            {
                Value.UpdateFrom(InstanceFactory.CreateInstance<T>(new object[] { _operator }));
                return;
            }
            var selectedData = Activator.CreateInstance(typeof(T), new object[] { _operator, Collection[index] });
            Value.UpdateFrom(selectedData);
            SelectedIndexChanged();
        }
        protected virtual void SelectedIndexChanged() { }

        public void SetSuggestItems(string name)
        {
            SuggestItems.Clear();
            if (string.IsNullOrEmpty(name))
            {
                SuggestItems.AddRange(Names);
            }
            else
            {
                SuggestItems.AddRange(Names.Where(n => n.Contains(name)));
            }
        }
    }

    public class DataAccessorWithAuthor<T> : DataAccessor<T>, IDataAccessorWithAuthor<T> where T : class, IDataModelWithAuthor, IDisposable
    {
        protected List<string> AuthorList { get; } = new List<string>();
        public ObservableCollectionEX<string> AuthorNames { get; } = new ObservableCollectionEX<string>();

        public DataAccessorWithAuthor(IDBOperator dBOperator) : base(dBOperator) 
        {
            _ = SetAuthorListAsync();
        }
        protected virtual async Task SetAuthorListAsync()
        {
            List<string> data = await _operator.GetNamesAsync<UserData>();
            AuthorList.AddRange(data);
        }
        public void SetSuggestAuthors(string name)
        {
            AuthorNames.Clear();
            if (string.IsNullOrEmpty(name))
            {
                AuthorNames.AddRange(AuthorList);
            }
            else
            {
                AuthorNames.AddRange(AuthorList.Where(n => n.Contains(name)));
            }
        }
        public override async Task SaveDataAsync()
        {
            try
            {
                await base.SaveDataAsync();
                if (string.IsNullOrEmpty(Value.AuthorName.Value) || AuthorNames.Any(a => a == Value.AuthorName.Value)) return;
                AuthorNames.Add(Value.AuthorName.Value);
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
    }
    public class DataAccessorOneRelation<T, U> : DataAccessor<T>, IDataAccessorOneRelation<T, U> where T : class, IDataModel, IDisposable where U : class, IDataModel
    {
        public DataAccessorOneRelation(IDBOperator dBOperator) : base(dBOperator) { }

        public ObservableCollectionEX<U> SubCollection_0 { get; } = new ObservableCollectionEX<U>();

        protected override async void SelectedIndexChanged()
        {
            SubCollection_0.Clear();
            SubCollection_0.AddRange(await _operator.GetCollectionByFKeyAsync<U>(Value.Id).ConfigureAwait(false));
        }
        protected override void ItemRenewed()
        {
            base.ItemRenewed();
            SubCollection_0.Clear();
        }
    }
    public class DataAccessorTwoRelation<T, U, V> : DataAccessorOneRelation<T, U>, IDataAccessorTwoRelation<T, U, V> where T : class, IDataModel, IDisposable where U : class, IDataModel where V : class, IDataModel
    {
        public DataAccessorTwoRelation(IDBOperator dBOperator) : base(dBOperator) { }

        public ObservableCollectionEX<V> SubCollection_1 { get; } = new ObservableCollectionEX<V>();

        protected override async void SelectedIndexChanged()
        {
            base.SelectedIndexChanged();

            SubCollection_1.Clear();
            SubCollection_1.AddRange(await _operator.GetCollectionByFKeyAsync<V>(Value.Id).ConfigureAwait(false));
        }
        protected override void ItemRenewed()
        {
            base.ItemRenewed();
            SubCollection_1.Clear();
        }
    }
}
