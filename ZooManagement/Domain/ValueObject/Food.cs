using System;

namespace ZooManagement.Domain.ValueObject
{
    public class Food : IEquatable<Food>
    {
        public string Name { get; }

        public Food(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override bool Equals(object obj) =>
            Equals(obj as Food);

        public bool Equals(Food other) =>
            other != null && Name == other.Name;

        public override int GetHashCode() =>
            Name.GetHashCode(StringComparison.Ordinal);

        public override string ToString() =>
            Name;

        public static bool operator ==(Food left, Food right) =>
            left?.Equals(right) ?? right is null;

        public static bool operator !=(Food left, Food right) =>
            !(left == right);
    }
}