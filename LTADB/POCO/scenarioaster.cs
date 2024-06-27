using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTADB.POCO
{
    [Table("scenariomaster")]
    public class scenarioaster
    {
        [Key]
        public int ID { get; set; }
        public String Name { get; set; }

    }
}
