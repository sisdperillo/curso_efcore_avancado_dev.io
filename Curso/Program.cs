using System;
using System.Linq;
using Curso.Data;
using Curso.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DominandoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //EnseureCreatedAndDeleted();
            //GapEnsureCreated();
            //HealthCheckDatabase();

            // _count = 0;
            // GerenciarEstadoDaConexao(false);
            // _count = 0;
            // GerenciarEstadoDaConexao(true);

            //SqlInjection();

            //MigracoesPendentes();

            //AplicarMigracaoEmTempodeExecucao();
        }

        /// <summary>
        /// Alternativa a criação de migrations
        /// Método que cria ou deleta a base de dados
        /// </summary>
        static void EnseureCreatedAndDeleted()
        {
            using var db = new ApplicationContext();

            //Cria o banco de dados
            db.Database.EnsureCreated();

            //Se existir o banco de dados ele excluirá toda a base
            //db.Database.EnsureDeleted();
        }

        /// <summary>
        /// Quando existe mais de um contexto pra msm base o EnsureCreated não funciona
        /// como deveria ou seja não cria a tabela do segundo contexto este método dispoe de uma 
        /// alternativa para esse fluxo
        /// </summary>
        static void GapEnsureCreated()
        {
            using var db1 = new ApplicationContext();
            using var db2 = new ApplicationContextCidade();

            db1.Database.EnsureCreated();
            db2.Database.EnsureCreated();

            //Alternativa para criação da tabela do segundo contexto
            var databaseCreator = db2.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }

        /// <summary>
        /// CanConnect método do EFCore que permite fazer um check no status do banco de dados
        /// </summary>
        static void HealthCheckDatabase()
        {
            using var db = new ApplicationContext();

            var canConnect = db.Database.CanConnect();

            if (canConnect)
                Console.WriteLine("Pode conectar");
            else
                Console.WriteLine("Não pode conectar");
        }

        /// <summary>
        /// Muitas vezes em cenarios onde é feitas muitas consultas
        /// deixar que o EFCore lide com a conexão pode não ser a melhor opção
        /// O metodo abaixo tem uma abordagem pra quando o ef lida com o genrenciamento da conexao gerenciarEstadoConexao = false
        /// e quando vc tem a possibilidade de gerenciar o estado gerenciarEstadoConexao = true
        /// Se desabilitado o pooling na connection string o tempo de resposta aumenta consideravelmente
        /// </summary>
        static int _count;
        static void GerenciarEstadoDaConexao(bool gerenciarEstadoConexao)
        {
            using var db = new ApplicationContext();
            var time = System.Diagnostics.Stopwatch.StartNew();

            //Retorna o objeto de conexão
            var conexao = db.Database.GetDbConnection();

            //Toda alteração de status entre a conexão com a base está sendo incrementada 
            conexao.StateChange += (_, __) => ++_count;

            if (gerenciarEstadoConexao)
            {
                conexao.Open();
            }

            for (var i = 0; i < 200; i++)
            {
                db.Marcas.AsNoTracking().Any();
            }

            time.Stop();
            var mensagem = $"Tempo: {time.Elapsed.ToString()}, Gerenciamento de Conexão: {gerenciarEstadoConexao}, Contador: {_count}";

            Console.WriteLine(mensagem);
        }

        static void ExecuteSQL()
        {
            using var db = new Curso.Data.ApplicationContext();

            // Primeira Opcao (mais usada)
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }

            // Segunda Opcao
            //ExecuteSqlRaw transforma o parametro em DbParameter
            var nome = "TESTE";
            db.Database.ExecuteSqlRaw("update marcas set nome={0} where id=1", nome);

            //Terceira Opcao
            //Transforma o valor da string interpolada em DbParameter
            db.Database.ExecuteSqlInterpolated($"update marcas set nome={nome} where id=1");

            //A segunda e terceira opção são mais seguras pois previnem SqlInjection.
        }

        static void SqlInjection()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Marcas.AddRange(
                new Marca
                {
                    Nome = "Honda"
                },
                       new Marca
                       {
                           Nome = "Toyota"
                       });
            db.SaveChanges();

            //var nome = "Honda ' or 1='1";

            //Priorizar sempre a passagem de parametros pelo argumento ou usando o ExecuteSqlInterpolated
            //db.Database.ExecuteSqlRaw("update marcas set nome='AtaqueSqlInjection' where nome={0}",nome);
            //db.Database.ExecuteSqlInterpolated($"update marcas set nome='AtaqueSqlInjection' where nome='{nome}'");

            //Não utilizar concatenando na string
            //db.Database.ExecuteSqlRaw($"update marcas set nome='AtaqueSqlInjection' where nome='{nome}'");

            foreach (var marca in db.Marcas.AsNoTracking())
            {
                Console.WriteLine($"Id: {marca.Id}, Nome: {marca.Nome}");
            }
        }

        /// <summary>
        /// Executando verificação de migrações pendentes
        /// </summary>
        static void MigracoesPendentes()
        {
            using var db = new ApplicationContext();

            var migracoesPendentes = db.Database.GetPendingMigrations();

            Console.WriteLine($"Total: {migracoesPendentes.Count()}");

            foreach (var migracao in migracoesPendentes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        /// <summary>
        /// Não é uma boa opção pois pode haver concorrencia
        /// </summary>
        static void AplicarMigracaoEmTempodeExecucao()
        {
            using var db = new ApplicationContext();

            db.Database.Migrate();
        }
    }
}
