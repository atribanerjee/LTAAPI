using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTADB.POCO
{
    [Table("scoreboard")]
    public class scoreboard
    {
        public Int64 ID { get; set; }
        public Int64 FKUserID { get; set; }
        public int FKScenarioID { get; set; }
        public int TotalScore { get; set; }
        public int UserScore { get; set; }
        public int Persentage { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
