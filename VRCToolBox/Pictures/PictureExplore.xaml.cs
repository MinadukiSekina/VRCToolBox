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
using VRCToolBox.Common;
using VRCToolBox.Data;
using Microsoft.EntityFrameworkCore;

namespace VRCToolBox.Pictures
{
    /// <summary>
    /// PictureExplore.xaml の相互作用ロジック
    /// </summary>
    public partial class PictureExplore : Window
    {
        public ObservableCollectionEX<Picture> Pictures { get; set; } = new ObservableCollectionEX<Picture>();
        public ObservableCollectionEX<DirectoryTreeItem> Directorys { get; set; } = new ObservableCollectionEX<DirectoryTreeItem>();
        public ObservableCollectionEX<PhotoTag> PictureTags { get; set; } = new ObservableCollectionEX<PhotoTag>();
        public PhotoData? PictureData { get; set; }
        public Tweet? Tweet { get; set; } 

        /// マウス押下中フラグ
        bool _isMouseMiddleButtonDown = false;
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
                if(_rotate > 360) _rotate -= 360;
            }
        }

        public PictureExplore()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void SelectionChanged(object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            try
            {
                // Scroll to top.
                ScrollViewer? scrollViewer = (ScrollViewer?)GetScrollViewer(Picture_View);
                if (scrollViewer is not null) scrollViewer.ScrollToTop();

                TreeViewItem selectedItem = (TreeViewItem)Directory_Tree.SelectedItem;
                string path = ((DirectoryTreeItem)selectedItem).DirectoryInfo.FullName;
                EnumeratePictures(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void EnumerateDirectories()
        {
            IEnumerable<string> drives = Directory.GetLogicalDrives();
            List<DirectoryTreeItem> items = new List<DirectoryTreeItem>();
            foreach (string drive in drives)
            {
                if (!Directory.Exists(drive)) continue;
                DirectoryTreeItem directoryTreeItem = new DirectoryTreeItem(new DirectoryInfo(drive));
                items.Add(directoryTreeItem);
            }
            Directorys.AddRange(items);
        }
        private void EnumeratePictures(string directoryPath)
        {
            Pictures.Clear();
            IEnumerable<string> pictureFiles = Directory.EnumerateFiles(directoryPath, "*", SearchOption.TopDirectoryOnly).
                                                     Where(x => ProgramConst.PictureLowerExtensions.Contains(System.IO.Path.GetExtension(x).ToLower()));
            List<Picture> pictureList = new List<Picture>();
            foreach (string pictureFile in pictureFiles)
            {
                Picture picture = new Picture 
                {
                    FileName = System.IO.Path.GetFileName(pictureFile),
                    Path = pictureFile,
                };
                pictureList.Add(picture);
            }
            Pictures.AddRange(pictureList);
        }
        // reference:https://code-examples.net/ja/q/107095
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

        private void Picture_View_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ShowPicture();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void PhotoViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void PhotoViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }
        // reference:https://qiita.com/tera1707/items/37af056540f23e73213f
        private void PhotoViewer_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (!_isMouseMiddleButtonDown)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        if (!File.Exists(Picture_Image.Tag as string)) return;
                        // Drag & Drop.
                        string[] fileNames = { (string)Picture_Image.Tag };
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PhotoViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseMiddleButtonDown = false;
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
                if (Picture_Image.Tag is string filePath)
                {
                    if (!File.Exists(filePath)) return;
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        TransformedBitmap transformedBitmap = new TransformedBitmap();
                        transformedBitmap.BeginInit();
                        transformedBitmap.Source = (BitmapImage)Picture_Image.Source;
                        transformedBitmap.Transform = new RotateTransform(Rotate);
                        transformedBitmap.EndInit();
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(transformedBitmap));
                        encoder.Save(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PhotoViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseMiddleButtonDown = true;
            MouseDonwStartPoint = e.GetPosition(PhotoViewer);
        }

        private void PhotoViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseMiddleButtonDown = false;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            try
            {
                EnumerateDirectories();
                EnumeratePictures(ProgramSettings.Settings.PicturesSavedFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void Open_Twitter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessEx.Start("https://twitter.com/home", true);
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Open_VRChat_Home_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessEx.Start("https://hello.vrchat.com", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Save_Picture_Content_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using(PhotoContext context = new PhotoContext())
                using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (PictureData is null || Tweet is null)
                        {
                            PhotoData photoData = new PhotoData();
                            Tweet tweet = new Tweet();
                            tweet.TweetId = Ulid.NewUlid();
                            tweet.Content = Tweet_Content.Text;
                            context.Tweets.Add(tweet);
                            Tweet = tweet;

                            photoData.FullName = (string)Picture_Image.Tag;
                            photoData.TweetId = tweet.TweetId;
                            photoData.Tags = PictureTags;
                            context.Photos.Add(photoData);
                            PictureData = photoData;
                        }
                        else
                        {
                            Tweet.Content = Tweet_Content.Text;
                            PictureData.Tags = PictureTags;
                            context.Update<Tweet>(Tweet);
                            context.Update<PhotoData>(PictureData);
                        }

                        if (!Directory.Exists(ProgramSettings.Settings.PicturesSelectedFolder)) Directory.CreateDirectory(ProgramSettings.Settings.PicturesSelectedFolder);
                        string destPath = $@"{ProgramSettings.Settings.PicturesSelectedFolder}\{PictureData.PhotoName}";
                        if (!File.Exists(destPath))
                        {
                            File.Copy(PictureData.FullName, destPath);
                        }
                        context.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Move_To_Upload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string? picturePath = Picture_Image.Tag as string;
                if (!File.Exists(picturePath)) return;
                if(System.IO.Path.GetFileName(picturePath) is string fileName)
                {
                    if(!Directory.Exists(ProgramSettings.Settings.PicturesUpLoadedFolder)) Directory.CreateDirectory(ProgramSettings.Settings.PicturesUpLoadedFolder);
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
                PhotoTag? photoTag;
                using (PhotoContext context = new PhotoContext())
                {
                    photoTag = context.PhotoTags.Include(x => x.Photos).SingleOrDefault(x => x.TagName == TX_PhotoTag.Text);
                }
                if(photoTag is null)
                {
                    photoTag = new PhotoTag();
                    photoTag.TagId = Ulid.NewUlid();
                    photoTag.TagName = TX_PhotoTag.Text;
                }
                if(photoTag.Photos is null)
                {
                    photoTag.Photos = new List<PhotoData>();
                }
                if(PictureData is null) MakePhotoData();
                photoTag.Photos.Add(PictureData);
                //using (PhotoContext context = new PhotoContext())
                //{
                //    context.Add(photoTag);
                //    context.SaveChanges();
                //}
                PictureTags.Add(photoTag);
                TX_PhotoTag.Text = String.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowPicture()
        {
            Picture? picture = Picture_View.SelectedItem as Picture;
            string? path = picture?.Path;
            if (picture is null || string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;

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
            Picture_Image.Tag = path;

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

            PictureTags.Clear();
            Tweet_Content.Text = String.Empty;
            using (PhotoContext photoContext = new PhotoContext())
            {
                PictureData = photoContext.Photos.Include(x => x.Tags).Include(x => x.Tweet).SingleOrDefault(x => x.PhotoName == picture.FileName);
            }
            if(PictureData is null) MakePhotoData();
            Tweet = PictureData?.Tweet;
            Tweet_Content.Text = Tweet?.Content;
            PictureTags.AddRange(PictureData?.Tags is null ? new ObservableCollectionEX<PhotoTag>() : PictureData.Tags);
        }
        private void MakePhotoData()
        {
            PhotoData photoData = new PhotoData();
            Tweet tweet = new Tweet();
            tweet.TweetId = Ulid.NewUlid();
            tweet.Content = Tweet_Content.Text;
            Tweet = tweet;

            photoData.FullName = (string)Picture_Image.Tag;
            photoData.TweetId = tweet.TweetId;
            photoData.Tags = PictureTags;
            PictureData = photoData;
        }
    }
}
