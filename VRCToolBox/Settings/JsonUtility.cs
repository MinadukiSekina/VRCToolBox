using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace VRCToolBox.Settings
{
    internal class JsonUtility
    {
        private static JsonSerializerOptions? _options;
        internal static JsonSerializerOptions Options
        {
            get
            {
                _options ??= new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All), WriteIndented = true };
                return _options;
            }
        }
        internal static async Task<T> LoadObjectJsonAsync<T>(string jsonPath, CancellationToken cancellationToken) where T : new()
        {
            if (File.Exists(jsonPath) == false) return new T();
            using (FileStream fs = new FileStream(jsonPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true))
            {
                return await JsonSerializer.DeserializeAsync<T>(fs, Options, cancellationToken) ?? new T();
            }
        }

    }
}
