namespace SalaryCalculation.Shared.Common.Validation;

public class EntityExistingException : Exception
{
    protected string _message = "Entity with id {0} was exist";
    public override string Message => string.Format(_message, MessageParams);
    public string[] MessageParams { get; private set; }

    public EntityExistingException(string id)
    {
        MessageParams = new[] { id };
    }
    
    public EntityExistingException(string message, params string[] messageParams)
    {
        MessageParams = messageParams;
        _message = message;
    }
}