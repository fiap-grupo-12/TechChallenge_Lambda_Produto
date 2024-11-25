using AutoMapper;
using FIAP.TechChallenge.LambdaProduto.Application.Models.Response;
using FIAP.TechChallenge.LambdaProduto.Application.UseCases.Interfaces;
using FIAP.TechChallenge.LambdaProduto.Domain.Repositories;

namespace FIAP.TechChallenge.LambdaProduto.Application.UseCases
{
    public class ObterProdutoPorCategoriaUseCase : IObterProdutoPorCategoriaUseCase
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;

        public ObterProdutoPorCategoriaUseCase(IProdutoRepository repository, ICategoriaRepository categoriaRepository, IMapper mapper)
        {
            _produtoRepository = repository;
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
        }

        public IList<ProdutoResponse> Execute(int request)
        {
            var result = new List<ProdutoResponse>();
            //var categoria = _categoriaRepository.GetByName(request);

            //if (categoria is null)
            //    return result;

            var produtos = _produtoRepository.GetByCategoria(request);

            return _mapper.Map<IList<ProdutoResponse>>(produtos);
        }
    }
}