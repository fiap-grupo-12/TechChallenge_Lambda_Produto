using FIAP.TechChallenge.LambdaProduto.Application.Models.Request;
using FIAP.TechChallenge.LambdaProduto.Application.Models.Response;

namespace FIAP.TechChallenge.LambdaProduto.Application.UseCases.Interfaces
{
    public interface IAtualizarProdutoUseCase : IUseCaseAsync<AtualizarProdutoRequest, bool>
    {
    }
}
