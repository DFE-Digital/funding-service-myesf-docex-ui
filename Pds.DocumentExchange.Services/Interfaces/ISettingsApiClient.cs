using Pds.DocumentExchange.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Interfaces
{
    /// <summary>
    /// An interface exposing methods for interacting with the Document Exchange Settings API.
    /// </summary>
    public interface ISettingsApiClient
    {
        /// <summary>
        /// Get a list of products that organisations can upload.
        /// </summary>
        /// <returns>A <see cref="Task{IEnumerable{Product}}"/> representing the result
        /// of the asynchronous operation.</returns>
        Task<IEnumerable<Product>> GetProductsThatOrganisationsCanUpload();

        /// <summary>
        /// Get a list of all products.
        /// </summary>
        /// <returns>A <see cref="Task{IEnumerable{Product}}"/> representing the result
        /// of the asynchronous operation.</returns>
        Task<IEnumerable<Product>> GetAllProducts();

        /// <summary>
        /// Get a product for a specified identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>A <see cref="Task{Product}"/> representing the result
        /// of the asynchronous operation.</returns>
        Task<Product> GetProduct(int identifier);

        /// <summary>
        /// Gets the list of all agency teams.
        /// </summary>
        /// <returns>The list of all teams.</returns>
        Task<IEnumerable<AgencyTeam>> GetTeams();

        /// <summary>
        /// Gets the list of all supported file extensions.
        /// </summary>
        /// <returns>The list of all supported file extensions.</returns>
        Task<IEnumerable<FileExtensionInfo>> GetFileExtensions();

        /// <summary>
        /// Adds or updates a product.
        /// </summary>
        /// <param name="oldIdentifier">The old product identifier.</param>
        /// <param name="newProductValue">The new product to add or update.</param>
        /// <returns>The product that was added or updated.</returns>
        Task<Product> AddOrUpdateProduct(int oldIdentifier, Product newProductValue);

        /// <summary>
        /// Gets the maximum file upload size in bytes.
        /// </summary>
        /// <returns>The maximum file upload size in bytes.</returns>
        Task<int> GetMaxFileUploadSize();
    }
}