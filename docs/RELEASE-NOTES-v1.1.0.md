# Cirreum.SecretsProvider 1.1.0 — The Credential Block Arrives on the Instance Base

## Why this release exists

Secrets provider implementations that authenticate with a platform identity have had no shared
configuration surface for *which* identity or *which* strategy — each implementation invented its
own properties, or hardcoded a default credential. `Cirreum.Providers` 1.3.0 introduced a
vendor-neutral credential vocabulary (`CredentialMode` + `CredentialSettings`); this release
surfaces it on the secrets instance base so every implementation exposes the same shape.

## What's new

`SecretsProviderInstanceSettings` gains one property:

```csharp
public CredentialSettings? Credential { get; set; }
```

configured as a nested block alongside the existing `Endpoint` and `Identifier`:

```json
"Instances": {
  "default": {
    "Endpoint": "<provider endpoint>",
    "Identifier": "<optional provider-interpreted id>",
    "Credential": { "Mode": "ManagedIdentity", "IdentityId": "<platform identity id>" }
  }
}
```

The property is nullable by design. Absent means the implementation's default credential behavior —
existing configurations are untouched. Present, it selects `Default` (the platform's default
credential chain), `ManagedIdentity` (the platform-assigned workload identity, deterministic), or
`Developer` (developer tooling credentials only), with `IdentityId` selecting a specific platform
identity when the host has several. Implementations document their mapping; key-only providers
ignore the block entirely.

## Compatibility

Purely additive — no existing property, method, or configuration key changes. The block takes
effect per implementation as each adopts it; the Azure Key Vault provider is the first, in its
next major.

## See also

- [CHANGELOG](CHANGELOG.md)
- [Cirreum.Providers — Credential Configuration](https://github.com/cirreum/Cirreum.Providers#credential-configuration)
