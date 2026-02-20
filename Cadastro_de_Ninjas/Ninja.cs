using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadastro_de_Ninjas
{
    public class Ninja
    {
        public string Nome { get; set; }
        public int Idade { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Genero { get; set; }
        public string VilaOrigem { get; set; }
        public string RankNinja { get; set; }
        public DadosNinja Habilidades { get; set; }

        public Ninja()
        {
            Habilidades = new DadosNinja();
        }
    }
}