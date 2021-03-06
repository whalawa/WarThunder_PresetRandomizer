﻿using Client.Wpf.Enumerations;
using Client.Wpf.Presenters.Interfaces;
using Core.Localization.Helpers;

namespace Client.Wpf.Commands.LocalizationWindow
{
    /// <summary> A command for selecting a localization language within the <see cref="Windows.LocalizationWindow"/>. </summary>
    public class SelectLocalizationCommand : Command
    {
        #region Constructors

        /// <summary> Creates a new command. </summary>
        public SelectLocalizationCommand()
            : base(ECommandName.SelectLocalization)
        {
        }

        #endregion Constructors

        /// <summary> Defines the method to be called when the command is invoked. </summary>
        /// <param name="parameter"> Data used by the command. An <see cref="ILocalizationWindowPresenter"/> is expected. </param>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            if (parameter is ILocalizationWindowPresenter presenter)
            {
                ApplicationHelpers.LocalisationManager = new LocalisationManager(ApplicationHelpers.FileReader, presenter.Language, ApplicationHelpers.Loggers);
                ApplicationHelpers.SettingsManager.Save(nameof(WpfSettings.Localization), presenter.Language);
            }
        }
    }
}