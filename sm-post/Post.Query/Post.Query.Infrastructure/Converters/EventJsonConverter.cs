using System.Text.Json;
using System.Text.Json.Serialization;
using CQRS.Core.Events;
using Post.Common.Events;

namespace Post.Query.Infrastructure.Converters
{
    public class EventJsonConverter : JsonConverter<BaseEvent>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsAssignableFrom(typeof(BaseEvent));
        }

        public override BaseEvent? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            if (!JsonDocument.TryParseValue(ref reader, out var document))
            {
                throw new JsonException($"Failed to parse JSON {nameof(JsonDocument)}.");
            }

            if (!document.RootElement.TryGetProperty("Type", out var typeProperty))
            {
                throw new JsonException($"Failed to get Type discriminator property from JSON.");
            }

            var typeDiscriminator = typeProperty.GetString();
            var json = document.RootElement.GetRawText();

            return typeDiscriminator switch
            {
                nameof(PostCreatedEvent)
                    => JsonSerializer.Deserialize<PostCreatedEvent>(json, options),
                nameof(MessageUpdatedEvent)
                    => JsonSerializer.Deserialize<MessageUpdatedEvent>(json, options),
                nameof(PostLikedEvent) => JsonSerializer.Deserialize<PostLikedEvent>(json, options),
                nameof(CommentAddedEvent)
                    => JsonSerializer.Deserialize<CommentAddedEvent>(json, options),
                nameof(CommentUpdatedEvent)
                    => JsonSerializer.Deserialize<CommentUpdatedEvent>(json, options),
                nameof(CommentRemovedEvent)
                    => JsonSerializer.Deserialize<CommentRemovedEvent>(json, options),
                nameof(PostRemovedEvent)
                    => JsonSerializer.Deserialize<PostRemovedEvent>(json, options),
                _
                    => throw new JsonException(
                        $"Unknown event type discriminator: {typeDiscriminator}."
                    )
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            BaseEvent value,
            JsonSerializerOptions options
        )
        {
            throw new NotImplementedException();
        }
    }
}
