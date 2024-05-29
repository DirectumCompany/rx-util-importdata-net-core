﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Контакт")]
    public class IContacts : IEntity
    {
		[PropertyOptions("Должность", RequiredType.NotRequired, PropertyType.Simple)]
		public string JobTitle { get; set; }

		[PropertyOptions("Телефон", RequiredType.NotRequired, PropertyType.Simple)]
		public string Phone { get; set; }

		[PropertyOptions("Факс", RequiredType.NotRequired, PropertyType.Simple)]
		public string Fax { get; set; }

		[PropertyOptions("Эл. Почта", RequiredType.NotRequired, PropertyType.Simple)]
		public string Email { get; set; }

		[PropertyOptions("Примечание", RequiredType.NotRequired, PropertyType.Simple)]
		public string Note { get; set; }

		[PropertyOptions("Сайт", RequiredType.NotRequired, PropertyType.Simple)]
		public string Homepage { get; set; }

		[PropertyOptions("ИНН", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
		public string Status { get; set; }

		[PropertyOptions("Организация", RequiredType.Required, PropertyType.EntityWithCreate)]
		public ICompanies Company { get; set; }

		[PropertyOptions("Персона", RequiredType.NotRequired, PropertyType.EntityWithCreate, AdditionalCharacters.CreateFromOtherProperties)]
        public IPersons Person { get; set; }

        new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
        {
            var name = propertiesForSearch[Constants.KeyAttributes.Name];
            return BusinessLogic.GetEntityWithFilter<IContacts>(x => x.Name == name, exceptionList, logger);
        }

		new public static string GetName(Entity entity)
		{
			var person = (IPersons)entity.ResultValues["Person"];
			return person.Name;
		}

		new public static bool FillProperies(Entity entity, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
		{
			entity.ResultValues["Name"] = GetName(entity);
			entity.ResultValues["Status"] = "Active";

			return false;
		}

		new public static void CreateOrUpdate(IEntity entity, bool isNewEntity, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
		{
			if (isNewEntity)
				BusinessLogic.CreateEntity((IContacts)entity, exceptionList, logger);
			else
				BusinessLogic.UpdateEntity((IContacts)entity, exceptionList, logger);
		}
	}
}
