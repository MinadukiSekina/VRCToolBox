using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VRCToolBox.Directories
{
    // ソース元：https://gogowaten.hatenablog.com/entry/2019/10/10/122459

    public class DirectoryTreeItem : TreeViewItem
    {
        public readonly System.IO.DirectoryInfo DirectoryInfo;
        private bool IsAdd;//サブフォルダを作成済みかどうか
        private TreeViewItem Dummy = new TreeViewItem();//ダミーアイテム


        public DirectoryTreeItem(System.IO.DirectoryInfo info)
        {
            DirectoryInfo = info;
            Header = info.Name;

            //サブフォルダが1つでもあれば
            //if (info.EnumerateDirectories().Any())
            ////展開できることを示す▷を表示するためにダミーのTreeViewItemを追加する
            //{
                Dummy = new TreeViewItem();
                Items.Add(Dummy);
            //}

            //イベント、ツリー展開時
            //サブフォルダを追加
            this.Expanded += (s, e) =>
            {
                if (IsAdd) return;//追加済みなら何もしない
                AddSubDirectory();
            };
        }

        //サブフォルダツリー追加
        public void AddSubDirectory()
        {
            Items.Remove(Dummy);//ダミーのTreeViewItemを削除

            //すべてのサブフォルダを追加
            System.IO.DirectoryInfo[] directories = DirectoryInfo.GetDirectories();
            for (int i = 0; i < directories.Length; i++)
            {
                //隠しフォルダ、システムフォルダは除外する
                var fileAttributes = directories[i].Attributes;
                if ((fileAttributes & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden ||
                        (fileAttributes & System.IO.FileAttributes.System) == System.IO.FileAttributes.System)
                {
                    continue;
                }
                //追加
                Items.Add(new DirectoryTreeItem(directories[i]));
            }
            IsAdd = true;//サブフォルダ作成済みフラグ
        }


        public override string ToString()
        {
            return DirectoryInfo.FullName;
        }
    }
}
