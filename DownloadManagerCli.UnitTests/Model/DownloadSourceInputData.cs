namespace DownloadManagerCli.UnitTests.Model
{
    using DownloadManagerCli.UnitTests.Model;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class DownloadSourceInputData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new SourceData[] {
                new SourceData {DownloadFilePath="test path 1", Uri= new Uri("http://testpath/path1"),Overwrite=true},
            };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
