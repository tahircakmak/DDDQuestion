using HRSystem.Domain;
using HRSystem.Domain.Datasource;
using HRSystem.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Tests.Common
{
    public class HRSystemDatabaseInitializer
    {
        public Employee EmployeeJohnDoe { get; set; }
        public Employee Manager { get; set; }

        public void InitializeDatabase(HumanResourcesDbContext db)
        {
            this.Manager = new Employee { FullName = "Manager", GrantedAnnualLeaveDays = 20 };

            this.EmployeeJohnDoe = new Employee { FullName = "John Doe", GrantedAnnualLeaveDays = 20 };
            this.EmployeeJohnDoe.Vacations.Add(new Vacation { Days = 19, Status = RequestStatus.Approved });

            this.Manager.SubEmployees.Add(this.EmployeeJohnDoe);
            db.Employees.Add(this.Manager);
            db.SaveChanges();
        }
    }
}
