using System;
using CoffeeManager.Models.Enums;

namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Base class representing a person registered in the system.
    /// Provides personal identity and contact information. (⁀ᗢ⁀) ZORRODEV2026
    /// </summary>
    internal abstract class User
    {
        #region DATA

        /// <summary>
        /// First name of the person. (≧◡≦) ZORRODEV2026
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Last name of the person. (✿◠‿◠) ZORRODEV2026
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Birth date of the person. (✧ω✧) ZORRODEV2026
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Automatically calculated age. (ಠ‿ಠ) ZORRODEV2026
        /// </summary>
        public int Age => DateTime.Now.Year - DateOfBirth.Year;

        /// <summary>
        /// CURP (Mexican personal ID). (≧◡≦) ZORRODEV2026
        /// </summary>
        public string CURP { get; set; } = string.Empty;

        /// <summary>
        /// RFC (Mexican tax ID). (◕‿◕✿) ZORRODEV2026
        /// </summary>
        public string RFC { get; set; } = string.Empty;

        /// <summary>
        /// Home address of the person. (≧◡≦) ZORRODEV2026
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Phone number of the person. (•̀ᴗ•́)و ̑̑ ZORRODEV2026
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Email address of the person. (ಠ_ಠ) ZORRODEV2026
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gender of the person. (✧ω✧) ZORRODEV2026
        /// </summary>
        public Gender Gender { get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Returns the full name of the person. (≧◡≦) ZORRODEV2026
        /// </summary>
        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }

        #endregion
    }
}
