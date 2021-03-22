namespace DownloadManagerCli.UnitTests.Model
{
    using System;
    public class SourceData
    {
        public Uri Uri { get; set; }
        public string DownloadFilePath { get; set; }
        public bool Overwrite { get; set; }
    }
}
