
using System.Threading.Tasks;

namespace DownloadManagerCliApp
{
    internal sealed class Program
    {
        static async Task Main(string[] args)
                                =>await FileDownloadManager
                                        .StartToDownloadAsync(args);
    }
}
