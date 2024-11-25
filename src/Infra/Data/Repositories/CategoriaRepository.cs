using FIAP.TechChallenge.LambdaProduto.Domain.Entities;
using FIAP.TechChallenge.LambdaProduto.Domain.Repositories;
using FIAP.TechChallenge.LambdaProduto.Infra.Data.Configurations;

namespace FIAP.TechChallenge.LambdaProduto.Infra.Data.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public IList<Categoria> GetAll()
        {
            try
            {
                return _context.Categorias.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao consultar categorias. {ex}");
            }
        }

        public Categoria GetByName(string nome)
        {
            try
            {
                return _context.Categorias.First(x => x.Nome == nome);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao consultar categoria. {ex}");
            }
        }
    }
}
