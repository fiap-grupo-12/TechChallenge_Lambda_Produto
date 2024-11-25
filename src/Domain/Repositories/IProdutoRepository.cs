using FIAP.TechChallenge.LambdaProduto.Domain.Entities;

namespace FIAP.TechChallenge.LambdaProduto.Domain.Repositories
{
    public interface IProdutoRepository
    {
        Produto GetById(int Id);
        Task Delete(Produto produto);
        Task<Produto> Post(Produto produto);
        Task<Produto> Update(Produto produto);
        IList<Produto> GetAll();
        IList<Produto> GetByCategoria(int IdCategoria);
    }
}
