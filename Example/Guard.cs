using System;

namespace Example
{
    public static class Guard
    {
        public static void Against(bool assertion, string message)
        {
            if (assertion)
                throw new InvalidOperationException(message);
        }

        public static void That(bool assertion, string message)
        {
            Guard.Against(!assertion, message);
        }

        public static void AgainstNullOrWhitespace(string s, string message)
        {
            Guard.Against(string.IsNullOrWhiteSpace(s), message);
        }
    }
}
