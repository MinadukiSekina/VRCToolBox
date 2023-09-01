using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    internal interface IImageConvertTarget
    {
        /// <summary>
        /// ファイルのフルパス。
        /// </summary>
        internal ReactivePropertySlim<string> ImageFullName { get; }

        /// <summary>
        /// 変換後の形式。
        /// </summary>
        internal ReactivePropertySlim<PictureFormat> ConvertFormat { get; }

        /// <summary>
        /// リサイズする際のオプション
        /// </summary>
        internal IResizeOptions ResizeOptions { get; }

        /// <summary>
        /// PNGへ変換する際のオプション保持用
        /// </summary>
        internal IPngEncoderOptions PngEncoderOptions { get; }

        /// <summary>
        /// JPEGへ変換する際のオプション保持用
        /// </summary>
        internal IJpegEncoderOptions JpegEncoderOptions { get; }

        /// <summary>
        /// WEBP（非可逆）へ変換する際のオプション保持用
        /// </summary>
        internal IWebpEncoderOptions WebpLossyEncoderOptions { get; }

        /// <summary>
        /// WEBP（可逆）へ変換する際のオプション保持用
        /// </summary>
        internal IWebpEncoderOptions WebpLosslessEncoderOptions { get; }

        /// <summary>
        /// 引数のモデルから値を読み込みます
        /// </summary>
        /// <param name="original">読み込む値を保持しているモデル</param>
        internal Task SetPropertiesAsync(IImageConvertTarget original, bool loadOptions);

        internal Task SaveConvertedImageAsync(string directoryPath, System.Threading.CancellationToken token);
    }

    internal interface IImageConvertTargetWithReactiveImage : IImageConvertTarget
    {
        //internal ReadOnlyReactivePropertySlim<SkiaSharp.SKBitmap> RawImage { get; }
        internal ReactivePropertySlim<SkiaSharp.SKData> PreviewData { get; }
        internal Reactive.Bindings.Notifiers.BusyNotifier IsMakingPreview { get; }

        /// <summary>
        /// 元データの保持用
        /// </summary>
        internal ReactivePropertySlim<SkiaSharp.SKData> RawData { get; }

        /// <summary>
        /// 初期処理を行います
        /// </summary>
        /// <returns></returns>
        internal Task<bool> InitializeAsync();

        /// <summary>
        /// オプションの変更通知を受け取る用
        /// </summary>
        internal Task RecieveOptionValueChangedAsync();

        //internal ReadOnlyReactivePropertySlim<int> OldHeight { get; }
        //internal ReadOnlyReactivePropertySlim<int> OldWidth { get; }
    }
}
