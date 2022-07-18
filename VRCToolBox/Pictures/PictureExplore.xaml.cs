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

        /// マウス押下中フラグ
        bool isMouseLeftButtonDown = false;
        /// マウスを押下した点を保存
        Point MouseDonwStartPoint = new Point(0, 0);
        /// マウスの現在地
        Point MouseCurrentPoint = new Point(0, 0);

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

        private void PhotoViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // クリックした位置を保存
            MouseDonwStartPoint = e.GetPosition(PhotoViewer);

            isMouseLeftButtonDown = true;
        }

        private void PhotoViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseLeftButtonDown = false;
        }

        private void PhotoViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseLeftButtonDown == false) return;

            // マウスの現在位置座標を取得（ScrollViewerからの相対位置）
            // ここは、位置の基準にするControl(GetPositionの引数)はScrollViewrでもthis(Window自体)でもなんでもいい。
            // Start時とマウス移動時の差分がわかりさえすればよし。
            MouseCurrentPoint = e.GetPosition(PhotoViewer);

            // 移動開始点と現在位置の差から、MouseMoveイベント1回分の移動量を算出
            double offsetX = MouseCurrentPoint.X - MouseDonwStartPoint.X;
            double offsetY = MouseCurrentPoint.Y - MouseDonwStartPoint.Y;

            // 動かす対象の図形からMatrixオブジェクトを取得
            // このMatrixオブジェクトを用いて図形を描画上移動させる
            Matrix matrix = ((MatrixTransform)Picture_Image.RenderTransform).Matrix;

            // TranslateメソッドにX方向とY方向の移動量を渡し、移動後の状態を計算
            matrix.Translate(offsetX, offsetY);

            // 移動後の状態を計算したMatrixオブジェクトを描画に反映する
            Picture_Image.RenderTransform = new MatrixTransform(matrix);

            // 移動開始点を現在位置で更新する
            // （今回の現在位置が次回のMouseMoveイベントハンドラで使われる移動開始点となる）
            MouseDonwStartPoint = MouseCurrentPoint;
        }

        private void PhotoViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            isMouseLeftButtonDown = false;
        }

        private void PhotoViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scale = 1.0;
            Matrix matrix = ((MatrixTransform)Picture_Image.RenderTransform).Matrix;

            // ScaleAt()の拡大中心点(引数3,4個目)に渡すための座標をとるときの基準Controlは、拡大縮小をしたいものの一つ上のControlにすること。
            // ここでは拡大縮小するGridを包んでいるScrollViewerを基準にした。
            MouseCurrentPoint = e.GetPosition(this);

            // ホイール上に回す→拡大 / 下に回す→縮小
            if (e.Delta > 0) scale = 1.25;
            else scale = 1 / 1.25;

            // 拡大実施
            matrix.ScaleAt(scale, scale, MouseCurrentPoint.X, MouseCurrentPoint.Y);
            Picture_Image.RenderTransform = new MatrixTransform(matrix);
        }
    }
}
