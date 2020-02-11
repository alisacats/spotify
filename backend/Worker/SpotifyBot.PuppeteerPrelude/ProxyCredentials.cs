using System;

namespace SpotifyBot.PuppeteerPrelude
{
    public sealed class ProxyCredentials
    {
        public string Login { get; }
        public string Password { get; }

        public ProxyCredentials(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public static ProxyCredentials Parse(string str)
        {
            var parts = str.Split(':');
            if (parts.Length != 2) throw new ArgumentException("str should have single ':'", nameof(str));

            return new ProxyCredentials(parts[0], parts[1]);
        }

        public override string ToString() => $"{Login}:{Password}";
    }
}
