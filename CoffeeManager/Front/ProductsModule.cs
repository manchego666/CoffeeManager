using System;
using System.Drawing;
using System.Windows.Forms;

namespace CoffeeManager.Front
{
    /// <summary>
    /// Products module for adding and editing product information.
    /// </summary>
    public class ProductsModule : Panel
    {
        #region Constructor
        public ProductsModule()
        {
            InitializeModule();
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes the visual structure of the Products module.
        /// </summary>
        private void InitializeModule()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(40, 40, 40, 180);

            // TODO: Add products UI here
        }
        #endregion
    }
}
