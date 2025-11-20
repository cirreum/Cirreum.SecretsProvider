# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Cirreum.SecretsProvider** is a foundational library providing abstractions and registration patterns for implementing secrets providers within the Cirreum Framework ecosystem. It follows the same registration pattern as authorization providers but focuses on secrets management and configuration.

## Build Commands

```bash
# Restore dependencies
dotnet restore Cirreum.SecretsProvider.slnx

# Build the solution
dotnet build Cirreum.SecretsProvider.slnx --configuration Release

# Pack for NuGet
dotnet pack Cirreum.SecretsProvider.slnx --configuration Release --output ./artifacts
```

## Key Architecture

### Core Components

- **SecretsProviderRegistrar<TSettings, TInstanceSettings>**: Abstract base class that implements `IProviderRegistrar` for standardized secrets provider registration
- **SecretsProviderSettings<TInstanceSettings>**: Base class for provider-level settings with tracing configuration and instance collections
- **SecretsProviderInstanceSettings**: Base class for individual instance configurations with endpoint validation

### Provider Registration Pattern

The library follows a multi-instance registration pattern where:

1. **Provider-level settings** contain multiple instances and global configuration (like tracing)
2. **Instance-level settings** contain endpoint and instance-specific configuration
3. **Duplicate prevention** ensures no two instances can register the same endpoint or registration key
4. **Validation framework** validates both endpoint connectivity and provider-specific settings

### Key Files

- `SecretsProviderRegistrar.cs:52-70` - Main registration method that processes all instances and configures tracing
- `SecretsProviderRegistrar.cs:95-129` - Individual instance registration with validation
- `SettingValidationExtensions.cs:9-35` - Endpoint validation and duplicate detection using SHA256 hashing

## Technology Stack

- **.NET 10.0** with latest C# language features
- **Microsoft.Extensions.Hosting.Abstractions** for dependency injection
- **OpenTelemetry.Extensions.Hosting** for observability
- **Cirreum.Providers** for base provider contracts

## Configuration Structure

Secrets providers are configured with hierarchical settings:

```json
{
  "MySecretsProvider": {
    "Tracing": true,
    "Instances": {
      "instance1": {
        "Endpoint": "provider-specific-connection-string",
        "Identifier": "optional-id"
      }
    }
  }
}
```

## Implementation Requirements

When implementing a secrets provider:

1. Inherit from `SecretsProviderRegistrar<TSettings, TInstanceSettings>`
2. Implement `AddSecretsProviderInstance()` to add the provider to the configuration builder
3. Override `ValidateSettings()` for provider-specific validation
4. Define `ActivitySourceNames` for OpenTelemetry tracing
5. Ensure instance settings inherit from `SecretsProviderInstanceSettings`

## Validation and Safety

- **Endpoint validation**: SHA256 hashing prevents duplicate endpoints across instances
- **Registration key validation**: Prevents duplicate provider registrations
- **Required endpoint**: All instances must have a valid endpoint configuration
- **Provider-specific validation**: Custom validation via `ValidateSettings()` override