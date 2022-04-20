using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using VRCToolBox.Directories;
using VRCToolBox.Settings;

namespace VRCToolBox.Pictures
{
    /// <summary>
    /// PictureExplore.xaml の相互作用ロジック
    /// </summary>
    public partial class PictureExplore : Window
    {
        public ObservableCollection<Picture> Pictures { get; set; } = new ObservableCollection<Picture>();
        public ObservableCollection<DirectoryTreeItem> Directorys { get; set; } = new ObservableCollection<DirectoryTreeItem>();

        public PictureExplore()
        {
            InitializeComponent();
            //Picture_View.ItemsSource = Pictures;
            //Directory_Tree.ItemsSource = Directorys;
            DataContext = this;
            IEnumerable<string> drives = Directory.GetLogicalDrives();
            foreach (string drive in drives)
            {
                if (!Directory.Exists(drive)) continue;
                DirectoryTreeItem directoryTreeItem = new DirectoryTreeItem(new DirectoryInfo(drive));
                Directorys.Add(directoryTreeItem);
            }
            EnumeratePictures(ProgramSettings.Settings.PicturesSavedFolder);
        }

        private void SelectionChanged(object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            TreeViewItem selectedItem = (TreeViewItem)Directory_Tree.SelectedItem;
            string path = ((DirectoryTreeItem)selectedItem).DirectoryInfo.FullName;
            EnumeratePictures(path);
        }
        private void EnumeratePictures(string directoryPath)
        {
            Pictures.Clear();
            IEnumerable<string> pictureFiles = Directory.EnumerateFiles(directoryPath, "*", SearchOption.TopDirectoryOnly).
                                                     Where(x => ProgramConst.PictureLowerExtensions.Contains(System.IO.Path.GetExtension(x).ToLower()));
            foreach (string pictureFile in pictureFiles)
            {
                Picture picture = new Picture 
                {
                    FileName = System.IO.Path.GetFileName(pictureFile),
                    Path = pictureFile,
                    Image = GetBitMapImageForThumbnail(pictureFile, 192)
                };
                Pictures.Add(picture);
            }
        }
        private BitmapImage GetBitMapImageForThumbnail(string imagePath, int decodePixelWidth)
        {
            BitmapImage bitmapImage = new BitmapImage();

            if(string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath)) return bitmapImage;

            using(FileStream fileStream = File.OpenRead(imagePath))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = fileStream;
                bitmapImage.DecodePixelWidth = decodePixelWidth;
                bitmapImage.EndInit();
                fileStream.Close();
            }
            return bitmapImage;
        }

        private void Picture_View_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            Picture picture = (Picture)Picture_View.SelectedItem;
            string path = picture.Path;
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;

            BitmapImage bitmapImage = new BitmapImage();

            using (FileStream fileStream = File.OpenRead(path))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = fileStream;
                bitmapImage.EndInit();
                fileStream.Close();
            }
            Picture_Image.Source = bitmapImage;
            //RenderOptions.SetEdgeMode(Picture_Image, EdgeMode.Aliased);
            RenderOptions.SetBitmapScalingMode(Picture_Image, BitmapScalingMode.Fant);
        }
    }
}
