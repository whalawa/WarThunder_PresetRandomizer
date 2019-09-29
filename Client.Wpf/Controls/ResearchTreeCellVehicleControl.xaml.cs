﻿using Client.Wpf.Controls.Strategies.Interfaces;
using Client.Wpf.Enumerations;
using Client.Wpf.Extensions;
using Core.DataBase.WarThunder.Enumerations;
using Core.DataBase.WarThunder.Objects.Interfaces;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client.Wpf.Controls
{
    /// <summary> Interaction logic for ResearchTreeCellVehicleControl.xaml. </summary>
    public partial class ResearchTreeCellVehicleControl : UserControl
    {
        #region Fields

        /// <summary> The research type of the <see cref="Vehicle"/> in the cell. </summary>
        private readonly EVehicleResearchType _reseachType;

        /// <summary> A strategy for generating a formatted string with <see cref="IVehicle"/> information for the given <see cref="EGameMode"/>. </summary>
        private readonly IDisplayVehicleInformationStrategy _displayVehicleInformationStrategy;

        #endregion Fields
        #region Properties

        /// <summary> The vehicle in the cell. </summary>
        internal IVehicle Vehicle { get; }

        #endregion Properties
        #region Constructors

        /// <summary> Creates a new control. </summary>
        public ResearchTreeCellVehicleControl()
        {
            InitializeComponent();
        }

        /// <summary> Creates a new control. </summary>
        /// <param name="vehicle"> The vehicle positioned in the cell. </param>
        public ResearchTreeCellVehicleControl(IVehicle vehicle, IDisplayVehicleInformationStrategy displayVehicleInformationStrategy)
            : this()
        {
            _displayVehicleInformationStrategy = displayVehicleInformationStrategy;

            Vehicle = vehicle;
            _name.Text = Vehicle.ResearchTreeName.GetLocalization(WpfSettings.LocalizationLanguage);

            if (Vehicle.IsSquadronVehicle)
                _reseachType = EVehicleResearchType.Squadron;
            else if (Vehicle.IsPremium)
                _reseachType = EVehicleResearchType.Premium;
            else
                _reseachType = EVehicleResearchType.Regular;

            ApplyIdleStyle();
        }

        #endregion Constructors
        #region Methods: Event Handlers

        /// <summary> Applies the highlighting style to the <see cref="_border"/>. </summary>
        /// <param name="sender"> The object that has triggered the event. A <see cref="Border"/> is expected. </param>
        /// <param name="eventArguments"> Not used. </param>
        private void OnMouseEnter(object sender, MouseEventArgs eventArguments)
        {
            if (sender is Border)
                ApplyHighlightStyle();
        }

        /// <summary> Applies the idle style to the <see cref="_border"/>. </summary>
        /// <param name="sender"> The object that has triggered the event. A <see cref="Border"/> is expected. </param>
        /// <param name="eventArguments"> Not used. </param>
        private void OnMouseLeave(object sender, MouseEventArgs eventArguments)
        {
            if (sender is Border)
                ApplyIdleStyle();
        }

        #endregion Methods: Event Handlers

        /// <summary> Displays vehicle information for the given <paramref name="gameMode"/>. </summary>
        /// <param name="gameMode"> The game mode for which to display the information. </param>
        public void DisplayVehicleInformation(EGameMode gameMode)
        {
            _battleRating.Text = _displayVehicleInformationStrategy.GetFormattedVehicleInformation(gameMode, Vehicle);
        }

        /// <summary> Applies the idle style to the <see cref="_border"/>. </summary>
        internal void ApplyIdleStyle()
        {
            _border.Style = _reseachType switch
            {
                EVehicleResearchType.Squadron => this.GetStyle(EStyleKey.Border.SquadronResearchTreeCell),
                EVehicleResearchType.Premium => this.GetStyle(EStyleKey.Border.PremiumResearchTreeCell),
                _ => this.GetStyle(EStyleKey.Border.ResearchTreeCell),
            };
        }

        /// <summary> Applies the highlighting style to the <see cref="_border"/>. </summary>
        internal void ApplyHighlightStyle()
        {
            _border.Style = _reseachType switch
            {
                EVehicleResearchType.Squadron => this.GetStyle(EStyleKey.Border.SquadronResearchTreeCellHighlighted),
                EVehicleResearchType.Premium => this.GetStyle(EStyleKey.Border.PremiumResearchTreeCellHighlighted),
                _ => this.GetStyle(EStyleKey.Border.ResearchTreeCellHighlighted),
            };
        }
    }
}