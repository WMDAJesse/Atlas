﻿using Microsoft.AspNetCore.Mvc;
using Nova.SearchAlgorithm.Data.Models;
using Nova.SearchAlgorithm.Services.Donors;
using System.Threading.Tasks;

namespace Nova.SearchAlgorithm.Api.Controllers
{
    [Route("donor")]
    public class DonorController : ControllerBase
    {
        private readonly IDonorService donorService;

        public DonorController(IDonorService donorService)
        {
            this.donorService = donorService;
        }

        [HttpPut]
        [Route("batch")]
        public async Task CreateOrUpdateDonorBatch([FromBody] InputDonorBatch donorBatch)
        {
            await donorService.CreateOrUpdateDonorBatch(donorBatch.Donors);
        }
    }
}
