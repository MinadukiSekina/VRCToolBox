using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.ViewModel
{
    public class AsyncImageViewModel : ViewModelBase
    {
        private string _imagePath;
        public ReactiveProperty<System.Windows.Media.Imaging.BitmapImage> Image { get; } = new ReactiveProperty<System.Windows.Media.Imaging.BitmapImage>();

        public AsyncImageViewModel(string path)
        {
            _imagePath = path;
            Image.AddTo(_compositeDisposable);
            _ = LoadImageAsync().ConfigureAwait(false);
        }
        private async Task LoadImageAsync()
        {
            Image.Value = await Task.Run(() => Pictures.ImageFileOperator.GetDecodedImage(_imagePath)).ConfigureAwait(false);
        }
    }
}
