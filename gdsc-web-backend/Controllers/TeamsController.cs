﻿using System.Collections.Generic;
using gdsc_web_backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace gdsc_web_backend.Controllers
{
    [ApiController]  
    [Route("api/[controller]")]  
    public class TeamsController : ControllerBase
    {
        [HttpGet]
        public List<TeamModel> Get()
        {
            return new List<TeamModel>
            {
                new TeamModel
                {
                    Id = "1",
                    Name = "FCB"
                },
                new TeamModel
                {
                    Id = "2",
                    Name = "LFC"
                }
            };
        }
    }
}