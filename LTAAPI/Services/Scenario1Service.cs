using LTAAPI.Interfaces;
using LTAAPI.Models;
using LTADB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using LTADB.POCO;
using System.Collections.Generic;

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

        public async Task<List<Scenario1Model>> GetScenario1()
        {
            List<Scenario1Model>? ReturnModel = new List<Scenario1Model>();
            try
            {

                ReturnModel = await (from sc in _context.Scenario1
                                     select new Scenario1Model
                                     {
                                         Id = sc.Id,
                                         JsonText = sc.JsonText

                                     }).ToListAsync();

            }
            catch (Exception Ex)
            {

            }

            return ReturnModel;
        }

        public async Task<bool> SaveScenario1(string jsondata)
        {
            bool result = false;
            try
            {
                var entity = new Scenario1();
                entity.JsonText = jsondata;

                await _context.Scenario1.AddAsync(entity);
                await _context.SaveChangesAsync();

                result = true;

                return result;

            }
            catch (Exception ex)
            {


            }
            return result;
        }
    }
}
