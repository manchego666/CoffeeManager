using System;
using System.Drawing;
using System.Windows.Forms;

namespace CoffeeManager.Front
{
    /// <summary>
    /// Reports module for generating and viewing system reports.
    /// </summary>
    public class ReportsModule : Panel
    {
        #region Constructor
        public ReportsModule()
        {
            InitializeModule();
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes the visual structure of the Reports module.
        /// </summary>
        private void InitializeModule()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(40, 40, 40, 180);

            // TODO: Add reports UI here
        }
        #endregion
    }
}
