
using System;
using System.Threading.Tasks;
using FIAP.TechChallenge.LambdaProduto.Application.UseCases.Interfaces;
using FIAP.TechChallenge.LambdaProduto.Domain.Entities;
//using Microsoft.AspNetCore.Mvc;
using Moq;
//using TechTalk.SpecFlow;
using Xunit;

namespace FIAP.TechChallenge.LambdaProduto.TestBdd
{
    public class TesteProduto
    {
        private readonly Mock<ICriarProdutoUseCase> _produtoUseCaseMock;
        private readonly ProdutoController _produtoController;
        private Produto _produto;
        private Exception _exception;
        private IActionResult _resultado;

        public ProdutoStepDefinitions()
        {
            _produtoUseCaseMock = new Mock<ICriarProdutoUseCase>();
            _produtoController = new ProdutoController(_produtoUseCaseMock.Object);
        }

        [Given(@"que eu tenho um produto válido para criar")]
        public void DadoQueEuTenhoUmProdutoValidoParaCriar()
        {
            _produto = new ProdutoModel { Nome = "Hamburguer Especial", Categoria = "Lanches", Descricao = "Delicioso", Preco = 25.00 };
        }

        [When(@"eu envio a solicitação de criação do produto")]
        public async Task QuandoEuEnvioASolicitacaoDeCriacaoDoProduto()
        {
            try
            {
                _produtoUseCaseMock.Setup(useCase => useCase.CriarProdutoAsync(It.IsAny<ProdutoModel>())).ReturnsAsync(_produto);
                _resultado = await _produtoController.CriarProduto(_produto);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [Then(@"o produto deve ser criado com sucesso")]
        public void EntaoOProdutoDeveSerCriadoComSucesso()
        {
            Assert.NotNull(_resultado);
            var createdResult = Assert.IsType<CreatedAtActionResult>(_resultado);
            var createdProduto = Assert.IsType<ProdutoModel>(createdResult.Value);
            Assert.Equal("Hamburguer Especial", createdProduto.Nome);
        }

        [Given(@"que eu tenho um produto sem preço para criar")]
        public void DadoQueEuTenhoUmProdutoSemPrecoParaCriar()
        {
            _produto = new ProdutoModel { Nome = "Hamburguer Sem Preço", Categoria = "Lanches", Descricao = "Delicioso", Preco = 0 };
        }

        [Then(@"uma exceção deve ser lançada indicando que o preço é obrigatório")]
        public void EntaoUmaExcecaoDeveSerLancadaIndicandoQueOPrecoEObrigatorio()
        {
            Assert.NotNull(_exception);
            Assert.IsType<ArgumentException>(_exception);
            Assert.Equal("Preço é obrigatório", _exception.Message);
        }

        [Given(@"que eu tenho um produto existente para atualizar")]
        public void DadoQueEuTenhoUmProdutoExistenteParaAtualizar()
        {
            _produto = new ProdutoModel { Id = 1, Nome = "Hamburguer Clássico", Categoria = "Lanches", Descricao = "Delicioso", Preco = 18.00 };
        }

        [When(@"eu envio a solicitação de atualização do produto")]
        public async Task QuandoEuEnvioASolicitacaoDeAtualizacaoDoProduto()
        {
            try
            {
                _produtoUseCaseMock.Setup(useCase => useCase.AtualizarProdutoAsync(It.IsAny<int>(), It.IsAny<ProdutoModel>())).ReturnsAsync(_produto);
                _resultado = await _produtoController.AtualizarProduto(_produto.Id, _produto);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [Then(@"o produto deve ser atualizado com sucesso")]
        public void EntaoOProdutoDeveSerAtualizadoComSucesso()
        {
            Assert.NotNull(_resultado);
            var okResult = Assert.IsType<OkObjectResult>(_resultado);
            var updatedProduto = Assert.IsType<ProdutoModel>(okResult.Value);
            Assert.Equal(18.00, updatedProduto.Preco);
        }
    }
}