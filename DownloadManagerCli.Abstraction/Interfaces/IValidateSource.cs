using DownloadManagerCli.Model.DownloadSource;

namespace DownloadManagerCli.Abstraction.Interfaces
{
    public interface IValidateSource
    {
        public void Validate(string sourceFile);
        public void ShowValidationDetails(string sourceFile);
    }
}
