using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Bogus;
using FilePocket.Application.IntegrationTests.Common.Fixtures.Authentication.Errors;
using FilePocket.Application.IntegrationTests.Common.Fixtures.Authentication.Mappings;
using FilePocket.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit;

namespace FilePocket.Application.IntegrationTests.Common.Fixtures.Authentication;

public class ClientAuthenticationFixture : IAsyncLifetime
{
    private static readonly Faker<UserRegistrationModel> UserRegistrationFaker = new Faker<UserRegistrationModel>()
        .RuleFor(x => x.Email, faker => faker.Person.Email)
        .RuleFor(x => x.Password, faker => faker.Person.Phone) // faker.Internet.Password could be used, but it breaks internal validation during registration
        .RuleFor(x => x.ConfirmPassword, (_, user) => user.Password);

    private static JwtSecurityTokenHandler JwtSecurityTokenHandler { get; } = new();

    private readonly IConfiguration _configuration;
    private readonly HttpClient _unauthenticatedClient;
    private readonly FilePocketWebAppFactory _filePocketWebAppFactory;

    private ClientAuthenticationFixture(FilePocketWebAppFactory factory, bool useJwtAuthentication, bool useApiKeyAuthentication)
    {
        _filePocketWebAppFactory = factory;

        _configuration = _filePocketWebAppFactory.Services.CreateScope()
            .ServiceProvider.GetRequiredService<IConfiguration>();

        _unauthenticatedClient = _filePocketWebAppFactory.CreateClient();

        UseJwtAuthentication = useJwtAuthentication;
        UseApiKeyAuthentication = useApiKeyAuthentication;
    }

    private bool UseJwtAuthentication { get; }
    private bool UseApiKeyAuthentication { get; }
    private HttpClient ApiKeyAuthenticatedClient { get; set; }
    public HttpClient JwtAuthenticatedClient { get; set; }
    private TokenModel JwtToken { get; set; }

    // Extracted from JwtToken
    public Guid JwtTokenUserId { get; private set; }
    public Guid DefaultPocketId { get; private set; }

    public async Task InitializeAsync()
    {
        var userRegistrationModel = UserRegistrationFaker.Generate();
        await RegisterUserAsync(userRegistrationModel);

        var userLoginModel = userRegistrationModel.ToUserLoginModel();
        await LoginUserAsync(userLoginModel);

        if (UseJwtAuthentication)
            InitializeJwtAuthenticatedClient(JwtToken);

        if (UseApiKeyAuthentication)
        {
            var apiKeyHeaderName = _configuration.GetValue<string>("ApiKeySettings:HeaderName");
            var apiKeyHeaderValue = _configuration.GetValue<string>("ApiKeySettings:HeaderValue");

            if (string.IsNullOrWhiteSpace(apiKeyHeaderName) || string.IsNullOrWhiteSpace(apiKeyHeaderValue))
                throw new ApiKeyClientAuthenticationFailedException(
                    $"ApiKey header name or value is not set in the configuration. Header: {apiKeyHeaderName}, Value: {apiKeyHeaderValue}");

            InitializeApiKeyAuthenticatedClient(apiKeyHeaderName, apiKeyHeaderValue);
        }
    }

    private async Task GetDefaultPocketIdAsync()
    {
        var pocketResponse = await JwtAuthenticatedClient.GetAsync("/api/pockets/default");
        pocketResponse.EnsureSuccessStatusCode();

        var content = await pocketResponse.Content.ReadAsStringAsync();

        var pocket = JsonConvert.DeserializeObject<PocketModel>(content);

        DefaultPocketId = pocket!.Id;
    }

    private async Task RegisterUserAsync(UserRegistrationModel userRegistrationRequest)
    {
        var registerResponse = await _unauthenticatedClient.PostAsJsonAsync("/api/authentication/register", userRegistrationRequest);
        registerResponse.EnsureSuccessStatusCode();
    }

    private async Task LoginUserAsync(UserLoginModel userLoginModel)
    {
        var loginResponse = await _unauthenticatedClient.PostAsJsonAsync("/api/authentication/login", userLoginModel);
        loginResponse.EnsureSuccessStatusCode();

        var tokenModel = await loginResponse.Content.ReadFromJsonAsync<TokenModel>();
        JwtToken = tokenModel ?? throw new JwtClientAuthenticationFailedException("Token was not fetched.");
    }

    private void InitializeJwtAuthenticatedClient(TokenModel token)
    {
        JwtAuthenticatedClient = _filePocketWebAppFactory.CreateClient();
        JwtAuthenticatedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

        JwtTokenUserId = GetUserIdFromJwtToken(token);
    }

    private void InitializeApiKeyAuthenticatedClient(string headerName, string apiKey)
    {
        ApiKeyAuthenticatedClient = _filePocketWebAppFactory.CreateClient();
        ApiKeyAuthenticatedClient.DefaultRequestHeaders.Add(headerName, apiKey);
    }

    private static Guid GetUserIdFromJwtToken(TokenModel token)
    {
        var jwtSecurityToken = JwtSecurityTokenHandler.ReadJwtToken(token.Token);
        return Guid.Parse(jwtSecurityToken.Claims.First(c => c.Type == "uid").Value);
    }

    public Task DisposeAsync()
    {
        _unauthenticatedClient.Dispose();
        JwtAuthenticatedClient.Dispose();
        return Task.CompletedTask;
    }

    // TODO [Issue] Admin seed is not started before tests run, so user might not be added.
    public static async Task<HttpClient> SignAsAdminUsingJwtSecurityAsync(FilePocketWebAppFactory factory)
    {
        var authFixture = new ClientAuthenticationFixture(
            factory,
            useJwtAuthentication: true,
            useApiKeyAuthentication: false);

        var adminSeedingDataModel = factory.Services.CreateScope()
            .ServiceProvider.GetRequiredService<IOptions<AdminSeedingDataModel>>().Value;

        var adminUserLogin = new UserLoginModel { Email = adminSeedingDataModel.Email, Password = adminSeedingDataModel.Password };

        await authFixture.LoginUserAsync(adminUserLogin);
        authFixture.InitializeJwtAuthenticatedClient(authFixture.JwtToken);

        return authFixture.JwtAuthenticatedClient;
    }

    public static async Task<ClientAuthenticationFixture> SignUpUserUsingJwtSecurity(FilePocketWebAppFactory factory, UserRegistrationModel? userRegistrationModel = null)
    {
        var authFixture = new ClientAuthenticationFixture(
            factory,
            useJwtAuthentication: true,
            useApiKeyAuthentication: false);

        userRegistrationModel ??= UserRegistrationFaker.Generate();
        await authFixture.RegisterUserAsync(userRegistrationModel);
        await authFixture.LoginUserAsync(userRegistrationModel.ToUserLoginModel());
        authFixture.InitializeJwtAuthenticatedClient(authFixture.JwtToken);

        await authFixture.GetDefaultPocketIdAsync();

        return authFixture;
    }
}