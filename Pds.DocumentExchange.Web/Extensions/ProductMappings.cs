namespace Pds.DocumentExchange.Web.Extensions
{
    /// <summary>
    /// Extension class to map <see cref="Services.Models.Product"/> to <see cref="Web.Models.Shared.Product"/>.
    /// </summary>
    public static class ProductMappings
    {
        /// <summary>
        /// Extension method to convert a <see cref="Services.Models.Product"/> to <see cref="Web.Models.Shared.Product"/>.
        /// </summary>
        /// <param name="source">the Services Product source object.</param>
        /// <returns>a Web Product.</returns>
        public static Web.Models.Shared.Product ToWebProduct(this Services.Models.Product source)
        {
            if (source == null)
            {
                return null;
            }

            return new Web.Models.Shared.Product
            {
                Identifier = source.Identifier,
                Name = source.Name,
                PluralName = source.PluralName
            };
        }
    }
}