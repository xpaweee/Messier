using Messier.Interfaces;
using Messier.MessageBroker.RawRabbit.Base;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit.Instantiation;

namespace Messier.MessageBroker.RawRabbit;

public static class Extensions
{
    public const string _sectionName = "rabbitmq";
    
    public static IMessierBuilder AddRawRabbit(this IMessierBuilder builder)
    {
        var options =  builder.GetRabbitOptions();
        builder.ServiceCollection.AddSingleton(options);

        RawRabbitFactory.CreateInstanceFactory(new RawRabbitOptions()
        {
            DependencyInjection = x =>
            {
                
            }
        })

        return builder;
    }
    
    public static RabbitOptions GetRabbitOptions(this IMessierBuilder builder)
        => builder.GetOptions<RabbitOptions>(_sectionName);
}