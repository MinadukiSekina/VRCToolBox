﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace VRCToolBox.Crypt
{
    internal static class Hash
    {
        // PBKDF2 でハッシュを計算します。
        internal static string GenerateHashPBKDF2(this byte[] target, byte[] salt, int Iteration = 10000, int length = 64)
        {
            using var deriver = new Rfc2898DeriveBytes(target, salt, Iteration);
            byte[] b = deriver.GetBytes(length);
            return Convert.ToBase64String(b);
        }
        internal static string GenerateHashPBKDF2(this string target, byte[] salt, int Iteration = 10000, int length = 64)
        {
            using var deriver = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(target), salt, Iteration);
            byte[] b = deriver.GetBytes(length);
            return Convert.ToBase64String(b);
        }
        // reference : https://mseeeen.msen.jp/csharp-static-method-for-generating-random-password-with-specified-chars/
        internal static string GeneratePass(int length = 32)
        {
            string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ+*.@;:!<>?#$&'()=-~^_[]";
            var r = new Random();
            return string.Join("", Enumerable.Range(0, length).Select(_ => chars[r.Next(chars.Length)]));
        }
    }
}
