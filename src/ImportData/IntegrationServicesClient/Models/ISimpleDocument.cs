using System;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Простой документ")]
    public class ISimpleDocument : IInternalDocumentBases
    {
        private DateTimeOffset? registrationDate;

        [PropertyOptions("ИД журнала регистрации", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
        new public IDocumentRegisters DocumentRegister { get; set; }

        [PropertyOptions("Дата документа", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
        new public DateTimeOffset? RegistrationDate
        {
            get { return registrationDate; }
            set { registrationDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
        }

        new public static ISimpleDocument FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
        {
            ISimpleDocument doc = null;
            var regNumber = propertiesForSearch[Constants.KeyAttributes.RegistrationNumber];
            var docRegisterId = propertiesForSearch[Constants.KeyAttributes.DocumentRegister];

            if (GetDate(propertiesForSearch[Constants.KeyAttributes.RegistrationDate], out var registrationDate) &&
              int.TryParse(docRegisterId, out int documentRegisterId))
            {
                var documentRegister = BusinessLogic.GetEntityWithFilter<IDocumentRegisters>(x => x.Id == documentRegisterId, exceptionList, logger);
                doc = BusinessLogic.GetEntityWithFilter<ISimpleDocument>(x => x.RegistrationNumber != null &&
                  x.RegistrationNumber == regNumber &&
                  x.RegistrationDate == registrationDate &&
                  x.DocumentRegister.Id == documentRegister.Id, exceptionList, logger);
            }
            return doc;
        }

        new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
        {

            if (isNewEntity)
            {
                var lifeCycleState = ((ISimpleDocument)entity).LifeCycleState;
                entity = BusinessLogic.CreateEntity((ISimpleDocument)entity, exceptionList, logger, isBatch);
                return ((ISimpleDocument)entity)?.UpdateLifeCycleState(lifeCycleState, isBatch);  
            }
            else
            {
                return BusinessLogic.UpdateEntity((ISimpleDocument)entity, exceptionList, logger);
            }
        }
    }
}

