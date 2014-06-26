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

        public override string ConvertTo
        {
            get
            {
                return "Umbraco.MultiNodeTreePicker";
            }
        }

        public override ConversionResult Convert(int sourceDataTypeId, int targetDataTypeId, bool updatePropertyTypes, bool publish, bool test = true)
        {
            var result = new ConversionResult(Services, sourceDataTypeId, targetDataTypeId, updatePropertyTypes, publish, test);

            foreach (var ac in result.AffectedContent)
            {
                var oldValue = ac.Content.GetValue<string>(ac.PropertyType.Alias);
                var newValue = convert(oldValue);
                var seemedToWork = (!String.IsNullOrWhiteSpace(newValue));

                if (!String.IsNullOrWhiteSpace(oldValue))
                { 
                    result.PropertyResults.Add(new PropertyResult()
                    {
                        ContentName = ac.Content.Name,
                        ContentId = ac.Content.Id,
                        PropertyAlias = ac.PropertyType.Alias,
                        PropertyValue = oldValue.TruncateAtWord(1000000),
                        NewValue = newValue,
                        IsCompatible = seemedToWork
                    });

                    if (!test && seemedToWork)
                    {
                        //save the new properties
                        ac.Content.SetValue(ac.PropertyType.Alias, newValue);
                        Services.ContentService.Save(ac.Content);
                        //publish?
                    }
                }
            }

            result = result.Summarize();
            result.Message = "Complete.";

            if (result.SuccessfulConversions >= result.FailedConversions)
            {
                result.IsCompatible = true;
                if (!test)
                {
                    result.UpdatePropertyTypes();
                }
            }
            else
            {
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

                foreach (XmlNode element in xd.SelectNodes("//mediaItem/node()"))
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
