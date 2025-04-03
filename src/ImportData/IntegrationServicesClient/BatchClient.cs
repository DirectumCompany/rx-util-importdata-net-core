using DocumentFormat.OpenXml.Office2010.Excel;
using ImportData.IntegrationServicesClient.Models;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImportData.IntegrationServicesClient
{
  public static class BatchClient
  {
    private static ODataClient client;

    private static ODataBatch batch;

    public const int MaxReqestsPerBatchConstraint = 200;

    public static int MaxRequests { get; private set; }

    private static int queuedRequestsCount;

    private static int idCounter = -1;

    public static int AvailableRequests => MaxRequests - queuedRequestsCount;

    public static void Setup(ODataClient client, int maxRequestsCount)
    {
      if (maxRequestsCount > MaxReqestsPerBatchConstraint)
        throw new ArgumentException("Количество запросов в одном batch-запросе не может быть больше 100");
      BatchClient.client = client;
      BatchClient.batch = new ODataBatch(client);
      MaxRequests = maxRequestsCount;
    }

    public static T CreateEntity<T>(T entity) where T : IEntityBase
    {
      entity.Id = idCounter--;
      batch += odata => odata.For<T>().Set(entity).InsertEntryAsync();
      queuedRequestsCount++;
      return entity;
    }

    public static IElectronicDocumentVersionss CreateVersionBatch<T>(T document, string nameVersion, IAssociatedApplications extention) where T : IElectronicDocuments
    {
      var version = new IElectronicDocumentVersionss() { Id = idCounter--, Number = 1, Note = nameVersion, Created = DateTime.Now, AssociatedApplication = extention };
      batch += odata => odata.For<T>()
      .Key(document)
      .NavigateTo(x => x.Versions)
      .Set(new { Id = version.Id, Number = 1, Note = nameVersion, Created = DateTime.Now, AssociatedApplication = new { Id = extention.Id } })
      .InsertEntryAsync();
      queuedRequestsCount++;
      return version;
    }

    public static void FillBody(IElectronicDocuments edoc, IElectronicDocumentVersionss version, string content = "")
    {
      byte[] encodingContent;

      if (version.Body.Value != null)
        encodingContent = version.Body.Value;
      else
        encodingContent = Convert.FromBase64String(Convert.ToBase64String(Encoding.Default.GetBytes(content)));

      batch += odata => odata.For<IElectronicDocuments>()
        .Key(edoc)
        .NavigateTo(x => x.Versions)
        .Key(version)
        .NavigateTo(v => v.Body)
        .Set(new { Value = encodingContent })
        .InsertEntryAsync();
      queuedRequestsCount++;
    }

    public static void AddRequest(Func<IODataClient, Task> action)
    {
      batch += action;
      queuedRequestsCount++;
    }

    public static void Execute()
    {
      try
      {
        batch.ExecuteAsync().Wait();
      }
      catch (Exception ex)
      {
        throw;
      }
      finally
      {
        batch = new ODataBatch(client);
        queuedRequestsCount = 0;
        idCounter = -1;
      }
    }
  }
}
