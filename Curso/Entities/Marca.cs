using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;

namespace Curso.Entities
{
    public class Marca
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }

        /// <summary>
        /// O virtual serve para o lazy loading sobrescrever o comportamento da propriedade, carregando os dados qdo é acessada
        /// </summary>
        public virtual List<Veiculo> Veiculos { get; set; }

        #region "Implementação manual do Lazy Loader"

        //public Marca()
        //{

        //}

        //private ILazyLoader _lazyLoader { get; set; }
        //private Marca(ILazyLoader lazyLoader)
        //{
        //    _lazyLoader = lazyLoader;
        //}

        //private List<Veiculo> _veiculos;
        //public List<Veiculo> Veiculos
        //{
        //    get => _lazyLoader.Load(this, ref _veiculos);
        //    set => _veiculos = value;
        //}

        //Desabilitar o UseProxies no contexto

        #endregion
    }
}