using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using FIAP.TechChallenge.LambdaProduto.API.Extensions;
using FIAP.TechChallenge.LambdaProduto.Application.Models.Request;
using FIAP.TechChallenge.LambdaProduto.Application.UseCases.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FIAP.TechChallenge.LambdaProduto.API;

public class Function
{
    private readonly IObterProdutoPorCategoriaUseCase _obterProdutoPorCategoria;
    private readonly ICriarProdutoUseCase _criarProduto;
    private readonly IAtualizarProdutoUseCase _atualizarProduto;
    private readonly IRemoverProdutoUseCase _removerProduto;


    public Function() : this(
        ServiceProviderFactory.Create().GetRequiredService<IObterProdutoPorCategoriaUseCase>(),
        ServiceProviderFactory.Create().GetRequiredService<ICriarProdutoUseCase>(),
        ServiceProviderFactory.Create().GetRequiredService<IAtualizarProdutoUseCase>(),
        ServiceProviderFactory.Create().GetRequiredService<IRemoverProdutoUseCase>())
    {
    }


    public Function(
        IObterProdutoPorCategoriaUseCase obterProdutoPorCategoria,
        ICriarProdutoUseCase criarProduto,
        IAtualizarProdutoUseCase atualizarProduto,
        IRemoverProdutoUseCase removerProduto)
    {
        _obterProdutoPorCategoria = obterProdutoPorCategoria;
        _criarProduto = criarProduto;
        _atualizarProduto = atualizarProduto;
        _removerProduto = removerProduto;
    }

    public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogLine($"HTTP Method: {request.HttpMethod}");
        context.Logger.LogLine($"Path: {request.Path}");
        context.Logger.LogLine($"Body: {request.Body}");

        try
        {
            return (request.HttpMethod.ToUpper(), request.Path) switch
            {
                ("GET", var path) when path.StartsWith("/produto/") => HandleGet(request, context),
                ("POST", "/produto") => HandlePost(request, context),
                ("PUT", "/produto") => HandlePut(request, context),
                ("DELETE", var path) when path.StartsWith("/produto/") => HandleDelete(request, context),
                _ => BuildResponse(404, new { Message = "Endpoint " + request.HttpMethod.ToUpper() + " - " + request.Path + " n�o encontrado" })
            };
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"Erro: {ex.Message}");
            return BuildResponse(500, new { Message = "Erro interno do servidor." });
        }
    }

    private APIGatewayProxyResponse HandleGet(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var categoria = ExtractPathParameter(request.Path, "produto");

        if (int.TryParse(categoria, out int idCategoria))
        {
            var result = _obterProdutoPorCategoria.Execute(idCategoria);
            return BuildResponse(200, result);
        }
        else
        {
            return BuildResponse(400, new { Message = "Categoria inv�lida." });
        }

    }

    private APIGatewayProxyResponse HandlePost(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var criarProdutoRequest = JsonSerializer.Deserialize<CriarProdutoRequest>(request.Body);
        var result = _criarProduto.Execute(criarProdutoRequest).Result;
        return result
            ? BuildResponse(201, new { Message = "Produto criado com sucesso" })
            : BuildResponse(400, new { Message = "Erro ao criar produto." });
    }

    private APIGatewayProxyResponse HandlePut(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var atualizarProdutoRequest = JsonSerializer.Deserialize<AtualizarProdutoRequest>(request.Body);
        var result = _atualizarProduto.Execute(atualizarProdutoRequest).Result;
        return result
            ? BuildResponse(200, new { Message = "Produto atualizado com sucesso" })
            : BuildResponse(400, new { Message = "Erro ao atualizar produto." });
    }

    private APIGatewayProxyResponse HandleDelete(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var id = ExtractPathParameter(request.Path, "produto");
        var result = _removerProduto.Execute(int.Parse(id)).Result;
        return result
            ? BuildResponse(200, new { Message = "Produto removido com sucesso" })
            : BuildResponse(404, new { Message = "Produto n�o encontrado." });
    }

    private static string ExtractPathParameter(string path, string basePath)
    {
        return path.Replace($"/{basePath}/", "").Split('/').FirstOrDefault();
    }

    private static APIGatewayProxyResponse BuildResponse(int statusCode, object body)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = statusCode,
            Body = JsonSerializer.Serialize(body),
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
}
