using Palaviajeros.Domain.ValueObjects;

namespace Palaviajeros.Domain.Entities;

public abstract class Entity
{
    public EntityId Id { get; set; }
}