
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

var secretName = "MyApp/ExternalService";
var client = new AmazonSecretsManagerClient(Amazon.RegionEndpoint.APSouth1);

string secretValue;

try
{
    var request = new GetSecretValueRequest
    {
        SecretId = secretName,
        VersionStage = "AWSCURRENT"
    };

    var response = await client.GetSecretValueAsync(request);
    secretValue = response.SecretString;
}
catch (Exception ex)
{
    Console.WriteLine($"Error fetching secret: {ex.Message}");
    throw;
}

Console.WriteLine(secretValue);


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();


