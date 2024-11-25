using FIAP.TechChallenge.LambdaProduto.Application;
using FIAP.TechChallenge.LambdaProduto.Domain.Entities;
using FIAP.TechChallenge.LambdaProduto.Infra.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FIAP.TechChallenge.LambdaProduto.Infra.Data.Extensions
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ApplicationDbContext dbContext;

        public DatabaseInitializer(ApplicationDbContext context)
        {
            dbContext = context;
        }
        public void Initialize()
        {
            //dbContext.Database.Migrate();

            //if (!dbContext.Categorias.Any())
            //{
            //    dbContext.AddRange(
            //        new Categoria { Nome = "Lanche" },
            //        new Categoria { Nome = "Acompanhamento" },
            //        new Categoria { Nome = "Bebida" },
            //        new Categoria { Nome = "Sobremesa" }
            //        );

            //    dbContext.SaveChanges();
            //}
        }
    }
}
