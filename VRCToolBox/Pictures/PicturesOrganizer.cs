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
            string NewFolderPath = "";
            string pictureName = "";
            string monthString = "";
            string dateString = "";
            string destPath = "";

            IEnumerable<string> pictures = Directory.EnumerateFiles(ProgramSettings.Settings.PicturesSavedFolder, "*", SearchOption.AllDirectories).
                                                     Where(x => PictureLowerExtensions.Contains(Path.GetExtension(x).ToLower()));

            foreach (string picture in pictures)
            {
                dateString  = File.GetCreationTime(picture).ToString("yyyyMMdd");
                monthString = File.GetCreationTime(picture).ToString("yyyyMM");
                pictureName = Path.GetFileName(picture);
                NewFolderPath = @$"{ProgramSettings.Settings.PicturesMovedFolder}\{monthString}{(ProgramSettings.Settings.MakeDayFolder ? @$"\{dateString}" : string.Empty)}";
                destPath = @$"{NewFolderPath}\{pictureName}";

                //写真の日付のフォルダがあるか
                Directory.CreateDirectory(NewFolderPath);

                // 写真の移動。エラー回避？
                if (!File.Exists(destPath)) File.Move(picture, destPath);
            }
        }

        // 選択した写真のコピー
        internal static void MoveSelectedPicture(string picturePath)
        {
            string pictureName = Path.GetFileName(picturePath);
            string destPath = $@"{ProgramSettings.Settings.PicturesSelectedFolder}\{pictureName}";

            // 既にあるなら移動させない
            if (File.Exists(destPath)) return;

            // なければフォルダを作る
            if (!Directory.Exists(ProgramSettings.Settings.PicturesSelectedFolder))
                Directory.CreateDirectory(ProgramSettings.Settings.PicturesSelectedFolder);

            File.Copy(picturePath, destPath);
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
