Demo project showing the usage .NET 6 with OpenTelemetry .NET SDK 1.8.1.

Additionally, this project has a sample log processor (`CustomLogProcessor`) that will conditionally mark traces as having errors based on the log level.

This demo will export to the console, and export telemetry via OTLP to a local collector running on the default ports.

To bypass collector instance and send directly to Grafana Cloud, follow the instructions in the [Grafana Cloud .NET quickstart guide](https://grafana.com/docs/grafana-cloud/monitor-applications/application-observability/setup/quickstart/dotnet/#configure-an-application) to obtain the needed configuration values (protocol, endpoint, access policy token).

If running on Windows, modify the provided environment variables to use `$env:` instead of `export`.

From shell format:
```shell
export OTEL_EXPORTER_OTLP_PROTOCOL="http/protobuf"
export OTEL_EXPORTER_OTLP_ENDPOINT="https://example.com/otlp"
export OTEL_EXPORTER_OTLP_HEADERS="Authorization=Basic your-access-policy-token"
```

To PowerShell format:
```powershell
$env:OTEL_EXPORTER_OTLP_PROTOCOL="http/protobuf"
$env:OTEL_EXPORTER_OTLP_ENDPOINT="https://example.com/otlp"
$env:OTEL_EXPORTER_OTLP_HEADERS="Authorization=Basic your-access-policy-token"
```

To run this project locally, clone this repository and execute the following command in the project directory:
```shell
dotnet run
```
