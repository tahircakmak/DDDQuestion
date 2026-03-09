namespace HRSystem.Domain
{
    using Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Employee")]
    public partial class Employee
    {
        public Employee()
        {
            this.SubEmployees = new HashSet<Employee>();
            Vacations = new HashSet<Vacation>();
        }

        public int Id { get; set; }

        [StringLength(100)]
        public string FullName { get; set; }

        public int? ManagerId { get; set; }
        public virtual Employee Manager { get; set; }

        protected internal virtual ICollection<Employee> SubEmployees { get; set; }

        public virtual ICollection<Vacation> Vacations { get; set; }

        public int GrantedAnnualLeaveDays { get; set; }

        public void OpenVacationRequest(int requestedDays)
        {
            this.Vacations.Add(Vacation.Create(this, requestedDays));
        }
    }
}
