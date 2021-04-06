namespace DownloadManagerCli.Engine.DownloadFiles
{
    using DownloadManagerCli.Abstraction.Interfaces;
    using Microsoft.Extensions.Logging;
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
        public async Task DownloadAsync(Uri uri, string downloadToFilePath, bool overwrite)
        {
            try
            {
                Console.WriteLine($"Current url{uri.AbsoluteUri}");

                //Console.WriteLine($"waiting url{uri.AbsoluteUri} is in");

                using var client = new WebClient();

                if (!File.Exists(downloadToFilePath)
                    || (File.Exists(downloadToFilePath) && overwrite))
                {
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgress);
                    await client
                                .DownloadFileTaskAsync(uri, downloadToFilePath)
                                    .ConfigureAwait(false);
                }

                Console.WriteLine();

                //if (!File.Exists(downloadToFilePath))
                //{
                //    _logger.LogInformation($"{uri.AbsoluteUri} : is downloaded");
                //    //Console.WriteLine($"{Path.GetFileName(uri.AbsoluteUri)} was't downloaded");
                //    return downloadToFilePath;
                //}

                //if (File.Exists(downloadToFilePath))
                //{
                //    //Console.WriteLine($"{Path.GetFileName(uri.AbsoluteUri)} downloaded");
                //    return downloadToFilePath;
                //}
                //Console.WriteLine($"releasing url{uri.AbsoluteUri} ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Console.WriteLine(ex.Message);
            }
            //return downloadToFilePath;
        }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            var result = (TaskCompletionSource<object>)e.UserState;
            var p = (result.Task.AsyncState);
            // Displays the operation identifier, and the transfer progress.

            decimal per = (decimal)e.BytesReceived / e.TotalBytesToReceive * 100;
            var rrr = decimal.Round(per, 2);

            if (e.TotalBytesToReceive > 0 && rrr % 2 > 0)
            {
                Console.WriteLine("{0}    downloaded {1} of {2} bytes. {3} % complete...",
                    (result.Task.AsyncState),
                    e.BytesReceived,
                    e.TotalBytesToReceive,
                    rrr
                    );
            }
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
