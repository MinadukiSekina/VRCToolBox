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
                                                                     Where(f => !string.IsNullOrWhiteSpace(f.DirectoryName) && 
                                                                                !f.DirectoryName.Contains("thumbnails", StringComparison.OrdinalIgnoreCase) && 
                                                                                !f.DirectoryName.Contains(ProgramSettings.Settings.PicturesMovedFolder   , StringComparison.OrdinalIgnoreCase) && 
                                                                                !f.DirectoryName.Contains(ProgramSettings.Settings.PicturesSelectedFolder, StringComparison.OrdinalIgnoreCase) && 
                                                                                !f.DirectoryName.Contains(ProgramSettings.Settings.PicturesUpLoadedFolder, StringComparison.OrdinalIgnoreCase)).
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
                // get original creation date.
                DateTime creationDate = picture.CreationTime;
                File.Move(picture.FullName, destPath);
                // set creation date from original.
                new FileInfo(destPath).CreationTime = creationDate;
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

            DateTime creationDate = pictureInfo.CreationTime;
            // get original creation date.
            File.Copy(picturePath, destPath);
            // set creation date from original.
            new FileInfo(destPath).CreationTime = creationDate;
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
        internal static string GetBase64Image(string path)
        {
            try
            {
                using(FileStream fs = File.OpenRead(path))
                {
                    byte[] bytes = new byte[fs.Length];
                    _ = fs.Read(bytes);
                    fs.Close();
                    return Convert.ToBase64String(bytes);
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

    }
}
