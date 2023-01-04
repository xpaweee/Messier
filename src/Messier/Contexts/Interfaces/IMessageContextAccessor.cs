namespace Messier.Api;

public interface IMessageContextAccessor
{
    MessageContext? MessageContext { get; set; }
}