using AutoMapper;
using FIAP.TechChallenge.LambdaProduto.Application.UseCases.Interfaces;
using FIAP.TechChallenge.LambdaProduto.Application.UseCases;
using FIAP.TechChallenge.LambdaProduto.Domain.Repositories;
using FIAP.TechChallenge.LambdaProduto.Infra.Data.Repositories;
using FIAP.TechChallenge.LambdaProduto.Infra.Data.Configurations;
using FIAP.TechChallenge.LambdaProduto.Application;
using FIAP.TechChallenge.LambdaProduto.Infra.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

//Extensions
namespace FIAP.TechChallenge.LambdaProduto.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services)
        { 
            // AutoMapper
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperConfig());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Repository DB
            //services.AddTransient<ICategoriaRepository, CategoriaRepository>();
            //services.AddTransient<IProdutoRepository, ProdutoRepository>();

            // Repository in memory
            services.AddTransient<ICategoriaRepository, CategoriaRepositoryInMemory>();
            services.AddTransient<IProdutoRepository, ProdutoRepositoryInMemory>();

            // UseCase
            services.AddTransient<IAtualizarProdutoUseCase, AtualizarProdutoUseCase>();
            services.AddTransient<ICriarProdutoUseCase, CriarProdutoUseCase>();
            services.AddTransient<IObterProdutoPorCategoriaUseCase, ObterProdutoPorCategoriaUseCase>();
            services.AddTransient<IRemoverProdutoUseCase, RemoverProdutoUseCase>();

            // Infra Data
            var connectionString = Environment.GetEnvironmentVariable("SQLServerConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("A variável de ambiente 'SQLServerConnection' não está configurada.");
            }
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

            return services;
        }
    }
}
