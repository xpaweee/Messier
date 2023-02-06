using Messier.Api;
using Messier.Interfaces;
using Messier.Messaging.Interfaces;

namespace Messier.Messaging;

public sealed class MessageBroker : IMessageBroker
{
    private readonly IMessageClient _client;
    private readonly IContextProvider _contextProvider;

    public MessageBroker(IMessageClient client, IContextProvider contextProvider)
    {
        _client = client;
        _contextProvider = contextProvider;
    }
    public async Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : IMessage
    {
        await _client.SendAsync(new MessageWrapper<TMessage>(message,new MessageContext(Guid.NewGuid().ToString("N"), _contextProvider.Current())), cancellationToken);
    }
}