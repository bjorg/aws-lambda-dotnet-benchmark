using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var lambdaRequestCounter = 0;

// accept connections only from localhost on port 8080
app.Urls.Add("http://localhost:8080");

// Endpoint to verify that the server is running.
app.MapGet("/", () => "Lambda endpoint server is running");

// Endpoint to return the next request payload of a Lambda invocation.
app.MapGet("/2018-06-01/runtime/invocation/next", (HttpRequest request, HttpResponse response) =>
{
    app.Logger.LogDebug($"Next: {++lambdaRequestCounter}");

    // add headers with Lambda request context
    response.Headers.Add("Lambda-Runtime-Aws-Request-Id", lambdaRequestCounter.ToString());
    response.Headers.Add("Lambda-Runtime-Trace-Id", "1234567890");
    response.Headers.Add("Lambda-Runtime-Client-Context", "{}");
    response.Headers.Add("Lambda-Runtime-Cognito-Identity", "{}");
    response.Headers.Add("Lambda-Runtime-Deadline-Ms", "30000");
    response.Headers.Add("Lambda-Runtime-Invoked-Function-Arn", "arn:aws:lambda:us-east-1:123456789012:function:my-function");

    // add Lambda request payload
    return response.WriteAsync("{}");
});

// Endpoint to receive the response of a successful Lambda invocation.
app.MapPost("/2018-06-01/runtime/invocation/{awsRequestId}/response", async (string awsRequestId, HttpRequest request, HttpResponse response) =>
{
    app.Logger.LogDebug($"Response for '{awsRequestId}': {await ReadRequestBodyAsync(request).ConfigureAwait(false)}");
    return Results.Accepted(value: new StatusResponse
    {
        Status = "ok"
    });
});

// Endpoint to receive the error message of a failed Lambda invocation.
app.MapPost("/2018-06-01/runtime/invocation/{awsRequestId}/error", async (string awsRequestId, HttpRequest request, HttpResponse response) =>
{
    app.Logger.LogDebug($"Error for '{awsRequestId}': {await ReadRequestBodyAsync(request).ConfigureAwait(false)}");
    return Results.Accepted();
});

// Endpoint to receive the error message of a failed Lambda initialization.
app.MapPost("/2018-06-01/runtime/init/error", async (string awsRequestId, HttpRequest request, HttpResponse response) =>
{
    app.Logger.LogDebug($"InitError for '{awsRequestId}': {await ReadRequestBodyAsync(request).ConfigureAwait(false)}");
    return Results.Accepted(value: new StatusResponse
    {
        Status = "ok"
    });
});

app.Run();

// local functions
async Task<string> ReadRequestBodyAsync(HttpRequest request)
{
    using var reader = new StreamReader(request.Body, Encoding.UTF8);
    return await reader.ReadToEndAsync().ConfigureAwait(false);
}

// types
internal partial class StatusResponse
{

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

internal partial class ErrorResponse
{
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }
    [JsonPropertyName("errorType")]
    public string? ErrorType { get; set; }
}
