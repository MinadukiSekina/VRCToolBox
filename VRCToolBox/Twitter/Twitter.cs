using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using VRCToolBox.Common;
using VRCToolBox.Crypt;
using VRCToolBox.Pictures;

namespace VRCToolBox.Twitter
{
    internal class Twitter
    {
        private string _userId = string.Empty;
        private string _rPass  = string.Empty;
        private string _pass   = string.Empty;
        private static readonly string s_FilePath = Path.Combine(Settings.ProgramConst.SettingsDirectoryPath, $@"{nameof(Twitter)}.dat");
        internal static async Task AuthenticateAsync(string password)
        {
            _ = Directory.CreateDirectory(Settings.ProgramConst.SettingsDirectoryPath);

            string userId   = Guid.NewGuid().ToString();
            byte[] encrypte = ProtectedData.Protect(Encoding.UTF8.GetBytes(userId), null, DataProtectionScope.CurrentUser);

            // Save user id.
            _ = Directory.CreateDirectory(Settings.ProgramConst.SettingsDirectoryPath);
            using var fs = File.Create(s_FilePath);
            using var bw = new BinaryWriter(fs);
            bw.Write(encrypte);

            string pass     = password.GenerateHashPBKDF2(Encoding.UTF8.GetBytes(userId));
            var parameters  = new Dictionary<string, string>() { { "userId", userId }, { "pass", pass } };
            var content     = new StringContent(JsonSerializer.Serialize(parameters));
            var response    = await Web.WebHelper.HttpClient.PostAsync("https://VRCToolBoxFront-g7agf3akbkbyf4db.z01.azurefd.net/api/Twitter/Login", content);

            if (!response.IsSuccessStatusCode)  throw new Exception("認証URLを取得できませんでした。");
            
            string authUrl = await response.Content.ReadAsStringAsync();
            ProcessEx.Start(authUrl, true);
        }
        internal async Task LogoutAsync(string password)
        {
            if (!File.Exists(s_FilePath)) return;
            if (string.IsNullOrWhiteSpace(_userId))
            {
                using var fs = File.OpenRead(s_FilePath);
                using var br = new BinaryReader(fs);
                byte[] data  = br.ReadBytes((int)fs.Length);
                _userId = Encoding.UTF8.GetString(ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser));
            }
            File.Delete(s_FilePath);

            // Get challange.
            if (string.IsNullOrWhiteSpace(_pass)) await GetChallange();

            string pass     = password.GenerateHashPBKDF2(Encoding.UTF8.GetBytes(_userId));
            var parameters  = new Dictionary<string, string>() { { "userId", _userId }, { "pass", pass } };
            var content     = new StringContent(JsonSerializer.Serialize(parameters));
            var response    = await Web.WebHelper.HttpClient.PostAsync("https://VRCToolBoxFront-g7agf3akbkbyf4db.z01.azurefd.net/api/Twitter/Logout", content);

            if (response.IsSuccessStatusCode)
            {
                System.Windows.MessageBox.Show($@"ログアウトしました。{Environment.NewLine}念のため、Twitter側でも確認をお願いします。");
            }
            else
            {
                System.Windows.MessageBox.Show($@"ログアウト処理に失敗しました。{Environment.NewLine}Twitter側から連携の解除をお願いします。");
            }

        }
        internal async Task TweetAsync(string tweet, IEnumerable<Data.PhotoData> pictures)
        {
            // Check and load user id.
            if (string.IsNullOrWhiteSpace(_userId))
            {
                if (!File.Exists(s_FilePath)) throw new FileNotFoundException($@"必要なファイルが見つかりません。{Environment.NewLine}連携処理を行ってください。");
                using var fs = File.OpenRead(s_FilePath);
                using var br = new BinaryReader(fs);
                byte[] data = br.ReadBytes((int)fs.Length);
                _userId = Encoding.UTF8.GetString(ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser));
            }
            if (string.IsNullOrWhiteSpace(_rPass))
            {
                PassWordWindow subWindow = new PassWordWindow();
                if (subWindow.ShowDialog() != true) return;
                string pass = ((VM_Password)subWindow.DataContext).Password;
                if (string.IsNullOrWhiteSpace(pass))
                {
                    System.Windows.MessageBox.Show("パスワードは入力してください。");
                    return;
                }
                _rPass = pass.GenerateHashPBKDF2(Encoding.UTF8.GetBytes( _userId));
            }

            // Get challange.
            if (string.IsNullOrWhiteSpace(_pass)) await GetChallange();

            var content = new MultipartFormDataContent();
            foreach (var photo in pictures)
            {
                using FileStream fileStream = File.OpenRead(photo.FullName);
                byte[] data = new byte[fileStream.Length];
                await fileStream.ReadAsync(data).ConfigureAwait(false);
                var fileContent = new ByteArrayContent(data);
                content.Add(fileContent, "image", photo.PhotoName);
            }
            content.Add(new StringContent(_userId), "userId");
            content.Add(new StringContent(_pass)  , "pass"  );
            content.Add(new StringContent(tweet)  , "tweet" );
            var responseMessage = await Web.WebHelper.HttpClient.PostAsync("https://VRCToolBoxFront-g7agf3akbkbyf4db.z01.azurefd.net/api/Twitter/Tweet", content).ConfigureAwait(false);
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.Forbidden) 
            {
                await GetChallange();
                responseMessage = await Web.WebHelper.HttpClient.PostAsync("https://VRCToolBoxFront-g7agf3akbkbyf4db.z01.azurefd.net/api/Twitter/Tweet", content).ConfigureAwait(false);
            }
            if (responseMessage.IsSuccessStatusCode)
            {
                System.Windows.MessageBox.Show($@"投稿しました。{Environment.NewLine}意図した通りか確認してください。");
            }
            else
            {
                string text = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                System.Windows.MessageBox.Show(text);
            }
        }
        private async Task GetChallange()
        {
            var parameters = new Dictionary<string, string>() { { "userId", _userId } };
            var content = new StringContent(JsonSerializer.Serialize(parameters));
            var response = await Web.WebHelper.HttpClient.PostAsync("https://VRCToolBoxFront-g7agf3akbkbyf4db.z01.azurefd.net/api/Twitter/Register", content);
            if (!response.IsSuccessStatusCode) throw new Exception("サーバーとの間で通信に失敗しました。");
            string salt = await response.Content.ReadAsStringAsync();
            _pass = Convert.FromBase64String(_rPass).GenerateHashPBKDF2(Convert.FromBase64String(salt), 100000);
        }
    }
}
