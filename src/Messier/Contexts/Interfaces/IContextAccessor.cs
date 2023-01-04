namespace Messier.Api;

public interface IContextAccessor
{
    IContext? Context { get; set; }
}