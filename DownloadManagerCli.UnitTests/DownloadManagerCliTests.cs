namespace DownloadManagerCli.UnitTests
{
    using DownloadManagerCli.Abstraction.Interfaces;
    using DownloadManagerCli.Engine.DownloadFiles;
    using DownloadManagerCli.Engine.ValidateInputSource;
    using DownloadManagerCli.Model.DownloadSource;
    using DownloadManagerCli.UnitTests.Model;
    using Moq;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Xunit;
    public class DownloadManagerCliTests
    {
        private const string filePath = "InputSource\\downloadsource.yml";
        private const string incompleteFilePath = "C:\\DownloadManagerCli\\DownloadManagerCli\\InputSource\\downloadsource";
        private const string outputDirectoryLocation = "C:\\DownloadManagerCli\\DownloadFiles";
        private const string url = "https://github.com/zemirco/sf-city-lots-json/blob/master/citylots.json";
        private const string fileName = "citylots.json";

        [Theory]
        [ClassData(typeof(DownloadSourceInputData))]
        public void DownloadAsync_WhenSourceIsValid_ReturnsNoException(SourceData sourceData)
        {
            //Arrange
            var mockCallRemoteServer = new Mock<ICallRemoteServer>();
            mockCallRemoteServer.Setup(h => h.DownloadAsync(sourceData.Uri, sourceData.DownloadFilePath, sourceData.Overwrite))
                .Returns(default(Task<string>));

            //Act
            var _sut = new DownloadSourceFile(mockCallRemoteServer.Object, null); // ILogger should be mocked
            Action action = async () => await _sut.DownloadAsync(new InputSource()
            {
                Downloads = new Download[] {

                    new Download()
                    {
                        Url = new Uri(url),
                        File = fileName,
                        Overwrite = true
                    }
                },
                Config = new Config()
                {
                    DownloadDirectory = outputDirectoryLocation,
                    ParallelDownloads = 2
                },
                Verbose = true
            });

            var exception = Record.Exception(action);

            //Assert
            Assert.Null(exception);
            Assert.IsNotType<Exception>(exception);
        }
        [Fact]
        public async Task DownloadAsync_WhenSourceIsValid_ReturnsException()
        {
            //Arrange
            var mockCallRemoteServer = new Mock<ICallRemoteServer>();
            mockCallRemoteServer.Setup(h => h.DownloadAsync(It.IsAny<Uri>(), It.IsAny<string>(), false))
                .Returns(default(Task<string>));

            //Act
            var _sut = new DownloadSourceFile(mockCallRemoteServer.Object, null);// ILogger should be mocked

            Func<Task> action = async () => await _sut.DownloadAsync(new InputSource()
            {
                Downloads = new Download[] {

                    new Download()
                    {
                        Url = new Uri(url),
                        File = fileName,
                        Overwrite = true
                    }
                },
                Config = new Config()
                {
                    DownloadDirectory = null,
                    ParallelDownloads = 2
                },
                Verbose = true
            });

            var exception = await Record.ExceptionAsync(action);

            //Assert
            Assert.NotNull(exception);
            Assert.Equal("Value cannot be null. (Parameter 'path')", exception.Message);

        }
        [Fact]
        public void Validate_WhenWithValidSourceFile_ReturnsNoException()
        {
            //Arrange
            var validate = new ValidateSource();

            //Act
            Action action = () => validate.Validate(GetFullPath());
            var ex = Record.Exception(action);

            //Assert
            Assert.Null(ex);
            Assert.IsNotType<Exception>(ex);
        }
        [Fact]
        public void Validate_WhenWithInValidSourceFile_ReturnsException()
        {
            //Arrange
            var validate = new ValidateSource();

            //Act
            Action action = () => validate.Validate(incompleteFilePath);
            var ex = Record.Exception(action);

            //Assert
            Assert.NotNull(ex);
            Assert.IsType<Exception>(ex);
        }
        private string GetFullPath()
            => Path
                  .Combine(Path
                     .GetDirectoryName(
                         new Uri(Assembly
                             .GetExecutingAssembly().Location).LocalPath), filePath);
    }
}
