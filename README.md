# WilliamBelle.Monitoring

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
- **It has no William Belle dependencies.** It ships into other people's
  applications, so it must be the smallest possible supply-chain surface. The
  HMAC signer is duplicated from the portal rather than shared, and a
  round-trip test in `WilliamBelle.Portal.Tests` pins the two implementations
  to each other — change one and the build fails until both match.

Keeping those properties true is the point of this package. Anything that would
give it an inbound channel or a dependency needs a very good reason.

## Naming

The `WilliamBelle.*` prefix is the established pattern for packages published by
William Belle LLC. This one is named for what it does rather than carrying a
product brand — see the decision log in
`docs/practice-platform/product-strategy.md`.
