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
        public bool IsTest { get; set; }
        public bool ShouldUpdatePropertyTypes { get; set; }
        public bool ShouldPublish { get; set; }
        public string Message { get; set; }
        public List<AffectedContent> AffectedContent { get; private set; }
        public List<IContentType> AffectedDocTypes { get; private set; }
        public IDataTypeDefinition SourceDataTypeDefinition { get; private set; }
        public IDataTypeDefinition TargetDataTypeDefinition { get; private set; }
        public List<PropertyResult> PropertyResults { get; private set; }
        public int SuccessfulConversions { get; private set; }
        public int FailedConversions { get; private set; }
        public int SuccessRate { get; private set; }

        public ConversionResult(ServiceContext services, int sourceDataTypeId, int targetDataTypeId, bool updatePropertyTypes, bool publish, bool isTest)
        {
            Init(services, sourceDataTypeId, targetDataTypeId, updatePropertyTypes, publish, isTest);
        }

        private void Init(ServiceContext services, int sourceDataTypeId, int targetDataTypeId, bool updatePropertyTypes, bool publish, bool isTest)
        {
            this.Services = services;

            this.SourceDataTypeDefinition = services.DataTypeService.GetAllDataTypeDefinitions(sourceDataTypeId).FirstOrDefault();
            this.TargetDataTypeDefinition = services.DataTypeService.GetAllDataTypeDefinitions(targetDataTypeId).FirstOrDefault();

            this.IsTest = isTest;
            this.ShouldPublish = publish;
            this.ShouldUpdatePropertyTypes = updatePropertyTypes;

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

        public ConversionResult Summarize(){
            SuccessfulConversions = this.PropertyResults.Where(x => x.IsCompatible).Count();
            FailedConversions = this.PropertyResults.Where(x => !x.IsCompatible).Count();
            
            SuccessRate = (FailedConversions + SuccessfulConversions != 0) ? (SuccessfulConversions * 100 / (FailedConversions + SuccessfulConversions)) : 0;
            
            return this;
        }

        public ConversionResult UpdatePropertyTypes()
        {
            if (IsTest || !ShouldUpdatePropertyTypes)
            {
                return this;
            }

            foreach (var contentType in AffectedDocTypes)
            {
                var affectedPropertyTypes = Services.ContentTypeService.GetContentType(contentType.Id).PropertyTypes.Where(y => y.DataTypeDefinitionId == SourceDataTypeDefinition.Id);
                foreach (var type in affectedPropertyTypes)
                {
                    type.DataTypeDefinitionId = TargetDataTypeDefinition.Id;
                    Services.ContentTypeService.Save(contentType);
                }
            }
            return this;
        }

        public ConversionResult Convert(Func<string, string> convertMethod)
        {
            foreach (var ac in this.AffectedContent)
            {
                var oldValue = ac.Content.GetValue<string>(ac.PropertyType.Alias);
                var newValue = convertMethod(oldValue);
                var seemedToWork = (!String.IsNullOrWhiteSpace(newValue));

                if (!String.IsNullOrWhiteSpace(oldValue))
                {
                    this.PropertyResults.Add(new PropertyResult()
                    {
                        ContentName = ac.Content.Name,
                        ContentId = ac.Content.Id,
                        PropertyAlias = ac.PropertyType.Alias,
                        PropertyValue = oldValue,
                        NewValue = newValue,
                        IsCompatible = seemedToWork
                    });

                    if (!IsTest && seemedToWork)
                    {
                        //save the new properties
                        ac.Content.SetValue(ac.PropertyType.Alias, newValue);
                        Services.ContentService.Save(ac.Content);
                        //publish?
                    }
                }
            }
            return this.Summarize().UpdatePropertyTypes();
        }
    }
}
