namespace DownloadManagerCli.Engine.DownloadFiles
{
    using DownloadManagerCli.Abstraction.Interfaces;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    internal sealed class CallRemoteServer : ICallRemoteServer
    {
        private readonly ILogger<CallRemoteServer> _logger;
        public CallRemoteServer(ILogger<CallRemoteServer> logger)
        {
            _logger = logger;
        }
        public async Task<string> DownloadAsync(Uri uri, string downloadToFilePath, bool overwrite)
        {
            Console.WriteLine();

            try
            {
                using var client = new WebClient();

                if (!File.Exists(downloadToFilePath))
                {
                    await client
                                .DownloadFileTaskAsync(uri, downloadToFilePath)
                                    .ConfigureAwait(false);
                }

                if (File.Exists(downloadToFilePath) && overwrite)
                {
                    await client
                                .DownloadFileTaskAsync(uri, downloadToFilePath)
                                    .ConfigureAwait(false);
                }

                Console.WriteLine();
                if (!File.Exists(downloadToFilePath))
                {
                    Console.WriteLine($"{Path.GetFileName(uri.AbsoluteUri)} was't downloaded");
                }
                else
                {
                    if (File.Exists(downloadToFilePath))
                    {
                        Console.WriteLine($"{Path.GetFileName(uri.AbsoluteUri)} downloaded");
                    }
                }
                Console.WriteLine();
                _logger.LogInformation($"{uri.AbsoluteUri} : is downloaded");


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return downloadToFilePath;
        }

        public void ValidateDownloadedFile(string[] filePaths)
        {
            Console.WriteLine();
            Console.WriteLine("Download details: ");
            Console.WriteLine();

            foreach (var filePath in filePaths)
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"{Path.GetFileName(filePath).ToUpper()} : was't downloaded");
                }
                else
                {
                    Console.WriteLine($"{Path.GetFileName(filePath).ToUpper()} : downloaded");
                }
            }

        }
    }
}
