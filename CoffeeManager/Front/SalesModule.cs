using System;
using System.Drawing;
using System.Windows.Forms;

namespace CoffeeManager.Front
{
    /// <summary>
    /// Sales module for managing and registering sales.
    /// </summary>
    public class SalesModule : Panel
    {
        #region Constructor
        public SalesModule()
        {
            InitializeModule();
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes the visual structure of the Sales module.
        /// </summary>
        private void InitializeModule()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(40, 40, 40, 180); // semi-transparent panel

            // TODO: Add sales UI here
        }
        #endregion
    }
}
