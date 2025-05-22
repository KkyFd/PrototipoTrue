using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrototipoTrue.Models
{
    public class Autor
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int ID { get; set; }

        [NotNull]
        public string Nome { get; set; }

        [NotNull]
        public string Pseudonimo { get; set; }

        public string Descricao { get; set; }
    }
}
