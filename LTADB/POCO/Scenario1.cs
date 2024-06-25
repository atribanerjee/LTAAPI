using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTADB.POCO
{
    [Table("users")]
    public class Scenario1
    {
        [Key]
        public Int32 Id { get; set; }
        public String? JsonText { get; set; }
    }
}
