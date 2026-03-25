// /UI/Program.cs

using Microsoft.Extensions.DependencyInjection;
using NAudio.Wave;
using SMP.App.Interfaces;
using SMP.App.Service;
using SMP.App.UseCases;
using SMP.Infrastructure.Audio;
using SMP.Infrastructure.Input;
using SMP.Infrastructure.Storage;
using SMP.Infrastructure.Tray;
using SMP.Infrastructure.Update;
using SMP.Infrastructure.Youtube;
using SMP.UI;

var services = new ServiceCollection();

// yt-dlp 실행 파일 경로
var exePath = Path.Combine(AppContext.BaseDirectory, "yt-dlp.exe");


// =========================
// Infrastructure
// =========================
services.AddSingleton<MediaKeyPatternDetector>();
services.AddSingleton<MediaKeyListener>();

// =========================
// Interface
// =========================
services.AddSingleton<IYoutubeService>(sp => new YtDlpService(exePath));

services.AddSingleton<IWavePlayer, WaveOutEvent>();
services.AddSingleton<IAudioPlayer, StreamingAudioPlayer>();
services.AddSingleton<ITrayService, TrayService>();
services.AddSingleton<IPlaylistRepository, PlaylistRepository>();

// (선택) 업데이트
services.AddSingleton<IUpdateService, UpdateService>();


// =========================
// App.Service
// =========================
services.AddSingleton<PlayerState>();
services.AddSingleton<PlayerFacade>();

// =========================
// UseCase
// =========================
services.AddSingleton<PlayUseCase>();
services.AddSingleton<PlayPauseUseCase>();
services.AddSingleton<StopUseCase>();
services.AddSingleton<PrevTrackUseCase>();
services.AddSingleton<NextTrackUseCase>();
services.AddSingleton<SetPlaylistUseCase>();
services.AddSingleton<SetVolumeUseCase>();
services.AddSingleton<SetLoopModeUseCase>();

// =========================
// UI
// =========================
services.AddSingleton<MainForm>();


var provider = services.BuildServiceProvider();

ApplicationConfiguration.Initialize();
Application.Run(provider.GetRequiredService<MainForm>());