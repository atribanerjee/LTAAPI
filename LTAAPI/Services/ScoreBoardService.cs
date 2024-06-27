using LTAAPI.Interfaces;
using LTAAPI.Models;
using LTADB;
using LTADB.POCO;

namespace LTAAPI.Services
{
    public class ScoreBoardService : IScoreBoardRepository
    {
        private readonly LTADBContext _context;
        private IConfiguration _Configuration;
        public ScoreBoardService(LTADBContext db, IConfiguration conf)
        {
            _context = db;
            _Configuration = conf;
        }

        public async Task<bool> SaveScore(ScoreBoardRequestModel model)
        {

            try
            {
                if (model != null && model.UserID > 0 && model.ScenarioID > 0)
                {
                    var entity = new scoreboard();
                    entity.FKUserID = model.UserID;
                    entity.FKScenarioID = model.ScenarioID;
                    entity.TotalScore = model.TotalScore;
                    entity.UserScore = model.UserScore;
                    entity.Persentage = model.Persentage;
                    entity.CreatedDateTime = DateTime.UtcNow;

                    await _context.Scoreboard.AddAsync(entity);
                    await _context.SaveChangesAsync();

                    return true;
                }

            }
            catch (Exception)
            {
            }

            return false;
        }
    }
}
