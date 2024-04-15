namespace LTAAPI.Models
{
    public class ResponseModel
    {
        public string prompt { get; set; }
        public long created { get; set; }
        public List<Link>? data { get; set; }
    }
}
