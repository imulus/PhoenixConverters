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
        public ConversionResult(ServiceContext services, int sourceDataTypeId)
        {
            this.SourceDataTypeDefinition = services.DataTypeService.GetAllDataTypeDefinitions(sourceDataTypeId).FirstOrDefault();

            this.AffectedContent = new List<AffectedContent>();

            foreach (var docType in services.ContentTypeService.GetAllContentTypes().Where(x => x.PropertyTypes.Where(y => y.DataTypeDefinitionId == sourceDataTypeId).Any()))
            {
                foreach (var content in services.ContentService.GetContentOfContentType(docType.Id).Where(x => !x.Trashed))
                {
                    foreach (var propertyType in content.PropertyTypes.Where(x => x.DataTypeDefinitionId == sourceDataTypeId))
                    {
                        this.AffectedContent.Add(new AffectedContent()
                        {
                            Content = content,
                            PropertyType = propertyType
                        });
                    }
                }
            }

            PropertyResults = new List<PropertyResult>();
        }

        public virtual ConversionResult Summarize(){
            SuccessfulConversions = this.PropertyResults.Where(x => x.IsCompatible).Count();
            FailedConversions = this.PropertyResults.Where(x => !x.IsCompatible).Count();
            SuccessRate = (SuccessfulConversions * 100 / (FailedConversions + SuccessfulConversions));
            
            return this;
        }

        public bool IsCompatible { get; set; }
        public string Message { get; set; }
        public List<AffectedContent> AffectedContent { get; private set; }
        public IDataTypeDefinition SourceDataTypeDefinition { get; private set; }
        public IDataTypeDefinition TargetDataTypeDefinition { get; private set; }
        public List<PropertyResult> PropertyResults { get; private set; }
        public int SuccessfulConversions { get; private set; }
        public int FailedConversions { get; private set; }
        public int SuccessRate { get; private set; }
    }
}
