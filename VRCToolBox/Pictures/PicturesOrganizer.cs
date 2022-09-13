using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using VRCToolBox.Settings;

namespace VRCToolBox.Pictures
{
    internal class PicturesOrganizer
    {
        internal static readonly string[] PictureLowerExtensions = { ".png", ".jpeg", ".jpg" };

        // 写真の移動処理（一括）
        internal static void OrganizePictures()
        {
            if (!string.IsNullOrWhiteSpace(ProgramSettings.Settings.PicturesSavedFolder) && Directory.Exists(ProgramSettings.Settings.PicturesSavedFolder)) 
                MovePictures(ProgramSettings.Settings.PicturesSavedFolder);

            if (!string.IsNullOrWhiteSpace(ProgramSettings.Settings.OtherPicturesSaveFolder) && Directory.Exists(ProgramSettings.Settings.OtherPicturesSaveFolder)) 
                MovePictures(ProgramSettings.Settings.OtherPicturesSaveFolder);
            System.Windows.MessageBox.Show("写真の移動と整理を終了しました。");
        }
        private static void MovePictures(string path)
        {
            string NewFolderPath = "";
            string pictureName = "";
            string monthString = "";
            string dateString = "";
            string destPath = "";

            IEnumerable<FileInfo> pictures = new DirectoryInfo(path).EnumerateFiles("*", SearchOption.AllDirectories).
                                                                     Where(f => PictureLowerExtensions.Contains(f.Extension.ToLower()));

            foreach (FileInfo picture in pictures)
            {
                dateString  = picture.CreationTime.ToString("yyyyMMdd");
                monthString = picture.CreationTime.ToString("yyyyMM");
                pictureName = picture.Name;
                NewFolderPath = @$"{ProgramSettings.Settings.PicturesMovedFolder}\{monthString}{(ProgramSettings.Settings.MakeDayFolder ? @$"\{dateString}" : string.Empty)}";
                destPath = @$"{NewFolderPath}\{pictureName}";

                //写真の日付のフォルダがあるか
                Directory.CreateDirectory(NewFolderPath);

                // 写真の移動。エラー回避？
                if (File.Exists(destPath)) return;
                File.Move(picture.FullName, destPath);
                new FileInfo(destPath).CreationTime = picture.CreationTime;
            }
        }

        // 選択した写真のコピー
        internal static void MoveSelectedPicture(string picturePath)
        {
            if (!File.Exists(picturePath)) return;
            FileInfo pictureInfo = new FileInfo(picturePath);
            string pictureName = pictureInfo.Name;
            string destPath = $@"{ProgramSettings.Settings.PicturesSelectedFolder}\{pictureName}";

            // 既にあるなら移動させない
            if (File.Exists(destPath)) return;

            // なければフォルダを作る
            if (!Directory.Exists(ProgramSettings.Settings.PicturesSelectedFolder))
                Directory.CreateDirectory(ProgramSettings.Settings.PicturesSelectedFolder);

            File.Copy(picturePath, destPath);
            new FileInfo(destPath).CreationTime = pictureInfo.CreationTime;
        }

        //// 投稿した写真の移動
        //internal static void MoveUpLoadedPicture(string picturePath)
        //{
        //    // 万が一写真が無ければ返す
        //    if (!File.Exists(picturePath)) return;

        //    string pictureName = Path.GetFileName(picturePath);
        //    string destPath = $"{ProgramSettings.Settings.DesignatedPicturesUpLoadedFolder}\\{pictureName}";

        //    // 既にあるなら移動させない
        //    if (File.Exists(destPath)) return;

        //    // なければフォルダを作る
        //    if (!Directory.Exists(ProgramSettings.Settings.DesignatedPicturesUpLoadedFolder))
        //        Directory.CreateDirectory(ProgramSettings.Settings.DesignatedPicturesUpLoadedFolder);

        //    File.Move(picturePath, destPath);

        //}

    }
}
