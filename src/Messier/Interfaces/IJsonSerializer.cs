namespace Messier.Interfaces;

public interface IJsonSerializer
{
    public string Serialize<TType>(TType value);
    public TType? Deserialize<TType>(string value);
}