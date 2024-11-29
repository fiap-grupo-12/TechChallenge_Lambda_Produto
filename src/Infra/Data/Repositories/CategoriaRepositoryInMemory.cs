using FIAP.TechChallenge.LambdaProduto.Domain.Entities;
using FIAP.TechChallenge.LambdaProduto.Domain.Repositories;
using System.Collections.Concurrent;

namespace FIAP.TechChallenge.LambdaProduto.Infra.Data.Repositories
{
    public class CategoriaRepositoryInMemory : ICategoriaRepository
    {
        private readonly ConcurrentDictionary<int, Categoria> _categorias;
        private int _nextId;

        public CategoriaRepositoryInMemory()
        {
            _categorias = new ConcurrentDictionary<int, Categoria>();
            _nextId = 1;
        }

        public IList<Categoria> GetAll()
        {
            return _categorias.Values.ToList();
        }

        public Categoria GetByName(string nome)
        {
            return _categorias.Values.FirstOrDefault(x => x.Nome == nome);
        }

        public Categoria Add(Categoria categoria)
        {
            categoria.Id = _nextId++;
            _categorias.TryAdd(categoria.Id, categoria);
            return categoria;
        }
    }
}
