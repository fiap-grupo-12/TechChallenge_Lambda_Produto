using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FIAP.TechChallenge.LambdaProduto.Application.Models.Request
{
    public class ProdutoQuantidade
    {
        [JsonPropertyName("idProduto")]
        public int IdProduto { get; set; }
        [JsonPropertyName("quantidade")]
        public int Quantidade { get; set; }
    }
}
