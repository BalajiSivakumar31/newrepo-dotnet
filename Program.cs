using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Load secret at startup
var secretJson = await GetSecretAsync();
var secrets = JsonSerializer.Deserialize<Dictionary<string, string>>(secretJson);

// Example: add secrets to configuration
if (secrets != null)
{
    foreach (var kv in secrets)
    {
        builder.Configuration[kv.Key] = kv.Value;
    }
}

// Add services
builder.Services.AddRazorPages();

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.Run();


// ---------------- HELPER METHOD ----------------

static async Task<string> GetSecretAsync()
{
    string secretName = "MyApp/ExternalService";
    string region = "ap-south-2";

    using var client = new AmazonSecretsManagerClient(
        RegionEndpoint.GetBySystemName(region));

    var request = new GetSecretValueRequest
    {
        SecretId = secretName,
        VersionStage = "AWSCURRENT"
    };

    try
    {
        var response = await client.GetSecretValueAsync(request);
        return response.SecretString 
               ?? throw new Exception("SecretString is null");
    }
    catch
    {
        // preserves stack trace
        throw;
    }
}

