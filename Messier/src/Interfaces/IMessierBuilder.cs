using Microsoft.Extensions.DependencyInjection;

namespace Messier.Interfaces;

public interface IMessierBuilder
{
     IServiceCollection ServiceCollection { get; }
}