using Gas.CosmosDb;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("local.settings.json", false, true);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ICosmosDbService>(new CosmosDbService(
    builder.Configuration.GetConnectionString("CosmosDb"),
    builder.Configuration.GetValue<string>("CosmosDb:DatabaseName"),
    (b) => b.AddContainer(builder.Configuration.GetValue<string>("CosmosDb:UserContainer"))));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();

