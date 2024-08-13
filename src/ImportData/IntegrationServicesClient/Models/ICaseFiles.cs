﻿using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Дело")]
  public class ICaseFiles : IEntity
  {
    private DateTimeOffset? startDate;
    private DateTimeOffset? endDate;

    [PropertyOptions("Заголовок", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string Title { get; set; }

    [PropertyOptions("Индекс", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string Index { get; set; }

    [PropertyOptions("Дата начала", RequiredType.Required, PropertyType.Simple)]
    public DateTimeOffset? StartDate
    {
      get { return startDate; }
      set { startDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("Дата окончания", RequiredType.NotRequired, PropertyType.Simple)]
    public DateTimeOffset? EndDate
    {
      get { return endDate; }
      set { endDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("Примечание", RequiredType.NotRequired, PropertyType.Simple)]
    public string Note { get; set; }

    [PropertyOptions("Подразделение", RequiredType.NotRequired, PropertyType.Entity)]
    public IDepartments Department { get; set; }

    [PropertyOptions("Наименование срока хранения", RequiredType.Required, PropertyType.EntityWithCreate, AdditionalCharacters.CreateFromOtherProperties)]
    public IFileRetentionPeriods RetentionPeriod { get; set; }

    public bool LongTerm { get; set; }
    public string Location { get; set; }

    [PropertyOptions("Группа регистрации", RequiredType.NotRequired, PropertyType.Entity)]
    public IRegistrationGroups RegistrationGroup { get; set; }

    public string Status { get; set; }

    [PropertyOptions("НОР", RequiredType.NotRequired, PropertyType.Entity)]
    public IBusinessUnits BusinessUnit { get; set; }

    new public static IEntity CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var title = propertiesForSearch[Constants.KeyAttributes.Title];
      var index = propertiesForSearch[Constants.KeyAttributes.Index];
      var name = string.Format("{0}. {1}", index, title);
      var startDate = DateTimeOffset.Parse(propertiesForSearch[Constants.KeyAttributes.StartDate]);
      var retentionPeriodName = propertiesForSearch[Constants.KeyAttributes.RetentionPeriod];
      var retentionPeriodTime = int.Parse(propertiesForSearch[Constants.KeyAttributes.RetentionPeriod]);
      var retentionPeriod = BusinessLogic.GetEntityWithFilter<IFileRetentionPeriods>(x => x.Name == retentionPeriodName && 
        x.RetentionPeriod == retentionPeriodTime, exceptionList, logger);

      return BusinessLogic.CreateEntity(new ICaseFiles()
      {
        Title = title,
        Index = index,
        StartDate = startDate,
        RetentionPeriod = retentionPeriod,
        Name = name,
        Status = Constants.AttributeValue[Constants.KeyAttributes.Status],
      }, exceptionList, logger);
    }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var title = propertiesForSearch[Constants.KeyAttributes.Title];
      var index = propertiesForSearch[Constants.KeyAttributes.Index];

      return BusinessLogic.GetEntityWithFilter<ICaseFiles>(x => x.Title == title && x.Index == index, exceptionList, logger);
    }

    new public static void CreateOrUpdate(IEntity entity, bool isNewEntity, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        BusinessLogic.CreateEntity((ICaseFiles)entity, exceptionList, logger);
      else
        BusinessLogic.UpdateEntity((ICaseFiles)entity, exceptionList, logger);
    }
  }
}
