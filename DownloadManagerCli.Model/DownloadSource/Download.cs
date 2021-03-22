namespace DownloadManagerCli.Model.DownloadSource
{
    using System;
    public partial class Download
    {
        public Uri Url { get; set; }
        public string File { get; set; }
        public string Sha256 { get; set; }
        public string Sha1 { get; set; }
        public bool Overwrite { get; set; }
    }
}
