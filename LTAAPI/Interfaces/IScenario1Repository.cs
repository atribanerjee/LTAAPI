using Microsoft.AspNetCore.Mvc;
using LTAAPI.Models;

namespace LTAAPI.Interfaces
{
    public interface IScenario1Repository
    {
        Task<List<Scenario1Model>> GetScenario1();
        Task<bool> SaveScenario1(string jsondata);
    }
}
