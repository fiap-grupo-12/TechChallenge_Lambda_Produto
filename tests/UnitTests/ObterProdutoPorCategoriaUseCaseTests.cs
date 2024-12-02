using AutoMapper;
using FIAP.TechChallenge.LambdaProduto.Application.Models.Request;
using FIAP.TechChallenge.LambdaProduto.Application.Models.Response;
using FIAP.TechChallenge.LambdaProduto.Application.UseCases;
using FIAP.TechChallenge.LambdaProduto.Domain.Entities;
using FIAP.TechChallenge.LambdaProduto.Domain.Repositories;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace FIAP.TechChallenge.LambdaProduto.UnitTests
{
    public class ObterProdutoPorCategoriaUseCaseTests
    {
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
        private readonly Mock<ICategoriaRepository> _categoriaRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ObterProdutoPorCategoriaUseCase _useCase;

        public ObterProdutoPorCategoriaUseCaseTests()
        {
            _produtoRepositoryMock = new Mock<IProdutoRepository>();
            _categoriaRepositoryMock = new Mock<ICategoriaRepository>();
            _mapperMock = new Mock<IMapper>();
            _useCase = new ObterProdutoPorCategoriaUseCase(_produtoRepositoryMock.Object, _categoriaRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void Execute_ShouldReturnMappedProdutos_WhenCategoriaExists()
        {
            // Arrange
            var produtos = new List<Produto> { new Produto { Id = 1, Nome = "Produto 1", Descricao = "Descricao 1", Valor = 50, CategoriaProduto = new Categoria { Id = 1, Nome = "Lanche" } }, new Produto { Id = 2, Nome = "Produto 2", Descricao = "Descricao 2", Valor = 100, CategoriaProduto = new Categoria { Id = 1, Nome = "Categoria Existente" } } };
            _produtoRepositoryMock.Setup(repo => repo.GetByCategoria(It.IsAny<int>())).Returns(produtos);

            var mappedProdutos = new List<ProdutoResponse> {
                new ProdutoResponse { Nome = "Produto 1", Descricao = "Descricao 1", Valor = 50, CategoriaProduto = produtos[0].CategoriaProduto },
                new ProdutoResponse { Nome = "Produto 2", Descricao = "Descricao 2", Valor = 100, CategoriaProduto = produtos[1].CategoriaProduto }
            };
            _mapperMock.Setup(mapper => mapper.Map<IList<ProdutoResponse>>(produtos)).Returns(mappedProdutos);

            // Act
            var result = _useCase.Execute("Lanche");

            // Assert
            Assert.Equal(mappedProdutos, result);
        }

        [Fact]
        public void Execute_ShouldReturnEmptyList_WhenNoProdutosFound()
        {
            // Arrange
            _produtoRepositoryMock.Setup(repo => repo.GetByCategoria(It.IsAny<int>())).Returns(new List<Produto>());

            _mapperMock.Setup(mapper => mapper.Map<IList<ProdutoResponse>>(It.IsAny<IList<Produto>>())).Returns(new List<ProdutoResponse>());

            // Act
            var result = _useCase.Execute("Lanche");

            // Assert
            Assert.Empty(result);
        }
    }
}
