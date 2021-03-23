namespace DownloadManagerCli.Engine.DownloadFiles
{
    using DownloadManagerCli.Abstraction.Interfaces;
    using DownloadManagerCli.Model.DownloadSource;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    internal sealed class DownloadSourceFile : IDownloadSourceFile
    {
        readonly ICallRemoteServer _remoteServerCall;
        private static int Count;
        private readonly ILogger<DownloadSourceFile> _logger;

        public DownloadSourceFile(ICallRemoteServer remoteServerCall, ILogger<DownloadSourceFile> logger)
        {
            _logger = logger;
            _remoteServerCall = remoteServerCall;
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
            var taskResults = new List<string>();

            for (var i = 0; i < inputSource.Downloads.Length; i = i + inputSource.Config.ParallelDownloads)
            {
                try
                {

                    var urlsToBeDownloaded = inputSource.Downloads
                                                                .Skip(i)
                                                                    .Take(inputSource.Config.ParallelDownloads);

                    Action<bool, bool, int, string> showMessageOnConsole = DisplayMessage;

                    var tasks = GetDownloadTasks(inputSource, urlsToBeDownloaded, showMessageOnConsole);


                    string[] results = await Task.WhenAll(tasks)
                                .ConfigureAwait(false);

                    taskResults.AddRange(results);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    Console.WriteLine($"{ex.Message}");
                }
            }

            var filePaths = taskResults?.Select(r => r != null ? Path.Combine(inputSource.Config.DownloadDirectory, r) : null);
            _remoteServerCall.ValidateDownloadedFile(filePaths?.Where(f => f != null).ToArray());
        }

        private void DisplayMessage(bool verbose,
            bool hasNoFileExtension,
            int numberOfFilesToDownload,
            string fileName)
        {
            Console.WriteLine();

            if (hasNoFileExtension)
            {
                Console.WriteLine($"File name : {fileName.ToUpper()} does not have an extension");
                return;
            }

            if (!verbose)
            {
                Console.WriteLine($"Download file # {Count}....");
            }

            if (verbose)
            {
                Console.WriteLine($"File # {Count} of {numberOfFilesToDownload} File name : {fileName.ToUpper()} is downloading");
            }

        }

        private IEnumerable<Task<string>> GetDownloadTasks(InputSource inputSource,
            IEnumerable<Download> urlsToBeDownloaded,
             Action<bool, bool, int, string> showMessageOnConsole)
        {
            foreach (var item in urlsToBeDownloaded)
            {
                Count += 1;

                showMessageOnConsole(inputSource.Verbose,
                    false,
                        inputSource.Downloads.Length,
                            item.File);

                var downloadToFilePath = Path.Combine(inputSource.Config.DownloadDirectory, item.File);
                if (!Path.HasExtension(downloadToFilePath))
                {
                    showMessageOnConsole(inputSource.Verbose,
                                         true,
                                         inputSource.Downloads.Length,
                                         item.File);
                    continue;
                }
                var task = _remoteServerCall
                                .DownloadAsync(item.Url, downloadToFilePath, item.Overwrite);

                yield return task;
            }
        }
    }
}
