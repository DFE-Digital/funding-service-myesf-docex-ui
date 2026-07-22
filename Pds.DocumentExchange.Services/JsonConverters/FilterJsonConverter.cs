using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pds.DocumentExchange.Services.Enums;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Pds.DocumentExchange.Services.Models.Filters.JsonConverters
{
    /// <summary>
    /// The filter JSON converter.
    /// </summary>
    public class FilterJsonConverter : JsonConverter<IFilter>
    {
        /// <inheritdoc/>
        public override IFilter ReadJson(JsonReader reader, Type objectType, [AllowNull] IFilter existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var filterOptionType = GetFilterTypeFromJsonObject(jsonObject);

            if (filterOptionType.HasValue)
            {
                return CreateFilterFromJsonObject(filterOptionType.Value, jsonObject, serializer);
            }

            throw new JsonSerializationException("The filter option type requires a value.");
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, [AllowNull] IFilter value, JsonSerializer serializer)
            => throw new NotImplementedException();

        /// <inheritdoc/>
        public override bool CanWrite
            => false;

        private FilterType? GetFilterTypeFromJsonObject(JObject jsonObject)
        {
            var tokenExists = jsonObject.TryGetValue(nameof(IFilter.Type), StringComparison.OrdinalIgnoreCase, out JToken jToken);

            if (!tokenExists)
            {
                return null;
            }

            return Enum.TryParse(jToken.ToString(), true, out FilterType result)
                ? result
                : default(FilterType?);
        }

        private IFilter CreateFilterFromJsonObject(FilterType filterType, JObject jsonObject, JsonSerializer serializer)
        {
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .First(p => typeof(IFilter).IsAssignableFrom(p) && p.Name == filterType.ToString());

            var filter = (IFilter)Activator.CreateInstance(type);
            serializer.Populate(jsonObject.CreateReader(), filter);

            return filter;
        }
    }
}