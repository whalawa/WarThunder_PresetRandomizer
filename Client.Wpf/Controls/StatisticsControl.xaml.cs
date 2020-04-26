﻿using Client.Wpf.Controls.Base;
using Client.Wpf.Enumerations;
using Client.Wpf.Enumerations.ShrinkProfiles;
using Client.Wpf.Presenters.Interfaces;
using Core.DataBase.WarThunder.Objects;
using Core.DataBase.WarThunder.Objects.Interfaces;
using Core.Enumerations;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Client.Wpf.Controls
{
    /// <summary> Interaction logic for StatisticsControl.xaml. </summary>
    public partial class StatisticsControl : LocalisedUserControl
    {
        #region Fields

        private bool _initialised;

        private IMainWindowPresenter _presenter;

        #endregion Fields
        #region Properties

        internal IDictionary<NationCountryPair, IOrderedEnumerable<IVehicle>> VehiclesByNationsAndCountries { get; private set; }

        internal IDictionary<NationCountryPair, IOrderedEnumerable<IVehicle>> VehiclesByCountriesAndNations { get; private set; }

        #endregion Properties
        #region Constructors

        public StatisticsControl()
        {
            InitializeComponent();
        }

        #endregion Constructors
        #region Methods: Overrides

        public override void Localise()
        {
            base.Localise();

            _vehicleCountsHeader.Text = ApplicationHelpers.LocalisationManager.GetLocalisedString(ELocalisationKey.VehicleCounts);
            _vehicleListHeader.Text = ApplicationHelpers.LocalisationManager.GetLocalisedString(ELocalisationKey.VehicleList);

            _vehicleCountsControl.Localise();
            _vehicleListControl.Localise();
        }

        #endregion Methods: Overrides
        #region Methods: Initialisation

        public void Initialise(IMainWindowPresenter presenter)
        {
            if (!_initialised && presenter is IMainWindowPresenter)
            {
                _presenter = presenter;
                _initialised = true;
            }

            if (_tabControl.SelectedItem == _vehicleCountsTab)
            {
                _vehicleCountsControl.Initialise(this);
                _vehicleListControl.Initialise(_presenter);
            }
        }

        internal void SetVehiclesByNationsAndCountries(IDictionary<NationCountryPair, IOrderedEnumerable<IVehicle>> collection) =>
            VehiclesByNationsAndCountries = collection;

        internal void SetVehiclesByCountriesAndNations(IDictionary<NationCountryPair, IOrderedEnumerable<IVehicle>> collection) =>
            VehiclesByCountriesAndNations = collection;

        #endregion Methods: Initialisation
        #region Methods: Event Handlers

        private void OnTabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source == _tabControl)
            {
                Initialise(_presenter);
                e.Handled = true;
            }
        }

        #endregion Methods: Event Handlers

        internal void SwitchVehicleListTo(string key, IEnumerable<IVehicle> vehicles, EVehicleProfile shrinkProfile, ELanguage language)
        {
            _presenter.ToggleLongOperationIndicator(true);

            _vehicleListControl.SetDataSource(key, vehicles, shrinkProfile, language);
            _vehicleListControl.AdjustControlVisibility();

            _tabControl.SelectedItem = _vehicleListTab;

            _presenter.ToggleLongOperationIndicator(false);
        }
    }
}