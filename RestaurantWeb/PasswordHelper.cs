﻿using System.Security.Cryptography;
using System.Text;

namespace RestaurantWeb
{
    public static class PasswordHelper
    {
        public static string ComputeHash(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
