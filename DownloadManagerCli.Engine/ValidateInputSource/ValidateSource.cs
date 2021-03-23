namespace DownloadManagerCli.Engine.ValidateInputSource
{
    using DownloadManagerCli.Abstraction.AbstractBase;
    using DownloadManagerCli.Abstraction.Interfaces;
    using DownloadManagerCli.Model.DownloadSource;
    using DownloadManagerCli.Model.Enums;
    using System;
    using System.IO;

    internal sealed class ValidateSource : IValidateSource
    {
        public void Validate(string sourceFile)
        {
            ValidateArguments(sourceFile);

            var source = GetSourceFromFile(sourceFile);
            ValidateSourceContent(source);
        }

        private InputSource GetSourceFromFile(string sourceFile)
        {
            var filePath = Path.GetExtension(sourceFile);
            var fileExtension = Path.GetExtension(filePath).Replace(".", "").ToUpper();
            DownloadSourceBase downloadSourceBase = GetDownloadSourceFromFactory(fileExtension);
            var source = downloadSourceBase.GetSourceObjectFromFile(sourceFile);
            return source;
        }

        private DownloadSourceBase GetDownloadSourceFromFactory(string fileExtension)
        {
            DownloadSourceBase downloadSourceBase = DownloadSourceFactory.GetDownloadSource(fileExtension.ToUpper());
            return downloadSourceBase;
        }

        private void ValidateArguments(string sourcePath)
        {
            try
            {

                if (sourcePath == null)
                    throw new Exception("Cli arguments can't be null");

                if (!File.Exists(sourcePath))
                    throw new Exception("Source file path is invalid");

                var sourceType = sourcePath != null ?
                                      Path.GetExtension(sourcePath).Replace(".", "") :
                                      null;
                if (sourceType == null)
                    throw new Exception("Source file type can't be empty or null");

                if (!Enum.IsDefined(typeof(DownloadSourceEnum), sourceType?.ToUpper()))
                    throw new Exception($"Invalid source file type : {sourceType}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void ValidateSourceContent(InputSource source)
        {
            try
            {

                if (source == null)
                    throw new Exception("Source object can't be null");

                if (source.Downloads == null)
                    throw new Exception("Download details in the source can't be null");

                if (source.Downloads.Length > 0)
                {
                    foreach (var d in source.Downloads)
                    {
                        if (d.Url == null)
                        {
                            throw new Exception("Download url can't be null");
                        }
                        if (d.File == null)
                        {
                            throw new Exception("Download file name can't be null");
                        }
                    }
                }

                if (source.Config == null)
                    throw new Exception("Target directory can't be null");

                if (source.Config.DownloadDirectory == null)
                    throw new Exception("Download directory can't be null");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ShowValidationDetails(string sourceFile)
        {
            try
            {
                var source = GetSourceFromFile(sourceFile);

                if (source.Config != null && source.Config.DownloadDirectory == null)
                {
                    Console.WriteLine($"Download directory : {source.Config.DownloadDirectory}");
                    Console.WriteLine($"# of ParallelDownloads : {source.Config.ParallelDownloads}");
                }
                if (source.Downloads != null && source.Downloads.Length > 0)
                {
                    foreach (var d in source.Downloads)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Url : {d.Url}");
                        Console.WriteLine($"File : {d.File}");
                        Console.WriteLine($"Overwrite : {d.Overwrite}");
                        Console.WriteLine($"Sha256 : {d.Sha256}");
                        Console.WriteLine($"Sha1 : {d.Sha1}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
