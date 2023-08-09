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
    public class ImageConverterViewmodel : ViewModelBase, ICloseWindow, IImageConverterViewModel, IResetImageView
    {
        /// <summary>
        /// 内包するモデル
        /// </summary>
        private IImageConverterModel _model;

        private int _oldIndexOfTargets;

        private System.Threading.CancellationTokenSource _cancellationTokenSource;

        public Reactive.Bindings.Notifiers.BusyNotifier IsConverting { get; } = new Reactive.Bindings.Notifiers.BusyNotifier();

        public ReactiveProperty<string> ButtonText { get; }

        public AsyncReactiveCommand ConvertImageFormatAsyncCommand { get; }

        public Dictionary<PictureFormat, string> ImageFormats { get; }

        public ReactiveProperty<PictureFormat> SelectFormat { get; private set; }

        public Action Close { get; set; } = () => { };

        public ReadOnlyReactivePropertySlim<string> FileExtension { get; private set; }

        public ReactiveProperty<int> IndexOfTargets { get; }

        public ReadOnlyReactiveCollection<string> TargetImages { get; private set; }

        public AsyncReactiveCommand ConvertImagesAsyncCommand { get; }

        public ReactiveCommand SelectImageFromTargets { get; }
        public ReactiveCommand CancellCommand { get; }

        public ReadOnlyReactivePropertySlim<SKBitmap?> SelectedPreviewImage { get; private set; }

        public Action ResetImageView { get; set; } = () => { };

        public ReadOnlyReactivePropertySlim<int> OldHeight { get; private set; }

        public ReadOnlyReactivePropertySlim<int> OldWidth { get; private set; }

        public IResizeOptionsViewModel ResizeOptions { get; private set; }

        public IPngEncoderOptionsViewModel PngEncoderOptions { get; private set; }

        public IJpegEncoderOptionsViewModel JpegEncoderOptions { get; private set; }

        public IWebpEncoderOptionsViewModel WebpLossyEncoderOptions { get; private set; }
        public IWebpEncoderOptionsViewModel WebpLosslessEncoderOptions { get; private set; }

        public ReadOnlyReactivePropertySlim<SKBitmap?> SelectedBaseImage { get;  private set; }

        public ReadOnlyReactivePropertySlim<string> FileSize { get; private set; }

        public ReadOnlyReactivePropertySlim<int> ChangedHeight { get; private set; }

        public ReadOnlyReactivePropertySlim<int> ChangedWidth { get; private set; }

        public ReactiveProperty<bool> IsMakingPreview { get; private set; }

        /// <summary>
        /// 画面にメインで表示するオプション
        /// </summary>
        public ReactiveProperty<System.ComponentModel.INotifyPropertyChanged> ConvertOptions { get; private set; }

        public Dictionary<ResizeMode, string> ResizeModes { get; }

        public ReactivePropertySlim<bool> ForceSameOption { get; private set; }

        public ReadOnlyReactivePropertySlim<string> NewFilSize { get; private set; }

        //public ReadOnlyReactiveCollection<string> TargetImages { get; }

        public NotifyTaskCompletion<bool> IsInitialized { get; }

        public ImageConverterViewmodel() : this(new string[] {$@"{Environment.GetFolderPath(Environment.SpecialFolder.Windows)}\Web\Wallpaper\Windows\img0.jpg" }) { }

        // reference : https://qiita.com/kwhrkzk/items/ed0f74bb2493cf1ce60f#booleannotifier
        public ImageConverterViewmodel(string[] targetFullNames)
        {
            _cancellationTokenSource = new System.Threading.CancellationTokenSource().AddTo(_compositeDisposable);

            // モデルとの連結
            _model = new Model.ImageConverterModel(targetFullNames).AddTo(_compositeDisposable);

            // 初期化処理の開始
            ButtonText = IsConverting.Select(v => v ? "変換中……" : "変換を実行").ToReactiveProperty<string>().AddTo(_compositeDisposable);
            ConvertImageFormatAsyncCommand = IsConverting.Select(v => !v).ToAsyncReactiveCommand().AddTo(_compositeDisposable);
            ConvertImageFormatAsyncCommand.Subscribe(async() => await DoConvertAsync()).AddTo(_compositeDisposable);

            IndexOfTargets = new ReactiveProperty<int>(0).AddTo(_compositeDisposable);

            SelectImageFromTargets = new ReactiveCommand().WithSubscribe(()=> SelectedImage()).AddTo(_compositeDisposable);
            ConvertImagesAsyncCommand = new AsyncReactiveCommand().AddTo(_compositeDisposable);

            CancellCommand = new ReactiveCommand().WithSubscribe(() => Close?.Invoke()).AddTo(_compositeDisposable);

            // 画面表示用にDictionaryを作る
            ImageFormats = Enum.GetValues(typeof(PictureFormat)).
                                Cast<PictureFormat>().
                                Select(v => (Value: v, Name: v.GetName())).
                                ToDictionary(e => e.Value, e => e.Name);

            ResizeModes  = Enum.GetValues(typeof(ResizeMode)).
                                Cast<ResizeMode>().
                                Select(v => (Value: v, Name: v.GetName())).
                                ToDictionary(e => e.Value, e => e.Name);

            var disposable = _model as IDisposable;
            disposable?.AddTo(_compositeDisposable);

            FileExtension = _model.SelectedPicture.ImageFullName.Select(x => Path.GetExtension(x).Replace(".", string.Empty).ToUpper())
                                                              .ToReadOnlyReactivePropertySlim(string.Empty).AddTo(_compositeDisposable);

            SelectedPreviewImage = _model.SelectedPicture.PreviewData.Select(x => SKBitmap.Decode(x)).ToReadOnlyReactivePropertySlim().AddTo(_compositeDisposable);
            SelectedBaseImage    = _model.SelectedPicture.RawData.Select(x => SKBitmap.Decode(x)).ToReadOnlyReactivePropertySlim().AddTo(_compositeDisposable);

            OldHeight = SelectedBaseImage.Select(x => x is null ? 0 : x.Height).ToReadOnlyReactivePropertySlim().AddTo(_compositeDisposable);
            OldWidth  = SelectedBaseImage.Select(x => x is null ? 0 : x.Width).ToReadOnlyReactivePropertySlim().AddTo(_compositeDisposable);

            TargetImages = _model.ConvertTargets.ToReadOnlyReactiveCollection(x => x.ImageFullName.Value).AddTo(_compositeDisposable);

            SelectFormat = _model.SelectedPicture.ConvertFormat.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(_compositeDisposable);

            ResizeOptions = new ResizeOptionsViewModel(_model.SelectedPicture.ResizeOptions).AddTo(_compositeDisposable);
            PngEncoderOptions = new PngEncoderOptionsViewModel(_model.SelectedPicture.PngEncoderOptions).AddTo(_compositeDisposable);
            JpegEncoderOptions = new JpegEncoderOptionsViewModel(_model.SelectedPicture.JpegEncoderOptions).AddTo(_compositeDisposable);

            WebpLossyEncoderOptions = new WebpEncoderOptionsViewModel(_model.SelectedPicture.WebpLossyEncoderOptions).AddTo(_compositeDisposable);
            WebpLosslessEncoderOptions = new WebpEncoderOptionsViewModel(_model.SelectedPicture.WebpLosslessEncoderOptions).AddTo(_compositeDisposable);

            ConvertOptions = _model.SelectedPicture.ConvertFormat.Select(x => ChangeConvertOptions(x)).ToReactiveProperty((System.ComponentModel.INotifyPropertyChanged)WebpLosslessEncoderOptions).AddTo(_compositeDisposable);

            FileSize = _model.SelectedPicture.FileSize.Select(x => ConvertFileSizeToString(x)).ToReadOnlyReactivePropertySlim(string.Empty).AddTo(_compositeDisposable);

            ChangedHeight = ResizeOptions.ScaleOfResize.Select(x => (int)(OldHeight.Value * (x / 100f))).ToReadOnlyReactivePropertySlim().AddTo(_compositeDisposable);
            ChangedWidth = ResizeOptions.ScaleOfResize.Select(x => (int)(OldWidth.Value * (x / 100f))).ToReadOnlyReactivePropertySlim().AddTo(_compositeDisposable);

            ForceSameOption = _model.ForceSameOptions.ToReactivePropertySlimAsSynchronized(x => x.Value).AddTo(_compositeDisposable);

            NewFilSize = _model.SelectedPicture.PreviewData.Select(x => ConvertFileSizeToString(x.Size)).ToReadOnlyReactivePropertySlim(string.Empty).AddTo(_compositeDisposable);

            IsMakingPreview = _model.SelectedPicture.IsMakingPreview.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(_compositeDisposable);

            // 画像データの読み込みを実行させる
            IsInitialized = new NotifyTaskCompletion<bool>(InitializeAsync());
        }

        private async Task<bool> InitializeAsync()
        {
            var tasks = new List<Task<bool>>();
            foreach(var target in _model.ConvertTargets)
            {
                tasks.Add(target.InitializeAsync());
            }
            tasks.Add(_model.SelectedPicture.InitializeAsync());

            _ = await Task.WhenAll(tasks);

            return true;
        }

        private async Task DoConvertAsync()
        {
            try
            {
                if (IsConverting.IsBusy) return;
                var dirPath = Path.Combine(Settings.ProgramSettings.Settings.PicturesMovedFolder, "Resize", DateTime.Now.ToString("yyyyMMddhhmmss"));
                using (IsConverting.ProcessStart())
                {
                    await _model.ConvertImagesAsync(dirPath, _cancellationTokenSource.Token).ConfigureAwait(false);
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
        private void SelectedImage()
        {
            try
            {
                _model.SelectTarget(_oldIndexOfTargets, IndexOfTargets.Value);
                _oldIndexOfTargets = IndexOfTargets.Value;
                ResetImageView();
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Text   = $"申し訳ありません。写真の表示中にエラーが発生しました。{Environment.NewLine}{ex.Message}",
                    Button = MessageButton.OK,
                    Icon   = MessageIcon.Error,
                };
                message.ShowMessage();
            }
        }
        internal async Task ConvertImageFormatAsync(string destDir, string fileName)
        {
            //await _model.ConvertToWebpAsync(destDir, fileName, QualityOfConvert.Value);
        }

        private string ConvertFileSizeToString(long fileSize)
        {
            var size = fileSize;
            var count = 0;
            while (size > 1024) 
            {
                size /= 1024;
                count++;
            }
            var unitNames = new Dictionary<int, string>()
            {
                { 0, "B"  },
                { 1, "KB" },
                { 2, "MB" },
                { 3, "TB" },
                { 4, "PB" },
                { 5, "EB" },
                { 6, "ZB" },
            };
            return unitNames.ContainsKey(count) ? $"約 {size} {unitNames[count]}" : $"約 {fileSize} B";
        }

        private System.ComponentModel.INotifyPropertyChanged ChangeConvertOptions(PictureFormat selectedFormat)
        {
            switch (selectedFormat)
            {
                case PictureFormat.Jpeg:
                    return (System.ComponentModel.INotifyPropertyChanged)JpegEncoderOptions;

                case PictureFormat.Png:
                    return (System.ComponentModel.INotifyPropertyChanged)PngEncoderOptions;

                case PictureFormat.WebpLossy:
                    //WebpEncoderOptions.WebpCompression.Value = WebpCompression.Lossy;
                    return (System.ComponentModel.INotifyPropertyChanged)WebpLossyEncoderOptions;

                case PictureFormat.WebpLossless:
                    //WebpEncoderOptions.WebpCompression.Value = WebpCompression.Lossless;
                    return (System.ComponentModel.INotifyPropertyChanged)WebpLosslessEncoderOptions;

                default:
                    throw new NotSupportedException("その形式への変換は未実装です。");
            }
        }
    }
    // reference：https://qiita.com/t13801206/items/3f9e5d125dd60c8e72c2#:~:text=Window1.xaml%E3%81%AB%E3%83%9C%E3%82%BF%E3%83%B3%E3%81%8C1%E3%81%A4%E9%85%8D%E7%BD%AE%E3%81%95%E3%82%8C%E3%81%A6%E3%81%84%E3%82%8B%E3%80%82%20%E3%81%9D%E3%81%AE%E3%83%9C%E3%82%BF%E3%83%B3%E3%82%92%E3%82%AF%E3%83%AA%E3%83%83%E3%82%AF%E3%81%99%E3%82%8B%E3%81%A8%E3%80%81Window%E3%81%8C%E9%96%89%E3%81%98%E3%82%8B%E3%80%82%20View%20%E3%83%9C%E3%82%BF%E3%83%B3%E3%81%AE%E8%A6%AAWindow%E3%82%92%E6%8E%A2%E3%81%97%E3%81%A6%E3%80%81%E3%81%9D%E3%82%8C%E3%82%92%E5%BC%95%E6%95%B0%E3%81%AB,CloseWindow%20%E3%82%92%E5%AE%9F%E8%A1%8C%E3%81%99%E3%82%8B%E3%80%82%20CloseWindow%20%E3%81%AF%E5%BE%8C%E8%BF%B0%E3%81%AEViewModel%E3%81%AB%E5%AE%9F%E8%A3%85%E3%81%99%E3%82%8B%E3%80%82
    // reference : https://www.youtube.com/embed/U7Qclpe2joo?start=120
    internal interface ICloseWindow
    {
        Action Close { get; set; }
    }

    internal interface IResetImageView
    {
        /// <summary>
        /// View に画像表示を初期化させます。
        /// </summary>
        Action ResetImageView { get; set; }
    }
}
