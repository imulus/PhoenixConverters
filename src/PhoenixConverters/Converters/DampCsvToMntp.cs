using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoenixConverters.Abstract;
using PhoenixConverters.Models;
using PhoenixConverters.Extentions;
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

            foreach (var content in result.AffectedContent)
            {
                foreach (var propertyType in result.AffectedProperties)
                {
                    LogHelper.Info<ConversionResult>(propertyType.Alias);
                    LogHelper.Info<ConversionResult>(content.GetValue<string>(propertyType.Alias));

                    result.PropertyResults.Add(new PropertyResult() { 
                        ContentName = content.Name, 
                        ContentId = content.Id,
                        PropertyAlias = propertyType.Alias, 
                        PropertyValue = content.GetValue<string>(propertyType.Alias).TruncateAtWord(100), 
                        NewValue = "[\"foo\": 123]",
                        Compatible = true 
                    });
                }
            }

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
