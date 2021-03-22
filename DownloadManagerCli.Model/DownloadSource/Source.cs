namespace DownloadManagerCli.Model.DownloadSource
{
    public class InputSource
    {
        public Config Config { get; set; }
        public Download[] Downloads { get; set; }
        public bool Verbose { get; set; }
    }
}
