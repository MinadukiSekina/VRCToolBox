using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Maintenance.Interface;

namespace VRCToolBox.Maintenance.Shared
{
    public class DataModelBase<T> : ModelBase, IDataModel
    {
        protected IDBOperator _operator;
        public Ulid Id { get; set; }

        public ReactivePropertySlim<string> Name { get; } = new ReactivePropertySlim<string>();

        public DataModelBase(IDBOperator dBOperator, DataModelBase<T> other) : this(dBOperator)
        {
            Id = other.Id;
            Name.Value = other.Name.Value;
        }
        public DataModelBase(IDBOperator dBOperator)
        {
            _operator = dBOperator;
            Name.AddTo(_compositeDisposable);
        }
        public async Task DeleteAsync()
        {
            if (Id == Ulid.Empty) return;
            await _operator.DeleteAsync<T>(this);
        }

        public virtual async Task SaveAsync()
        {
            if (Id == Ulid.Empty) 
            {
                Id = Ulid.NewUlid();
                try
                {
                    await _operator.InsertAsync<T>(this);
                }
                catch (Exception ex)
                {
                    Id = Ulid.Empty;
                    throw;
                }
            }
            else
            {
                await _operator.UpdateAsync<T>(this);
            }
        }

        public void UpdateFrom()
        {
            throw new NotImplementedException();
        }
    }
    public class DataModelWithAuthor<T> : DataModelBase<T> ,IDataModelWithAuthor
    {
        public DataModelWithAuthor(IDBOperator dBOperator) : base(dBOperator) 
        {
            AuthorName.AddTo(_compositeDisposable);
        }
        public DataModelWithAuthor(IDBOperator dBOperator, DataModelWithAuthor<T> other) : base(dBOperator, other) 
        { 
            AuthorId = other.AuthorId;
            AuthorName.Value = other.AuthorName.Value;
            AuthorName.AddTo(_compositeDisposable);
        }

        public Ulid? AuthorId { get; set; }

        public ReactivePropertySlim<string?> AuthorName { get; } = new ReactivePropertySlim<string?>();

        public override async Task SaveAsync()
        {
            if (string.IsNullOrEmpty(AuthorName.Value))
            {
                AuthorId = null;
            }
            else
            {
                if (_operator.GetData<DataModelBase<Data.UserData>>() is DataModelBase<Data.UserData> user)
                {
                    if (AuthorId != user.Id) AuthorId = user.Id;
                }
                else
                {
                    AuthorId = Ulid.NewUlid();
                }
            }
            await base.SaveAsync();
        }
    }
}
