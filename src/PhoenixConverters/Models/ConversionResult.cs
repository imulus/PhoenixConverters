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

            this.AffectedDocTypes = new List<IContentType>();
            this.AffectedContent = new List<IContent>();

            this.AffectedDocTypes.AddRange(services.ContentTypeService.GetAllContentTypes().Where(x => x.PropertyTypes.Where(y => y.DataTypeDefinitionId == targetDataTypeId).Any()));
            
            foreach (var docType in this.AffectedDocTypes)
            {
                this.AffectedContent.AddRange(services.ContentService.GetContentOfContentType(docType.Id).Where(x => !x.Trashed));
            }

            PropertyResults = new List<PropertyResult>();
        }

        public bool IsCompatible { get; set; }
        public string Message { get; set; }
        public List<IContentType> AffectedDocTypes { get; private set; }
        public List<IContent> AffectedContent { get; private set; }
        public IDataTypeDefinition SourceDataTypeDefinition { get; private set; }
        public IDataTypeDefinition TargetDataTypeDefinition { get; private set; }
        public List<PropertyResult> PropertyResults { get; private set; }
    }
}
