﻿using System.IO;

namespace Core.Helpers.Interfaces
{
    /// <summary> Provides methods to read files. </summary>
    public interface IFileReader
    {
        #region Methods: CreateTextReader()

        /// <summary> Creates a text reader from the file under the specified path. </summary>
        /// <param name="path"> The full path to the file. </param>
        /// <returns></returns>
        TextReader CreateTextReader(string path);

        /// <summary> Creates a text reader from the specified file. </summary>
        /// <param name="file"> The file to create a text reader with. </param>
        /// <returns></returns>
        TextReader CreateTextReader(FileInfo file);

        #endregion Methods: CreateTextReader()
        #region Methods: Read()

        /// <summary> Reads contents of the file under the specified path. </summary>
        /// <param name="path"> The full path to the file. </param>
        /// <returns></returns>
        string Read(string path);

        /// <summary> Reads contents of the specified file. </summary>
        /// <param name="file"> The file to read. </param>
        /// <returns></returns>
        string Read(FileInfo file);

        #endregion Methods: Read()
    }
}
