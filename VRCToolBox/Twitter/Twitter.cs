using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using VRCToolBox.Web;
using VRCToolBox.Common;
using System.Security.Cryptography;

namespace VRCToolBox.Twitter
{
    public class Twitter
    {
        private static readonly string s_redirectUri = "http://localhost:10000/";
        public static async Task Authenticate()
        {

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(s_redirectUri);
            listener.Start();

            try
            {
                List<Scope> scopes = new List<Scope>();
                scopes.Add(Scope.OfflineAccess);
                scopes.Add(Scope.TweetWrite);

                string state = Ulid.NewUlid().ToString();
                string code_Verifier = GenetateCodeVerifier(128);
                ProcessEx.Start(ConstructAuthorizeUrl(scopes.ToArray(), state, code_Verifier, "plain"), true);

                HttpListenerContext context = await listener.GetContextAsync().ConfigureAwait(false);

                // return response.
                string responseString = "<html lang=\"ja\"><head><title>VRCToolBox</title><meta charset=\"UTF-8\"></head><body><h1>タブを閉じて、アプリにお戻りください。</h1></body></html>";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                context.Response.ContentLength64 = buffer.Length;
                await context.Response.OutputStream.WriteAsync(buffer);
                context.Response.OutputStream.Close();

                if (context.Request.QueryString.Get("code") is string code) 
                {
                    if (context.Request.QueryString.Get("state") is string returnState) 
                    {
                        if (state != returnState) throw new InvalidDataException("state is not valid");
                        string token = await GetAccessToken(code, code_Verifier);
                    }
                }
                
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
            finally
            {
                listener.Stop();
            }
            
            
        }
        private static string GenetateCodeVerifier(int length)
        {
            string chars = "0123456789abcdefghijklmnopqrstuvwxyz-._~";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());
        }
        private static string ConstructAuthorizeUrl(IEnumerable<Scope> scopes, string state, string challenge, string challengeMethod = "S256")
        {
            System.Collections.Specialized.NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("response_type", "code");
            query.Add("client_id"    , Authentication.S_TwitterClientID);
            query.Add("redirect_uri" , s_redirectUri);
            query.Add("scope", string.Join(" ", scopes.Select(x => x.GetValue())));
            query.Add("state", state);
            query.Add("code_challenge", challenge);
            query.Add("code_challenge_method", challengeMethod);

            var uriBuilder = new UriBuilder("https://twitter.com/i/oauth2/authorize")
            {
                Query = query.ToString()
            };
            return uriBuilder.Uri.AbsoluteUri;
        }
        public string RetrieveAuthorizationCode(string callbackedUri, string state)
        {
            var uri = new Uri(callbackedUri);
            var query = HttpUtility.ParseQueryString(uri.Query);
            if (query.Get("state") != state)
            {
                throw new InvalidDataException("state is not valid.");
            }
            return query.Get("code");
        }
        private static async Task<string> GetAccessToken(string code, string verifier)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", s_redirectUri },
                { "code_verifier", verifier },
            };
            var content = new FormUrlEncodedContent(parameters);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/2/oauth2/token");
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Authentication.S_TwitterClientID}:{Authentication.S_TwitterClientSecret}"))
            );
            request.Content = content;
            var response = await WebHelper.HttpClient.SendAsync(request).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        public async Task<string> RefreshToken(string clientId, string clientSecret, string refreshToken)
        {
            var parameters = new Dictionary<string, string>()
    {
        { "refresh_token", refreshToken },
        { "grant_type", "refresh_token" },
    };
            var content = new FormUrlEncodedContent(parameters);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/2/oauth2/token");
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"))
            );
            request.Content = content;
            var response = await WebHelper.HttpClient.SendAsync(request).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        public async Task<string> RevokeToken(string clientId, string clientSecret, string accessToken)
        {
            var parameters = new Dictionary<string, string>()
    {
        { "token", accessToken },
        { "token_type_hint", "access_token" }
    };
            var content = new FormUrlEncodedContent(parameters);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/2/oauth2/revoke");
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"))
            );
            request.Content = content;
            var response = await WebHelper.HttpClient.SendAsync(request).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        public async Task<string> GetTweet(string accessToken, long id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.twitter.com/2/tweets/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await WebHelper.HttpClient.SendAsync(request).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        public async Task<string> PostTweet(string accessToken, string text)
        {
            var parameters = new Dictionary<string, string>()
    {
        { "text", text },
    };
            var json = JsonSerializer.Serialize(parameters);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/2/tweets");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = content;
            var response = await WebHelper.HttpClient.SendAsync(request).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }

    public enum Scope
    {
        TweetRead,
        TweetWrite,
        TweetModerateWrite,
        UsersRead,
        FollowRead,
        FollowWrite,
        OfflineAccess,
        SpaceRead,
        MuteRead,
        MuteWrite,
        LikeRead,
        LikeWrite,
        ListRead,
        ListWrite,
        BlockRead,
        BlockWrite,
        BookmarkRead,
        BookmarkWrite,
    }
    public static class ScopeExtension
    {
        private static Dictionary<Scope, string> Values = new Dictionary<Scope, string>()
    {
        { Scope.TweetRead, "tweet.read" },
        { Scope.TweetWrite, "tweet.write" },
        { Scope.TweetModerateWrite, "tweet.moderate.write" },
        { Scope.UsersRead, "users.read" },
        { Scope.FollowRead, "follow.read" },
        { Scope.FollowWrite, "follow.write" },
        { Scope.OfflineAccess, "offline.access" },
        { Scope.SpaceRead, "space.read" },
        { Scope.MuteRead, "mute.read" },
        { Scope.MuteWrite, "mute.write" },
        { Scope.LikeRead, "like.read" },
        { Scope.LikeWrite, "like.write" },
        { Scope.ListRead, "list.read" },
        { Scope.ListWrite, "list.write" },
        { Scope.BlockRead, "block.read" },
        { Scope.BlockWrite, "block.write" },
        { Scope.BookmarkRead, "bookmark.read" },
        { Scope.BookmarkWrite, "bookmark.write" },
    };

        public static string GetValue(this Scope scope)
        {
            return Values[scope];
        }
    }
}
