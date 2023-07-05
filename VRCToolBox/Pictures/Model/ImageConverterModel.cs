using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Shared;

namespace VRCToolBox.Pictures.Model
{
    public class ImageConverterModel : DisposeBase
    {
        internal async Task ConvertToWebpAsync(string fileName, int quality)
        {
            var path = await ImageFileOperator.ConvertToWebpAsync(fileName, quality);
            ProcessEx.Start(path, true);
        }
    }
}
