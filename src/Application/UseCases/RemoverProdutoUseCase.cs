using FIAP.TechChallenge.LambdaProduto.Application.UseCases.Interfaces;
using FIAP.TechChallenge.LambdaProduto.Domain.Repositories;

namespace FIAP.TechChallenge.LambdaProduto.Application.UseCases
{
    public class RemoverProdutoUseCase : IRemoverProdutoUseCase
    {
        private readonly IProdutoRepository _produtoRepository;

        public RemoverProdutoUseCase(IProdutoRepository repository)
        {
            _produtoRepository = repository;
        }

        public async Task<bool> Execute(int id)
        {
            var produto = _produtoRepository.GetById(id);

            if(produto is null)
                return false;

            await _produtoRepository.Delete(produto);

            return true;
        }
    }
}
