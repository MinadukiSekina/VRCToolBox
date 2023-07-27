using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using System.IO;
using VRCToolBox.Pictures.Shared;
using VRCToolBox.Pictures.Interface;
using SkiaSharp;

namespace VRCToolBox.Pictures.ViewModel
{
    public class ImageConverterViewmodel : ViewModelBase, ICloseWindow, IImageConverterViewModel
    {
        /// <summary>
        /// 内包するモデル
        /// </summary>
        private IImageConverterModel _model;

        public ReactiveProperty<int> QualityOfConvert { get; }
        public Reactive.Bindings.Notifiers.BusyNotifier IsConverting { get; } = new Reactive.Bindings.Notifiers.BusyNotifier();

        public ReactiveProperty<string> ButtonText { get; }

        public AsyncReactiveCommand ConvertImageFormatAsyncCommand { get; }

        public Dictionary<PictureFormat, string> ImageFormats { get; }

        public ReactiveProperty<PictureFormat> SelectFormat { get; }

        /// <summary>
        /// 変換対象の一覧
        /// </summary>
        public string[] TargetFiles { get; }

        public Action Close { get; set; } = () => { };

        public ReactiveProperty<string> ImagePath { get; }

        public ReactiveProperty<string> FileExtension { get; }

        public ReactiveProperty<int> ScaleOfResize { get; }

        public ReactiveProperty<int> IndexOfTargets { get; }

        public ReadOnlyReactiveCollection<string> TargetImages { get; }

        public AsyncReactiveCommand ConvertImagesAsyncCommand()
        {
            throw new NotImplementedException();
        }

        public ReactiveCommand SelectImageFromTargets { get; }

        public ReactiveProperty<SKImage> SelectedPreviewImage { get; }

        //public ReadOnlyReactiveCollection<string> TargetImages { get; }
        public ImageConverterViewmodel() : this(Array.Empty<string>()) { }

        // reference : https://qiita.com/kwhrkzk/items/ed0f74bb2493cf1ce60f#booleannotifier
        public ImageConverterViewmodel(string[] targetFullNames)
        {
            // モデルとの連結
            _model = new Model.ImageConverterModel(targetFullNames).AddTo(_compositeDisposable);

            QualityOfConvert = _model.QualityOfConvert.ToReactivePropertyAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
            SelectFormat     = _model.SelectedFormat.ToReactivePropertyAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
            ImagePath        = _model.TargetFileFullName.ToReactivePropertyAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
            FileExtension    = _model.FileExtensionName.ToReactivePropertyAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
            ScaleOfResize    = _model.ScaleOfResize.ToReactivePropertyAsSynchronized(v => v.Value).AddTo(_compositeDisposable);

            SelectedPreviewImage = new ReactiveProperty<SKImage>().AddTo(_compositeDisposable);

            ButtonText = IsConverting.Select(v => v ? "変換中……" : "変換を実行").ToReactiveProperty<string>().AddTo(_compositeDisposable);
            ConvertImageFormatAsyncCommand = IsConverting.Select(v => !v).ToAsyncReactiveCommand().AddTo(_compositeDisposable);
            ConvertImageFormatAsyncCommand.Subscribe(async() => await DoConvertAsync()).AddTo(_compositeDisposable);
            //TargetFiles = targetFullNames;
            TargetImages = _model.ConvertTargets.ToReadOnlyReactiveCollection(x => x.ImageFullName).AddTo(_compositeDisposable);

            IndexOfTargets = new ReactiveProperty<int>(0).AddTo(_compositeDisposable);
            SelectImageFromTargets = new ReactiveCommand().WithSubscribe(()=> _model.SelectTarget(IndexOfTargets.Value)).AddTo(_compositeDisposable);

            // 画面表示用にDictionaryを作る
            ImageFormats = Enum.GetValues(typeof(PictureFormat)).
                                Cast<PictureFormat>().
                                Select(v => (Value: v, Name: v.GetName())).
                                ToDictionary(e => e.Value, e => e.Name);

            //TargetImages = _model.ConvertTargets.ToReadOnlyReactiveCollection(x => x.RawImage).AddTo(_compositeDisposable); 

            var disposable = _model as IDisposable;
            disposable?.AddTo(_compositeDisposable);

        }
        private async Task DoConvertAsync()
        {
            try
            {
                if (IsConverting.IsBusy) return;
                using (IsConverting.ProcessStart())
                {
                    await ConvertImagesAsync();
                }
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Text = $"変換中にエラーが発生しました。{Environment.NewLine}{ex.Message}",
                    Button = MessageButton.OK,
                    Icon = MessageIcon.Error
                };
                message.ShowMessage();
            }
            finally
            {
                Close?.Invoke();
            }
        }
        private async Task ConvertImagesAsync()
        {
            var dirPath = Path.Combine(Settings.ProgramSettings.Settings.PicturesMovedFolder, "Resize", DateTime.Now.ToString("yyyyMMddhhmmss"));
            foreach (var file in TargetFiles)
            {
                await ConvertImageFormatAsync(dirPath, file);
            }
            var message = new MessageContent()
            {
                Text = "変換を完了しました。",
                Button = MessageButton.OK,
                Icon = MessageIcon.Information,
            };
            message.ShowMessage();
            ProcessEx.Start(dirPath, true);
        }
        internal async Task ConvertImageFormatAsync(string destDir, string fileName)
        {
            //await _model.ConvertToWebpAsync(destDir, fileName, QualityOfConvert.Value);
        }

        ReactiveCommand IImageConverterViewModel.SelectImageFromTargets()
        {
            throw new NotImplementedException();
        }
    }
    // reference：https://qiita.com/t13801206/items/3f9e5d125dd60c8e72c2#:~:text=Window1.xaml%E3%81%AB%E3%83%9C%E3%82%BF%E3%83%B3%E3%81%8C1%E3%81%A4%E9%85%8D%E7%BD%AE%E3%81%95%E3%82%8C%E3%81%A6%E3%81%84%E3%82%8B%E3%80%82%20%E3%81%9D%E3%81%AE%E3%83%9C%E3%82%BF%E3%83%B3%E3%82%92%E3%82%AF%E3%83%AA%E3%83%83%E3%82%AF%E3%81%99%E3%82%8B%E3%81%A8%E3%80%81Window%E3%81%8C%E9%96%89%E3%81%98%E3%82%8B%E3%80%82%20View%20%E3%83%9C%E3%82%BF%E3%83%B3%E3%81%AE%E8%A6%AAWindow%E3%82%92%E6%8E%A2%E3%81%97%E3%81%A6%E3%80%81%E3%81%9D%E3%82%8C%E3%82%92%E5%BC%95%E6%95%B0%E3%81%AB,CloseWindow%20%E3%82%92%E5%AE%9F%E8%A1%8C%E3%81%99%E3%82%8B%E3%80%82%20CloseWindow%20%E3%81%AF%E5%BE%8C%E8%BF%B0%E3%81%AEViewModel%E3%81%AB%E5%AE%9F%E8%A3%85%E3%81%99%E3%82%8B%E3%80%82
    // reference : https://www.youtube.com/embed/U7Qclpe2joo?start=120
    internal interface ICloseWindow
    {
        Action Close { get; set; }
    }
}
