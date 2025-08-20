using LikeICQ.Hubs; // VoiceHub burada tanımlı

var builder = WebApplication.CreateBuilder(args);

// SignalR servislerini ekle
builder.Services.AddSignalR();

var app = builder.Build();

// wwwroot klasöründeki statik dosyaları sun
app.UseStaticFiles();

// Root (/) isteğinde index.html dosyasını aç
app.MapFallbackToFile("index.html");

// VoiceHub bağlantısını ayarla
app.MapHub<VoiceHub>("/voiceHub");

app.Run();
