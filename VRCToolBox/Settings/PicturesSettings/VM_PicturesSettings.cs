using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using VRCToolBox.Common;

namespace VRCToolBox.Settings.PicturesSettings
{
    public class VM_PicturesSettings : ViewModelBase
    {
        private M_Settings _settings;

        public ReactiveProperty<string> PicturesSavedFolder { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> PicturesMovedFolder { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> PicturesSelectedFolder { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> PicturesUpLoadedFolder { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> OtherPicturesSaveFolder { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<bool> MakeYearFolder { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> MakeDayFolder { get; } = new ReactiveProperty<bool>();
        internal VM_PicturesSettings(M_Settings m_Settings)
        {
            _settings = m_Settings;
            PicturesSavedFolder = _settings.PicturesSavedFolder.ToReactivePropertyAsSynchronized(f => f.Value).AddTo(_compositeDisposable);
            PicturesMovedFolder = _settings.PicturesMovedFolder.ToReactivePropertyAsSynchronized(f => f.Value).AddTo(_compositeDisposable);
            PicturesSelectedFolder = _settings.PicturesSelectedFolder.ToReactivePropertyAsSynchronized(f => f.Value).AddTo(_compositeDisposable);
            PicturesUpLoadedFolder = _settings.PicturesUpLoadedFolder.ToReactivePropertyAsSynchronized(f => f.Value).AddTo(_compositeDisposable);
            MakeYearFolder = _settings.MakeYearFolder.ToReactivePropertyAsSynchronized(b => b.Value).AddTo(_compositeDisposable);
            MakeDayFolder = _settings.MakeDayFolder.ToReactivePropertyAsSynchronized(b => b.Value).AddTo(_compositeDisposable);
            OtherPicturesSaveFolder = _settings.OtherPicturesSaveFolder.ToReactivePropertyAsSynchronized(f => f.Value).AddTo(_compositeDisposable);
        }
    }
}
