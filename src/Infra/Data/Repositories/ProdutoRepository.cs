using FIAP.TechChallenge.LambdaProduto.Domain.Entities;
using FIAP.TechChallenge.LambdaProduto.Domain.Repositories;
using FIAP.TechChallenge.LambdaProduto.Infra.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FIAP.TechChallenge.LambdaProduto.Infra.Data.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProdutoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Delete(Produto produto)
        {
            try
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar cliente. {ex}");
            }
        }

        public Produto GetById(int Id)
        {
            try
            {
                return _context.Produtos
                    .Include(x => x.CategoriaProduto)
                    .FirstOrDefault(x => x.Id == Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao consultar produto. {ex}");
            }
        }

        public IList<Produto> GetAll()
        {
            try
            {
                return _context.Produtos
                    .Include(x => x.CategoriaProduto).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao consultar produtos. {ex}");
            }
        }

        public IList<Produto> GetByCategoria(int IdCategoria)
        {
            try
            {
                return _context.Produtos
                    .Include(x => x.CategoriaProduto).Where(x => x.CategoriaProduto.Id == IdCategoria).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao consultar produtos. {ex}");
            }
        }

        public async Task<Produto> Post(Produto produto)
        {
            try
            {
                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync();
                return produto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar cliente. {ex}");
            }
        }

        public async Task<Produto> Update(Produto produto)
        {
            try
            {
                var trackedEntity = _context.Produtos.Local.FirstOrDefault(x => x.Id == produto.Id);
                if (trackedEntity != null)
                {
                    _context.Entry(trackedEntity).State = EntityState.Detached;
                }

                _context.Produtos.Update(produto);
                await _context.SaveChangesAsync();
                return produto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar cliente. {ex}");
            }
        }
    }
}
