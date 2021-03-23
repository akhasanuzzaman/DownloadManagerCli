using DownloadManagerCli.Model.DownloadSource;
using System.Threading.Tasks;

namespace DownloadManagerCli.Abstraction.Interfaces
{
    public interface IDownloadSourceFile
    {
        Task DownloadAsync(InputSource downloadSource);
        void ExecuteDryRun(bool isVerbose, Download[] downloads);
    }
}
