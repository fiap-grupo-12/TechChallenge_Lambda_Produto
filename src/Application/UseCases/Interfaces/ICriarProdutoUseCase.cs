using FIAP.TechChallenge.LambdaProduto.Application.Models.Request;

namespace FIAP.TechChallenge.LambdaProduto.Application.UseCases.Interfaces
{
    public interface ICriarProdutoUseCase : IUseCaseAsync<CriarProdutoRequest, bool>
    {
    }
}
