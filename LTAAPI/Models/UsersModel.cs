namespace LTAAPI.Models
{
    public class UsersModel
    {
        public Int64 ID { get; set; }
        public String FirstName { get; set; }=String.Empty;
        public String LastName { get; set; } = String.Empty;
        public String Email { get; set; } = String.Empty;
        public String UserName { get; set; } = String.Empty;
        public String PhoneNo { get; set; } = String.Empty;
        public String Password { get; set; } = String.Empty;
        public Boolean IsActive { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
