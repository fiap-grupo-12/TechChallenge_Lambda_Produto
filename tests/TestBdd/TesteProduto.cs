using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using FIAP.TechChallenge.LambdaProduto.Application.Models.Request;
using FIAP.TechChallenge.LambdaProduto.API;
using FIAP.TechChallenge.LambdaProduto.Application.UseCases.Interfaces;
using Moq;
using TechTalk.SpecFlow;
using Xunit;

namespace FIAP.TechChallenge.LambdaProduto.TestBdd
{
    [Binding]
    public class TesteProduto
    {
        private readonly Mock<IObterProdutoPorCategoriaUseCase> _obterProdutoPorCategoriaMock;
        private readonly Mock<ICriarProdutoUseCase> _criarProdutoMock;
        private readonly Mock<IAtualizarProdutoUseCase> _atualizarProdutoMock;
        private readonly Mock<IRemoverProdutoUseCase> _removerProdutoMock;
        private readonly Function _function;
        private APIGatewayProxyRequest _request;
        private APIGatewayProxyResponse _response;
        private Exception _exception;

        public TesteProduto()
        {
            _obterProdutoPorCategoriaMock = new Mock<IObterProdutoPorCategoriaUseCase>();
            _criarProdutoMock = new Mock<ICriarProdutoUseCase>();
            _atualizarProdutoMock = new Mock<IAtualizarProdutoUseCase>();
            _removerProdutoMock = new Mock<IRemoverProdutoUseCase>();

            _function = new Function(
                _obterProdutoPorCategoriaMock.Object,
                _criarProdutoMock.Object,
                _atualizarProdutoMock.Object,
                _removerProdutoMock.Object
            );
        }

        [Fact]
        [Given(@"que eu tenho uma requisição válida para criar um produto")]
        public void DadoQueEuTenhoUmaRequisicaoValidaParaCriarUmProduto()
        {
            var criarProdutoRequest = new CriarProdutoRequest
            {
                Nome = "Hamburguer Especial",
                NomeCategoria = "Lanches",
                Descricao = "Delicioso",
                Valor = 25.00
            };
            _request = new APIGatewayProxyRequest
            {
                HttpMethod = "POST",
                Path = "/produto",
                Body = JsonSerializer.Serialize(criarProdutoRequest)
            };
        }

        [Fact]
        [When(@"eu envio a requisição para a Lambda de criação de produto")]
        public void QuandoEuEnvioARequisicaoParaALambdaDeCriacaoDeProduto()
        {
            try
            {
                _criarProdutoMock.Setup(useCase => useCase.Execute(It.IsAny<CriarProdutoRequest>())).ReturnsAsync(true);
                _response = _function.FunctionHandler(_request, new TestLambdaContext());
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        //[Fact]
        //[Then(@"o produto deve ser criado com sucesso na Lambda")]
        //public void EntaoOProdutoDeveSerCriadoComSucessoNaLambda()
        //{
        //    Assert.NotNull(_response);
        //    Assert.Equal(201, _response.StatusCode);
        //    Assert.Contains("Produto criado com sucesso", _response.Body);
        //}

        [Fact]
        [Given(@"que eu tenho uma requisição sem preço para criar um produto")]
        public void DadoQueEuTenhoUmaRequisicaoSemPrecoParaCriarUmProduto()
        {
            var criarProdutoRequest = new CriarProdutoRequest
            {
                Nome = "Hamburguer Sem Preço",
                NomeCategoria = "Lanches",
                Descricao = "Delicioso",
                Valor = 0
            };
            _request = new APIGatewayProxyRequest
            {
                HttpMethod = "POST",
                Path = "/produto",
                Body = JsonSerializer.Serialize(criarProdutoRequest)
            };
        }

        //[Fact]
        //[Then(@"uma exceção deve ser lançada indicando que o preço é obrigatório na Lambda")]
        //public void EntaoUmaExcecaoDeveSerLancadaIndicandoQueOPrecoEObrigatorioNaLambda()
        //{
        //    Assert.NotNull(_response);
        //    Assert.Equal(400, _response.StatusCode);
        //    Assert.Contains("Erro ao criar produto.", _response.Body);
        //}

        [Fact]
        [Given(@"que eu tenho uma requisição válida para atualizar um produto existente")]
        public void DadoQueEuTenhoUmaRequisicaoValidaParaAtualizarUmProdutoExistente()
        {
            var atualizarProdutoRequest = new AtualizarProdutoRequest
            {
                Id = 1,
                Nome = "Hamburguer Clássico Atualizado",
                NomeCategoria = "Lanches",
                Descricao = "Delicioso",
                Valor = 20.00
            };
            _request = new APIGatewayProxyRequest
            {
                HttpMethod = "PUT",
                Path = "/produto",
                Body = JsonSerializer.Serialize(atualizarProdutoRequest)
            };
        }
        [Fact]
        [When(@"eu envio a requisição para a Lambda de atualização de produto")]
        public void QuandoEuEnvioARequisicaoParaALambdaDeAtualizacaoDeProduto()
        {
            try
            {
                _atualizarProdutoMock.Setup(useCase => useCase.Execute(It.IsAny<AtualizarProdutoRequest>())).ReturnsAsync(true);
                _response = _function.FunctionHandler(_request, new TestLambdaContext());
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }
        //[Fact]
        //[Then(@"o produto deve ser atualizado com sucesso na Lambda")]
        //public void EntaoOProdutoDeveSerAtualizadoComSucessoNaLambda()
        //{
        //    Assert.NotNull(_response);
        //    Assert.Equal(200, _response.StatusCode);
        //    Assert.Contains("Produto atualizado com sucesso", _response.Body);
        //}
    }

    public class TestLambdaContext : ILambdaContext
    {
        public string AwsRequestId => Guid.NewGuid().ToString();
        public IClientContext ClientContext => null;
        public string FunctionName => "TestFunction";
        public string FunctionVersion => "1.0";
        public ICognitoIdentity Identity => null;
        public string InvokedFunctionArn => "arn:aws:lambda:region:account-id:function:TestFunction";
        public ILambdaLogger Logger => new TestLambdaLogger();
        public string LogGroupName => "/aws/lambda/TestFunction";
        public string LogStreamName => "2024/11/30/[$LATEST]abcdef123456";
        public int MemoryLimitInMB => 512;
        public TimeSpan RemainingTime => TimeSpan.FromMinutes(1);
    }

    public class TestLambdaLogger : ILambdaLogger
    {
        public void Log(string message) => Console.WriteLine(message);
        public void LogLine(string message) => Console.WriteLine(message);
    }
}