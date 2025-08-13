using System.Text.Json.Serialization;

namespace OpenTelemetry.Kafka.Domain.Events;

[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization, TypeDiscriminatorPropertyName = Discriminator)]
[JsonDerivedType(typeof(PostCreated), nameof(PostCreated))]
[JsonDerivedType(typeof(PostUpdated), nameof(PostUpdated))]
[JsonDerivedType(typeof(PostDeleted), nameof(PostDeleted))]
public abstract record PostEvent
{
	private const string Discriminator = "eventType";
}

public record PostCreated(Guid Id, DateTime CreatedAt) : PostEvent;
public record PostUpdated(Guid Id, DateTime UpdatedAt) : PostEvent;
public record PostDeleted(Guid Id, DateTime DeletedAt) : PostEvent;
