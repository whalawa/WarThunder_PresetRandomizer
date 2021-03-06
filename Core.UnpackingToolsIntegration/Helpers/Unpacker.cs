﻿using Core.Enumerations;
using Core.Enumerations.Logger;
using Core.Extensions;
using Core.Helpers.Interfaces;
using Core.Helpers.Logger;
using Core.Helpers.Logger.Interfaces;
using Core.UnpackingToolsIntegration.Enumerations;
using Core.UnpackingToolsIntegration.Exceptions;
using Core.UnpackingToolsIntegration.Helpers.Interfaces;
using Core.WarThunderExtractionToolsIntegration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnpackingToolsIntegration.Enumerations.Logger;

namespace Core.UnpackingToolsIntegration.Helpers
{
    /// <summary> Provides methods to unpack War Thunder files. </summary>
    public class Unpacker : LoggerFluency, IUnpacker
    {
        #region Constants

        private const string _outputDirectorySuffix = "_u";
        private const string _outputFileSuffix = "x";

        #endregion Constants
        #region Fields

        /// <summary> An instance of a file manager. </summary>
        private readonly IFileManager _fileManager;

        /// <summary> A map of unpacking tool file names onto file extensions. </summary>
        private readonly IDictionary<string, string> _toolFileNames;

        #endregion Fields
        #region Constructors

        /// <summary> Creates a new unpacker. </summary>
        /// <param name="fileManager"> An instance of a file manager. </param>
        /// <param name="loggers"> Instances of loggers. </param>
        public Unpacker(IFileManager fileManager, params IConfiguredLogger[] loggers)
            : base(ECoreLogCategory.Unpacker, loggers)
        {
            LogDebug(ECoreLogMessage.Created.FormatFluently(ECoreLogCategory.Unpacker));

            _fileManager = fileManager;
            _toolFileNames = new Dictionary<string, string>
            {
                { EFileExtension.Blk, ETool.BlkUnpacker },
                { EFileExtension.Clog, ETool.ClogUnpacker },
                { EFileExtension.Ddsx, ETool.DdsxUnpacker },
                { EFileExtension.Dxp, ETool.DxpUnpacker },
                { EFileExtension.Bin, ETool.VromfsBinUnpacker },
                { EFileExtension.Wrpl, ETool.WrplUnpacker },
            };
        }

        #endregion Constructors
        #region Methods: Fluency

        /// <summary> Creates a new instance of <see cref="FileInfo"/> with a file name targeting <see cref="Settings.KlensysWarThunderToolsLocation"/>. </summary>
        /// <param name="fileName"> The file name. </param>
        /// <returns></returns>
        private FileInfo GetToolFileInfo(string fileName) =>
            _fileManager.GetFileInfo(Settings.KlensysWarThunderToolsLocation, fileName);

        /// <summary> Creates a new instance of <see cref="FileInfo"/> with a file name targeting <see cref="Settings.TempLocation"/>. </summary>
        /// <param name="fileName"> The file name. </param>
        /// <returns></returns>
        private FileInfo GetTempFileInfo(string fileName) =>
            _fileManager.GetFileInfo(Settings.TempLocation, fileName);

        /// <summary> Runs a shell command that calls the specified tool with the given directory/file as a parameter. </summary>
        /// <param name="toolPath"> The path to the tool file. </param>
        /// <param name="argumentFilePath"> The path to the argument directory. </param>
        /// <returns></returns>
        private Process RunShellCommand(string toolPath, string argument)
        {
            var process = Process.Start(new ProcessStartInfo(toolPath, $"\"{argument}\""));

            process.WaitForExit();

            return process;
        }

        /// <summary> Logs a general error and rethrows the exception that caused it. </summary>
        /// <param name="exception"> The exception that has been thrown. </param>
        private void LogErrorAndRethrow(Exception exception)
        {
            LogError(EUnpackingToolsIntegrationLogMessage.ErrorRunningUnpackingTool, exception);
            throw exception;
        }

        #endregion Methods: Fluency

        /// <summary> Selects an appropriate unpacking tool file name (see <see cref="ETool"/>) for a given file extension. </summary>
        /// <param name="fileExtension"> The file extension to look for a match for. The register and period characters are ignored. </param>
        /// <returns></returns>
        private string GetToolFileNameByFileExtension(string fileExtension)
        {
            if (_toolFileNames.TryGetValue(fileExtension.ToLower().Skip(EInteger.Number.One).StringJoin(), out var toolFileName))
            {
                LogDebug(EUnpackingToolsIntegrationLogMessage.UnpackingToolSelected.FormatFluently(toolFileName));
                return toolFileName;
            }
            else
            {
                LogErrorAndThrow<FileExtensionNotSupportedException>
                (
                    EUnpackingToolsIntegrationLogMessage.FileExtensionNotSupportedByUnpackingTools.FormatFluently(fileExtension),
                    EUnpackingToolsIntegrationLogMessage.ErrorMatchingUnpakingToolToFileExtension
                );
                return null;
            }
        }

        /// <summary> Gets an output path for the specified file according to its extension. </summary>
        /// <param name="file"> The file for which to generate the output path. </param>
        /// <returns> A patched version of the output path. </returns>
        private string GetOutputPath(FileInfo file)
        {
            var outputPath = $@"{file.Directory}\{file.Name}";

            switch (file.Extension.Split(ECharacter.Period).Last().ToLower())
            {
                case EFileExtension.Bin:
                {
                    outputPath = $"{outputPath}{_outputDirectorySuffix}";
                    break;
                }
                case EFileExtension.Blk:
                {
                    outputPath = $"{outputPath}{_outputFileSuffix}";
                    break;
                }
                case EFileExtension.Ddsx:
                {
                    outputPath = $@"{file.Directory}\{file.GetNameWithoutExtension()}.{EFileExtension.Dds}";
                    break;
                }
                default:
                {
                    throw new NotImplementedException(EUnpackingToolsIntegrationLogMessage.OutputPathGenerationForFileExtensionNotYetImplemented.FormatFluently(file.Extension));
                }
            }
            return outputPath;
        }

        #region Method: Unpack()

        /// <summary> Copies the file to unpack into the <see cref="Settings.TempLocation"/> and unpacks it. </summary>
        /// <param name="sourceFile"> The file to unpack. </param>
        /// <param name="overwrite"> Whether to overwrite existing files. </param>
        /// <returns> The path to the output file / directory. </returns>
        public string Unpack(FileInfo sourceFile, bool overwrite = false)
        {
            // Preparing temp files.

            LogDebug(EUnpackingToolsIntegrationLogMessage.PreparingToUnpack.FormatFluently(sourceFile.FullName));

            _fileManager.CopyFile(sourceFile.FullName, Settings.TempLocation, overwrite, true);

            var toolFile = GetToolFileInfo(GetToolFileNameByFileExtension(sourceFile.Extension));
            var tempFile = GetTempFileInfo(sourceFile.Name);

            // Unpacking proper.

            LogDebug(EUnpackingToolsIntegrationLogMessage.Unpacking.FormatFluently(tempFile.Name));

            try
            {
                RunShellCommand(toolFile.FullName, tempFile.FullName);
                var outputPath = GetOutputPath(tempFile);

                LogDebug(EUnpackingToolsIntegrationLogMessage.Unpacked.FormatFluently(tempFile.Name));
                return outputPath;
            }
            catch (Exception exception)
            {
                LogErrorAndRethrow(exception);
                return null;
            }
        }

        /// <summary> Unpacks all valid files in the specified folder with the given tool. </summary>
        /// <param name="sourceDirectory"> The directory to unpack unpack files from. </param>
        /// <param name="toolName"> The name of the unpacking tool to use. </param>
        public void Unpack(DirectoryInfo sourceDirectory, string toolName)
        {
            LogDebug(EUnpackingToolsIntegrationLogMessage.Unpacking.FormatFluently(sourceDirectory.FullName));

            try
            {
                RunShellCommand(GetToolFileInfo(toolName).FullName, sourceDirectory.FullName);
                LogDebug(EUnpackingToolsIntegrationLogMessage.Unpacked.FormatFluently(sourceDirectory.Name));
            }
            catch (Exception exception)
            {
                LogErrorAndRethrow(exception);
            }
        }

        #endregion Method: Unpack()
    }
}