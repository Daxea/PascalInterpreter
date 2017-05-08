using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalInterpreter
{
    internal static class Extensions
    {
        public static T As<T>(this object instance) => (T)instance;
    }
}