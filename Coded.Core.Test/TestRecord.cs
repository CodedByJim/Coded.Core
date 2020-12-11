using System.ComponentModel.DataAnnotations;

namespace Coded.Core.Test
{
    public sealed record TestRecord
    {
        [Range(0, 100)]
        public int Id { get; init; }

        [Required]
        public string? Value { get; init; }

        public TestRecord[]? NestedObjects { get; init; }

        public TestRecord? NestedObject { get; set; }
    }
}