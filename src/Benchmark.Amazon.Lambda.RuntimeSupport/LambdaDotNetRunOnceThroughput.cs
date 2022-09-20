using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;

namespace Benchmark.Amazon.Lambda.RuntimeSupport;

[Config(typeof(Config))]
public class LambdaDotNetRunOnceThroughput
{
    private class Config : ManualConfig
    {
        public Config()
        {
            AddDiagnoser(MemoryDiagnoser.Default);
            AddJob(
                Job.Default
                    .WithEnvironmentVariables(
                        new EnvironmentVariable("AWS_LAMBDA_DOTNET_DEBUG_RUN_ONCE", "true"),
                        new EnvironmentVariable("AWS_LAMBDA_RUNTIME_API", "localhost:8080")
                    )
                    .WithNuGet("Amazon.Lambda.RuntimeSupport", "1.7.0")
                    .WithId("Legacy")
            );
            AddJob(
                Job.Default
                    .AsBaseline()
                    .WithEnvironmentVariables(
                        new EnvironmentVariable("AWS_LAMBDA_DOTNET_DEBUG_RUN_ONCE", "true"),
                        new EnvironmentVariable("AWS_LAMBDA_RUNTIME_API", "localhost:8080")
                    )
                    .WithNuGet("Amazon.Lambda.RuntimeSupport", "1.8.2")
                    .WithId("Latest")
            );
        }
    }

    [Benchmark]
    public async Task LambdaRuntime()
    {
        await LambdaBootstrapBuilder.Create(Handler, new DefaultLambdaJsonSerializer())
            .Build()
            .RunAsync();
        Task Handler(Stream request, ILambdaContext context) => Task.CompletedTask;
    }
}
