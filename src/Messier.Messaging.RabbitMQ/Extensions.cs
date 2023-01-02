using EasyNetQ;
using Messier.Messaging.Interfaces;
using Messier.Messaging.RabbitMQ.Base;
using Messier.Messaging.RabbitMQ.Conventions;
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
        });
        
        serviceCollection.AddSingleton(bus);
        serviceCollection.AddSingleton<IMessageClient, RabbitmqClient>();
        serviceCollection.AddSingleton<IMessageSubscriber, RabbitmqSubscriber>();
        serviceCollection.AddSingleton<IMessageHandler, MessageHandler>();

        return serviceCollection;
    }
}