using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using like_icq.Hubs;

var builder = WebApplication.CreateBuilder(args);

// SignalR servisini ekle
builder.Services.AddSignalR();

var app = builder.Build();

// wwwroot klasöründen statik dosyaları sun
app.UseStaticFiles();

// SignalR Hub'ı bağla
app.MapHub<VoiceHub>("/voicehub");

// index.html'e yönlendir
app.MapFallbackToFile("index.html");

// 🔹 Render için port ayarı
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

app.Run();
