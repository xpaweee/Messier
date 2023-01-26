using Messier.Api;
using Messier.Interfaces;

namespace Messier.Messaging;

public record MessageWrapper<T>(T Message, MessageContext Context) where T : IMessage;