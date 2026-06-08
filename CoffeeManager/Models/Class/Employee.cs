using CoffeeManager.Models.Enums;

namespace CoffeeManager.Models.Class
{
    internal class Employee : User
    {
        public string SocialSecurityNumber { get; set; }
        public decimal Salary { get; set; }
        public EmployeeType EmployeeType { get; set; }

        public Employee() { }

    }
}
