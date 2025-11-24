using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using OaigLoan.Bff.Contracts.Auth;
using OaigLoan.Bff.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<BackendOptions>(builder.Configuration.GetSection(BackendOptions.SectionName));

builder.Services.AddHttpClient("BackendApi", (sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<BackendOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(_ => true);
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseCors("frontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var authGroup = app.MapGroup("/bff/auth");

authGroup.MapPost("/register", async Task<IResult> (
    RegisterRequest request,
    IHttpClientFactory httpClientFactory,
    CancellationToken cancellationToken) =>
{
    return await ForwardPost("/api/auth/register", request, httpClientFactory, cancellationToken);
});

authGroup.MapPost("/login", async Task<IResult> (
    LoginRequest request,
    IHttpClientFactory httpClientFactory,
    CancellationToken cancellationToken) =>
{
    return await ForwardPost("/api/auth/login", request, httpClientFactory, cancellationToken);
});

app.MapHealthChecks("/health");
app.Run();

static async Task<IResult> ForwardPost<TPayload>(
    string path,
    TPayload payload,
    IHttpClientFactory httpClientFactory,
    CancellationToken cancellationToken)
{
    var client = httpClientFactory.CreateClient("BackendApi");
    var response = await client.PostAsJsonAsync(path, payload, cancellationToken);
    var body = await response.Content.ReadAsStringAsync();
    var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";
    return Results.Content(body, contentType, statusCode: (int)response.StatusCode);
}
