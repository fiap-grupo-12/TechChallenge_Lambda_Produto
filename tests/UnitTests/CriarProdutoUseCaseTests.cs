using FIAP.TechChallenge.LambdaProduto.Application.Models.Request;
using FIAP.TechChallenge.LambdaProduto.Application.UseCases;
using FIAP.TechChallenge.LambdaProduto.Domain.Entities;
using FIAP.TechChallenge.LambdaProduto.Domain.Repositories;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace FIAP.TechChallenge.LambdaProduto.Tests
{
    public class CriarProdutoUseCaseTests
    {
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
        private readonly Mock<ICategoriaRepository> _categoriaRepositoryMock;
        private readonly CriarProdutoUseCase _useCase;

        public CriarProdutoUseCaseTests()
        {
            _produtoRepositoryMock = new Mock<IProdutoRepository>();
            _categoriaRepositoryMock = new Mock<ICategoriaRepository>();
            _useCase = new CriarProdutoUseCase(_produtoRepositoryMock.Object, _categoriaRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldReturnFalse_WhenCategoriaNotFound()
        {
            // Arrange
            _categoriaRepositoryMock.Setup(repo => repo.GetByName(It.IsAny<string>())).Returns((Categoria)null);

            var request = new CriarProdutoRequest
            {
                Nome = "Produto Teste",
                Descricao = "Descricao Teste",
                Valor = 100,
                NomeCategoria = "Categoria Inexistente"
            };

            // Act
            var result = await _useCase.Execute(request);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Execute_ShouldReturnTrue_WhenProdutoCreatedSuccessfully()
        {
            // Arrange
            var categoria = new Categoria { Id = 1, Nome = "Categoria Existente" };
            _categoriaRepositoryMock.Setup(repo => repo.GetByName(It.IsAny<string>())).Returns(categoria);

            var request = new CriarProdutoRequest
            {
                Nome = "Produto Teste",
                Descricao = "Descricao Teste",
                Valor = 100,
                NomeCategoria = "Categoria Existente"
            };

            // Act
            var result = await _useCase.Execute(request);

            // Assert
            Assert.True(result);
            _produtoRepositoryMock.Verify(repo => repo.Post(It.IsAny<Produto>()), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldCallPost_WithCorrectProduto()
        {
            // Arrange
            var categoria = new Categoria { Id = 1, Nome = "Categoria Existente" };
            _categoriaRepositoryMock.Setup(repo => repo.GetByName(It.IsAny<string>())).Returns(categoria);

            var request = new CriarProdutoRequest
            {
                Nome = "Produto Teste",
                Descricao = "Descricao Teste",
                Valor = 100,
                NomeCategoria = "Categoria Existente"
            };

            // Act
            await _useCase.Execute(request);

            // Assert
            _produtoRepositoryMock.Verify(repo => repo.Post(It.Is<Produto>(p =>
                p.Nome == request.Nome &&
                p.Descricao == request.Descricao &&
                p.Valor == request.Valor &&
                p.CategoriaProduto == categoria
            )), Times.Once);
        }
    }
}