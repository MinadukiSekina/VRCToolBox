using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;

namespace VRCToolBox.Pictures.ViewModel
{
    public class ImageConverterViewmodel : ViewModelBase, ICloseWindow
    {
        private Model.ImageConverterModel _model = new Model.ImageConverterModel();
        public ReactiveProperty<int> QualityOfConvert { get; } = new ReactiveProperty<int> { Value = 100 };
        public Reactive.Bindings.Notifiers.BusyNotifier IsConverting { get; } = new Reactive.Bindings.Notifiers.BusyNotifier();

        public ReactiveProperty<string> ButtonText { get; }

        public AsyncReactiveCommand ConvertImageFormatAsyncCommand { get; }

        internal string[] TargetFiles { get; set; }
        public Action Close { get; set; } = () => { };

        public ImageConverterViewmodel()
        {
            QualityOfConvert.AddTo(_compositeDisposable);
            _model.AddTo(_compositeDisposable);
            ButtonText = IsConverting.Select(v => v ? "変換中……" : "変換を実行" ).ToReactiveProperty<string>().AddTo(_compositeDisposable);
            ConvertImageFormatAsyncCommand = IsConverting.Select(v => !v).ToAsyncReactiveCommand().AddTo(_compositeDisposable);
            ConvertImageFormatAsyncCommand.Subscribe(async() => await DoConvertAsync()).AddTo(_compositeDisposable);
            TargetFiles ??= new string[0];
        }
        private async Task DoConvertAsync()
        {
            using (IsConverting.ProcessStart())
            {
                await ConvertImagesAsync();
            }
        }
        private async Task ConvertImagesAsync()
        {
            try
            {
                foreach (var file in TargetFiles)
                {
                    await ConvertImageFormatAsync(file);
                }
                var message = new MessageContent()
                {
                    Text = "変換を完了しました。",
                    Button = MessageButton.OK,
                    Icon = MessageIcon.Information,
                };
                message.ShowMessage();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Close?.Invoke();
            }
        }
        internal async Task ConvertImageFormatAsync(string fileName)
        {
            await _model.ConvertToWebpAsync(fileName, QualityOfConvert.Value);
        }
    }
    // reference：https://qiita.com/t13801206/items/3f9e5d125dd60c8e72c2#:~:text=Window1.xaml%E3%81%AB%E3%83%9C%E3%82%BF%E3%83%B3%E3%81%8C1%E3%81%A4%E9%85%8D%E7%BD%AE%E3%81%95%E3%82%8C%E3%81%A6%E3%81%84%E3%82%8B%E3%80%82%20%E3%81%9D%E3%81%AE%E3%83%9C%E3%82%BF%E3%83%B3%E3%82%92%E3%82%AF%E3%83%AA%E3%83%83%E3%82%AF%E3%81%99%E3%82%8B%E3%81%A8%E3%80%81Window%E3%81%8C%E9%96%89%E3%81%98%E3%82%8B%E3%80%82%20View%20%E3%83%9C%E3%82%BF%E3%83%B3%E3%81%AE%E8%A6%AAWindow%E3%82%92%E6%8E%A2%E3%81%97%E3%81%A6%E3%80%81%E3%81%9D%E3%82%8C%E3%82%92%E5%BC%95%E6%95%B0%E3%81%AB,CloseWindow%20%E3%82%92%E5%AE%9F%E8%A1%8C%E3%81%99%E3%82%8B%E3%80%82%20CloseWindow%20%E3%81%AF%E5%BE%8C%E8%BF%B0%E3%81%AEViewModel%E3%81%AB%E5%AE%9F%E8%A3%85%E3%81%99%E3%82%8B%E3%80%82
    // reference : https://www.youtube.com/embed/U7Qclpe2joo?start=120
    internal interface ICloseWindow
    {
        Action Close { get; set; }
    }
}
