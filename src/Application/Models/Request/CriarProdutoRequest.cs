using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FIAP.TechChallenge.LambdaProduto.Application.Models.Request
{
    public class CriarProdutoRequest
    {

        [Required(ErrorMessage = "É obrigatório informar o Nome.")]
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "É obrigatório informar a Descricao.")]
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "É obrigatório informar o Valor.")]
        [JsonPropertyName("valor")]
        public double Valor { get; set; }//{ get => String.Format("{0:0.00}", _valor); set => _valor = Convert.ToDouble(value); }

        [Required(ErrorMessage = "É obrigatório informar o nome da Categoria.")]
        [JsonPropertyName("nomeCategoria")]
        public string NomeCategoria { get; set; }
    }
}
