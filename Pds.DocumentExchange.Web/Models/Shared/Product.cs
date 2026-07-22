namespace Pds.DocumentExchange.Web.Models.Shared
{
    /// <summary>The class representing a document exchange product.</summary>
    public class Product
    {
        /// <summary>Gets or sets the identifier.</summary>
        public int Identifier { get; set; }

        /// <summary>Gets or sets the product name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the plural form of product name.</summary>
        public string PluralName { get; set; }
    }
}