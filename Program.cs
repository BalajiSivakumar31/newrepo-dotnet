using Amazon.Extensions.Configuration.SecretsManager;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Load appsettings.json
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// 2️⃣ Load AWS Secrets Manager
builder.Configuration.AddSecretsManager(options =>
{
    options.SecretFilter = entry => entry.Name == "MyApp/ExternalService";
    options.KeyGenerator = (entry, key) => key;
});



var app = builder.Build();

// 4️⃣ Configure middleware
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
