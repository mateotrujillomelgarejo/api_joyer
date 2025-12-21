using System;

namespace api_joyeria.Domain
{
    // Excepción base del dominio para invariantes y reglas del negocio
    public class DomainException : Exception
    {
        public DomainException() { }
        public DomainException(string message) : base(message) { }
        public DomainException(string message, Exception inner) : base(message, inner) { }
    }
}