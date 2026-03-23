using Microsoft.Extensions.DependencyInjection;
using SMP.App;
using SMP.App.Interfaces;
using SMP.App.Service;
using SMP.Infrastructure.Storage;
using SMP.Infrastructure.Tray;
using SMP.Infrastructure.Update;
using SMP.Infrastructure.Youtube;
using SMP.UI;

var services = new ServiceCollection();

// yt-dlp
var exePath = Path.Combine(AppContext.BaseDirectory, "yt-dlp.exe");
services.AddSingleton(new YtDlpService(exePath));

// core
services.AddSingleton<PlayerService>();

// infra
services.AddSingleton<TrayService>();
services.AddSingleton<PlaylistRepository>();
services.AddSingleton<IUpdateService, UpdateService>();

// UI
services.AddSingleton<MainForm>();

var provider = services.BuildServiceProvider();

ApplicationConfiguration.Initialize();
Application.Run(provider.GetRequiredService<MainForm>());