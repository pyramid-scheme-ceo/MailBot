using System;

namespace MailBot.Domain.BusinessLayer.Extensions
{
    public static class StringExtensions
    {
        public static string ValueOrThrow(this string input)
            => string.IsNullOrEmpty(input) ? throw new ArgumentNullException(nameof(input)) : input;
    }
}