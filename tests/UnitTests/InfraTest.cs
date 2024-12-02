using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using FIAP.TechChallenge.LambdaProduto.Domain.Entities;
using FIAP.TechChallenge.LambdaProduto.Infra.Data.Configurations;
using FIAP.TechChallenge.LambdaProduto.Infra.Data.Repositories;
using AutoMapper;

namespace FIAP.TechChallenge.LambdaProduto.UnitTests
{
    public class InfraTest
    {
        private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}") // Banco único por teste
                .Options;
        }

        [Fact]
        public void MapperConfig_ShouldMapEntitiesCorrectly()
        {
            // Arrange
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MapperConfig()));
            var mapper = config.CreateMapper();
            var categoria = new Categoria { Id = 1, Nome = "Teste Categoria" };

            // Act
            var categoriaDto = mapper.Map<Categoria>(categoria);

            // Assert
            Assert.Equal(categoria.Id, categoriaDto.Id);
            Assert.Equal(categoria.Nome, categoriaDto.Nome);
        }

        [Fact]
        public void ProdutoRepository_ShouldRetrieveProdutoById()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ApplicationDbContext(options);
            var repository = new ProdutoRepository(context);

            var categoria = new Categoria { Id = 1, Nome = "Lanche" };
            var produto = new Produto { Id = 1, Nome = "Teste Produto", CategoriaProduto = categoria };
            context.Categorias.Add(categoria);
            context.Produtos.Add(produto);
            context.SaveChanges();

            // Act
            var result = repository.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Teste Produto", result.Nome);
        }

        [Fact]
        public void ProdutoRepository_ShouldRetrieveAllProdutos()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ApplicationDbContext(options);
            var repository = new ProdutoRepository(context);

            context.Produtos.Add(new Produto { Id = 1, Nome = "Produto 1" });
            context.Produtos.Add(new Produto { Id = 2, Nome = "Produto 2" });
            context.SaveChanges();

            // Act
            var result = repository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void ProdutoRepository_ShouldRetrieveProdutosByCategoria()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ApplicationDbContext(options);
            var repository = new ProdutoRepository(context);

            var categoria = new Categoria { Id = 1, Nome = "Categoria Teste" };
            var produto = new Produto { Id = 1, Nome = "Produto Categoria", CategoriaProduto = categoria };
            context.Categorias.Add(categoria);
            context.Produtos.Add(produto);
            context.SaveChanges();

            // Act
            var result = repository.GetByCategoria(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Produto Categoria", result[0].Nome);
        }

        [Fact]
        public void ProdutoRepository_ShouldAddProduto()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ApplicationDbContext(options);
            var repository = new ProdutoRepository(context);

            var produto = new Produto { Id = 1, Nome = "Teste Produto" };

            // Act
            var result = repository.Post(produto).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Teste Produto", result.Nome);
        }

        [Fact]
        public void ProdutoRepository_ShouldUpdateProduto()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ApplicationDbContext(options);
            var repository = new ProdutoRepository(context);

            var produto = new Produto { Id = 1, Nome = "Produto Antigo" };
            context.Produtos.Add(produto);
            context.SaveChanges();

            // Act
            produto.Nome = "Produto Atualizado";
            var result = repository.Update(produto).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Produto Atualizado", result.Nome);
        }

        [Fact]
        public void ProdutoRepository_ShouldDeleteProduto()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ApplicationDbContext(options);
            var repository = new ProdutoRepository(context);

            var produto = new Produto { Id = 1, Nome = "Teste Produto" };
            context.Produtos.Add(produto);
            context.SaveChanges();

            // Act
            repository.Delete(produto).Wait();

            // Assert
            var result = context.Produtos.Find(produto.Id);
            Assert.Null(result);
        }
        [Fact]
        public void CategoriaRepository_ShouldRetrieveAllCategorias()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ApplicationDbContext(options);
            var repository = new CategoriaRepository(context);

            context.Categorias.Add(new Categoria { Id = 1, Nome = "Categoria 1" });
            context.Categorias.Add(new Categoria { Id = 2, Nome = "Categoria 2" });
            context.SaveChanges();

            // Act
            var result = repository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void CategoriaRepository_ShouldRetrieveCategoriaByName()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ApplicationDbContext(options);
            var repository = new CategoriaRepository(context);

            var categoria = new Categoria { Id = 1, Nome = "Categoria Teste" };
            context.Categorias.Add(categoria);
            context.SaveChanges();

            // Act
            var result = repository.GetByName("Categoria Teste");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Categoria Teste", result.Nome);
        }
        [Fact]
        public void CategoriaRepositoryInMemory_ShouldAddCategoria()
        {
            // Arrange
            var repository = new CategoriaRepositoryInMemory();

            var categoria = new Categoria { Nome = "Categoria Teste" };

            // Act
            var result = repository.Add(categoria);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Categoria Teste", result.Nome);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void CategoriaRepositoryInMemory_ShouldRetrieveAllCategorias()
        {
            // Arrange
            var repository = new CategoriaRepositoryInMemory();

            repository.Add(new Categoria { Nome = "Categoria 1" });
            repository.Add(new Categoria { Nome = "Categoria 2" });

            // Act
            var result = repository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void CategoriaRepositoryInMemory_ShouldRetrieveCategoriaByName()
        {
            // Arrange
            var repository = new CategoriaRepositoryInMemory();

            repository.Add(new Categoria { Nome = "Categoria Teste" });

            // Act
            var result = repository.GetByName("Categoria Teste");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Categoria Teste", result.Nome);
        }

        [Fact]
        public void CategoriaRepositoryInMemory_ShouldHandleCategoriaNotFound()
        {
            // Arrange
            var repository = new CategoriaRepositoryInMemory();

            // Act
            var result = repository.GetByName("Inexistente");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CategoriaRepositoryInMemory_ShouldHandleConcurrentAccess()
        {
            // Arrange
            var repository = new CategoriaRepositoryInMemory();

            // Act
            var categoria1 = repository.Add(new Categoria { Nome = "Categoria 1" });
            var categoria2 = repository.Add(new Categoria { Nome = "Categoria 2" });

            // Assert
            Assert.NotNull(categoria1);
            Assert.NotNull(categoria2);
            Assert.NotEqual(categoria1.Id, categoria2.Id);
        }
        [Fact]
        public async Task ProdutoRepositoryInMemory_ShouldAddProduto()
        {
            // Arrange
            var repository = new ProdutoRepositoryInMemory();

            var produto = new Produto { Nome = "Produto Teste", CategoriaProduto = new Categoria { Id = 1, Nome = "Categoria" } };

            // Act
            var result = await repository.Post(produto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Produto Teste", result.Nome);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void ProdutoRepositoryInMemory_ShouldRetrieveAllProdutos()
        {
            // Arrange
            var repository = new ProdutoRepositoryInMemory();

            repository.Post(new Produto { Nome = "Produto 1", CategoriaProduto = new Categoria { Id = 1, Nome = "Categoria 1" } }).Wait();
            repository.Post(new Produto { Nome = "Produto 2", CategoriaProduto = new Categoria { Id = 2, Nome = "Categoria 2" } }).Wait();

            // Act
            var result = repository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void ProdutoRepositoryInMemory_ShouldRetrieveProdutoById()
        {
            // Arrange
            var repository = new ProdutoRepositoryInMemory();

            var produto = repository.Post(new Produto { Nome = "Produto Teste", CategoriaProduto = new Categoria { Id = 1, Nome = "Categoria" } }).Result;

            // Act
            var result = repository.GetById(produto.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Produto Teste", result.Nome);
        }

        [Fact]
        public void ProdutoRepositoryInMemory_ShouldRetrieveProdutosByCategoria()
        {
            // Arrange
            var repository = new ProdutoRepositoryInMemory();

            repository.Post(new Produto { Nome = "Produto 1", CategoriaProduto = new Categoria { Id = 1, Nome = "Categoria 1" } }).Wait();
            repository.Post(new Produto { Nome = "Produto 2", CategoriaProduto = new Categoria { Id = 2, Nome = "Categoria 2" } }).Wait();
            repository.Post(new Produto { Nome = "Produto 3", CategoriaProduto = new Categoria { Id = 1, Nome = "Categoria 1" } }).Wait();

            // Act
            var result = repository.GetByCategoria(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task ProdutoRepositoryInMemory_ShouldUpdateProduto()
        {
            // Arrange
            var repository = new ProdutoRepositoryInMemory();

            var produto = await repository.Post(new Produto { Nome = "Produto Antigo", CategoriaProduto = new Categoria { Id = 1, Nome = "Categoria" } });

            // Act
            produto.Nome = "Produto Atualizado";
            var result = await repository.Update(produto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Produto Atualizado", result.Nome);
        }

        [Fact]
        public async Task ProdutoRepositoryInMemory_ShouldDeleteProduto()
        {
            // Arrange
            var repository = new ProdutoRepositoryInMemory();

            var produto = await repository.Post(new Produto { Nome = "Produto Teste", CategoriaProduto = new Categoria { Id = 1, Nome = "Categoria" } });

            // Act
            await repository.Delete(produto);
            var result = repository.GetById(produto.Id);

            // Assert
            Assert.Null(result);
        }

    }
}