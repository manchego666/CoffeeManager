using System;
using CoffeeManager.Models.Enums;

namespace CoffeeManager.Models.Class
{
    internal abstract class User
    {
        #region DATA
        public string Name { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public int Age => DateTime.Now.Year - DateOfBirth.Year;
        public string RFC { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public Gender Gender { get; set; }
        #endregion
    }
}
