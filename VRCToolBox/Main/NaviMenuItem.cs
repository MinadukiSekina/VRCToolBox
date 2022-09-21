using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Common;
using ModernWpf.Controls;

namespace VRCToolBox.Main
{
    public enum PageId
    {
        Home,
        Pictures,
        VRCLog,
        Unity,
        Settings
    }
    public class NaviMenuItem
    {
        private static IReadOnlyDictionary<PageId, Type> _pageTypes = new Dictionary<PageId, Type>() { { PageId.Pictures, typeof(Pictures.PicturesPage) } };
        public Symbol IconSymbol { get; set; }
        private SymbolIcon? symbolIcon;
        public IconElement Icon => symbolIcon ??= new SymbolIcon(IconSymbol);
        public string ItemName { get; set; } = string.Empty;
        public PageId Id { get; set; }
        public Type PageType => _pageTypes[Id];

        public NaviMenuItem(Symbol symbol, string itemName, PageId pageId)
        {
            IconSymbol = symbol;
            ItemName   = itemName;
            Id         = pageId;
        }
    }
}
