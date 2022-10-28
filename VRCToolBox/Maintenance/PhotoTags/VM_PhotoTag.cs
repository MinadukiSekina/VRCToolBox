using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Settings.DataSettings.PhotoTags
{
    public class VM_PhotoTag : ViewModelBase
    {
        private M_PhotoTag _photoTag;

        public ReactiveProperty<string> TagName { get; } = new ReactiveProperty<string>();

        public AsyncReactiveCommand SaveAsyncCommand { get; } = new AsyncReactiveCommand();

        public VM_PhotoTag() : this(new M_PhotoTag()) { }
        public VM_PhotoTag(M_PhotoTag m_PhotoTag)
        {
            _photoTag = m_PhotoTag;
            _photoTag.AddTo(_compositeDisposable);
            TagName = _photoTag.TagName.ToReactivePropertyAsSynchronized(t => t.Value).AddTo(_compositeDisposable);
            SaveAsyncCommand.Subscribe(_ => SaveAsync()).AddTo(_compositeDisposable);
        }

        private async Task SaveAsync()
        {
            try
            {
                await _photoTag.SaveTagAsync();
                var message = new MessageContent()
                {
                    Button = MessageButton.OK,
                    DefaultResult = MessageResult.OK,
                    Icon = MessageIcon.Information,
                    Text = $@"タグを保存しました。"
                };
                message.ShowMessage();
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Button = MessageButton.OK,
                    DefaultResult = MessageResult.OK,
                    Icon = MessageIcon.Exclamation,
                    Text = $@"申し訳ありません。エラーが発生しました。{Environment.NewLine}{ex.Message}"
                };
                message.ShowMessage();
            }
        }
    }
}
