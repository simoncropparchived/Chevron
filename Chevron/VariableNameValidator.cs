using System;
using System.Linq;

namespace Chevron
{
    public static class VariableNameValidator
    {
        public static void ValidateSuffix(string variableName)
        {
            if (variableName.Any(ch => !IsValidChar(ch)))
            {
                throw new Exception(string.Format("The string '{0}' is not a valid name.", variableName));
            }
        }

        static bool IsValidChar(char ch)
        {
            return IsANumber(ch) ||
                   char.IsLetter(ch) ||
                   ch == '_' ||
                   ch == '$';
        }

        public static bool IsANumber(this char ch)
        {
            return ch >= '0' && ch <= '9';
        }
    }
}