﻿using Client.Wpf.Enumerations;
using Client.Wpf.Helpers.Interfaces;
using Core.Helpers.Logger.Interfaces;
using Core.Json.WarThunder.Helpers.Interfaces;
using Core.Organization.Helpers;
using Core.Organization.Helpers.Interfaces;
using Core.Randomization.Helpers.Interfaces;
using Core.UnpackingToolsIntegration.Helpers.Interfaces;
using Core.WarThunderExtractionToolsIntegration;

namespace Client.Wpf.Helpers
{
    /// <summary> Controls the flow of the application. </summary>
    public class WpfClientManager: Manager, IWpfClientManager
    {
        #region Constructors

        /// <summary> Creates a new manager and loads settings stored in the <see cref="EWpfClientFile.Settings"/> file. </summary>
        public WpfClientManager
        (
            IWarThunderFileManager fileManager,
            IWarThunderFileReader fileReader,
            IWarThunderSettingsManager settingsManager,
            IParser parser,
            IUnpacker unpacker,
            IWarThunderJsonHelper jsonHelper,
            IRandomizer randomizer,
            IVehicleSelector vehicleSelector,
            params IConfiguredLogger[] loggers
        ) : base(fileManager, fileReader, settingsManager, parser, unpacker, jsonHelper, randomizer, vehicleSelector, loggers)
        {
        }

        #endregion Constructors
        #region Methods: Settings

        /// <summary> Loads settings from the file attached to the settings manager. </summary>
        protected override void LoadSettings()
        {
            WpfSettings.Localization = _settingsManager.GetSetting(nameof(WpfSettings.Localization));

            base.LoadSettings();
        }

        #endregion Methods: Settings
    }
}