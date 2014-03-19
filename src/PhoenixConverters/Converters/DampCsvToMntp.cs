using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
                return "Umbraco.MultiNodeTreePicker";
            }
        }

        public override ConversionResult Convert(int targetDataTypeId, bool test = true)
        {
            var result = new ConversionResult(Services, targetDataTypeId);

            foreach (var content in result.AffectedContent)
            {
                foreach (var propertyType in content.PropertyTypes.Where(x => x.DataTypeDefinitionId == targetDataTypeId))
                {
                    if (!String.IsNullOrWhiteSpace(content.GetValue<string>(propertyType.Alias)))
                    {
                        var oldValue = content.GetValue<string>(propertyType.Alias);
                        var newValue = convert(oldValue);

                        result.PropertyResults.Add(new PropertyResult()
                        {
                            ContentName = content.Name,
                            ContentId = content.Id,
                            PropertyAlias = propertyType.Alias,
                            PropertyValue = oldValue.TruncateAtWord(1000000),
                            NewValue = newValue,
                            IsCompatible = (!String.IsNullOrWhiteSpace(newValue))
                        });
                    }               
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

        private string convert(string input)
        {
            try
            {
                var xd = new XmlDocument();
                xd.LoadXml(input);

                var idList = new List<string>();

                foreach (XmlNode element in xd.SelectNodes("//Image"))
                {
                    idList.Add(element.Attributes["id"].Value);
                }

                return string.Join(",", idList);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
