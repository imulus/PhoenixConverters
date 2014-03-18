using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoenixConverters.Abstract;

namespace PhoenixConverters.Core
{
    public static class PhoenixCore
    {
        public static IEnumerable<DataTypeConverterBase> GetAllConverters()
        {
            var interfaceType = typeof(DataTypeConverterBase);
            return AppDomain.CurrentDomain.GetAssemblies()
              .SelectMany(x => x.GetTypes())
              .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
              .Select(x => (DataTypeConverterBase)Activator.CreateInstance(x));
        }
    }
}
