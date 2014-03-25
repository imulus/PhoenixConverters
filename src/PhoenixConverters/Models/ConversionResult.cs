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
        public ServiceContext Services;
        public bool IsCompatible { get; set; }
        public string Message { get; set; }
        public List<AffectedContent> AffectedContent { get; private set; }
        public List<IContentType> AffectedDocTypes { get; private set; }
        public IDataTypeDefinition SourceDataTypeDefinition { get; private set; }
        public IDataTypeDefinition TargetDataTypeDefinition { get; private set; }
        public List<PropertyResult> PropertyResults { get; private set; }
        public int SuccessfulConversions { get; private set; }
        public int FailedConversions { get; private set; }
        public int SuccessRate { get; private set; }

        public ConversionResult(ServiceContext services, int sourceDataTypeId, int targetDataTypeId)
        {
            this.Services = services;
            
            this.SourceDataTypeDefinition = services.DataTypeService.GetAllDataTypeDefinitions(sourceDataTypeId).FirstOrDefault();
            this.TargetDataTypeDefinition = services.DataTypeService.GetAllDataTypeDefinitions(targetDataTypeId).FirstOrDefault();

            this.AffectedContent = new List<AffectedContent>();

            this.AffectedDocTypes = new List<IContentType>();
            var parentDocTypes = new List<IContentType>();

            parentDocTypes.AddRange(services.ContentTypeService.GetAllContentTypes().Where(x => x.PropertyTypes.Where(y => y.DataTypeDefinitionId == sourceDataTypeId).Any()));

            //add their children
            foreach (var parentDocType in parentDocTypes)
            {
                this.AffectedDocTypes.AddRange(services.ContentTypeService.GetContentTypeChildren(parentDocType.Id));
            }

            //add in parents
            this.AffectedDocTypes.AddRange(parentDocTypes);

            foreach (var docType in this.AffectedDocTypes)
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
            
            SuccessRate = (FailedConversions + SuccessfulConversions != 0) ? (SuccessfulConversions * 100 / (FailedConversions + SuccessfulConversions)) : 0;
            
            return this;
        }

        public virtual void UpdatePropertyTypes()
        {
            foreach (var contentType in AffectedDocTypes)
            {
                var affectedPropertyTypes = Services.ContentTypeService.GetContentType(contentType.Id).PropertyTypes.Where(y => y.DataTypeDefinitionId == SourceDataTypeDefinition.Id);
                foreach (var type in affectedPropertyTypes)
                {
                    type.DataTypeDefinitionId = TargetDataTypeDefinition.Id;
                    Services.ContentTypeService.Save(contentType);
                }
            }
        }
    }
}
