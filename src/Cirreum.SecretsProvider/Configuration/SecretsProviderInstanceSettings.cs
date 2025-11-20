namespace Cirreum.SecretsProvider.Configuration;

using Cirreum.Providers.Configuration;

/// <summary>
/// Base class for individual secrets provider instance configuration settings.
/// </summary>
/// <remarks>
/// This class provides the fundamental configuration properties required by all secrets provider instances,
/// including endpoint configuration and optional identifier. Derived classes can extend this to add
/// provider-specific settings while maintaining compatibility with the registration framework.
/// </remarks>
public class SecretsProviderInstanceSettings
	: IProviderInstanceSettings {

	/// <summary>
	/// Gets or sets an Id associated with the provider, if required.
	/// </summary>
	public string? Identifier { get; set; }

	/// <summary>
	/// Gets or sets the uri/url/arn etc. of where the provider connects to.
	/// </summary>
	public string Endpoint { get; set; } = "";

	/// <summary>
	/// Allow the implementor the opportunity to parse the <see cref="Endpoint"/> value.
	/// </summary>
	/// <remarks>
	/// <para>
	/// By default, this is a no-op function unless the implementor provides an override.
	/// </para>
	/// </remarks>
	public virtual void ParseEndPoint() {
		// No-Op
	}

}