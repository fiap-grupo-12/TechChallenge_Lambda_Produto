using FIAP.TechChallenge.LambdaProduto.Application.Models.Request;
using FIAP.TechChallenge.LambdaProduto.Application.UseCases;
using FIAP.TechChallenge.LambdaProduto.Domain.Entities;
using FIAP.TechChallenge.LambdaProduto.Domain.Repositories;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace FIAP.TechChallenge.LambdaProduto.Tests
{
    public class AtualizarProdutoUseCaseTests
    {
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
        private readonly Mock<ICategoriaRepository> _categoriaRepositoryMock;
        private readonly AtualizarProdutoUseCase _useCase;

        public AtualizarProdutoUseCaseTests()
        {
            _produtoRepositoryMock = new Mock<IProdutoRepository>();
            _categoriaRepositoryMock = new Mock<ICategoriaRepository>();
            _useCase = new AtualizarProdutoUseCase(_produtoRepositoryMock.Object, _categoriaRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldReturnFalse_WhenCategoriaNotFound()
        {
            // Arrange
            _categoriaRepositoryMock.Setup(repo => repo.GetByName(It.IsAny<string>())).Returns((Categoria)null);

            var request = new AtualizarProdutoRequest
            {
                Id = 1,
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
        public async Task Execute_ShouldReturnTrue_WhenProdutoUpdatedSuccessfully()
        {
            // Arrange
            var categoria = new Categoria { Id = 1, Nome = "Categoria Existente" };
            _categoriaRepositoryMock.Setup(repo => repo.GetByName(It.IsAny<string>())).Returns(categoria);

            var request = new AtualizarProdutoRequest
            {
                Id = 1,
                Nome = "Produto Teste",
                Descricao = "Descricao Teste",
                Valor = 100,
                NomeCategoria = "Categoria Existente"
            };

            // Act
            var result = await _useCase.Execute(request);

            // Assert
            Assert.True(result);
            _produtoRepositoryMock.Verify(repo => repo.Update(It.IsAny<Produto>()), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldCallUpdate_WithCorrectProduto()
        {
            // Arrange
            var categoria = new Categoria { Id = 1, Nome = "Categoria Existente" };
            _categoriaRepositoryMock.Setup(repo => repo.GetByName(It.IsAny<string>())).Returns(categoria);

            var request = new AtualizarProdutoRequest
            {
                Id = 1,
                Nome = "Produto Teste",
                Descricao = "Descricao Teste",
                Valor = 100,
                NomeCategoria = "Categoria Existente"
            };

            // Act
            await _useCase.Execute(request);

            // Assert
            _produtoRepositoryMock.Verify(repo => repo.Update(It.Is<Produto>(p =>
                p.Id == request.Id &&
                p.Nome == request.Nome &&
                p.Descricao == request.Descricao &&
                p.Valor == request.Valor &&
                p.CategoriaProduto == categoria
            )), Times.Once);
        }
    }
}
