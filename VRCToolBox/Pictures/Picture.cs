using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VRCToolBox.Common;

namespace VRCToolBox.Pictures
{
    public class Picture : ViewModelBase
    {
        public string FileName { get; set; } = string.Empty;
        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                RaisePropertyChanged();
            }
        }
        public BitmapImage? Image { get; set; }
    }
}
