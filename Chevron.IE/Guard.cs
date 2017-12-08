using System;

namespace Chevron
{
    static class Guard
    {
        // ReSharper disable UnusedParameter.Global
        public static void AgainstNull(object value, string argumentName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void AgainstNullAndEmpty(string value, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}