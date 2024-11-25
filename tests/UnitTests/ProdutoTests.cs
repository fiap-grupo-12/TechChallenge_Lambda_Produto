using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.EntityFrameworkCore;
using FIAP.TechChallenge.LambdaProduto.Domain.Entities;
using FIAP.TechChallenge.LambdaProduto.Application.UseCases;
using FIAP.TechChallenge.LambdaProduto.Application.UseCases.Interfaces;
using FIAP.TechChallenge.LambdaProduto.Application.Models.Request;
using FIAP.TechChallenge.LambdaProduto.Application.Models.Response;
using FIAP.TechChallenge.LambdaProduto.Domain.Repositories;
using FIAP.TechChallenge.LambdaProduto.Infra.Data.Repositories;

namespace FIAP.TechChallenge.LambdaProduto.UnitTests
    {
        public class ProdutoTests
        {
            [Fact]
            public void CriarProduto_DeveExecutarComSucesso()
            {
                // Arrange
                var mockProdutoRepository = new Mock<IProdutoRepository>();
                var mockCategoriaRepository = new Mock<ICategoriaRepository>();
                var useCase = new CriarProdutoUseCase(mockProdutoRepository.Object, mockCategoriaRepository.Object);
                var request = new CriarProdutoRequest
                {
                    Nome = "Hamburguer",
                    Descricao = "Lanche com hamburger de 150 g",
                    Valor = 30.00,
                    NomeCategoria = "Lanche"
                };

                // Act
                var resultado = useCase.Execute(request);

                // Assert
                resultado.Should().NotBeNull();
                mockProdutoRepository.Verify(repo => repo.Post(It.IsAny<Produto>()), Times.Once);
            }

            [Fact]
            public void AtualizarProduto_DeveAtualizarComSucesso()
            {
                // Arrange
                var mockProdutoRepository = new Mock<IProdutoRepository>();
                var mockCategoriaRepository = new Mock<ICategoriaRepository>();

                var categoriaMock = new Categoria { Id = 1, Nome = "Lanches" };
                var produtoExistente = new Produto { Id = 1, Nome = "Original", Valor = 20.00, CategoriaProduto = categoriaMock };

                mockProdutoRepository.Setup(repo => repo.GetById(1)).Returns(produtoExistente);
                mockCategoriaRepository.Setup(repo => repo.GetByName("Lanches")).Returns(categoriaMock);

                var useCase = new AtualizarProdutoUseCase(mockProdutoRepository.Object, mockCategoriaRepository.Object);
                var request = new AtualizarProdutoRequest
                {
                    Id = 1,
                    Nome = "Hamburguer Atualizado",
                    Descricao = "Descrição Atualizada",
                    Valor = 30.00,
                    NomeCategoria = "Lanches"
                };

                // Act
                var resultado = useCase.Execute(request);

            // Assert
            //resultado.Should().Be;
            //    mockProdutoRepository.Verify(repo => repo.Update(It.Is<Produto>(p =>
            //        p.Nome == "Hamburguer Atualizado" &&
            //        p.Descricao == "Descrição Atualizada" &&
            //        p.Valor == 30.00 &&
            //        p.CategoriaProduto.Nome == "Lanches"
            //    )), Times.Once);
            }

        }
    }
}