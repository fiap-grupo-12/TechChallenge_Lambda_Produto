using AutoMapper;
using FIAP.TechChallenge.LambdaProduto.Application.Models.Response;
using FIAP.TechChallenge.LambdaProduto.Domain.Entities;
using FIAP.TechChallenge.LambdaProduto.Domain.Entities.Enum;

namespace FIAP.TechChallenge.LambdaProduto.Infra.Data.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Produto, ProdutoResponse>().ReverseMap();
        }
    }
}
