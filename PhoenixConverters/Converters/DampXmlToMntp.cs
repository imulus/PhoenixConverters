using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoenixConverters.Abstract;
using PhoenixConverters.Models;

namespace PhoenixConverters.Converters
{
    public class DampXmlToMntp : DataTypeConverterBase
    {
        public override string Alias
        {
            get
            {
                return "dampXmlToMntp";
            }
        }

        public override string Name
        {
            get
            {
                return "DAMP (XML) to MNTP";
            }
        }

        public override string TargetPropertyTypeAlias
        {
            get
            {
                return "Umbraco.MultipleMediaPicker";
            }
        }

        public override ConversionResult Convert(int targetDataTypeId, bool preview = true)
        {
            return new ConversionResult(Services, targetDataTypeId);
        }
    }
}
