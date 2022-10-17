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
using VRCToolBox.SystemIO;
using VRCToolBox.Settings;
using VRCToolBox.Common;
using VRCToolBox.Data;
using Microsoft.EntityFrameworkCore;

namespace VRCToolBox.Pictures
{
    /// <summary>
    /// PictureExplore.xaml の相互作用ロジック
    /// </summary>
    public partial class PictureExplore : UserControl
    {
        public ObservableCollectionEX<PhotoTag> PictureTags { get; set; } = new ObservableCollectionEX<PhotoTag>();
        public PhotoData? PictureData { get; set; }
        public Tweet? Tweet { get; set; }

        /// マウス押下中フラグ
        bool _isMouseMiddleButtonDown = false;
        bool _isMouseLeftButtonDown = false;
        /// マウスを押下した点を保存
        Point MouseDonwStartPoint = new Point(0, 0);
        /// マウスの現在地
        Point MouseCurrentPoint = new Point(0, 0);
        // 回転量の保持。
        int _rotate = 0;
        int Rotate {
            get { return _rotate; }
            set {
                _rotate = value;
                if (_rotate > 360) _rotate -= 360;
            }
        }

        private PictureExploreViewModel _pictureExploreViewModel;
        public PictureExplore()
        {
            InitializeComponent();
            _pictureExploreViewModel = (PictureExploreViewModel)DataContext;
            //DataContext = this;
        }


        // reference:https://code-examples.net/ja/q/107095
        private DependencyObject? GetScrollViewer(DependencyObject dependencyObject)
        {
            if (dependencyObject is ScrollViewer) return dependencyObject;
            int childrenCount = VisualTreeHelper.GetChildrenCount(dependencyObject);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, i);
                DependencyObject? result = GetScrollViewer(child);
                if (result is null) continue;
                return result;
            }
            return null;
        }

        private void PhotoViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseMiddleButtonDown = false;
            _isMouseLeftButtonDown = true;
        }

        private void PhotoViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseLeftButtonDown = false;
        }
        // reference:https://qiita.com/tera1707/items/37af056540f23e73213f
        private void PhotoViewer_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (!_isMouseMiddleButtonDown)
                {
                    if (!File.Exists(_pictureExploreViewModel.PictureData.FullName)) return;
                    if (_isMouseLeftButtonDown)
                    {
                        // Drag & Drop.
                        IEnumerable<string> files = _pictureExploreViewModel.OtherPictures.Select(o => o.FullName);
                        string[] fileNames = files.Any() ? files.ToArray() : new string[] { _pictureExploreViewModel.PictureData.FullName };
                        DataObject dataObject = new DataObject(DataFormats.FileDrop, fileNames);
                        dataObject.SetData(DataFormats.Bitmap, Picture_Image.Source);
                        DragDrop.DoDragDrop(this, dataObject, DragDropEffects.All);
                    }
                    return;
                }

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PhotoViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseMiddleButtonDown = false;
            _isMouseLeftButtonDown = false;
        }

        private void ImageBase_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                MouseCurrentPoint = e.GetPosition(PhotoViewer);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void B_RotateLeft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Matrix matrix = ((MatrixTransform)Picture_Image.LayoutTransform).Matrix;
                matrix.Rotate(270);
                Picture_Image.LayoutTransform = new MatrixTransform(matrix);
                Rotate += 270;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void B_RotateRight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Matrix matrix = ((MatrixTransform)Picture_Image.LayoutTransform).Matrix;
                matrix.Rotate(90);
                Picture_Image.LayoutTransform = new MatrixTransform(matrix);
                Rotate += 90;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void B_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Picture_Image.Source is null) return;
                _pictureExploreViewModel.SavePhotoRotation(Rotate, (BitmapImage)Picture_Image.Source);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PhotoViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _isMouseMiddleButtonDown = true;
                MouseDonwStartPoint = e.GetPosition(PhotoViewer);
            }
            else
            {
                _isMouseLeftButtonDown = true;
            }
        }

        private void PhotoViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseMiddleButtonDown = false;
            _isMouseLeftButtonDown = false;
        }

        private void Move_To_Upload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string? picturePath = Picture_Image.Tag as string;
                if (!File.Exists(picturePath)) return;
                if (System.IO.Path.GetFileName(picturePath) is string fileName)
                {
                    if (!Directory.Exists(ProgramSettings.Settings.PicturesUpLoadedFolder)) Directory.CreateDirectory(ProgramSettings.Settings.PicturesUpLoadedFolder);
                    string destination = $@"{ProgramSettings.Settings.PicturesUpLoadedFolder}\{fileName}";
                    File.Move(picturePath, destination);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TX_PhotoTag_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key != Key.Enter) return;
                //_pictureExploreViewModel.AddNewTag(TX_PhotoTag.Text);
                //TX_PhotoTag.Text = String.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowPicture(string path)
        {
            if (!File.Exists(path)) return;

            _pictureExploreViewModel.GetPicture(path, true);

            ResetImageControl();
        }

        private void ResetImageControl()
        {
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

            Rotate = 0;

            RenderOptions.SetBitmapScalingMode(Picture_Image, BitmapScalingMode.Fant);
        }
        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Hold_View.SelectedItem is Picture picture) ShowPicture(picture.FullName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void WorldVisitList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (WorldVisitList.SelectedItem is WorldVisit worldVisit)
                {
                    _pictureExploreViewModel.UserList.Clear();
                    using (UserActivityContext userActivityContext = new UserActivityContext())
                    {
                        _pictureExploreViewModel.UserList.AddRange(userActivityContext.UserActivities.AsNoTracking().Where(u => u.WorldVisitId == worldVisit.WorldVisitId).GroupBy(u => u.UserName).OrderBy(u => u.Key).Select(u => u.Key));
                    }
                    using (PhotoContext photoContext = new PhotoContext())
                    {
                        _pictureExploreViewModel.WorldData = photoContext.Worlds.Include(w => w.Author).AsNoTracking().Where(w => w.WorldName == worldVisit.WorldName).SingleOrDefault() ?? new WorldData() { WorldName = worldVisit.WorldName };
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Directory_Tree_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is TreeViewItem item && item.Items.CurrentItem is DirectoryEntry directoryEntry)
                {
                    directoryEntry.AddSubDirectory();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CommandReference_CommandExecuted(object sender, CommandExecutedEventArgs e)
        {
            if (e.Error is null)
            {
                ScrollViewer? scrollViewer = (ScrollViewer?)GetScrollViewer(Picture_View);
                if (scrollViewer is not null) scrollViewer.ScrollToTop();
            }
            else
            {
                MessageBox.Show($"申し訳ありません。エラーが発生しました。{Environment.NewLine}{e.Error.Message}");
                e.ErrorHandled = true;
            }
        }

        private void CommandReference_CommandExecuting(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void CommandReference_CommandExecuted_1(object sender, CommandExecutedEventArgs e)
        {
            if (e.Error is null)
            {
                // no problem.
                CommandManager.InvalidateRequerySuggested();
            }
            else
            {
                MessageBox.Show($"申し訳ありません。エラーが発生しました。{Environment.NewLine}{e.Error.Message}");
                e.ErrorHandled = true;
            }
        }

        private void CommandReference_CommandExecuting_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SearchConditionWindow subWindow = new SearchConditionWindow() { DataContext = ((PictureExploreViewModel)DataContext).SubViewModel, Owner = Window.GetWindow(this) };
            bool? result = subWindow.ShowDialog();
            e.Cancel = !result.HasValue || !result.Value;
        }
        private void BeforeShowPicture(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Picture_View.SelectedItem is FileSystemInfoEx fileInfo && fileInfo.IsDirectory)
            {
                ScrollViewer? scrollViewer = (ScrollViewer?)GetScrollViewer(Picture_View);
                if (scrollViewer is not null) scrollViewer.ScrollToTop();
            }
        }

        private void AfterShowPicture(object sender, CommandExecutedEventArgs e)
        {
            if (e.Error is null)
            {
                ResetImageControl();
                CommandManager.InvalidateRequerySuggested();
            }
            else
            {
                MessageBox.Show($"申し訳ありません。エラーが発生しました。{Environment.NewLine}{e.Error.Message}");
                e.ErrorHandled = true;
            }
        }
    }
}
