using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace VRCToolBox.Main
{
    internal class TypeToViewConverter : IValueConverter
    {
        private static IReadOnlyDictionary<Type, Type> _views = new Dictionary<Type, Type>()
        {
            { typeof(VM_Home)                           , typeof(UC_Home)                   },
            { typeof(Pictures.ViewModel.PhotoExploreViewModel)  , typeof(Pictures.PictureExplore)   },
            { typeof(VRCLog.LogViewerViewModel)         , typeof(VRCLog.LogViewer)          },
            { typeof(Settings.SettingsWindowViewModel)  , typeof(Settings.SettingsWindow)   },
            { typeof(UnityEntry.UnityListViewModel)     , typeof(UnityEntry.UnityList)      }
        };
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            try
            {
                return Activator.CreateInstance(_views[(Type)value]) ?? new object();
            }
            catch (Exception ex)
            {
                // TODO Do something.
            }
            return new Object();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}
