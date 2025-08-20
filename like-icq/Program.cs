using like_icq.Hubs;

var builder = WebApplication.CreateBuilder(args);

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// wwwroot (index.html vb.)
app.UseDefaultFiles();
app.UseStaticFiles();

// Hub
app.MapHub<VoiceHub>("/voicehub");

app.Run();
