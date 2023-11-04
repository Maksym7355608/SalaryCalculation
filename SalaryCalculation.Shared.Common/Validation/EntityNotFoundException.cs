using System.Text;

namespace SalaryCalculation.Shared.Common.Validation;

public class EntityNotFoundException : Exception
{
    protected string _message = "Entity with id {0} was not found";
    public override string Message => string.Format(_message, MessageParams);
    public string[] MessageParams { get; private set; }

    public EntityNotFoundException(string id)
    {
        MessageParams = new[] { id };
    }
    
    public EntityNotFoundException(string message, params string[] messageParams)
    {
        MessageParams = messageParams;
        _message = message;
    }
    
}