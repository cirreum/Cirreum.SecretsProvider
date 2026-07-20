# Cirreum.SecretsProvider Changelog

All notable changes to **Cirreum.SecretsProvider** are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.22] - 2026-07-20

### Fixed

- The registrar's duplicate-registration guards were process-global static state
  (instance keys and endpoint hashes): a second host composed in the same process
  (the integration-test norm) was rejected for re-using an instance key or endpoint
  its own composition had never seen. All guard state now lives in the service
  collection, so hosts are fully isolated (ADR-0028 principle; same defect class
  fixed in `Cirreum.AuthenticationProvider` 1.3.0 and `Cirreum.IdentityProvider`
  1.0.8). The public `ValidateEndpoint` extension gains an `IServiceCollection`
  parameter to carry the composition scope — source-breaking in the strict sense,
  shipped as a patch because the method's only caller is the registrar base itself
  and no external consumers exist (same precedent as the 1.1.3
  `AddCoordination` → `ConfigureCoordination` rename in
  `Cirreum.AuthenticationProvider`).
- First test suite (`tests/Cirreum.SecretsProvider.Tests.slnx`): the registration
  guards and cross-composition isolation.

## [1.0.21] - 2026-07-18

### Updated

- Updated NuGet packages.

## [1.0.20] - 2026-07-04

### Updated

- Updated NuGet packages.

## [1.0.19] - 2026-07-04

### Updated

- Updated NuGet packages.

## [1.0.18] - 2026-06-05

### Fixed

- Updated dependencies to their latest versions to pick up upstream fixes:
  - `Cirreum.Providers` `1.1.1` → `1.2.1`
  - `Microsoft.Extensions.Hosting.Abstractions` `10.0.7` → `10.0.8`

## [1.0.17]

Baseline entry. Prior history predates this changelog.
