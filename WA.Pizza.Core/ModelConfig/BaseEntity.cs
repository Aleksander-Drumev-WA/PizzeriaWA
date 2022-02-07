using System.ComponentModel.DataAnnotations;

namespace WA.Pizza.Core.ModelConfig
{
    public abstract class BaseEntity
    {
        [Key]
        public long Id { get; init; }
    }
}
