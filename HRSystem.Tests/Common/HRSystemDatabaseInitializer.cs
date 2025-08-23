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
        public Employee EmployeeAliVeli { get; set; }
        public Employee Manager { get; set; }

        public void InitializeDatabase(HumanResourcesDbContext db)
        {

            this.Manager = new Employee { FullName = "Manager", GrantedAnnualLeaveDays = 20 };

            this.EmployeeAliVeli = new Employee { FullName = "Ali Veli", GrantedAnnualLeaveDays = 20 };
            this.EmployeeAliVeli.Vacations.Add(new Vacation { Days = 19, Status = RequestStatus.Approved });

            this.Manager.SubEmployees.Add(this.EmployeeAliVeli);
            db.Employees.Add(this.Manager);
            db.SaveChanges();
        }
    }
}
