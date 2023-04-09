using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class DBModel : IDBModel
    {
        public string Name { get; }

        public Ulid Id { get; }

        public DBModel(string name, Ulid id)
        {
            Id = id;
            Name = name;
        }
    }
    internal class DBModelWithAuthor : IDBModelWithAuthor
    {
        public Ulid? AuthorId { get; }

        public string? AuthorName { get; }

        public string Name { get; }

        public Ulid Id { get; }

        internal DBModelWithAuthor(string name, Ulid id, string? authorName, Ulid? authorId)
        {
            AuthorId   = authorId;
            AuthorName = authorName;
            Id   = id;
            Name = name;
        }
    }
}
