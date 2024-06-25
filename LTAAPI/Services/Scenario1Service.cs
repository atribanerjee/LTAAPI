using LTAAPI.Interfaces;
using LTAAPI.Models;
using LTADB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LTAAPI.Services
{
    public class Scenario1Service : IScenario1Repository
    {
        private readonly LTADBContext _context;
        private IConfiguration _Configuration;
        public Scenario1Service(LTADBContext db, IConfiguration conf)
        {
            _context = db;
            _Configuration = conf;
        }

        public async Task<Scenario1Model> GetScenario1()
        {
            Scenario1Model? ReturnModel = new Scenario1Model();
            try
            {

                ReturnModel = await (from sc in _context.Scenario1
                                     select new Scenario1Model
                                     {
                                         Id = sc.Id,
                                         JsonText = sc.JsonText

                                     }).FirstOrDefaultAsync();

            }
            catch (Exception Ex)
            {

            }

            return ReturnModel;
        }
    }
}
