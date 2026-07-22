using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Authentication.Interfaces
{
    /// <summary>
    /// Interface for the sign out method.
    /// </summary>
    public interface ISignOutMethod
    {
        /// <summary>
        /// Executes the sign out method.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns>The sign out result.</returns>
        Task<IActionResult> SignOut(Controller controller);
    }
}