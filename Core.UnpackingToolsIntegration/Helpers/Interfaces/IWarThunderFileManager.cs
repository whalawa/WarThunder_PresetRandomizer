﻿using Core.Enumerations;
using Core.Helpers.Interfaces;
using Core.UnpackingToolsIntegration.Enumerations;
using Core.WarThunderExtractionToolsIntegration;
using System;
using System.Collections.Generic;

namespace Core.UnpackingToolsIntegration.Helpers.Interfaces
{
    public interface IWarThunderFileManager: IFileManager
    {
        /// <summary> Removes all directories and files in <see cref="Settings.TempLocation"/>. </summary>
        void CleanUpTempDirectory();

        /// <summary> Gets names of all <see cref="EFileExtension.SqLite3"/> database files for specific game versions. </summary>
        /// <returns></returns>
        IEnumerable<string> GetWarThunderDatabaseFileNames();

        /// <summary> Gets all client versions for which a database is found. </summary>
        /// <returns></returns>
        IEnumerable<Version> GetWarThunderDatabaseVersions();

        /// <summary> Checks whether the directory with the specified path has all required files as listed with public constants in the given type (See <see cref="EFile"/>). </summary>
        /// <param name="path"> The path of the directory to validate. </param>
        /// <param name="constantType"> The type that contains public constants whose values correspond with required files. </param>
        /// <returns></returns>
        bool LocationIsValid(string path, Type constantType);

        /// <summary> Checks whether the directory with the specified path contains files required from War Thunder. </summary>
        /// <param name="path"> The path of the directory to validate. </param>
        /// <returns></returns>
        bool WarThunderLocationIsValid(string path);
        
        /// <summary> Checks whether the directory with the specified path contains files required from Klensy's War Thunder Tools. </summary>
        /// <param name="path"> The path of the directory to validate. </param>
        /// <returns></returns>
        bool KlensysWarThunderToolLocationIsValid(string path);
    }
}