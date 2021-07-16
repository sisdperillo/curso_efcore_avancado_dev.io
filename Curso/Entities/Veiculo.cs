namespace Curso.Entities
{
    public class Veiculo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Modelo { get; set; }
        public int Ano { get; set; }
        public int MarcaId { get; set; }
        public Marca Marca { get; set; }
    }
}