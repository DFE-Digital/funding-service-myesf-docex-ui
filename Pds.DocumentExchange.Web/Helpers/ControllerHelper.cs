using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Pds.DocumentExchange.Web.Helpers
{
    /// <summary>
    /// Class containing helper methods for dealing with controllers.
    /// </summary>
    public static class ControllerHelper
    {
        private static readonly Regex _controllerRegex
            = new Regex(@"Controller$", RegexOptions.Compiled);

        /// <summary>
        /// Gets the name of the specified controller.
        /// </summary>
        /// <typeparam name="T">The type of the controller to get the name of.</typeparam>
        /// <returns>The name of the controller of type <typeparamref name="T"/>.</returns>
        public static string NameOf<T>()
            where T : Controller
        {
            string typeName = typeof(T).Name;
            return _controllerRegex.Replace(typeName, string.Empty);
        }
    }
}