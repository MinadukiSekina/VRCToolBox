using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Maintenance.Avatars;
using VRCToolBox.Maintenance.Shared;

namespace VRCToolBox.Maintenance
{
    public class VM_MaintenanceBase : ViewModelBase
    {
        public ReactivePropertySlim<VM_DataMaintenanceBase> Content { get; } = new ReactivePropertySlim<VM_DataMaintenanceBase>();
        public ReactiveCommand<NavigationViewItem> ChangeContentCommand { get; } = new ReactiveCommand<NavigationViewItem>();
        public VM_MaintenanceBase()
        {
            ChangeContentCommand.Subscribe(n => ChangeContent(n)).AddTo(_compositeDisposable);
            Content.Value = new VM_Avatars();
            Content.AddTo(_compositeDisposable);
        }
        private void ChangeContent(NavigationViewItemBase item)
        {
            try
            {
                if (item is null) return;
                var vm = Activator.CreateInstance((Type)item.Tag);
                if (vm is null) return;
                Content.Value.Dispose();
                Content.Value = (VM_DataMaintenanceBase)vm;
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
        }
    }
}
