using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PokerApp.Domain.Common
{

    public static class StringExtension
    {
        public static bool IsNullOrBlank(this string inputString)
        {
            return string.IsNullOrEmpty(inputString);
        }

        public static string IsNull(this string inputString)
        {
            return string.IsNullOrEmpty(inputString)?string.Empty:inputString;
        }

        public static bool EqualsIgnoreCase(this string inputString, string otherString)
        {
            return inputString.IsNull().ToLowerInvariant().Equals(otherString.IsNull().ToLowerInvariant());
        }

    }

}
