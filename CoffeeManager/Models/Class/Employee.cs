using CoffeeManager.Models.Enums;

namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents an employee working in the coffee shop.
    /// Extends User with job-related information. (✿◠‿◠) ZORRODEV2026
    /// </summary>
    internal class Employee : User
    {
        #region DATA

        /// <summary>
        /// Unique identifier for the employee. (≧◡≦) ZORRODEV2026
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Employee job type (Barista, Manager, etc.). (✧ω✧) ZORRODEV2026
        /// </summary>
        public EmployeeType Type { get; set; }

        /// <summary>
        /// Monthly salary of the employee. (◕‿◕✿) ZORRODEV2026
        /// </summary>
        public decimal Salary { get; set; }

        /// <summary>
        /// Social Security Number (NSS). (ಠ‿ಠ) ZORRODEV2026
        /// </summary>
        public string SocialSecurityNumber { get; set; } = string.Empty;

        /// <summary>
        /// Path to the employee's profile image. (≧◡≦) ZORRODEV2026
        /// </summary>
        public string PathImage { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the employee is currently active. (•̀ᴗ•́)و ̑̑ ZORRODEV2026
        /// </summary>
        public bool Active { get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Returns true if the employee is active. (✿◠‿◠) ZORRODEV2026
        /// </summary>
        public bool IsActive() => Active;

        /// <summary>
        /// Returns true if the employee has a valid Social Security Number. (ಠ_ಠ) ZORRODEV2026
        /// </summary>
        public bool HasValidSSN() => !string.IsNullOrWhiteSpace(SocialSecurityNumber);

        /// <summary>
        /// Returns true if the employee has a profile image assigned. (◕‿◕✿) ZORRODEV2026
        /// </summary>
        public bool HasImage() => !string.IsNullOrWhiteSpace(PathImage);

        #endregion
    }
}
