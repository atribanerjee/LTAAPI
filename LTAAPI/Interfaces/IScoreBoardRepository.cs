using LTAAPI.Models;

namespace LTAAPI.Interfaces
{
    public interface IScoreBoardRepository
    {
        Task<Boolean> SaveScore(ScoreBoardRequestModel model);
    }
}
