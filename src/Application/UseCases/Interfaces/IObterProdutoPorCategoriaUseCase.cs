﻿using FIAP.TechChallenge.LambdaProduto.Application.Models.Response;

namespace FIAP.TechChallenge.LambdaProduto.Application.UseCases.Interfaces
{
    public interface IObterProdutoPorCategoriaUseCase : IUseCase<int, IList<ProdutoResponse>>
    {
    }
}
