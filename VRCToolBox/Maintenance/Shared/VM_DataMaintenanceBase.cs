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
        public ReactivePropertySlim<string> SearchText { get; } = new ReactivePropertySlim<string>();
        public ReactiveCommand RenewCommand { get; } = new ReactiveCommand();

        public AsyncReactiveCommand SaveDataAsyncCommand { get; } = new AsyncReactiveCommand();

        public AsyncReactiveCommand DeleteDataAsyncCommand { get; } = new AsyncReactiveCommand();
    }
    public class VM_DataMaintenanceBase<T> : VM_DataMaintenanceBase where T : class, IDataModel, IDisposable, new()
    {
        protected IDataAccessor<T> _dataAccessor;

        [Required(ErrorMessage = "名前は必須です。")]
        public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();

        public ReadOnlyReactivePropertySlim<string?> ErrorMessage { get; }

        public ReactivePropertySlim<int> SelectIndex { get; } = new ReactivePropertySlim<int>(-1);


        public ReadOnlyReactiveCollection<VM_DataListItems<T>> ListItems { get; }
        public VM_DataMaintenanceBase(IDataAccessor<T> dataAccessor)
        {
            _dataAccessor = dataAccessor;
            _dataAccessor.AddTo(_compositeDisposable);

            Name = _dataAccessor.Value.Name.ToReactivePropertyAsSynchronized(d => d.Value, ReactivePropertyMode.Default | ReactivePropertyMode.IgnoreInitialValidationError, true).
                                            SetValidateAttribute(() => Name).
                                            AddTo(_compositeDisposable);

            ErrorMessage = Name.ObserveErrorChanged.Select(e => e?.Cast<string>().FirstOrDefault()).ToReadOnlyReactivePropertySlim().AddTo(_compositeDisposable);

            SearchText.Subscribe(async n => await _dataAccessor.SearchCollectionAsync(n)).AddTo(_compositeDisposable);

            SelectIndex.Where(i => i >= 0).Subscribe(i => _dataAccessor.SelectDataFromCollection(i)).AddTo(_compositeDisposable);

            SaveDataAsyncCommand.Subscribe(_ => _dataAccessor.SaveDataAsync()).AddTo(_compositeDisposable);
            DeleteDataAsyncCommand.Subscribe(_ => _dataAccessor.DeleteDataAsync(SelectIndex.Value)).AddTo(_compositeDisposable);
            RenewCommand.Subscribe(_ => _dataAccessor.RenewData()).AddTo(_compositeDisposable);

            ListItems = _dataAccessor.Collection.
                                      ToReadOnlyReactiveCollection(c => Activator.CreateInstance(typeof(VM_DataListItems<T>), c) as VM_DataListItems<T> ?? new VM_DataListItems<T>()).
                                      AddTo(_compositeDisposable);
        }
    }
    public class VM_DataMaintenanceWithAuthor<T> : VM_DataMaintenanceBase<T> where T : class, IDataModelWithAuthor, IDisposable, new()
    {
        public ReactivePropertySlim<string?> AuthorName { get; } = new ReactivePropertySlim<string?>();

        public VM_DataMaintenanceWithAuthor(IDataAccessorWithAuthor<T> dataAccessor) : base(dataAccessor)
        {
            AuthorName = _dataAccessor.Value.AuthorName.ToReactivePropertySlimAsSynchronized(a => a.Value).AddTo(_compositeDisposable);
        }
    }
}
