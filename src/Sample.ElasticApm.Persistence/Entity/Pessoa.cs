using System;

namespace Sample.ElasticApm.Persistence.Entity
{
    public class Pessoa
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}
