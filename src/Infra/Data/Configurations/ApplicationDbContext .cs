using FIAP.TechChallenge.LambdaProduto.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FIAP.TechChallenge.LambdaProduto.Infra.Data.Configurations
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Produto> Produtos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categoria>()
                .Property(x => x.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Produto>()
                .Property(x => x.Id).ValueGeneratedOnAdd();

            base.OnModelCreating(modelBuilder);
        }


    }


}
