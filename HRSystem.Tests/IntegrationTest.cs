using System;
using Castle.Windsor;
using HRSystem.Domain.ServiceClients;
using HRSystem.Tests.Common;
using Moq;
using Castle.MicroKernel.Registration;
using HRSystem.Domain.Datasource;
using FrameworkX.Common.Infrastructure;
using HRSystem.Web.Controllers;
using System.Linq;
using HRSystem.Domain;
using Xunit;

namespace HRSystem.Tests
{
    public class IntegrationTest
    {
        #region Test Init and Configure
        private WindsorContainer container;
        private Mock<IPayrollSystem> mockPayroll;
        private Mock<IUserProvider> mockUser;

        public HRSystemDatabaseInitializer DbInitializer { get; private set; }

        public IntegrationTest()
        {
            this.container = CastleBootstrapper.Init();

            this.mockPayroll = new Mock<IPayrollSystem>();
            container.RegisterMock(mockPayroll);

            this.mockUser = new Mock<IUserProvider>();
            container.RegisterMock(mockUser);

            InitDB();
        }

        private void InitDB()
        {
            this.DbInitializer = new HRSystemDatabaseInitializer();
            var dbContext = container.Resolve<HumanResourcesDbContext>();
            dbContext.Database.EnsureCreated();
            this.DbInitializer.InitializeDatabase(dbContext);
        }

        private void SwitchToUser(Employee user)
        {
            this.mockUser.SetupGet(u => u.CurrentUserId).Returns(user.Id);
        }
        #endregion

        [Fact]
        public void End2EndVacationRequestTest()
        {
            SwitchToUser(this.DbInitializer.EmployeeAliVeli);

            var controller = container.Resolve<VacationController>();

            controller.OpenVacationRequest(requestedDays: 3);
            SwitchToUser(this.DbInitializer.Manager);
            var openedRequest = controller.GetOpenRequests().First();

            Assert.Equal(openedRequest.EmployeeId, this.DbInitializer.EmployeeAliVeli.Id);
            Assert.Equal(3, openedRequest.Days);

            controller.ApproveRequest(openedRequest.Id);

            this.mockPayroll.Verify(m => m.AddUnpaidVacation(2));
        }
    }
}
