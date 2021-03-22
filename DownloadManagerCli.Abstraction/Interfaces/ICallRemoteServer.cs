﻿namespace DownloadManagerCli.Abstraction.Interfaces
{
    using System;
    using System.Threading.Tasks;
    public interface ICallRemoteServer
    {
        Task<string> DownloadAsync(Uri uri, string downloadToFilePath, bool overwrite);
        void ValidateDownloadedFile(string[] filePaths);
    }
}
