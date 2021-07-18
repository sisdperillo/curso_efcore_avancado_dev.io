using System;
using Curso.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Curso.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "Data source=(localdb)\\mssqllocaldb; Initial Catalog=C002;Integrated Security=true;pooling=true;";

            optionsBuilder
                .UseSqlServer(strConnection)
                .EnableSensitiveDataLogging()
                .UseLazyLoadingProxies() //Ativando o proxy global para toda a aplicação
                .LogTo(Console.WriteLine, LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Filtro global de Marca, somente marcas ativas serão retornadas
            //modelBuilder.Entity<Marca>().HasQueryFilter(x => x.Ativo);
            //modelBuilder.Entity<Veiculo>().HasQueryFilter(x => x.Ativo);
        }

    }
}