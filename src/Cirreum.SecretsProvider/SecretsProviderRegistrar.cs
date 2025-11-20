namespace Cirreum.SecretsProvider;

using Cirreum.SecretsProvider.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Abstract base class for implementing secrets provider registrars that follow the Cirreum Framework provider pattern.
/// </summary>
/// <typeparam name="TSettings">The provider-level settings type that inherits from <see cref="SecretsProviderSettings{TInstanceSettings}"/>.</typeparam>
/// <typeparam name="TInstanceSettings">The instance-level settings type that inherits from <see cref="SecretsProviderInstanceSettings"/>.</typeparam>
/// <remarks>
/// This class provides the foundation for implementing secrets providers within the Cirreum ecosystem.
/// It handles multi-instance registration, validation, duplicate detection, and OpenTelemetry tracing integration.
/// </remarks>
public abstract class SecretsProviderRegistrar<TSettings, TInstanceSettings>
	: IProviderRegistrar<TSettings, TInstanceSettings>
	where TInstanceSettings : SecretsProviderInstanceSettings
	where TSettings : SecretsProviderSettings<TInstanceSettings> {

	private static readonly Dictionary<string, string> processedInstances = [];

	/// <inheritdoc/>
	public abstract ProviderType ProviderType { get; }

	/// <inheritdoc/>
	public abstract string ProviderName { get; }

	/// <inheritdoc/>
	public virtual void ValidateSettings(TInstanceSettings settings) {
	}

	/// <summary>
	/// The activity sources for use with Diagnostics and logging.
	/// </summary>
	public abstract string[] ActivitySourceNames { get; }

	/// <summary>
	/// Registers all of the provider's configured service implementations with the configuration manager.
	/// </summary>
	/// <param name="providerSettings">An instance of the provider-specific settings populated from application settings.</param>
	/// <param name="services">The DI container's service collection where other services will be registered.</param>
	/// <param name="builder">The root configuration object providing access to the full application configuration and where the secrets provider will be added.</param>
	/// <remarks>
	/// This method performs the complete service registration process:
	/// <list type="bullet">
	///   <item>
	///     <description>Reads provider-specific configuration settings</description>
	///   </item>
	///   <item>
	///     <description>Registers service implementations with their appropriate DI lifetimes (Singleton, Scoped, or Transient)</description>
	///   </item>
	///   <item>
	///     <description>Configures and initializes any required service dependencies</description>
	///   </item>
	///   <item>
	///     <description>Adds its configuration provider to the configuration manager for subsequent configuration access.</description>
	///   </item>
	/// </list>
	/// </remarks>
	public virtual void Register(
		TSettings providerSettings,
		IServiceCollection services,
		IConfigurationBuilder builder) {

		if (providerSettings is null || providerSettings.Instances.Count == 0) {
			return;
		}

		foreach (var (key, settings) in providerSettings.Instances) {
			this.RegisterInstance(key, settings, services, builder);
		}

		if (providerSettings.Tracing && this.ActivitySourceNames is not null && this.ActivitySourceNames.Length > 0) {
			services.AddOpenTelemetry()
				.WithTracing(traceBuilder => traceBuilder.AddSource(this.ActivitySourceNames));
		}

	}

	/// <summary>
	/// Registers a single provider instance with the dependency injection container.
	/// </summary>
	/// <param name="key">The unique identifier for this provider instance, typically derived from configuration.</param>
	/// <param name="settings">The configuration settings specific to this provider instance.</param>
	/// <param name="services">The DI container's service collection where services will be registered.</param>
	/// <param name="builder">The root configuration object providing access to the full application configuration.</param>
	/// <remarks>
	/// This method handles the registration of an individual service provider instance:
	/// <list type="bullet">
	///   <item>
	///     <description>Validates the instance-specific settings</description>
	///   </item>
	///   <item>
	///     <description>Registers the instance's services with appropriate DI lifetimes</description>
	///   </item>
	///   <item>
	///     <description>Configures instance-specific dependencies and health checks</description>
	///   </item>
	/// </list>
	/// This method is called by <see cref="Register"/> for each configured instance, but can also be used
	/// independently to register single instances when needed.
	/// </remarks>
	public virtual void RegisterInstance(
		string key,
		TInstanceSettings settings,
		IServiceCollection services,
		IConfigurationBuilder builder) {

		// Ensure no duplicate registration keys
		var providerRegistrationKey = $"Cirreum.{this.ProviderType}.{this.ProviderName}::{key}";
		if (!processedInstances.TryAdd(providerRegistrationKey, settings.Endpoint)) {
			throw new InvalidOperationException($"A service with the key of '{key}' has already been registered.");
		}

		// Must have settings...
		if (settings is null) {
			throw new InvalidOperationException($"Missing required settings for the service '{key}'");
		}

		// Get and/or parse connection string
		if (!string.IsNullOrWhiteSpace(settings.Endpoint)) {
			settings.ParseEndPoint();
		} else {
			// Must have a configured endpoint
			throw new InvalidOperationException($"Missing required endpoint.");
		}

		// Validate Endpoint regardless of provider specific validation
		settings.ValidateEndpoint(key, this.ProviderType, this.ProviderName);

		// Provider specific validation...
		this.ValidateSettings(settings);

		// Add the ServiceProvider...
		this.AddSecretsProviderInstance(builder, settings);

	}

	/// <summary>
	/// Adds the secrets provider instance to the configuration builder.
	/// </summary>
	/// <param name="builder">The configuration builder to add the secrets provider to.</param>
	/// <param name="providerSettings">The instance settings containing the configuration for this specific provider instance.</param>
	/// <remarks>
	/// This method is responsible for integrating the secrets provider into the .NET configuration system.
	/// Implementation should add the provider as a configuration source, allowing secrets to be accessed
	/// through the standard IConfiguration interface.
	/// </remarks>
	public abstract void AddSecretsProviderInstance(
		IConfigurationBuilder builder,
		TInstanceSettings providerSettings);

}