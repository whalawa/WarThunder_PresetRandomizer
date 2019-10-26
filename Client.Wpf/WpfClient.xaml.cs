﻿using Client.Wpf.Enumerations;
using Client.Wpf.Enumerations.Logger;
using Client.Wpf.Windows;
using Client.Wpf.Windows.Interfaces.Base;
using Core.DataBase.WarThunder.Enumerations;
using Core.Enumerations;
using Core.Enumerations.Logger;
using Core.Extensions;
using Core.Helpers.Logger.Interfaces;
using Core.Localization.Helpers.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Client.Wpf
{
    /// <summary> Interaction logic for App.xaml. </summary>
    public partial class WpfClient : Application, IWpfClient
    {
        #region Constants

        /// <summary> The default <see cref="TextBlock.FontSize"/>.</summary>
        public const int DefaultFontSize = EInteger.Number.Sixteen;

        #endregion Constants
        #region Properties

        /// <summary> An instance of an active logger. </summary>
        public IActiveLogger Log { get; private set; }

        #endregion Properties
        #region Methods: Event Handlers

        /// <summary> Organizes initialization and the overall flow, handles exceptions. </summary>
        /// <param name="sender"> Not used. </param>
        /// <param name="eventArguments"> Not used, launch arguments currently aren't supported. </param>
        [STAThread]
        private void OnStartup(object sender, StartupEventArgs eventArguments)
        {
            try
            {
                Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                ApplicationHelpers.InitializeLoggers();

                Log = ApplicationHelpers.CreateActiveLogger(EWpfClientLogCategory.WpfClient);
                Log.Info(ECoreLogMessage.Started);

                ApplicationHelpers.Initialize();
                ApplicationHelpers.Manager.RemoveOldLogFiles();

                ApplicationHelpers.WindowFactory.CreateLoadingWindow().ShowDialog();
                ApplicationHelpers.WindowFactory.CreateMainWindow().ShowDialog();
            }
            catch (Exception exception)
            {
                ShowErrorMessage(exception);
                Log?.Fatal(ECoreLogMessage.AnErrorHasOccurred, exception);
                Environment.Exit(1);
            }
        }

        #endregion Methods: Event Handlers
        #region Constuctors

        /// <summary> Creates a new application. </summary>
        public WpfClient()
        {
            var vectorImageKeys = typeof(EVectorImageKey.Flag)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .ToDictionary(property => property.Name, property => property.GetValue(null).ToString())
            ;

            foreach(var item in typeof(ECountry).GetEnumerationItems<ECountry>())
            {
                if (vectorImageKeys.TryGetValue(item.ToString(), out var resourceKey))
                    EReference.CountryIconsKeys.Add(item, resourceKey);
            }
        }

        #endregion Constuctors

        /// <summary>
        /// Shows the error message.
        /// If the <see cref="ApplicationHelpers.Loggers"/> are not yet initialized the call stack is shown within the message instead of the logs.
        /// If the <see cref="ApplicationHelpers.LocalizationManager"/> is not yet initialized, the message is show in English.
        /// </summary>
        /// <param name="exception"> The exception whose stack trace to use if <see cref="ApplicationHelpers.Loggers"/> are not yet initialized. </param>
        private void ShowErrorMessage(Exception exception)
        {
            string title;
            string message;
            string getInstruction(string localizedString) => Log is null ? $"\n{exception}" : localizedString;

            if (ApplicationHelpers.LocalizationManager is ILocalizationManager localizationManager)
            {
                title = localizationManager.GetLocalizedString(ELocalizationKey.Error);
                message = $"{localizationManager.GetLocalizedString(ELocalizationKey.FatalErrorShutdown)}\n{getInstruction(localizationManager.GetLocalizedString(ELocalizationKey.SeeLogsForDetails))}";
            }
            else
            {
                title = EWord.Error;
                message = $"{ECoreLogMessage.FatalErrorShutdown}\n{getInstruction(ECoreLogMessage.SeeLogsForDetails)}";
            }

            MessageBox.Show(Current.Windows.OfType<BaseWindow>().LastOrDefault(), message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}