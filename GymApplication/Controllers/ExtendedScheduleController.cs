using GymApplication.Filters;
using GymApplication.Models;
using GymApplication.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GymApplication.Controllers
{
    [Authorize]
    [TwoFactorAuth]
    public class ExtendedScheduleController : ApiController
    {
        private ScheduleRepository _repository;

        public ExtendedScheduleController()
        {
            _repository = new ScheduleRepository();
        }

        // GET: api/ExtendedSchedules
        public IEnumerable<ExtendedScheduleView> GetExtendedSchedules()
        {
            var result = _repository.GetSchedules();

            return result;
        }
    }
}
