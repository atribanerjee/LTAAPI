namespace LTAAPI.Models
{
    public class ScoreBoardModel
    {
        public Int64 ID { get; set; }
        public Int64 FKUserID { get; set; }
        public int FKScenarioID { get; set; }
        public int TotalScore { get; set; }
        public int UserScore { get; set; }
        public int Persentage { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }


    public class ScoreBoardRequestModel
    {
        public Int64 UserID { get; set; }
        public int ScenarioID { get; set; }
        public int TotalScore { get; set; }
        public int UserScore { get; set; }
        public int Persentage { get; set; }
        
    }

}
