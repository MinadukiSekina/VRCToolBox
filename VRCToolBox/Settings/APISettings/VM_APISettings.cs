using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Settings.APISettings
{
    public class VM_APISettings : SettingsViewModelBase
    {
        private string _rawPass = string.Empty;
        public AsyncReactiveCommand TwitterAuthAsyncCommand { get; } = new AsyncReactiveCommand();
        public AsyncReactiveCommand TwitterLogoutAsyncCommand { get; } = new AsyncReactiveCommand();
        public VM_APISettings() : this(new M_Settings()) { }
        public VM_APISettings(M_Settings m_Settings) : base(m_Settings)
        {
            TwitterAuthAsyncCommand.Subscribe(_ => TwitterAuthAsync()).AddTo(_compositeDisposable);
            TwitterLogoutAsyncCommand.Subscribe(_ => TwitterLogoutAsync()).AddTo(_compositeDisposable);
        }
        private async Task TwitterAuthAsync()
        {
            try
            {
                await Twitter.Twitter.AuthenticateAsync();
            }
            catch (Exception ex)
            {
                var message = new MessageContent() { Button = MessageButton.OK, DefaultResult = MessageResult.OK, Icon = MessageIcon.Exclamation, Text = ex.Message };
                message.ShowMessage();
            }
            finally
            {
                _rawPass = string.Empty;
            }
        }
        private async Task TwitterLogoutAsync()
        {
            try
            {
                await new Twitter.Twitter().LogoutAsync(_rawPass).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var message = new MessageContent() { Button = MessageButton.OK, DefaultResult = MessageResult.OK, Icon = MessageIcon.Exclamation, Text = ex.Message };
                message.ShowMessage();
            }
            finally
            {
                _rawPass = string.Empty;
            }
        }
    }
}
