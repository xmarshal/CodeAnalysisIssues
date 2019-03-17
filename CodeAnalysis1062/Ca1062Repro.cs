using System;
using Annotations;

[assembly: CLSCompliant(true)]

namespace CodeAnalysis1062
{
    public static class Ca1062Repro
    {

        public static string ToString(object param)
        {
            Check.NotNull(param, nameof(param));

            return param.ToString();
        }
    }
}
