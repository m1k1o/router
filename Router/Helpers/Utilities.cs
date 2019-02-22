using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.Helpers
{
    class Utilities
    {

    }

    static class StringExtensions
    {
        public static string Or(this string Str, string Default = null)
        {
            if (string.IsNullOrEmpty(Str))
            {
                return Default;
            }

            return Str;
        }
    }
}
