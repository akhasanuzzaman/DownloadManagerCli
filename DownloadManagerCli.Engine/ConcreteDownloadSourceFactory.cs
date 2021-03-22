
namespace DownloadManagerCli.Engine
{
    using DownloadManagerCli.Abstraction.AbstractBase;
    using DownloadManagerCli.Engine.ProcessInputFile;
    using DownloadManagerCli.Model.Enums;
    
    internal static class DownloadSourceFactory 
    {
        public static DownloadSourceBase GetDownloadSource(string sourceType)
        {
            return sourceType switch
            {
                nameof(DownloadSourceEnum.YMAL) or nameof(DownloadSourceEnum.YML) => new ReadYamlSource(),
                nameof(DownloadSourceEnum.JSON) => new ReadJsonSource(),
                nameof(DownloadSourceEnum.XML) => new ReadXmlSource(),
                _ => throw new System.Exception("Source type provided is invalid"),
            };
        }
    }
}
