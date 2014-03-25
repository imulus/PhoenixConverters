using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoenixConverters.Models;
using Umbraco.Core;
using Umbraco.Core.Services;

namespace PhoenixConverters.Abstract
{
    public abstract class DataTypeConverterBase
    {
        public ServiceContext Services;

        public DataTypeConverterBase()
        {
            Services = ApplicationContext.Current.Services;
        }

        public abstract string Alias { get; }
        public abstract string Name { get; }
        public abstract string ConvertTo { get; }
        public abstract ConversionResult Convert(int sourceDataTypeId, int targetDataTypeId, bool updatePropertyTypes, bool publish, bool preview = true);
    }
}
