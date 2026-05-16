using System;
using System.Linq;

namespace Clean_Connect.Domain.Utilities
{
    public static class ReferralCodeGenerator
    {
        private static readonly Random _random = new Random();
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string Generate(string firstName)
        {
            var prefix = string.Concat(firstName.Where(char.IsLetter)).ToUpperInvariant();
            if (prefix.Length > 4) prefix = prefix.Substring(0, 4);
            
            var suffix = new string(Enumerable.Repeat(Chars, 6)
                .Select(s => s[_random.Next(s.Length)]).ToArray());

            return $"{prefix}-{suffix}";
        }
    }
}
