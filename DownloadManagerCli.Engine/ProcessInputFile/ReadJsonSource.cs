namespace DownloadManagerCli.Engine.ProcessInputFile
{
    using DownloadManagerCli.Abstraction.AbstractBase;
    using DownloadManagerCli.Model.DownloadSource;
    using System;
    internal sealed class ReadJsonSource : DownloadSourceBase
    {
        public sealed override InputSource GetSourceObjectFromFile(string path)
        {
            return base.GetSourceObjectFromFile(path);
        }
        protected sealed override InputSource ConverSourceToObject(string json)
        {
            throw new NotImplementedException();
        }
    }
}
