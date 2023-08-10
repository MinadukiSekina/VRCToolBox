using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.ViewModel
{
    public class ResizeOptionsViewModel : ViewModelBase, IResizeOptionsViewModel
    {
        private IResizeOptions _model;

        public ReactiveProperty<int> ScaleOfResize { get; }

        public ReactiveProperty<ResizeMode> ResizeMode { get; }

        internal ResizeOptionsViewModel(IResizeOptions resizeOptions)
        {
            _model        = resizeOptions;
            ScaleOfResize = _model.ScaleOfResize.ToReactivePropertyAsSynchronized(x => x.Value, x => (int)(x * 100), x => (float)x / 100f, ignoreValidationErrorValue: true, mode: ReactivePropertyMode.Default | ReactivePropertyMode.IgnoreInitialValidationError).
                                                 SetValidateNotifyError(x => x <= 0 ? "その倍率は指定できません。" : null).
                                                 AddTo(_compositeDisposable);
            ResizeMode    = _model.ResizeMode.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(_compositeDisposable);
        }
    }
}
