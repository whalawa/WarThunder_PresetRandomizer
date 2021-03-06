﻿using Core.Enumerations;
using Core.Enumerations.Logger;
using Core.Extensions;
using Core.Helpers.Interfaces;
using Core.Helpers.Logger;
using Core.Helpers.Logger.Interfaces;
using Core.Localization.Enumerations.Logger;
using Core.Localization.Helpers.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Core.Localization.Helpers
{
    /// <summary> Provide methods to work with localisation. </summary>
    public class LocalisationManager : LoggerFluency, ILocalisationManager
    {
        #region Fields

        private readonly IFileReader _fileReader;

        private readonly IDictionary<string, string> _localisation;

        #endregion Fields
        #region Constructors

        /// <summary> Creates a new localization manager for a specified localization file and loads it into memory. </summary>
        /// <param name="fileReader"> The file reader to use. </param>
        /// <param name="localizationFileName"> The name of the localization file to load into memory. </param>
        /// <param name="loggers"> Instancs of loggers. </param>
        public LocalisationManager(IFileReader fileReader, string localizationFileName, params IConfiguredLogger[] loggers)
            : base(ELocalisationLogCategory.LocalisationManager, loggers)
        {
            _fileReader = fileReader;

            var localizationFile = new FileInfo(Path.Combine(EWord.Localisation, $"{localizationFileName}{ECharacter.Period}{EFileExtension.Xml}"));

            if (!localizationFile.Exists)
                throw new FileNotFoundException(ECoreLogMessage.NotFound.FormatFluently(localizationFile.FullName));

            _localisation = LoadLocalisation(localizationFile);

            LogDebug(ECoreLogMessage.Created.FormatFluently(ELocalisationLogCategory.LocalisationManager));
        }

        #endregion Constructors

        /// <summary> Loads the specified localisation file into memory. </summary>
        /// <param name="localisationFile"> The file to load. </param>
        /// <returns></returns>
        private IDictionary<string, string> LoadLocalisation(FileInfo localisationFile)
        {
            var fileContents = _fileReader.Read(localisationFile);

            return XElement
                .Parse(fileContents)
                .Elements(EWord.Line)
                .ToDictionary
                (
                    element => (string)element.Attribute(EWord.Key.ToLower()),
                    element => (string)element.Attribute(EWord.Value.ToLower())
                )
            ;
        }

        /// <summary> Returns a localised string by its key. </summary>
        /// <param name="key"> The key of the localised string. </param>
        /// <param name="suppressWarnings"> Whether to suppress warnings if a key was not found. </param>
        /// <returns></returns>
        public string GetLocalisedString(string key, bool suppressWarnings = false)
        {
            if (key is null)
                return string.Empty;

            if (_localisation.ContainsKey(key))
                return _localisation[key];

            if (!suppressWarnings)
                LogWarn(ELocalisationLogMessage.LocalisationKeyNotFound.FormatFluently(key));

            return key;
        }

        public string GetLocalisedString(object key, bool suppressWarnings = false) =>
            GetLocalisedString(key?.ToString(), suppressWarnings);
    }
}