using System;

namespace SneakerAPI.Core.Libraries;
    public class HandleString
{   
    public const string DefaultImage="default.jpg";
    public static string GenerateVerifyCode()
    {
        Random random = new Random();
        string numberString = random.Next(1000, 9999).ToString();
        return numberString;
    }
    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";
        Random random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}