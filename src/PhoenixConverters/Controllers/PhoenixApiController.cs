using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using PhoenixConverters.Core;

namespace PhoenixConverters.Controllers
{
    public class PhoenixApiController : UmbracoAuthorizedApiController
    {
        public object GetConverterByAlias(string alias)
        {
            var converter = PhoenixCore.GetAllConverters().Where(x => x.Alias.ToLower() == alias.ToLower()).FirstOrDefault();

            if (converter == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            return new { alias = converter.Alias, name = converter.Name, targetPropertyTypeAlias = converter.ConvertTo };
        }

        [HttpGet]
        public object Test(string alias, int sourceDataTypeId, int targetDataTypeId, bool updatePropertyTypes = false, bool publish = false)
        {
            var converter = PhoenixCore.GetAllConverters().Where(x => x.Alias.ToLower() == alias.ToLower()).FirstOrDefault();

            if (converter == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            var result = converter.Convert(sourceDataTypeId, targetDataTypeId, updatePropertyTypes, publish, true);

            return new { 
                converterName = converter.Name, 
                sourceDataTypeId = result.SourceDataTypeDefinition.Id, 
                isCompatible = result.IsCompatible, 
                resultMessage = result.Message, 
                propertyResults = result.PropertyResults,
                successRate = result.SuccessRate,
                successfulConversions = result.SuccessfulConversions,
                failedConversions = result.FailedConversions,
                affectedDocTypes = result.AffectedDocTypes.Select(x => x.Name).ToList()
            };
        }

        [HttpGet]
        public object Conversion(string alias, int sourceDataTypeId, int targetDataTypeId, bool updatePropertyTypes = false, bool publish = false)
        {
            var converter = PhoenixCore.GetAllConverters().Where(x => x.Alias.ToLower() == alias.ToLower()).FirstOrDefault();

            if (converter == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            var result = converter.Convert(sourceDataTypeId, targetDataTypeId, updatePropertyTypes, publish, false);

            return new
            {
                converterName = converter.Name,
                sourceDataTypeId = result.SourceDataTypeDefinition.Id,
                isCompatible = result.IsCompatible,
                resultMessage = result.Message,
                propertyResults = result.PropertyResults,
                successRate = result.SuccessRate,
                successfulConversions = result.SuccessfulConversions,
                failedConversions = result.FailedConversions,
                affectedDocTypes = result.AffectedDocTypes.Select(x => x.Name).ToList()
            };
        }

        [HttpGet]
        public object GetSourceDataTypes()
        {
            var docTypes = Services.ContentTypeService.GetAllContentTypes();

            var propertyIds = new List<int>();
            var list = new List<object>();

            foreach (var docType in docTypes)
            {
                foreach(var property in docType.PropertyTypes)
                {
                    if(!propertyIds.Contains(property.DataTypeDefinitionId))
                    {
                        var dtd = Services.DataTypeService.GetDataTypeDefinitionById(property.DataTypeDefinitionId);

                        list.Add(new { id = dtd.Id, name = dtd.Name });
                        propertyIds.Add(dtd.Id);
                    }
                }
            }

            return list;
        }

        [HttpGet]
        public object GetDataTypesByAlias(string dataTypeAlias)
        {
            var dataTypes = Services.DataTypeService.GetDataTypeDefinitionByPropertyEditorAlias(dataTypeAlias);
            if (dataTypes == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            var list = new List<object>();

            foreach (var dataType in dataTypes)
            {
                list.Add(new { id = dataType.Id, name = dataType.Name });
            }

            return list;
        }
    }
}
