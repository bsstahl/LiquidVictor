using System;
using System.Collections.Generic;
using System.Linq;

namespace LiquidVictor.Exceptions;

/// <summary>
/// Exception thrown when duplicate IDs are detected within the same entity type space
/// (e.g., two slides sharing the same ID within a slide deck collection).
/// </summary>
public class DuplicateEntityIdException : Exception
{
    public DuplicateEntityIdException(string entityType, IEnumerable<Guid> duplicateIds)
        : base(CreateMessage(entityType, duplicateIds))
    {
        EntityType = entityType;
        DuplicateIds = duplicateIds.ToList();
    }

    public DuplicateEntityIdException(string entityType, IEnumerable<Guid> duplicateIds, Exception innerException)
        : base(CreateMessage(entityType, duplicateIds), innerException)
    {
        EntityType = entityType;
        DuplicateIds = duplicateIds.ToList();
    }

    /// <summary>The entity type (e.g., "Slide", "ContentItem", "SlideDeck") that has duplicate IDs.</summary>
    public string EntityType { get; }

    /// <summary>The IDs that appear more than once within the entity type space.</summary>
    public IEnumerable<Guid> DuplicateIds { get; }

    private static string CreateMessage(string entityType, IEnumerable<Guid> duplicateIds)
    {
        var idsText = string.Join(", ", duplicateIds);
        return $"Duplicate {entityType} IDs detected: {idsText}";
    }
}
