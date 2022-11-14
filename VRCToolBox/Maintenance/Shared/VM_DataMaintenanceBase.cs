using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Maintenance.Interface;

namespace VRCToolBox.Maintenance.Shared
{
    public class VM_DataMaintenanceBase : ViewModelBase
    {
        public string TypeName { get; protected set; } = string.Empty;
        public ReactivePropertySlim<string> SearchText { get; } = new ReactivePropertySlim<string>();

        public ReadOnlyReactiveCollection<string>? SuggestItems { get; protected set; }
        public ReactiveCommand RenewCommand { get; } = new ReactiveCommand();

        public AsyncReactiveCommand SaveDataAsyncCommand { get; } = new AsyncReactiveCommand();

        public AsyncReactiveCommand DeleteDataAsyncCommand { get; } = new AsyncReactiveCommand();

        public ReactiveCommand<(bool, string)> SetSuggestItemsCommand { get; } = new ReactiveCommand<(bool, string)>();

        public ReactiveCommand<string> QuerySubmitesCommand { get; } = new ReactiveCommand<string>();

        public VM_DataMaintenanceBase()
        {
            SearchText.AddTo(_compositeDisposable);
            RenewCommand.AddTo(_compositeDisposable);
            SaveDataAsyncCommand.AddTo(_compositeDisposable);
            DeleteDataAsyncCommand.AddTo(_compositeDisposable);
            SetSuggestItemsCommand.AddTo(_compositeDisposable);
            QuerySubmitesCommand.AddTo(_compositeDisposable);
        }
    }
    public class VM_DataMaintenanceBase<T> : VM_DataMaintenanceBase where T : class, IDataModel, IDisposable
    {
        protected IDataAccessor<T> _dataAccessor;

        [Required(ErrorMessage = "名前は必須です。")]
        public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();

        public ReadOnlyReactivePropertySlim<string?> ErrorMessage { get; }

        public ReactivePropertySlim<int> SelectIndex { get; } = new ReactivePropertySlim<int>(-1);


        public ReadOnlyReactiveCollection<VM_DataListItems> ListItems { get; }
        public VM_DataMaintenanceBase(IDataAccessor<T> dataAccessor) : base()
        {
            _dataAccessor = dataAccessor;
            _dataAccessor.AddTo(_compositeDisposable);

            TypeName = _dataAccessor.TypeName;

            Name = _dataAccessor.Value.Name.ToReactivePropertyAsSynchronized(d => d.Value, ReactivePropertyMode.Default | ReactivePropertyMode.IgnoreInitialValidationError, true).
                                            SetValidateAttribute(() => Name).
                                            AddTo(_compositeDisposable);

            ErrorMessage = Name.ObserveErrorChanged.Select(e => e?.Cast<string>().FirstOrDefault()).ToReadOnlyReactivePropertySlim().AddTo(_compositeDisposable);

            //SearchText.Subscribe(async n => await _dataAccessor.SearchCollectionAsync(n));

            //SelectIndex.Where(i => i >= 0).Subscribe(i => _dataAccessor.SelectDataFromCollection(i)).AddTo(_compositeDisposable);
            SelectIndex = _dataAccessor.SelectedIndex.ToReactivePropertySlimAsSynchronized(i => i.Value).AddTo(_compositeDisposable);

            SaveDataAsyncCommand.Subscribe(_ => _dataAccessor.SaveDataAsync());
            DeleteDataAsyncCommand.Subscribe(_ => _dataAccessor.DeleteDataAsync(SelectIndex.Value));
            RenewCommand.Subscribe(_ => _dataAccessor.RenewData());

            ListItems = _dataAccessor.Collection.
                                      ToReadOnlyReactiveCollection(c => new VM_DataListItems(c)).
                                      AddTo(_compositeDisposable);

            SuggestItems = _dataAccessor.SuggestItems.ToReadOnlyReactiveCollection(n => n).AddTo(_compositeDisposable);
            SetSuggestItemsCommand.Subscribe<(bool isUserInput, string text)>((t) => SetSuggestItems(t.isUserInput, t.text));
            QuerySubmitesCommand.Subscribe(s => QuerySubmit(s));
        }

        protected virtual void SetSuggestItems(bool isUserInput, string text)
        {
            if (!isUserInput) return;
            _dataAccessor.SetSuggestItems(text);
        }

        protected virtual void QuerySubmit(string text)
        {
            //if (!chosenSuggestion) return;
            _dataAccessor.SearchCollectionAsync(text);
        }
    }
    public class VM_DataMaintenanceWithAuthor<T> : VM_DataMaintenanceBase<T> where T : class, IDataModelWithAuthor, IDisposable
    {
        protected new IDataAccessorWithAuthor<T> _dataAccessor;

        public ReactivePropertySlim<string?> AuthorName { get; } = new ReactivePropertySlim<string?>();

        public ReadOnlyReactiveCollection<string> AuthorNames { get; }

        public ReactiveCommand<(bool isUserInput, string text)> SetSuggestAuthorsCommand { get; } = new ReactiveCommand<(bool isUserInput, string text)>();
        public VM_DataMaintenanceWithAuthor(IDataAccessorWithAuthor<T> dataAccessor) : base(dataAccessor)
        {
            _dataAccessor = dataAccessor;
            AuthorName = _dataAccessor.Value.AuthorName.ToReactivePropertySlimAsSynchronized(a => a.Value).AddTo(_compositeDisposable);
            AuthorNames = _dataAccessor.AuthorNames.ToReadOnlyReactiveCollection(a => a).AddTo(_compositeDisposable);
            SetSuggestAuthorsCommand.Subscribe(p =>SetSuggestAuthors(p.isUserInput, p.text)).AddTo(_compositeDisposable);
        }
        protected virtual void SetSuggestAuthors(bool isUserInput, string text)
        {
            if (!isUserInput) return;
            _dataAccessor.SetSuggestAuthors(text);
        }
    }
    public class VM_DataMaintenanceOneRelation<T, U> : VM_DataMaintenanceBase<T> where T : class, IDataModel, IDisposable where U : class, IDataModel, IDisposable
    {
        protected new IDataAccessorOneRelation<T, U> _dataAccessor;
        public ReadOnlyReactiveCollection<VM_DataListItems> SubCollection_0 { get; }

        public VM_DataMaintenanceOneRelation(IDataAccessorOneRelation<T, U> dataAccessor) : base(dataAccessor)
        {
            _dataAccessor = dataAccessor;
            SubCollection_0 = _dataAccessor.SubCollection_0.ToReadOnlyReactiveCollection(r => new VM_DataListItems(r)).AddTo(_compositeDisposable);
        }
    }
    public class VM_DataMaintenanceTwoRelation<T, U, V> : VM_DataMaintenanceOneRelation<T, U> where T : class, IDataModel, IDisposable where U : class, IDataModel, IDisposable where V : class, IDataModel, IDisposable
    {
        protected new IDataAccessorTwoRelation<T, U, V> _dataAccessor;
        public ReadOnlyReactiveCollection<VM_DataListItems> SubCollection_1 { get; }

        public VM_DataMaintenanceTwoRelation(IDataAccessorTwoRelation<T, U, V> dataAccessor) : base(dataAccessor)
        {
            _dataAccessor = dataAccessor;
            SubCollection_1 = _dataAccessor.SubCollection_1.ToReadOnlyReactiveCollection(r => new VM_DataListItems(r)).AddTo(_compositeDisposable);
        }
    }
}
