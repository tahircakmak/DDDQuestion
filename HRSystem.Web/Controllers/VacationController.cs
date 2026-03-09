using FrameworkX.Common.Infrastructure;
using HRSystem.Domain;
using HRSystem.Domain.Datasource;
using HRSystem.Domain.ServiceClients;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VacationController : ControllerBase
    {
        public HRUnitOfWork UnitOfWork { get; set; }
        public IPayrollSystem PayrollClient { get; set; }
        public IUserProvider User { get; set; }

        [HttpPost("OpenVacationRequest")]
        public IActionResult OpenVacationRequest(int requestedDays)
        {
            var currentEmployee = UnitOfWork.EmployeeRepository.FindById(this.User.CurrentUserId);
            //ToDo: Should EmployeeRepository can access IUserProvider
            //var currentEmployee = UnitOfWork.EmployeeRepository.GetCurrentEmployee();

            currentEmployee.OpenVacationRequest(requestedDays);
            UnitOfWork.SaveChanges();
            return Ok();
        }

        [HttpGet("GetOpenRequests")]
        public IEnumerable<Vacation> GetOpenRequests()
        {
            //ToDo: Should VacationRepository can access IUserProvider
            var result = this.UnitOfWork.VacationRepository.GetOpenRequestsForUser(this.User.CurrentUserId);
            return result;
        }

        [HttpPost("ApproveRequest")]
        public IActionResult ApproveRequest(int vacationId)
        {
            var vacation = this.UnitOfWork.VacationRepository.GetById(vacationId);
            vacation.Approve(this.UnitOfWork.VacationRepository, this.PayrollClient);

            UnitOfWork.SaveChanges();

            return Ok();
        }
    }
}
