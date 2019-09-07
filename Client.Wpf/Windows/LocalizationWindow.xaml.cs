﻿using Client.Shared.Enumerations;
using Client.Wpf.Enumerations;
using Client.Wpf.Enumerations.Logger;
using Client.Wpf.Presenters.Interfaces;
using Client.Wpf.Windows.Interfaces;
using Client.Wpf.Windows.Interfaces.Base;
using Core.Enumerations.Logger;
using Core.Localization.Enumerations;
using Core.WarThunderExtractionToolsIntegration;
using System;
using System.Diagnostics;
using System.Windows;

namespace Client.Wpf.Windows
{
    /// <summary> Interaction logic for LocalizationWindow.xaml. </summary>
    public partial class LocalizationWindow : BaseWindow, ILocalizationWindow
    {
        #region Fields

        /// <summary> Indicates whether the application is to be restarted after selecting a language. </summary>
        private readonly bool _restartAfterSelection;

        #endregion Fields
        #region Properties

        /// <summary> An instance of a presenter. </summary>
        new public ILocalizationWindowPresenter Presenter { get; private set; }

        /// <summary> An instance of a presenter. </summary>
        IPresenter IWindowWithPresenter.Presenter => Presenter;

        #endregion Properties
        #region Constructors

        /// <summary> Creates a new localization window. </summary>
        /// <param name="presenter"> The presenter to attach. </param>
        /// <param name="restartAfterSelection"> Whether the application is to be restarted after selecting a language. </param>
        public LocalizationWindow(ILocalizationWindowPresenter presenter, bool restartAfterSelection = false)
            : base(EWpfClientLogCategory.LocalizationWindow)
        {
            _restartAfterSelection = restartAfterSelection;

            Presenter = presenter;
            Presenter.SetParentWindow(this);

            InitializeComponent();
            Localize();

            Log.Debug(ECoreLogMessage.Initialized);
        }

        #endregion Constructors
        #region Methods: Event Handlers

        /// <summary> Logs showing of the window. </summary>
        /// <param name="sender"> Not used. </param>
        /// <param name="eventArguments"> Not used. </param>
        private void OnLoaded(object sender, RoutedEventArgs eventArguments) =>
            Log.Debug(ECoreLogMessage.Shown);

        /// <summary> Select US English as the localization language. </summary>
        /// <param name="sender"> Not used. </param>
        /// <param name="eventArguments"> Not used. </param>
        private void OnClick_SelectEnglishUsa(object sender, RoutedEventArgs eventArguments) =>
            SelectLocalization(ELanguage.EnglishUsa);

        /// <summary> Logs closing of the window and shuts the application down if no localization is selected (in case of the first launch). </summary>
        /// <param name="sender"> Not used. </param>
        /// <param name="eventArguments"> Not used. </param>
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs eventArguments)
        {
            Log.Debug(ECoreLogMessage.Closed);

            if (string.IsNullOrWhiteSpace(Presenter.Language) && string.IsNullOrWhiteSpace(WpfSettings.Localization))
                Environment.Exit(0);
        }

        #endregion Methods: Event Handlers

        /// <summary> Applies localization to visible text in the window. </summary>
        public override void Localize()
        {
            Title = ApplicationHelpers.LocalizationManager is null
                ? EClientApplicationName.WarThunderPresetRandomizerAbbreviated
                : ApplicationHelpers.LocalizationManager.GetLocalizedString(ELocalizationKey.ApplicationName);
        }

        /// <summary> Selects the given language for localization and restarts the application if it's not the first start. </summary>
        /// <param name="language"> The language to select for localization. </param>
        private void SelectLocalization(ELanguage language)
        {
            if (_restartAfterSelection && !RestartApproved())
                return;

            Presenter.Language = language.ToString();
            Presenter.ExecuteCommand(ECommandName.SelectLocalization);

            if (_restartAfterSelection)
            {
                Process.Start(Application.ResourceAssembly.Location);
                Environment.Exit(0);
            }
            else
            {
                Close();
            }
        }

        /// <summary> Show a dialog warning the user about restarting the application and asking for confirmation. </summary>
        /// <returns></returns>
        private bool RestartApproved()
        {
            var title = ApplicationHelpers.LocalizationManager.GetLocalizedString(ELocalizationKey.ApplicationName);
            var message = ApplicationHelpers.LocalizationManager.GetLocalizedString(ELocalizationKey.ApplicationWillBeRestarted);

            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }
    }
}