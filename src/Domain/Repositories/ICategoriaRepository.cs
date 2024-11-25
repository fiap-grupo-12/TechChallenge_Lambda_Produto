using FIAP.TechChallenge.LambdaProduto.Domain.Entities;

namespace FIAP.TechChallenge.LambdaProduto.Domain.Repositories
{
    public interface ICategoriaRepository
    {
        IList<Categoria> GetAll();
        Categoria GetByName(string nome);
    }
}
