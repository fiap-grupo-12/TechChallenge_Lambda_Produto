using System.ComponentModel.DataAnnotations;

namespace FIAP.TechChallenge.LambdaProduto.Domain.Entities
{
    public abstract class EntityBase
    {
        [Key]
        public int Id { get; set; }
    }
}
