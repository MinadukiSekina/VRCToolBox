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
        public ReactiveCommand RenewCommand { get; } = new ReactiveCommand();

        public AsyncReactiveCommand SaveDataAsyncCommand { get; } = new AsyncReactiveCommand();

        public AsyncReactiveCommand DeleteDataAsyncCommand { get; } = new AsyncReactiveCommand();
    }
    public class VM_DataMaintenanceBase<T> : VM_DataMaintenanceBase where T : class, IDataModel, IDisposable
    {
        protected IDataAccessor<T> _dataAccessor;

        [Required(ErrorMessage = "名前は必須です。")]
        public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();

        public ReadOnlyReactivePropertySlim<string?> ErrorMessage { get; }

        public ReactivePropertySlim<int> SelectIndex { get; } = new ReactivePropertySlim<int>(-1);


        public ReadOnlyReactiveCollection<VM_DataListItems> ListItems { get; }
        public VM_DataMaintenanceBase(IDataAccessor<T> dataAccessor)
        {
            _dataAccessor = dataAccessor;
            _dataAccessor.AddTo(_compositeDisposable);

            TypeName = _dataAccessor.TypeName;

            Name = _dataAccessor.Value.Name.ToReactivePropertyAsSynchronized(d => d.Value, ReactivePropertyMode.Default | ReactivePropertyMode.IgnoreInitialValidationError, true).
                                            SetValidateAttribute(() => Name).
                                            AddTo(_compositeDisposable);

            ErrorMessage = Name.ObserveErrorChanged.Select(e => e?.Cast<string>().FirstOrDefault()).ToReadOnlyReactivePropertySlim().AddTo(_compositeDisposable);

            SearchText.Subscribe(async n => await _dataAccessor.SearchCollectionAsync(n)).AddTo(_compositeDisposable);

            //SelectIndex.Where(i => i >= 0).Subscribe(i => _dataAccessor.SelectDataFromCollection(i)).AddTo(_compositeDisposable);
            SelectIndex = _dataAccessor.SelectedIndex.ToReactivePropertySlimAsSynchronized(i => i.Value).AddTo(_compositeDisposable);

            SaveDataAsyncCommand.Subscribe(_ => _dataAccessor.SaveDataAsync()).AddTo(_compositeDisposable);
            DeleteDataAsyncCommand.Subscribe(_ => _dataAccessor.DeleteDataAsync(SelectIndex.Value)).AddTo(_compositeDisposable);
            RenewCommand.Subscribe(_ => _dataAccessor.RenewData()).AddTo(_compositeDisposable);

            ListItems = _dataAccessor.Collection.
                                      ToReadOnlyReactiveCollection(c => new VM_DataListItems(c)).
                                      AddTo(_compositeDisposable);
        }
    }
    public class VM_DataMaintenanceWithAuthor<T> : VM_DataMaintenanceBase<T> where T : class, IDataModelWithAuthor, IDisposable
    {
        public ReactivePropertySlim<string?> AuthorName { get; } = new ReactivePropertySlim<string?>();

        public VM_DataMaintenanceWithAuthor(IDataAccessor<T> dataAccessor) : base(dataAccessor)
        {
            AuthorName = _dataAccessor.Value.AuthorName.ToReactivePropertySlimAsSynchronized(a => a.Value).AddTo(_compositeDisposable);
        }
    }
    public class VM_DataMaintenanceTag<T, U> : VM_DataMaintenanceBase<T> where T : class, IDataModel, IDisposable where U : class, IDataModel, IDisposable
    {
        protected new IDataAccessorOneRelation<T, U> _dataAccessor;
        public ReadOnlyReactiveCollection<VM_DataListItems> RelatedPhotos { get; }

        public VM_DataMaintenanceTag(IDataAccessorOneRelation<T, U> dataAccessor) : base(dataAccessor)
        {
            _dataAccessor = dataAccessor;
            RelatedPhotos = _dataAccessor.RelatedPhotos.ToReadOnlyReactiveCollection(r => new VM_DataListItems(r)).AddTo(_compositeDisposable);
        }
    }
}
