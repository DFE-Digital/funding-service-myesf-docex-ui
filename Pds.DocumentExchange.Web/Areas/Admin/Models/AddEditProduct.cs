using Pds.Core.Web.Administration.Models;

namespace Pds.DocumentExchange.Web.Areas.Admin.Models
{
    /// <summary>
    /// View model for the add and edit product pages.
    /// </summary>
    public class AddEditProduct : BaseSettingEditViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddEditProduct"/> class.
        /// </summary>
        /// <param name="setting">The setting.</param>
        public AddEditProduct(SettingViewModel setting) : base(setting)
        {
        }
    }
}