using Messier.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Messier;

public class MessierBuilder : IMessierBuilder
{
    public IServiceCollection ServiceCollection { get; }
     

    
    internal MessierBuilder(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }


    
}