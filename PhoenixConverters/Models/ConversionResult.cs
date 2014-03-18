using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Logging;

namespace PhoenixConverters.Models
{
    public class ConversionResult
    {
        public ConversionResult(ServiceContext services, int targetDataTypeId)
        {
            this.SourceDataTypeDefinition = services.DataTypeService.GetAllDataTypeDefinitions(targetDataTypeId).FirstOrDefault();

            this.AffectedDocTypes = services.ContentTypeService.GetAllContentTypes().Where(x => x.PropertyTypes.Where(y => y.DataTypeDefinitionId == targetDataTypeId).Any());
            
            foreach (var docType in this.AffectedDocTypes)
            {
                this.AffectedContent = services.ContentService.GetContentOfContentType(docType.Id);
            }
        }

        public bool IsCompatible { get; set; }
        public string Message { get; set; }
        public IEnumerable<IContentType> AffectedDocTypes { get; private set; }
        public IEnumerable<IContent> AffectedContent { get; private set; }
        public IDataTypeDefinition SourceDataTypeDefinition { get; private set; }
        public IDataTypeDefinition TargetDataTypeDefinition { get; private set; }
    }
}
