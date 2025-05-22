using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
namespace PrototipoTrue.Models
{
    public class Livro
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int ID { get; set; }

        [NotNull]
        public string Nome { get; set; }

        [NotNull]
        public int Ano { get; set; }

        [NotNull]
        public string ISBN { get; set; }

        public string Descricao { get; set; }

        [NotNull]
        public int AutorID { get; set; }

        [NotNull]
        public int EditoraID { get; set; }

        [Ignore] 
        public string AutorNome { get; set; }

        [Ignore]
        public string EditoraNome { get; set; }
    }
}
