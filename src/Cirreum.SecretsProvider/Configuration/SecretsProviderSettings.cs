namespace Cirreum.SecretsProvider.Configuration;

using Cirreum.Providers.Configuration;

/// <summary>
/// Abstract base class for provider-level settings that contain multiple instances and global configuration options.
/// </summary>
/// <typeparam name="TInstanceSettings">The type of instance settings that inherits from <see cref="SecretsProviderInstanceSettings"/>.</typeparam>
/// <remarks>
/// This class serves as the root configuration object for secrets providers, containing a collection of 
/// provider instances and provider-wide settings such as tracing configuration.
/// </remarks>
public abstract class SecretsProviderSettings<TInstanceSettings>
	: IProviderSettings<TInstanceSettings>
	where TInstanceSettings : SecretsProviderInstanceSettings {

	/// <summary>
	/// Gets or sets a boolean value that indicates whether the OpenTelemetry tracing is enabled or not.
	/// </summary>
	/// <value>
	/// The default value is <see langword="true"/>.
	/// </value>
	public bool Tracing { get; set; } = true;

	/// <summary>
	/// Collection of Provider instance settings
	/// </summary>
	public Dictionary<string, TInstanceSettings> Instances { get; set; } = [];

}