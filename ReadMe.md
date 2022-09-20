# AWS Lambda .NET Benchmark

This repository is used for benchmarking the [Amazon.Lambda.RuntimeSupport](https://github.com/aws/aws-lambda-dotnet/tree/master/Libraries/src/Amazon.Lambda.RuntimeSupport) package, which is used by AWS Lambda .NET functions.

The repository contains two projects:
1. [Benchmark.Amazon.Lambda.EndpointServer](src/Benchmark.Amazon.Lambda.EndpointServer/): a mock implementation of the AWS Lambda service endpoint
1. [Benchmark.Amazon.Lambda.RuntimeSupport](src/Benchmark.Amazon.Lambda.RuntimeSupport/): Benchmark project for _Amazon.Lambda.RuntimeSupport_

First launch the AWS Lambda Endpoint server, then perform the benchmark.

## Benchmark.Amazon.Lambda.EndpointServer

This project is a mock implementation of the AWS Lambda service. It runs as an ASP.NET Core Minimal API application
 on localhost:8080 used by the benchmark.

The implementation provides four endpoints:
* `/2018-06-01/runtime/invocation/next`: This endpoint returns the payload for the next Lambda invocation request.
* `/2018-06-01/runtime/invocation/{awsRequestId}/response`: This endpoint receives the response of a successful Lambda invocation.
* `/2018-06-01/runtime/invocation/{awsRequestId}/error`: This endpoint receives the error message of a failed Lambda invocation.
* `/2018-06-01/runtime/init/error`: This endpoint receives the error message of a failed Lambda initialization.

## Benchmark.Amazon.Lambda.RuntimeSupport

This project benchmarks the `LambdaBootstrapBuilder` class of the `Amazon.Lambda.RuntimeSupport` package. This class is responsible for interfacing between a .NET Lambda function and the AWS Lambda service.

