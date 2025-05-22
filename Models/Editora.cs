using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace PrototipoTrue.Models
{
    public class Editora
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int ID { get; set; }

        [NotNull]
        public string Nome { get; set; }

        [NotNull]
        public string Sigla { get; set; }

        public string Descricao { get; set; }
    }
}
