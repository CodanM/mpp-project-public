using System;
using System.Linq;

namespace model
{
    [Serializable]
    public class Entity<TId>
    {
        public TId Id { get; set; } = default!;

        public override string ToString()
        {
            var str = $"{GetType().Name} {{ ";
            str = GetType().GetProperties().Aggregate(str, (current, prop) => current + $"{prop.Name}: {prop.GetValue(this)}, ");
            str = str.Remove(str.Length - 2, 1);
            str += "}";
            return str;
        }
    }
}