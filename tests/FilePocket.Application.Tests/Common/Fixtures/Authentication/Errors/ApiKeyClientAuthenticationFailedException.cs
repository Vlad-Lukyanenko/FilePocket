namespace FilePocket.Application.IntegrationTests.Common.Fixtures.Authentication.Errors;

public sealed class ApiKeyClientAuthenticationFailedException(string context) : Exception($"Api key client authentication failed. Context: {context}");