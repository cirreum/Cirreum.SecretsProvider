namespace Cirreum.SecretsProvider.Tests;

using Cirreum.SecretsProvider.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Unit tests for the secrets provider registrar's collection-scoped registration
/// guards: duplicate instance keys and duplicate endpoints are rejected within one
/// composition but never across compositions (multiple hosts in one process are
/// isolated).
/// </summary>
public class SecretsProviderRegistrarTests {

	[Fact]
	public void RegisterInstance_DuplicateKeyInSameCollection_Throws() {
		var services = new ServiceCollection();
		new TestSecretsRegistrar()
			.RegisterInstance("vault", CreateInstance("https://one.vault.example"), services, new ConfigurationBuilder());

		var act = () => new TestSecretsRegistrar()
			.RegisterInstance("vault", CreateInstance("https://two.vault.example"), services, new ConfigurationBuilder());

		act.Should().Throw<InvalidOperationException>()
			.WithMessage("*vault*");
	}

	[Fact]
	public void RegisterInstance_SameKeyInFreshCollection_DoesNotThrow() {
		// Two hosts composed in one process must be isolated: the guard is
		// collection-scoped, not process-global.
		new TestSecretsRegistrar()
			.RegisterInstance("vault", CreateInstance("https://one.vault.example"), new ServiceCollection(), new ConfigurationBuilder());

		var act = () => new TestSecretsRegistrar()
			.RegisterInstance("vault", CreateInstance("https://one.vault.example"), new ServiceCollection(), new ConfigurationBuilder());

		act.Should().NotThrow();
	}

	[Fact]
	public void RegisterInstance_DuplicateEndpointInSameCollection_Throws() {
		var services = new ServiceCollection();
		new TestSecretsRegistrar()
			.RegisterInstance("primary", CreateInstance("https://shared.vault.example"), services, new ConfigurationBuilder());

		var act = () => new TestSecretsRegistrar()
			.RegisterInstance("secondary", CreateInstance("https://shared.vault.example"), services, new ConfigurationBuilder());

		act.Should().Throw<InvalidOperationException>()
			.WithMessage("*endpoint*");
	}

	[Fact]
	public void RegisterInstance_SameEndpointInFreshCollection_DoesNotThrow() {
		new TestSecretsRegistrar()
			.RegisterInstance("primary", CreateInstance("https://shared.vault.example"), new ServiceCollection(), new ConfigurationBuilder());

		var act = () => new TestSecretsRegistrar()
			.RegisterInstance("secondary", CreateInstance("https://shared.vault.example"), new ServiceCollection(), new ConfigurationBuilder());

		act.Should().NotThrow();
	}

	private static SecretsProviderInstanceSettings CreateInstance(string endpoint) => new() {
		Endpoint = endpoint,
	};

}
