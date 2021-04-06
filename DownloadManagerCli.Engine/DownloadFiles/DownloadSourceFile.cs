namespace DownloadManagerCli.Engine.DownloadFiles
{
    using DownloadManagerCli.Abstraction.Interfaces;
    using DownloadManagerCli.Model.DownloadSource;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    internal sealed class DownloadSourceFile : IDownloadSourceFile
    {
        readonly ICallRemoteServer _remoteServerCall;
        private static int Count;
        private readonly ILogger<DownloadSourceFile> _logger;
        static SemaphoreSlim _semaphoreSlim = null;

        public DownloadSourceFile(ICallRemoteServer remoteServerCall, ILogger<DownloadSourceFile> logger)
        {
            _logger = logger;
            _remoteServerCall = remoteServerCall;
            _semaphoreSlim = new SemaphoreSlim(3, 3);
        }
        
        public async Task DownloadAsync(InputSource downloadSource)
        {
            if (!Directory.Exists(downloadSource.Config.DownloadDirectory))
                Directory.CreateDirectory(downloadSource.Config.DownloadDirectory);

            await ProcessParallelDownloadsAsync(downloadSource)
                            .ConfigureAwait(false);

            if (!downloadSource.Verbose)
                Console.WriteLine("Download completed.");
        }
        
        public void ExecuteDryRun(bool isVerbose, Download[] downloads)
        {
            if (isVerbose)
            {
                Console.WriteLine("A dry-run shows the followings : ");
                Console.WriteLine();
                foreach (var d in downloads)
                {
                    var s = $"Url : {d.Url}{Environment.NewLine}File: {d.File}{Environment.NewLine}Sha256: {d.Sha256}{Environment.NewLine}Sha1: {d.Sha1}{Environment.NewLine}Overwrite: {d.Overwrite}";
                    Console.WriteLine(s);
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("A dry-run is carried out; you're good to download.");
            }
        }

        private async Task ProcessParallelDownloadsAsync(InputSource inputSource)
        {
            _semaphoreSlim = new SemaphoreSlim(inputSource.Config.ParallelDownloads, inputSource.Config.ParallelDownloads);

            var tasks = GetDownloadTasks(inputSource, inputSource.Downloads.AsEnumerable());

            await Task
                    .WhenAll(tasks)
                       .ConfigureAwait(false);

            _logger.LogInformation("All tasks are completed");
        }

        private IEnumerable<Task> GetDownloadTasks(InputSource inputSource,
            IEnumerable<Download> urlsToBeDownloaded)
        {
            var r = urlsToBeDownloaded?.ToArray();
            var tasks = new Task[r.Length];

            for (int j = 0; j < r?.Length; j++)
            {
                var item = r[j];

                var downloadToFilePath = Path.Combine(inputSource.Config.DownloadDirectory, item.File);

                tasks[j] = Task.Run(async () =>
                {
                    Count += 1;
                    await DownloadAsync(item, downloadToFilePath, Count);
                });
            }
            return tasks;
        }

        private async Task DownloadAsync(Download item, string downloadToFilePath, int count)
        {
            Console.WriteLine($"Task {count} begins and waits for the semaphore.");

            _semaphoreSlim.Wait();

            try
            {
                Console.WriteLine($"Task {count} enters the semaphore.");

                await _remoteServerCall
                            .DownloadAsync(item.Url, downloadToFilePath, item.Overwrite)
                                .ConfigureAwait(false);

                Console.WriteLine($"Task {count} releases the semaphore");
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
