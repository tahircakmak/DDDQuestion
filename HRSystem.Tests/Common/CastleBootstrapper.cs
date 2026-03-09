using Castle.MicroKernel.Registration;
using Castle.Windsor;
using HRSystem.Domain.Datasource;
using HRSystem.Domain.Repositories;
using HRSystem.Web.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Tests.Common
{
    public class CastleBootstrapper
    {
        public static WindsorContainer Init()
        {
            var container = new WindsorContainer();

            // Register SQLite in-memory options for HumanResourcesDbContext
            var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<HumanResourcesDbContext>()
                .UseSqlite("Data Source=hrsystem_test.db")
                .Options;
            container.Register(Component.For<Microsoft.EntityFrameworkCore.DbContextOptions<HumanResourcesDbContext>>().Instance(options));
            container.Register(Component.For<VacationController>());
            container.Register(Component.For<HumanResourcesDbContext>());
            container.Register(Component.For<VacationRepository>());
            container.Register(Component.For<EmployeeRepository>());
            container.Register(Component.For<HRUnitOfWork>());

            return container;
        }
    }
}
