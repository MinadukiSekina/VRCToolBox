using System;
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

namespace VRCToolBox.Pictures
{
    /// <summary>
    /// ImageConverterView.xaml の相互作用ロジック
    /// </summary>
    public partial class ImageConverterView : Window
    {

        /// マウス押下中フラグ
        bool _isMouseMiddleButtonDown = false;
        bool _isMouseLeftButtonDown = false;
        /// マウスを押下した点を保存
        Point MouseDonwStartPoint = new Point(0, 0);
        /// マウスの現在地
        Point MouseCurrentPoint = new Point(0, 0);

        public ImageConverterView()
        {
            InitializeComponent();
            Loaded += ImageConverterView_Loaded;
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

        private void ImageConverterView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Pictures.ViewModel.ICloseWindow vm) 
            {
                // await 後にスレッドが変わってしまうとエラーになるようなので、念のため Dispatcher 経由にする
                vm.Close += () => Dispatcher.Invoke(new Action(()=> Close()));
            }
            if (DataContext is ViewModel.IResetImageView vm2) 
            {
                // await 後にスレッドが変わってしまうとエラーになるようなので、念のため Dispatcher 経由にする
                vm2.ResetImageView += () => Dispatcher.Invoke(new Action(() => ResetImageControl()));
            }
            //RenderOptions.SetBitmapScalingMode(Picture_Image, BitmapScalingMode.Fant);
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

        private void PhotoViewer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ResetImageControl();
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Text = $"エラーが発生しました。{Environment.NewLine}{ex.Message}",
                    Icon = MessageIcon.Error,
                    Button = MessageButton.OK
                };
                message.ShowMessage();
            }
        }

        // reference:https://qiita.com/tera1707/items/37af056540f23e73213f
        private void PhotoViewer_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (!_isMouseMiddleButtonDown)
                {
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
                var message = new MessageContent()
                {
                    Text = $"エラーが発生しました。{Environment.NewLine}{ex.Message}",
                    Icon = MessageIcon.Error,
                    Button = MessageButton.OK
                };
                message.ShowMessage();
            }
        }

        private void PhotoViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseMiddleButtonDown = false;
            _isMouseLeftButtonDown = false;
        }

        private void PhotoViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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
                var message = new MessageContent()
                {
                    Text = $"エラーが発生しました。{Environment.NewLine}{ex.Message}",
                    Icon = MessageIcon.Error,
                    Button = MessageButton.OK
                };
                message.ShowMessage();
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

            //RenderOptions.SetBitmapScalingMode(Picture_Image, BitmapScalingMode.Fant);
        }
    }
}
