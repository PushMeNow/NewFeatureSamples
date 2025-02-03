using Refit;
using Refit.Samples.ThirdParties;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRefitClient<ICountriesApiClient>()
       .ConfigureHttpClient((serviceProvider, client) => { client.BaseAddress = new Uri("https://api.country.is"); });

builder.Services.AddRefitClient<IHabrClientInternal>()
       .ConfigureHttpClient((serviceProvider, client) => { client.BaseAddress = new Uri("https://habr.com"); });

builder.Services.AddScoped<HabrClient>();

var app = builder.Build();

app.MapGet("api/countries", (ICountriesApiClient client, CancellationToken cancellationToken) => client.GetCountries(cancellationToken));
app.MapGet("api/habr", (HabrClient client, CancellationToken cancellationToken) => client.GetMain(cancellationToken));

app.Run();
