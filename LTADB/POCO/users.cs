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
    public class users
    {
        [Key]
        public Int64 ID { get; set; }
        public String? FirstName { get; set; }
        public String? LastName { get; set; }
        public String Email { get; set; }
        public  String UserName { get; set; }
        public String? PhoneNo { get; set; }
        public String Password { get; set; }
        public String? Address { get; set; }
        public Boolean IsActive { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public Guid ResetPasswordToken { get; set; }
        public DateTime TokenValidity { get; set; }
    }
}
