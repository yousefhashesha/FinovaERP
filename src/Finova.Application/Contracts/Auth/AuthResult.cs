using System;

namespace Finova.Application.Contracts.Auth
{
    public sealed class AuthResult
    {
        public AuthResult(bool success, string message, Guid? userId = null, string? displayName = null)
        {
            Success = success;
            Message = message;
            UserId = userId;
            DisplayName = displayName;
        }

        public bool Success { get; }
        public string Message { get; }
        public Guid? UserId { get; }
        public string? DisplayName { get; }
    }
}
