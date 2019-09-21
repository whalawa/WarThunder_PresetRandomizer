﻿using Core.Helpers.Interfaces;

namespace Core.UnpackingToolsIntegration.Helpers.Interfaces
{
    /// <summary> Handles work with settings files. </summary>
    public interface IWarThunderSettingsManager : ISettingsManager
    {
        #region Methods: Validation

        /// <summary> Checks whether the currently loaded location of Klensy's War Thunder Tools is valid. </summary>
        /// <returns></returns>
        bool KlensysWarThunderToolLocationIsValid();

        /// <summary> Checks whether the currently loaded location of War Thunder is valid. </summary>
        /// <returns></returns>
        bool WarThunderLocationIsValid();

        #endregion Methods: Validation
    }
}