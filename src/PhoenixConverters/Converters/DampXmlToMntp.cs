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
                    var oldValue = content.GetValue<string>(propertyType.Alias);
                    var newValue = convert(oldValue);
                    var seemedToWork = (!String.IsNullOrWhiteSpace(newValue));

                    if (!String.IsNullOrWhiteSpace(oldValue))
                    { 
                        result.PropertyResults.Add(new PropertyResult()
                        {
                            ContentName = content.Name,
                            ContentId = content.Id,
                            PropertyAlias = propertyType.Alias,
                            PropertyValue = oldValue.TruncateAtWord(1000000),
                            NewValue = newValue,
                            IsCompatible = seemedToWork
                        });

                        if (!test && seemedToWork)
                        {
                            //save the new properties
                            content.SetValue(propertyType.Alias, newValue);
                            Services.ContentService.Save(content);
                            //publish?
                        }
                    }
                }
            }

            var successfulCount = result.PropertyResults.Where(x => x.IsCompatible).Count();
            var failureCount = result.PropertyResults.Where(x => !x.IsCompatible).Count();
            var percent = (successfulCount * 100 / (failureCount + successfulCount));

            if (successfulCount >= failureCount)
            {
                result.Message = "This converter can convert (" + successfulCount+ "/" + (failureCount + successfulCount) + ") " + percent + "% of the items!";
                result.IsCompatible = true;
            }
            else
            {
                result.Message = "It looks like there was a high failure rate!";
                result.IsCompatible = false;
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
