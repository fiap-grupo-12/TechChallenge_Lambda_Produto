using FIAP.TechChallenge.LambdaProduto.Application;
using FIAP.TechChallenge.LambdaProduto.Application.UseCases.Interfaces;
using FIAP.TechChallenge.LambdaProduto.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace FIAP.TechChallenge.LambdaProduto.API.Extensions;

public static class ServiceProviderFactory
{
    private static ServiceProvider? _serviceProvider;

    public static ServiceProvider Create()
    {
        if (_serviceProvider == null)
        {
            var serviceCollection = new ServiceCollection();

            // Usar o método de extensão para configurar as dependências
            serviceCollection.AddProjectDependencies();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        return _serviceProvider;
    }
}