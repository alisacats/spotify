using System;
using System.Linq;

namespace SpotifyBot.PuppeteerPrelude
{
    public sealed class Proxy
    {
        public string Address { get; }
        public ProxyCredentials Credentials { get; }

        public Proxy(string address, ProxyCredentials credentials)
        {
            Address = address;
            Credentials = credentials;
        }

        public static Proxy Parse(string str)
        {
            var parts = str.Split('@');
            if (parts.Length > 2) throw new ArgumentException("more than 2 '@'", nameof(str));

            return parts.Length == 2
                ? new Proxy(parts[1], ProxyCredentials.Parse(parts[0])) // USERNAME:PASSWORD@PROXYIP:PROXYPORT
                : new Proxy(parts[0], null); // PROXYIP:PROXYPORT
        }

        public override string ToString() =>
            string.Join("@", new[] { Credentials?.ToString(), Address }.Where(x => x != null));
    }
}
