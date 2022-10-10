using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Common;

namespace VRCToolBox.Twitter
{
    internal class VM_Password : ViewModelBase
    {
        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChanged();
            }
        }
    }
}
