using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface IDBViewModel
    {
        public string Name { get; }
        public Ulid Id { get; }

        public string AuthorName { get; }
    }
}
