using LikeICQ.Hubs; // VoiceHub burada tanımlı
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// SignalR servisini ekle
builder.Services.AddSignalR();

// Static dosyaları (index.html) okumak için
builder.Services.AddControllersWithViews();

var app = builder.Build();

// wwwroot klasöründeki dosyaları sun
app.UseStaticFiles();

app.MapGet("/", () => Results.Redirect("/index.html"));

// SignalR hub route
app.MapHub<VoiceHub>("/hub/voice");

app.Run();
