using System;

namespace api_joyeria.Domain.ValueObjects
{
    public sealed class Email : IEquatable<Email>
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Of(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new DomainException("Email is required");
            // Validación ligera (no dependencias externas); ajusta si quieres más robusta
            if (!email.Contains("@") || email.Length < 5) throw new DomainException("Email is invalid");
            return new Email(email.Trim().ToLowerInvariant());
        }

        public override bool Equals(object obj) => Equals(obj as Email);
        public bool Equals(Email other) => other != null && Value == other.Value;
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
    }
}