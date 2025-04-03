using ImportData.IntegrationServicesClient.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.Entities.Databooks
{
  public class DocumentRegister : Entity
  {
    public override int PropertiesCount { get { return 8; } }
    protected override Type EntityType { get { return typeof(IDocumentRegisters); } }
    
    // Количество по умолчанию в коробке RX.
    const int DefaultNumberOfDigitsInNumber = 3;

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.RegisterType] = BusinessLogic.GetDocumentRegisterType((string)ResultValues[Constants.KeyAttributes.RegisterType]);
      ResultValues[Constants.KeyAttributes.DocumentFlow] = BusinessLogic.GetDocumentFlow((string)ResultValues[Constants.KeyAttributes.DocumentFlow]);
      ResultValues[Constants.KeyAttributes.NumberingSection] = BusinessLogic.GetNumberingSection((string)ResultValues[Constants.KeyAttributes.NumberingSection]);
      ResultValues[Constants.KeyAttributes.NumberingPeriod] = BusinessLogic.GetNumberingPeriod((string)ResultValues[Constants.KeyAttributes.NumberingPeriod]);
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];

      // Если число введено некорректно, то выставим значение по умолчанию.
      ResultValues[Constants.KeyAttributes.NumberOfDigitsInNumber] = int.TryParse((string)ResultValues[Constants.KeyAttributes.NumberOfDigitsInNumber], out int numberOfDigitsInNumber) ? 
        numberOfDigitsInNumber : 
        DefaultNumberOfDigitsInNumber;

      // Поле Группа регистрации обязательно для заполнения, если тип журнала - Регистрация.
      if ((string)ResultValues[Constants.KeyAttributes.RegisterType] == Constants.AttributeValue[Constants.KeyAttributes.RegisterType] &&
           ResultValues[Constants.KeyAttributes.RegistrationGroup] == null)
      {
        BusinessLogic.GetErrorResult(exceptionList, logger, Constants.Resources.EmptyColumn, Constants.Resources.RegistrationGroupName);
        return true;
      }

      return false;
    }
  }
}
