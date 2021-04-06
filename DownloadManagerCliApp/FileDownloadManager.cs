namespace DownloadManagerCliApp
{
    using CommandLine;
    using DownloadManagerCli.Abstraction.AbstractBase;
    using DownloadManagerCliApp.Dependency_Injection;
    using DownloadManagerCli.Engine;
    using DownloadManagerCli.Engine.DownloadFiles;
    using DownloadManagerCli.Engine.ValidateInputSource;
    using DownloadManagerCli.Model.CliArgumentOptions;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using DownloadManagerCli.Model.DownloadSource;

    internal static class FileDownloadManager
    {
        public static async Task StartToDownloadAsync(string[] args)
        {
            try
            {
                Log.Logger.Information("Download cli application has started");
                Console.WriteLine("Cli application has started.");

                var types = LoadVerbs();


                Parser.Default.ParseArguments(args, types?.ToArray())
                    .WithParsed<DownloadArguments>(async (downloadArguments) =>
                    {
                        if (downloadArguments == null)
                        {
                            throw new Exception("CLI arrguments can't be null");
                        }

                        downloadArguments.InputFilePath = GetFullInputFilePath(downloadArguments.InputFilePath);
                        InputSource inputSource = GetInputSource(downloadArguments.InputFilePath,
                                                                 downloadArguments.Verbose,
                                                                 downloadArguments.NumberOfParallelDownloads);

                        await ExecuteDownloadCommandAsync(downloadArguments, inputSource)
                                    .ConfigureAwait(false);

                    })
                    .WithParsed<ValidationArguments>(arguments =>
                    {
                        ExecuteValidateCommand(arguments);
                        return;
                    })
                    .WithNotParsed(HandleParseError);


                Log.Logger.Information("Download completed");

                await Task.FromResult(0);

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Console.WriteLine();
                Console.WriteLine($"Cli application terminated with the following issue(s) :{Environment.NewLine}");
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }

        private static string GetFullInputFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new Exception("Input source file path can't be null or empty.");

            var fullPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location)?.LocalPath), filePath);

            if (!File.Exists(fullPath))
                throw new Exception("Input source file does not exists.");

            if (!Path.HasExtension(fullPath))
                throw new Exception("Input needs to have an extension.");

            return fullPath;
        }

        private static InputSource GetInputSource(string filePath, bool verbose, int numberOfParallelDownloads)
        {
            DownloadSourceBase downloadSourceBase
                               = GetDownloadSourceFromFactory(filePath);

            InputSource inputSource = downloadSourceBase.GetSourceObjectFromFile(filePath);

            var validateSource = ServiceProvider.GetRequiredService<ValidateSource>();
            validateSource.Validate(filePath);

            if (inputSource == null || inputSource.Downloads == null)
                throw new Exception("Invalid source file");

            var rootPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly()?.Location)?.LocalPath);
            inputSource.Config.DownloadDirectory = Path.Combine(rootPath, inputSource.Config.DownloadDirectory);

            if (inputSource?.Config?.DownloadDirectory == null || inputSource?.Config?.DownloadDirectory == null)
                throw new Exception("Download directory can't be null");


            inputSource.Verbose = verbose;
            inputSource.Config.ParallelDownloads = numberOfParallelDownloads > 0 ?
                                                            numberOfParallelDownloads :
                                                                   inputSource.Config.ParallelDownloads;

            return inputSource;
        }

        private static DownloadSourceBase GetDownloadSourceFromFactory(string sourcePath)
        {
            var fileExtension = Path.GetExtension(sourcePath).Replace(".", "").ToUpper();
            DownloadSourceBase downloadSourceBase = DownloadSourceFactory
                                                        .GetDownloadSource(fileExtension);
            return downloadSourceBase;
        }

        private static void HandleParseError(IEnumerable<Error> errs)
            => Console.WriteLine("Provided arguments can't be parsed successfully");

        private static ServiceProvider ServiceProvider
                                             => Dependency
                                                     .ConfigureServices()
                                                             .BuildServiceProvider();

        private static async Task ExecuteDownloadCommandAsync(DownloadArguments downloadArguments, InputSource fileSource)
        {

            DownloadSourceFile downloadFile = ServiceProvider.GetRequiredService<DownloadSourceFile>();
            if (downloadArguments.DryRun)
            {
                downloadFile
                        .ExecuteDryRun(fileSource.Verbose, fileSource.Downloads);
            }

            if (downloadArguments.Download)
            {
                await downloadFile
                            .DownloadAsync(fileSource)
                                  .ConfigureAwait(false);
            }
        }

        private static void ExecuteValidateCommand(ValidationArguments validationArguments)
        {
            var validateSource = ServiceProvider.GetRequiredService<ValidateSource>();

            var root = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly()?.Location)?.LocalPath);
            validationArguments.FilePath = Path.Combine(root, validationArguments.FilePath);

            if (validationArguments.Verbose)
            {
                validateSource.Validate(validationArguments.FilePath);
                Console.WriteLine("File is valid and the details :");
                Console.WriteLine();
                validateSource.ShowValidationDetails(validationArguments.FilePath);
            }
            else
            {
                validateSource.Validate(validationArguments.FilePath);
                Console.WriteLine("Input file is valid");
            }
        }

        private static IEnumerable<Type> LoadVerbs()
                => from assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                   let assembly = Assembly.Load(assemblyName)
                   from type in assembly.GetTypes()
                    .Where(t => t.GetCustomAttribute<VerbAttribute>() != null)
                        .ToArray()
                   select type;
    }
}
