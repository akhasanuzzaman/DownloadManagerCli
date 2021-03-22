namespace DownloadManagerCli.Engine.ProcessInputFile
{
    using DownloadManagerCli.Abstraction.AbstractBase;
    using DownloadManagerCli.Model.DownloadSource;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    internal sealed partial class ReadYamlSource : DownloadSourceBase
    {
        public sealed override InputSource GetSourceObjectFromFile(string path)
        {
            return base.GetSourceObjectFromFile(path);
        }
        protected sealed override InputSource ConverSourceToObject(string yaml)
        {
            var deserializer = new DeserializerBuilder()
                   .WithNamingConvention(UnderscoredNamingConvention.Instance)
                   .Build();

            var source = deserializer.Deserialize<InputSource>(yaml);
            return source;
        }
    }
}
