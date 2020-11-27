using System.ComponentModel.DataAnnotations;

namespace Coded.Core.Test
{
    public class TestRecord
    {
        [Range(0, 100)]
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public TestRecord[] NestedObjects { get; set; }

        public TestRecord NestedObject { get; set; }
    }
}