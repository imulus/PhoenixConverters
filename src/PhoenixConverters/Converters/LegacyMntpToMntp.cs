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
    public class LegacyMntpToMntp : DataTypeConverterBase
    {
        public override string Alias
        {
            get
            {
                return "legacyMntpToMntp";
            }
        }

        public override string Name
        {
            get
            {
                return "Legacy MNTP to MNTP";
            }
        }

        public override string ConvertTo
        {
            get
            {
                return "Umbraco.MultiNodeTreePicker";
            }
        }

        public override ConversionResult Convert(int sourceDataTypeId, int targetDataTypeId, bool updatePropertyTypes, bool publish, bool isTest = true)
        {
            var result = new ConversionResult(Services, sourceDataTypeId, targetDataTypeId, updatePropertyTypes, publish, isTest);

            result = result.Convert(convert);
            result.Message = "Complete.";

            if (result.SuccessfulConversions >= result.FailedConversions)
            {
                result.IsCompatible = true;
            }
            else
            {
                result.IsCompatible = false;
            }

            return result;
        }

        private string convert(string input)
        {
            /*
             *              * <MultiNodePicker type="content">                 <nodeId>1196</nodeId>                 <nodeId>1197</nodeId>               </MultiNodePicker>
             * 
             */
            try
            {
                var xd = new XmlDocument();
                xd.LoadXml(input);

                var idList = new List<string>();

                foreach (XmlNode element in xd.SelectNodes("//nodeId"))
                {
                    idList.Add(element.InnerText);
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
