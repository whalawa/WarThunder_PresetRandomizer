﻿using Core.Enumerations;
using Core.Enumerations.Logger;
using Core.Extensions;
using Core.Helpers.Interfaces;
using Core.Helpers.Logger;
using Core.Helpers.Logger.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Helpers
{
    /// <summary> Provides methods to manage files. </summary>
    public class FileManager : LoggerFluency, IFileManager
    {
        #region Constructors

        /// <summary> Creates a new file manager. </summary>
        /// <param name="loggers"> Instances of loggers. </param>
        public FileManager(params IConfiguredLogger[] loggers)
            : base(ECoreLogCategory.FileManager, loggers)
        {
            LogDebug(ECoreLogMessage.Created.FormatFluently(ECoreLogCategory.FileManager));
        }

        #endregion Constructors
        #region Methods: Copying

        /// <summary> Copies a file into a specified directory. </summary>
        /// <param name="filePath"> The absolute name of the file. </param>
        /// <param name="destination"> The destination path. </param>
        /// <param name="overwrite"> Whether to overwrite colliding files at the destination. </param>
        /// <param name="createDirectories"> Whether to create the destination directory. </param>
        public void CopyFile(string filePath, string destination, bool overwrite = false, bool createDirectories = false) =>
            CopyFile(new FileInfo(filePath), destination, overwrite, createDirectories);

        /// <summary> Copies a file into a specified directory. </summary>
        /// <param name="file"> The file to copy. </param>
        /// <param name="destination"> The destination path. </param>
        /// <param name="overwrite"> Whether to overwrite colliding files at the destination. </param>
        /// <param name="createDirectories"> Whether to create the destination directory. </param>
        public void CopyFile(FileInfo file, string destination, bool overwrite = false, bool createDirectories = false)
        {
            var destinationDirectory = new DirectoryInfo(destination);

            // Pre-copying checks and directory creation.

            if (!file.Exists)
            {
                LogDebug(ECoreLogMessage.DoesNotExist_CopyingAborted.FormatFluently(file.FullName));
                return;
            }
            else if (!destinationDirectory.Exists)
            {
                if (createDirectories)
                {
                    LogDebug(ECoreLogMessage.Creating_InQuotes.FormatFluently(destinationDirectory.FullName));

                    destinationDirectory.Create();

                    LogDebug(ECoreLogMessage.Created_InQuotes.FormatFluently(destinationDirectory.FullName));
                }
                else
                {
                    LogDebug(ECoreLogMessage.DoesNotExist_CopyingSomethingAborted.ResetFormattingPlaceholders().FormatFluently(destinationDirectory.FullName, file.FullName));
                    return;
                }
            }

            // Copying proper.

            LogDebug(ECoreLogMessage.Copying.ResetFormattingPlaceholders().FormatFluently(file.FullName, destinationDirectory.FullName));

            var filePath = Path.Combine(destinationDirectory.FullName, file.Name);

            if (File.Exists(filePath))
            {
                if (!overwrite)
                {
                    LogDebug(ECoreLogMessage.AlreadyExists_CopyingSkipped.FormatFluently(file.Name));
                    return;
                }
                LogDebug(ECoreLogMessage.Overwriting);
            }
            file.CopyTo(filePath, overwrite);

            LogDebug(ECoreLogMessage.Copied.FormatFluently(file.FullName));
        }

        /// <summary> Creates a backup copy of the given file, with its name appended with ".bak". </summary>
        /// <param name="file"> The file to back up. </param>
        public void BackUpFile(FileInfo file) =>
            file.CopyTo(Path.Combine(file.DirectoryName, $"{file.Name}{ECharacter.Period}{EFileExtension.Bak}"), true);

        #endregion Methods: Copying
        #region Methods: Deletion

        /// <summary> Deletes the file with the specified name if it exists. </summary>
        /// <param name="fileName"> The name of the file to delete. </param>
        public void DeleteFileSafely(string fileName) =>
            DeleteFileSafely(new FileInfo(fileName));

        /// <summary> Deletes the specified file if it exists. </summary>
        /// <param name="file"> The file to delete. </param>
        public void DeleteFileSafely(FileInfo file)
        {
            LogTrace(ECoreLogMessage.Deleting.FormatFluently(file.Name));

            try
            {
                file.Refresh();

                if (file.Exists)
                    file.Delete();
                else
                    LogTrace(ECoreLogMessage.DoesNotExist_NoNeedToDelete.FormatFluently(file.Name));
            }
            catch (Exception exception)
            {
                LogError(ECoreLogMessage.ErrorDeletingFile, exception);
                throw;
            }

            LogTrace(ECoreLogMessage.FileDeleted);
        }

        #region DeleteFiles()

        /// <summary> Deletes all files under names/paths listed in the specified collection. </summary>
        /// <param name="filePaths"> A collection of file names/paths. </param>
        public void DeleteFiles(IEnumerable<string> filePaths) =>
            DeleteFiles(filePaths.Select(filePath => Path.IsPathRooted(filePath) ? filePath : Path.Combine(Directory.GetCurrentDirectory(), filePath)));

        /// <summary> Deletes all files listed in the specified collection. </summary>
        /// <param name="files"> A collection of file information. </param>
        private void DeleteFiles(IEnumerable<FileInfo> files)
        {
            LogDebug(ECoreLogMessage.DeletingFiles.FormatFluently(files.Count()));

            foreach (var file in files)
                DeleteFileSafely(file);

            LogDebug(ECoreLogMessage.FilesDeleted);
        }

        /// <summary> Deletes all files in a directory. </summary>
        /// <param name="path"> The path to a directory. </param>
        /// <param name="includeNested"> Whether to delete files in all nested directories. </param>
        /// <param name="deleteEmptyDirectories"> Whether to delete directories if they are empty after file deletion regardless of whether they had files prior. </param>
        public void DeleteFiles(string path, bool includeNested = false, bool deleteEmptyDirectories = false) =>
            DeleteFiles(path, new List<string>(), includeNested, deleteEmptyDirectories);

        /// <summary> Deletes all files with the specified extension in a directory. </summary>
        /// <param name="path"> The path to a directory. </param>
        /// <param name="fileExtension"> A collection of file extensions to delete from the folder. </param>
        public void DeleteFiles(string path, params string[] fileExtensions) =>
            DeleteFiles(path, fileExtensions.ToList());

        /// <summary> Deletes all files with specified extensions in a directory. </summary>
        /// <param name="path"> The path to a directory. </param>
        /// <param name="fileExtension"> A collection of file extensions to delete from the folder. </param>
        /// <param name="includeNested"> Whether to delete files in all nested directories. </param>
        /// <param name="deleteEmptyDirectories"> Whether to delete directories if they are empty after file deletion regardless of whether they had files prior. </param>
        public void DeleteFiles(string path, IEnumerable<string> fileExtensions, bool includeNested = false, bool deleteEmptyDirectories = false)
        {
            var rootDirectory = new DirectoryInfo(path);

            if (!rootDirectory.Exists)
            {
                LogDebug(ECoreLogMessage.DirectoryDoesNotExist_DeletingAborted.FormatFluently(rootDirectory.FullName));
                return;
            }

            // Reading all file information.

            LogDebug(ECoreLogMessage.SelectingAllFilesFromDirectory.FormatFluently(rootDirectory.FullName));

            var files = rootDirectory.GetFiles();
            if (files.IsEmpty())
            {
                LogDebug(ECoreLogMessage.DirectoryIsEmpty.FormatFluently(rootDirectory.FullName));

                if (includeNested)
                    DeleteFilesInSubdirectories(rootDirectory, fileExtensions, deleteEmptyDirectories);

                return;
            }

            // Filtering files by specified extensions.

            if (fileExtensions?.Any() ?? false)
            {
                LogDebug(ECoreLogMessage.FilteringFilesFromSelection.FormatFluently(fileExtensions.StringJoin(", ")));
                files = files
                    .Where
                    (
                        file => file.GetExtensionWithoutPeriod().ToLower().IsIn(fileExtensions.Select(extension => extension.ToLower()))
                            && !file.Attributes.HasFlag(FileAttributes.Directory | FileAttributes.Hidden | FileAttributes.ReadOnly | FileAttributes.System)
                            && !file.IsReadOnly
                    )
                    .ToArray()
                ;

                if (files.IsEmpty())
                {
                    LogDebug(ECoreLogMessage.NoFilesOfSpecifiedFormatToDelete);
                    return;
                }
            }

            // Deleting files.

            LogDebug(ECoreLogMessage.SelectedFileCount.FormatFluently(files.Count()));
            DeleteFiles(files);

            if (includeNested)
                DeleteFilesInSubdirectories(rootDirectory, fileExtensions, deleteEmptyDirectories);
        }

        #endregion DeleteFiles()
        #region DeleteFilesInSubdirectories()

        /// <summary> Deletes all files with specified extensions in all subfolders. </summary>
        /// <param name="directory"> A directory from whose subfolders to delete files. </param>
        /// <param name="fileExtension"> A collection of file extensions to delete from the folder. </param>
        /// <param name="deleteEmptyDirectories"> Whether to delete directories if they are empty after file deletion regardless of whether they had files prior. </param>
        private void DeleteFilesInSubdirectories(DirectoryInfo directory, IEnumerable<string> fileExtensions, bool deleteEmptyDirectories = false)
        {
            LogDebug(ECoreLogMessage.CheckingSubdirectories);

            var subdirectories = directory.GetDirectories();
            if (subdirectories.IsEmpty())
            {
                LogDebug(ECoreLogMessage.NoSubdirectories);
                return;
            }

            LogDebug(ECoreLogMessage.SubdirectoriesFound.FormatFluently(subdirectories.Count()));

            for (var i = 0; i < subdirectories.Count(); i++)
            {
                var subdirectory = subdirectories[i];
                DeleteFiles(subdirectory.FullName, fileExtensions, true, deleteEmptyDirectories);

                if (deleteEmptyDirectories)
                    DeleteEmptyDirectory(subdirectory.FullName);
            }
        }

        #endregion DeleteFilesInSubdirectories()

        /// <summary> Empties the specified directory. </summary>
        /// <param name="path"> The path to a directory. </param>
        public void EmptyDirectory(string path)
        {
            LogDebug(ECoreLogMessage.EmptyingDirectory.FormatFluently(path));

            DeleteFiles(path, true, true);

            LogDebug(ECoreLogMessage.DirectoryEmptied.FormatFluently(path));
        }

        /// <summary> Deletes an empty directory. It does not check whether the directory is empty or not. </summary>
        /// <param name="path"> The path to a directory. </param>
        private void DeleteEmptyDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                LogDebug(ECoreLogMessage.DeletingEmptyDirectory.FormatFluently(path));

                Directory.Delete(path);

                LogDebug(ECoreLogMessage.Deleted.FormatFluently(path));
            }
            else
            {
                LogDebug(ECoreLogMessage.DoesNotExist_NoNeedToDelete.FormatFluently(path));
            }
        }

        /// <summary> Deletes the specified directory. </summary>
        /// <param name="path"> The path to a directory. </param>
        public void DeleteDirectory(string path)
        {
            EmptyDirectory(path);
            DeleteEmptyDirectory(path);
        }

        /// <summary> Deletes all files (non-recursively) older than the given timestamp. </summary>
        /// <param name="path"> The path to files. </param>
        /// <param name="deletionDeadline"> The timestamp before which all files should be deleted. </param>
        public void DeleteOldFiles(string path, DateTime deletionDeadline)
        {
            var files = Directory
                .GetFiles(path)
                .Select(filePath => new FileInfo(filePath))
            ;

            foreach (var file in files)
            {
                if (file.LastWriteTimeUtc < deletionDeadline)
                    DeleteFileSafely(file.FullName);
            }
        }

        #endregion Methods: Deletion
        #region Methods: Fluency

        /// <summary> Creates a new instance of <see cref="FileInfo"/> with a directory path and a file name. </summary>
        /// <param name="directoryPath"> The directory path. </param>
        /// <param name="fileName"> The file name. </param>
        /// <returns></returns>
        public FileInfo GetFileInfo(string directoryPath, string fileName) =>
            new FileInfo(Path.Combine(directoryPath, fileName));

        #endregion Methods: Fluency
    }
}