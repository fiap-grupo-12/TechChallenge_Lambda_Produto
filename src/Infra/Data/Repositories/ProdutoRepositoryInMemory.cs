using FIAP.TechChallenge.LambdaProduto.Domain.Entities;
using FIAP.TechChallenge.LambdaProduto.Domain.Repositories;
using System.Collections.Concurrent;

namespace FIAP.TechChallenge.LambdaProduto.Infra.Data.Repositories
{
    public class ProdutoRepositoryInMemory : IProdutoRepository
    {
        private readonly ConcurrentDictionary<int, Produto> _produtos;
        private int _nextId;

        public ProdutoRepositoryInMemory()
        {
            _produtos = new ConcurrentDictionary<int, Produto>();
            _nextId = 1;
        }

        public async Task Delete(Produto produto)
        {
            _produtos.TryRemove(produto.Id, out _);
            await Task.CompletedTask;
        }

        public Produto GetById(int Id)
        {
            _produtos.TryGetValue(Id, out Produto produto);
            return produto;
        }

        public IList<Produto> GetAll()
        {
            return _produtos.Values.ToList();
        }

        public IList<Produto> GetByCategoria(int IdCategoria)
        {
            return _produtos.Values.Where(x => x.CategoriaProduto.Id == IdCategoria).ToList();
        }

        public async Task<Produto> Post(Produto produto)
        {
            produto.Id = _nextId++;
            _produtos.TryAdd(produto.Id, produto);
            return await Task.FromResult(produto);
        }

        public async Task<Produto> Update(Produto produto)
        {
            if (_produtos.ContainsKey(produto.Id))
            {
                _produtos[produto.Id] = produto;
            }
            return await Task.FromResult(produto);
        }
    }
}
