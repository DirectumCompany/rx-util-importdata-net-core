﻿using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Официальный документ")]
    public class IOfficialDocuments : IElectronicDocuments
    {
        private DateTimeOffset? registrationDate;
		private DateTimeOffset? documentDate;
        private DateTimeOffset? placedToCaseFileDate;

		public string RegistrationNumber { get; set; }
        public DateTimeOffset? RegistrationDate
		{
			get { return registrationDate; }
			set { registrationDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
		}
		public string Subject { get; set; }
        public string Note { get; set; }
        public DateTimeOffset? DocumentDate
		{
			get { return documentDate; }
			set { documentDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
		}
		public string LifeCycleState { get; set; }
        public string RegistrationState { get; set; }
        public IDocumentRegisters DocumentRegister { get; set; }
        public IDocumentKinds DocumentKind { get; set; }
        public IOfficialDocuments LeadingDocument { get; set; }
        public IDocumentGroupBases DocumentGroup { get; set; }
        public IDepartments Department { get; set; }
        public IEmployees DeliveredTo { get; set; }
        public IEmployees OurSignatory { get; set; }
        public IEmployees PreparedBy { get; set; }
        public ICaseFiles CaseFile { get; set; }
        public DateTimeOffset? PlacedToCaseFileDate
		{
			get { return placedToCaseFileDate; }
			set { placedToCaseFileDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
		}

		public static (IOfficialDocuments leadingDocument, string errorMessage) GetLeadingDocument(Logger logger, string registrationNumber, DateTimeOffset regDate, int counterpartyId = -1)
        {
            var leadingDocuments = BusinessLogic.GetEntitiesWithFilter<IContracts>(d => d.RegistrationNumber == registrationNumber &&
                    d.RegistrationDate.Value.ToString("d") == regDate.ToString("d") &&
                    (counterpartyId == -1 || d.Counterparty.Id == counterpartyId), 
                    new List<Structures.ExceptionsStruct>(), logger, true);

            var message = string.Empty;
            if (!leadingDocuments.Any())
                message = string.Format("Не найден ведущий документ с реквизитами \"Дата документа\" {0}, \"Рег. номер\" {1}.", regDate.ToString("d"), registrationNumber);

            if (leadingDocuments.Count() > 1)
                message = string.Format("Найдено несколько ведущих документов с реквизитами \"Дата документа\" {0}, \"Рег. номер\" {1}.", regDate.ToString("d"), registrationNumber);

            if (!string.IsNullOrEmpty(message))
            {
                logger.Error(message);
                return (null, message);
            }

            var leadingDocument = leadingDocuments.FirstOrDefault();
            return (leadingDocument, message);
        }
    }

    public static class IOfficialDocumentsExtensions
    {
        /// <summary>
        /// Обновить свойство LifeCycleState.
        /// </summary>
        /// <param name="entity">Сущность, свойство которого необходимо обновить.</param>
        /// <param name="lifeCycleState">Новое значение свойства LifeCycleState.</param>
        /// <returns>Обновленная сущность.</returns>
        public static T UpdateLifeCycleState<T>(this T entity, string lifeCycleState) where T : IOfficialDocuments
        {
            if (!string.IsNullOrEmpty(lifeCycleState))
                entity = Client.Instance()
                                 .For<T>()
                                 .Key(entity)
                                 .Set(new { LifeCycleState = lifeCycleState })
                                 .UpdateEntryAsync().Result;

            return entity;
        }
    }
}
