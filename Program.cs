using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;

const string secretName = "MyApp/ExternalService";
const string region = "ap-south-2";

var builder = WebApplication.CreateBuilder(args);

// --- Load AWS Secret ---
var client = new AmazonSecretsManagerClient(Amazon.RegionEndpoint.GetBySystemName(region));
try
{
    var secretValue = await client.GetSecretValueAsync(new GetSecretValueRequest
    {
        SecretId = secretName
    });

    if (!string.IsNullOrEmpty(secretValue.SecretString))
    {
        var secretJson = JsonSerializer.Deserialize<Dictionary<string, string>>(secretValue.SecretString);
        if (secretJson != null)
        {
            // Override configuration with secret values
            builder.Configuration.AddInMemoryCollection(secretJson);
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error fetching secret from AWS Secrets Manager: {ex.Message}");
}

// --- Add services to the container ---
builder.Services.AddRazorPages();

var app = builder.Build();

// --- Configure the HTTP request pipeline ---
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
