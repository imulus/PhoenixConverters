using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoenixConverters.Abstract;
using PhoenixConverters.Models;
using Umbraco.Core.Logging;

namespace PhoenixConverters.Converters
{
    public class DampCsvToMntp : DataTypeConverterBase
    {
        public override string Alias
        {
            get
            {
                return "dampCsvToMntp";
            }
        }

        public override string Name
        {
            get
            {
                return "DAMP (CSV) to MNTP";
            }
        }

        public override string TargetPropertyTypeAlias
        {
            get
            {
                return "Umbraco.MultipleMediaPicker";
            }
        }

        public override ConversionResult Convert(int targetDataTypeId, bool test = true)
        {
            var result = new ConversionResult(Services, targetDataTypeId);

            if (true)
            {
                result.IsCompatible = true;
                result.Message = "Success!";
            }
            else
            {
                result.IsCompatible = false;
                result.Message = "Failure!";
            }

            return result;
        }
    }
}
