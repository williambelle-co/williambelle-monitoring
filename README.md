# WilliamBelle.Monitoring

[![CI](https://github.com/williambelle-co/williambelle-monitoring/actions/workflows/ci.yml/badge.svg)](https://github.com/williambelle-co/williambelle-monitoring/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/WilliamBelle.Monitoring.svg)](https://www.nuget.org/packages/WilliamBelle.Monitoring)

An in-app monitoring agent for ASP.NET Core applications. It reports, on a
schedule, what only the inside of a running process can see: the runtime it is
actually on, the environment it thinks it is in, and the package versions
actually loaded.

That last one matters most. A repository can say a vulnerable package was
upgraded while production still runs the old version — this is what notices.

## Usage

```csharp
builder.Services.AddWilliamBelleMonitoring(o =>
{
    o.IngestUrl  = builder.Configuration["Monitoring:IngestUrl"]!;
    o.AppId      = builder.Configuration["Monitoring:AppId"]!;
    o.SigningKey = builder.Configuration["Monitoring:SigningKey"]!;  // Key Vault, never source
});
```

The application id and signing key are issued by William Belle LLC.

## What it does, exactly

Every 24 hours by default it collects:

- the runtime servicing level (`RuntimeInformation.FrameworkDescription`)
- the environment name — which catches `Development` running in production
- loaded assembly names and versions

It signs that with HMAC-SHA256 and POSTs it. That is the complete data surface.

## What it deliberately does not do

- **It accepts no inbound anything.** No endpoint, no commands, no remote
  configuration, no code execution. The channel is one-way by construction.
- **It collects no logs, no request payloads, and no user data.**
- **It cannot break the host.** Every reporting cycle is wrapped in
  catch-log-continue; an unreachable ingest endpoint costs the host nothing.
- **It carries no proprietary dependencies.** It ships into other people's
  applications, so it is kept to the smallest possible supply-chain surface:
  two Microsoft.Extensions abstractions and nothing else.

Keeping those properties true is the point of this package.

## Building it yourself

```bash
dotnet test          # the signature scheme and what a snapshot contains
dotnet pack src/WilliamBelle.Monitoring -c Release
```

That suite pins this package's signing implementation to the endpoint that
verifies it, and no release goes out without it passing.

The published package is built by GitHub Actions from a tag, using
[Trusted Publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing) —
no long-lived API key exists for this package. Source Link is enabled, so a
debugger can step straight from the published package into this source.

## Versioning

Pre-1.0 while the reporting contract settles. Breaking changes before 1.0 will
be released as a minor version bump and described in the release notes.

## Support

Issued application ids, signing keys, and questions about a monitored
application: [support@williambelle.co](mailto:support@williambelle.co).

Licensed MIT — read it, audit it, and verify it does what this page says.
