namespace Cirreum.SecretsProvider.Tests;

using Cirreum.SecretsProvider;
using Cirreum.SecretsProvider.Configuration;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Concrete test doubles for exercising
/// <see cref="SecretsProviderRegistrar{TSettings, TInstanceSettings}"/> registration
/// behavior.
/// </summary>
internal sealed class TestSecretsSettings : SecretsProviderSettings<SecretsProviderInstanceSettings> {
}

internal sealed class TestSecretsRegistrar : SecretsProviderRegistrar<TestSecretsSettings, SecretsProviderInstanceSettings> {

	public override string ProviderName => "TestSecrets";

	public override string[] ActivitySourceNames => [];

	public override void AddSecretsProviderInstance(
		IConfigurationBuilder builder,
		SecretsProviderInstanceSettings providerSettings) {
	}

}
