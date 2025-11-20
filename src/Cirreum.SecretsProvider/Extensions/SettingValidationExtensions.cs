namespace Cirreum.SecretsProvider.Configuration;

using System.Collections.Generic;

/// <summary>
/// Provides extension methods for validating secrets provider instance settings.
/// </summary>
/// <remarks>
/// This class contains validation logic to ensure endpoint uniqueness across provider instances
/// and prevent duplicate registrations using SHA256 hashing for endpoint comparison.
/// </remarks>
public static class SettingValidationExtensions {

	private static readonly Dictionary<string, string> processedEndpoints = [];

	/// <summary>
	/// Validates that the endpoint is properly configured and not already registered by another instance.
	/// </summary>
	/// <param name="settings">The instance settings containing the endpoint to validate.</param>
	/// <param name="key">The unique key identifying this provider instance.</param>
	/// <param name="providerServiceType">The type of provider service being registered.</param>
	/// <param name="providerServiceName">The name of the provider service being registered.</param>
	/// <exception cref="InvalidOperationException">
	/// Thrown when the endpoint is missing, cannot be resolved, or has already been registered by another instance.
	/// </exception>
	/// <remarks>
	/// This method ensures endpoint uniqueness by creating a SHA256 hash of the endpoint string
	/// and tracking it in a static dictionary to prevent duplicate registrations across provider instances.
	/// </remarks>
	public static void ValidateEndpoint(
		this SecretsProviderInstanceSettings settings,
		string key,
		ProviderType providerServiceType,
		string providerServiceName) {

		var endpoint = settings.Endpoint;
		if (string.IsNullOrWhiteSpace(endpoint)) {
			throw new InvalidOperationException(
				$"The 'Endpoint' is missing for service instance '{key}'");
		}

		var endpointHash = GetEndpointHash(endpoint);

		if (string.IsNullOrWhiteSpace(endpointHash)) {
			throw new InvalidOperationException(
				"A service could not be configured. Unable to resolve an 'Endpoint'");
		}

		var endpointKey = $"Cirreum.{providerServiceType}.{providerServiceName}.Connections:{endpointHash}";

		if (!processedEndpoints.TryAdd(endpointKey, key)) {
			throw new InvalidOperationException(
				$"An endpoint string for service instance '{key}' has already been configured. Cannot register the same endpoint with multiple instances.");
		}

	}

	/// <summary>
	/// Creates a SHA256 hash of the connection string to generate a consistent, unique identifier for endpoint comparison.
	/// </summary>
	/// <param name="connectionString">The connection string to hash.</param>
	/// <returns>A Base64-encoded SHA256 hash of the connection string.</returns>
	/// <remarks>
	/// This method is used internally to create unique identifiers for endpoints that allow for
	/// duplicate detection without exposing sensitive connection string information in error messages.
	/// </remarks>
	private static string GetEndpointHash(string connectionString) {
		// Use a hash function to create a consistent, unique identifier
		var bytes = System.Text.Encoding.UTF8.GetBytes(connectionString);
		var hash = System.Security.Cryptography.SHA256.HashData(bytes);
		return Convert.ToBase64String(hash);
	}

}