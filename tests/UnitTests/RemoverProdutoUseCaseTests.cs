using FIAP.TechChallenge.LambdaProduto.Application.UseCases;
using FIAP.TechChallenge.LambdaProduto.Domain.Entities;
using FIAP.TechChallenge.LambdaProduto.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.TechChallenge.LambdaProduto.UnitTests
{
    public class RemoverProdutoUseCaseTests
    {
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
        private readonly RemoverProdutoUseCase _useCase;

        public RemoverProdutoUseCaseTests()
        {
            _produtoRepositoryMock = new Mock<IProdutoRepository>();
            _useCase = new RemoverProdutoUseCase(_produtoRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldReturnFalse_WhenProdutoNotFound()
        {
            // Arrange
            _produtoRepositoryMock.Setup(repo => repo.GetById(It.IsAny<int>())).Returns((Produto)null);

            // Act
            var result = await _useCase.Execute(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Execute_ShouldReturnTrue_WhenProdutoDeletedSuccessfully()
        {
            // Arrange
            var produto = new Produto { Id = 1, Nome = "Produto Teste" };
            _produtoRepositoryMock.Setup(repo => repo.GetById(It.IsAny<int>())).Returns(produto);

            // Act
            var result = await _useCase.Execute(1);

            // Assert
            Assert.True(result);
            _produtoRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Produto>()), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldCallDelete_WithCorrectProduto()
        {
            // Arrange
            var produto = new Produto { Id = 1, Nome = "Produto Teste" };
            _produtoRepositoryMock.Setup(repo => repo.GetById(It.IsAny<int>())).Returns(produto);

            // Act
            await _useCase.Execute(1);

            // Assert
            _produtoRepositoryMock.Verify(repo => repo.Delete(It.Is<Produto>(p => p.Id == produto.Id && p.Nome == produto.Nome)), Times.Once);
        }
    }
}