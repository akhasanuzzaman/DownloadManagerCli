namespace DownloadManagerCli.Engine.DownloadFiles
{
    using DownloadManagerCli.Abstraction.Interfaces;
    using System.IO;
    using System.Threading.Tasks;
    public class ContentWriter : IContentWriter
    {
        public async Task WriteToDiskAsync(string downloadToFilePath, byte[] fileBytes)
        {
            var fileStream = File.Create(downloadToFilePath);
            await fileStream.WriteAsync(fileBytes);
        }
        public Task WriteToDbAsync(string downloadToFilePath, byte[] fileBytes)
            => throw new System.NotImplementedException();
    }
}
