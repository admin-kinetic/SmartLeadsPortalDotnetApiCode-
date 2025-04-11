using System;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;

namespace SmartLeadsPortalDotNetApi.Factories;

public class MicrosoftGraphServiceClientFactory
{
    private readonly MicrosoftGraphAuthProvider _authProvider;

    public MicrosoftGraphServiceClientFactory(MicrosoftGraphAuthProvider authProvider)
    {
        _authProvider = authProvider;
    }

    public async Task<GraphServiceClient?> GetAuthenticatedGraphClient()
    {
        try
        {
            var accessToken = await _authProvider.GetAccessTokenAsync();

            // Create the authentication provider
            var authProvider = new BaseBearerTokenAuthenticationProvider(
                new TokenProvider(accessToken)
            );

            return new GraphServiceClient(authProvider);
        }
        catch (Exception ex)
        {
            // Consider logging the exception here
            return null;
        }
    }

    // Helper class to provide the token
    private class TokenProvider : IAccessTokenProvider
    {
        private readonly string _accessToken;

        public TokenProvider(string accessToken)
        {
            _accessToken = accessToken;
        }

        public Task<string> GetAuthorizationTokenAsync(
            Uri uri,
            Dictionary<string, object>? additionalAuthenticationContext = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_accessToken);
        }

        public AllowedHostsValidator AllowedHostsValidator { get; } = new();
    }
}
