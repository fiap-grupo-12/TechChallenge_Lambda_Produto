namespace FIAP.TechChallenge.LambdaProduto.Domain.Entities
{
    public class Produto : EntityBase
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public double Valor { get; set; }//{ get => String.Format("{0:0.00}", _valor); set => _valor = Convert.ToDouble(value); }
        public Categoria CategoriaProduto { get; set; }
    }
}
