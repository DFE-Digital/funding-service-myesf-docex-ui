using Pds.DocumentExchange.Services.Enums;
using System;

namespace Pds.DocumentExchange.Services.Models
{
    /// <summary>
    /// Class representing an event occurring regarding an exchanged document.
    /// </summary>
    public class ExchangeDocumentEvent
    {
        /// <summary>
        /// Gets or sets the type of this event.
        /// </summary>
        public ExchangeDocumentEventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the information about the user for this event.
        /// </summary>
        public UserInfo UserInfo { get; set; }

        /// <summary>
        /// Gets or sets the event date and time.
        /// </summary>
        public DateTime EventDateTime { get; set; }
    }
}