using System.Collections.Generic;

namespace Curso.Entities
{
    public class Marca
    {
        public int Id { get; set; } 
        public string Nome { get; set; }
        public List<Veiculo> Veiculos { get; set; }
    }
}