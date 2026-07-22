using System;

namespace Pds.DocumentExchange.Web.Attributes
{
    /// <summary>
    /// A user permission.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class UserPermissionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserPermissionAttribute"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public UserPermissionAttribute(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; private set; }
    }
}