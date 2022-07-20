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
            // Scroll to top.
            ScrollViewer? scrollViewer = (ScrollViewer?)GetScrollViewer(Picture_View);
            if(scrollViewer is not null) scrollViewer.ScrollToTop();

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
                    //Image = GetBitMapImageForThumbnail(pictureFile, 192)
                };
                Pictures.Add(picture);
            }
        }
        private DependencyObject? GetScrollViewer(DependencyObject dependencyObject)
        {
            if (dependencyObject is ScrollViewer) return dependencyObject;
            int childrenCount = VisualTreeHelper.GetChildrenCount(dependencyObject);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child  = VisualTreeHelper.GetChild(dependencyObject, i);
                DependencyObject? result = GetScrollViewer(child);
                if(result is null) continue;
                return result;
            }
            return null;
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

            // 画像表示部の初期化
            Matrix matrix = matrixTransform.Matrix;
            matrix.M11 = 1.0;
            matrix.M12 = 0.0;
            matrix.M21 = 0.0;
            matrix.M22 = 1.0;
            matrix.OffsetX = 0.0;
            matrix.OffsetY = 0.0;
            matrixTransform.Matrix = matrix;

            matrix = ((MatrixTransform)Picture_Image.RenderTransform).Matrix;
            matrix.M11 = 1.0;
            matrix.M12 = 0.0;
            matrix.M21 = 0.0;
            matrix.M22 = 1.0;
            matrix.OffsetX = 0.0;
            matrix.OffsetY = 0.0;
            Picture_Image.RenderTransform = new MatrixTransform(matrix);
            Picture_Image.LayoutTransform = new MatrixTransform(matrix);

            PhotoViewer.ScrollToLeftEnd();
            PhotoViewer.ScrollToTop();

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
            // 拡大縮小をするので、倍率の逆数を掛けて移動量を平準化しておく。
            double offsetX = (MouseCurrentPoint.X - MouseDonwStartPoint.X) * (matrixTransform.Matrix.M11 == 0 ? 1 : 1 / matrixTransform.Matrix.M11);
            double offsetY = (MouseCurrentPoint.Y - MouseDonwStartPoint.Y) * (matrixTransform.Matrix.M11 == 0 ? 1 : 1 / matrixTransform.Matrix.M11);

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

        private void ImageBase_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            MouseCurrentPoint = e.GetPosition(ImageBase);
            //  マウスホイールのイベントを受け取り、スライダーをずらす
            var scale = e.Delta > 0 ? 1.25 : 1 / 1.25;

            Matrix matrix = ((MatrixTransform)Picture_Image.RenderTransform).Matrix;
            matrix.ScaleAt(scale, scale, MouseCurrentPoint.X, MouseCurrentPoint.Y);
            Picture_Image.RenderTransform = new MatrixTransform(matrix);

            // scrollViewerのスクロールバーの位置をマウス位置を中心とする。
            double x_barOffset = (PhotoViewer.HorizontalOffset + MouseCurrentPoint.X) * scale - MouseCurrentPoint.X;
            PhotoViewer.ScrollToHorizontalOffset(x_barOffset);

            double y_barOffset = (PhotoViewer.VerticalOffset + MouseCurrentPoint.Y) * scale - MouseCurrentPoint.Y;
            PhotoViewer.ScrollToVerticalOffset(y_barOffset);
        }

        private void B_RotateLeft_Click(object sender, RoutedEventArgs e)
        {
            Matrix matrix = ((MatrixTransform)Picture_Image.LayoutTransform).Matrix;
            matrix.Rotate(270);
            Picture_Image.LayoutTransform = new MatrixTransform(matrix);
        }

        private void B_RotateRight_Click(object sender, RoutedEventArgs e)
        {
            Matrix matrix = ((MatrixTransform)Picture_Image.LayoutTransform).Matrix;
            matrix.Rotate(90);
            Picture_Image.LayoutTransform = new MatrixTransform(matrix);
        }
    }
}
