
namespace DownloadManagerCli.Abstraction.Interfaces
{ 
    using System.IO;
    using System.Threading.Tasks;
    public interface IContentWriter
    {
        Task WriteToDiskAsync(string downloadToFilePath, byte[] fileBytes);
        Task WriteToDbAsync(string downloadToFilePath, byte[] fileBytes);
    }
}
