namespace DownloadManagerCliApp
{
    internal sealed class Program
    {
        static void Main(string[] args)
                                => FileDownloadManager
                                        .StartToDownload(args);
    }
}
