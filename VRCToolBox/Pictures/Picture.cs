using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VRCToolBox.Pictures
{
    public class Picture
    {
        public string FileName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public BitmapImage? Image { get; set; }
    }
}
