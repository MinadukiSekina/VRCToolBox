using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.ViewModel
{
    public class DBViewModel : ViewModelBase, IDBViewModel
    {
        public string Name { get; } = string.Empty;

        public Ulid Id { get; }

        public string AuthorName { get; } = string.Empty;

        public DBViewModel(IDBModelWithAuthor model)
        {
            Name = model.Name;
            Id = model.Id;
            AuthorName = model.AuthorName ?? string.Empty;
        }
    }
}
