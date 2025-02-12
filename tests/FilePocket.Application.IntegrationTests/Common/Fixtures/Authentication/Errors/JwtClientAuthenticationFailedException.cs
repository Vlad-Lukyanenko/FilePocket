namespace FilePocket.Application.IntegrationTests.Common.Fixtures.Authentication.Errors;

public sealed class JwtClientAuthenticationFailedException(string context) : Exception($"Jwt client authentication failed. Context: {context}");