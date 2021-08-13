using IdentityServer4Templates;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()          //This is for dev only scenarios when you don¡¯t have a certificate to use.
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryClients(Config.Clients);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseDeveloperExceptionPage();


app.UseIdentityServer();
app.Run();
 
