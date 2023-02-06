using EasyNetQ;
using EasyNetQ.Consumer;
using Humanizer;
using Messier.Api;
using Messier.Messaging.Interfaces;
using Messier.Messaging.RabbitMQ.Base;
using Messier.Messaging.RabbitMQ.Conventions;
using Messier.Messaging.RabbitMQ.Factory;
using Messier.Messaging.RabbitMQ.Interfaces;
using Messier.Messaging.RabbitMQ.Registry;
using Messier.Messaging.RabbitMQ.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using IMessageHandler = Messier.Messaging.RabbitMQ.Interfaces.IMessageHandler;
using MessageHandler = Messier.Messaging.RabbitMQ.Handlers.MessageHandler;

namespace Messier.Messaging.RabbitMQ;

public static class Extensions
{ 
    public static IServiceCollection AddRabbitmq(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var section = configuration.GetSection("rabbitmq");
        var options = section.BindOptions<RabbitMQOptions>();

        if (!options.Enabled)
        {
            return serviceCollection;
        }

        var contextAccessor = new ContextAccessor();
        var messageContextAccessor = new MessageContextAccessor();
        var messageTypeRegistry = new MessageTypeRegistry();;
        
        // var bus = RabbitHutch.CreateBus(options.ConnectionString,services => services.Register<IConventions>(c => new CustomConventions2(c.Resolve<ITypeNameSerializer>()))); 
        var bus = RabbitHutch.CreateBus(options.ConnectionString, registerServices =>
        {
            registerServices.EnableNewtonsoftJson(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter(new CamelCaseNamingStrategy())
                },
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            registerServices.Register(typeof(IConventions), typeof(CustomConventions));
            registerServices.Register(typeof(IMessageSerializationStrategy), typeof(CustomMessageSerializationStrategy));
            registerServices.Register(typeof(IHandlerCollectionFactory), typeof(CustomHandlerCollectionFactory));
            registerServices.Register(typeof(IMessageTypeRegistry), messageTypeRegistry);
            registerServices.Register(typeof(IContextAccessor), contextAccessor);
            registerServices.Register(typeof(IMessageContextAccessor), messageContextAccessor);
        });
        
        serviceCollection.AddSingleton(bus);
        serviceCollection.AddSingleton<IMessageClient, RabbitmqBroker>();
        serviceCollection.AddSingleton<IMessageSubscriber, RabbitmqSubscriber>();
        serviceCollection.AddSingleton<IMessageHandler, MessageHandler>();
        serviceCollection.AddSingleton<IMessageTypeRegistry>(messageTypeRegistry);
        serviceCollection.AddSingleton<IContextAccessor>(contextAccessor);
        serviceCollection.AddSingleton<IMessageContextAccessor>(messageContextAccessor);
        
        serviceCollection.AddSingleton<IMessageTypeRegistry>(messageTypeRegistry);

        return serviceCollection;
    }
    
    internal static string ToMessageKey(this string messageType) => messageType.Underscore();
}