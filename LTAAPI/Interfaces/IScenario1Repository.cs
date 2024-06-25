using Microsoft.AspNetCore.Mvc;
using LTAAPI.Models;

namespace LTAAPI.Interfaces
{
    public interface IScenario1Repository
    {
        Task<Scenario1Model> GetScenario1();
    }
}
