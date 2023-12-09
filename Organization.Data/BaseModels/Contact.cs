using Organization.Data.Enums;

namespace Organization.Data.BaseModels;

public class Contact
{
    public int Kind { get; set; }
    public string Value { get; set; }

    public Contact()
    {
        
    }

    public Contact(int kind, string value)
    {
        Kind = kind;
        Value = value;
    }

    public Contact(EContactType kind, string value)
    {
        Kind = (int)kind;
        Value = value;
    }
}