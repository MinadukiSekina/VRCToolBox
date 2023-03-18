using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    public class DirectoryModel : IDirectory
    {
        private DirectoryInfo? _directory;
        private bool _isExpanded;
        private bool _isLoaded;

        public string Name { get; } = string.Empty;

        public ObservableCollectionEX<IDirectory> Children { get; } = new ObservableCollectionEX<IDirectory>();

        public DirectoryModel(string dirPath) : this(new DirectoryInfo(dirPath))
        {
        }
        public DirectoryModel(DirectoryInfo info)
        {
            _directory = info;
            Name       = _directory.Name;
            // Add dummy.
            Children.Add(new DirectoryModel());
        }
        private DirectoryModel()
        {
            _directory = null;
            Name = string.Empty;
        }

        public void Expand()
        {
            if (!_isLoaded) 
            {
                _isLoaded = true;
                return;
            }
            if (_isExpanded) return;
            try
            {
                // Remove sub directories.
                Children.Clear();

                // Search children.
                var subDirectories = _directory?.EnumerateDirectories();

                if (subDirectories is null || !subDirectories.Any())
                {
                    _isExpanded = true;
                    //subDirectories = null;
                    return;
                }

                //// Add children.
                Children.AddRange(subDirectories.Where(d => ((d.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden) && ((d.Attributes & FileAttributes.System) != FileAttributes.System)).
                                                 Select(d => new DirectoryModel(d)));  
                // Sub directories added.
                _isExpanded = true;
            }
            catch (Exception ex)
            {
                // ToDo : do something.
            }
        }
    }
}
