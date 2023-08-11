using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.ViewModel
{
    public class ResizeOptionsViewModel : ViewModelBase, IResizeOptionsViewModel
    {
        private IResizeOptions _model;

        public ReadOnlyReactivePropertySlim<int> ScaleOfResize { get; }

        public ReactiveProperty<ResizeMode> ResizeMode { get; }

        public ReactiveProperty<string> ScaleOfResizeString { get; }

        internal ResizeOptionsViewModel(IResizeOptions resizeOptions)
        {
            _model        = resizeOptions;

            // 変換後の縦・横を出す用
            ScaleOfResize = _model.ScaleOfResize.Select(x => (int)(x * 100)).ToReadOnlyReactivePropertySlim(100).AddTo(_compositeDisposable);

            ScaleOfResizeString = _model.ScaleOfResize.ToReactivePropertyAsSynchronized(propertySelector: x => x.Value, 
                                                                                        convert:          x => ((int)(x * 100)).ToString(), 
                                                                                        convertBack:      x => float.Parse(x) / 100f, 
                                                                                        ignoreValidationErrorValue: true, 
                                                                                        mode: ReactivePropertyMode.Default | ReactivePropertyMode.IgnoreInitialValidationError).
                                                                                        SetValidateNotifyError(x => Validate(x)).
                                                                                        AddTo(_compositeDisposable);
            ResizeMode    = _model.ResizeMode.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(_compositeDisposable);
        }
        private string? Validate(string target)
        {
            var result = string.IsNullOrEmpty(target) || !int.TryParse(target, out int value) || value <= 0;
            return result ? "その倍率は指定できません。" : null;
        }
    }
}
