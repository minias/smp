// File: Program.cs

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

// yt-dlp 실행 파일 경로
var exePath = Path.Combine(AppContext.BaseDirectory, "yt-dlp.exe");

// ✅ 생성자 시그니처에 맞게 exePath만 전달
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