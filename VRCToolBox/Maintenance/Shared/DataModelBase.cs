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

        public DataModelBase(IDBOperator dBOperator)
        {
            _operator = dBOperator;
            Name.AddTo(_compositeDisposable);
        }
        public DataModelBase(IDBOperator dBOperator, object other) : this(dBOperator)
        {
            _operator = dBOperator;
            Name.AddTo(_compositeDisposable);
            this.UpdateFrom(other);
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

    }
    public class DataModelWithAuthor<T> : DataModelBase<T> ,IDataModelWithAuthor
    {
        public DataModelWithAuthor(IDBOperator dBOperator) : base(dBOperator) 
        {
            AuthorName.AddTo(_compositeDisposable);
        }

        public DataModelWithAuthor(IDBOperator dBOperator, object other) : base(dBOperator, other)
        {
            AuthorName.AddTo(_compositeDisposable);
        }

        public Ulid? AuthorId { get; set; }

        public ReactivePropertySlim<string?> AuthorName { get; } = new ReactivePropertySlim<string?>();

        public override async Task SaveAsync()
        {
            Ulid? oldAuthorId = AuthorId;
            try
            {
                if (string.IsNullOrEmpty(AuthorName.Value))
                {
                    AuthorId = null;
                }
                else
                {
                    if (await _operator.GetDataAsync<DataModelUser>(AuthorName.Value) is DataModelUser user)
                    {
                        if (AuthorId != user.Id) AuthorId = user.Id;
                    }
                    else
                    {
                        using var author  = new DataModelUser(_operator);
                        author.Id         = AuthorId ?? Ulid.NewUlid();
                        author.Name.Value = AuthorName.Value;
                        await _operator.InsertAsync<Data.UserData>(author);
                    }
                }
                await base.SaveAsync();
            }
            catch (Exception ex)
            {
                AuthorId = oldAuthorId;
                throw;
            }
        }
    }
    public class DataModelUser : DataModelBase<Data.UserData>, IDataUser
    {
        public ReactivePropertySlim<string?> TwitterId { get; } = new ReactivePropertySlim<string?>();

        public ReactivePropertySlim<string?> TwitterName { get; } = new ReactivePropertySlim<string?>();
        public DataModelUser(IDBOperator dBOperator) : base(dBOperator) { }
        public DataModelUser(IDBOperator dBOperator, object other) : base(dBOperator, other)
        {
            TwitterId.AddTo(_compositeDisposable);
            TwitterName.AddTo(_compositeDisposable);
            this.UpdateFrom(other);
        }
    }

}
