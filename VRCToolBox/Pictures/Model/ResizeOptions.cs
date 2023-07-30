using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class ResizeOptions : Shared.DisposeBase, IResizeOptions
    {
        private bool _disposed;
        private CompositeDisposable _disposables = new();

        /// <summary>
        /// リサイズ時のスケール
        /// </summary>
        private ReactivePropertySlim<float> ScaleOfResize { get; }

        /// <summary>
        /// リサイズ時の品質
        /// </summary>
        private ReactivePropertySlim<ResizeMode> ResizeMode { get; }

        private void SetOptions(IResizeOptions options)
        {
            ScaleOfResize.Value = options.ScaleOfResize.Value;
            ResizeMode.Value    = options.ResizeMode.Value;
        }

        ReactivePropertySlim<float> IResizeOptions.ScaleOfResize => ScaleOfResize;
        ReactivePropertySlim<ResizeMode> IResizeOptions.ResizeMode => ResizeMode;

        internal ResizeOptions()
        {
            ScaleOfResize = new ReactivePropertySlim<float>(1f).AddTo(_disposables);
            ResizeMode = new ReactivePropertySlim<ResizeMode>(Interface.ResizeMode.None).AddTo(_disposables);
        }
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _disposables.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        void IResizeOptions.SetOptions(IResizeOptions options) => SetOptions(options);
    }
}
