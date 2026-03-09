using FrameworkX.Common;
using FrameworkX.Common.Infrastructure;
using HRSystem.Domain.Datasource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Domain.Repositories
{
    public class EmployeeRepository: EFRepositoryBase<HumanResourcesDbContext, Employee>
    {
        public EmployeeRepository(HumanResourcesDbContext db) : base(db)
        {
        }

        // public HumanResourcesDbContext db { get; set; }
        // public Employee GetById(int employeeId)
        // {
        //     return db.Employees.Find(employeeId);
        // }
        //ToDo: Is it Ok?

        //public IUserProvider User { get; set; }
        //public Employee GetCurrentEmployee()
        //{
        //    return db.Employees.Find(this.User.CurrentUserId);
        //}
    }
}
