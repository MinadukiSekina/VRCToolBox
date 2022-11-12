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
            {typeof(DataModelBase<PhotoTag>)  , "タグ"     },
            {typeof(Data.UserData)  , "ユーザー" }
        };
        protected IDBOperator _operator;
        public T Value { get; protected set; }

        public ReactivePropertySlim<int> SelectedIndex { get; } = new ReactivePropertySlim<int>();
        public ObservableCollectionEX<T> Collection { get; } = new ObservableCollectionEX<T>();

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
                if (!Collection.Any(d => d.Id == Value.Id))
                {
                    var newTag = InstanceFactory.CreateInstance<T>(new object[] { _operator, Value });
                    if (newTag is not null) Collection.Add(newTag);
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
            if (index < 0 || Collection.Count <= index) 
            {
                Value.UpdateFrom(InstanceFactory.CreateInstance<T>(new object[] { _operator }));
                return;
            }
            var selectedData = InstanceFactory.CreateInstance<T>(new object[] { _operator, Collection[index] });
            Value.UpdateFrom(selectedData);
            SelectedIndexChanged();
        }
        protected virtual void SelectedIndexChanged() { }
    }

    public class DataAccessorTag<T, U> : DataAccessor<T>, IDataAccessorOneRelation<T, U> where T : class, IDataModel, IDisposable where U : class, IDataModel
    {
        public DataAccessorTag(IDBOperator dBOperator) : base(dBOperator)
        {
        }

        public ObservableCollectionEX<U> RelatedPhotos { get; } = new ObservableCollectionEX<U>();

        protected override async void SelectedIndexChanged()
        {
            RelatedPhotos.Clear();
            RelatedPhotos.AddRange(await _operator.GetCollectionByFKeyAsync<U>(Value.Id).ConfigureAwait(false));
        }
        protected override void ItemRenewed()
        {
            base.ItemRenewed();
            RelatedPhotos.Clear();
        }
    }
}
