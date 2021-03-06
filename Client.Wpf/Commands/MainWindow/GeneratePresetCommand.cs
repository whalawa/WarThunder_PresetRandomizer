﻿using Client.Wpf.Enumerations;
using Client.Wpf.Presenters.Interfaces;
using Core.Enumerations;
using Core.Extensions;
using Core.Organization.Enumerations;
using Core.Organization.Objects.SearchSpecifications;
using System;
using System.Linq;

namespace Client.Wpf.Commands.MainWindow
{
    /// <summary> A command for generating vehicle presets. </summary>
    public class GeneratePresetCommand : Command
    {
        #region Constructors

        /// <summary> Creates a new command. </summary>
        public GeneratePresetCommand()
            : base(ECommandName.GeneratePreset)
        {
        }

        #endregion Constructors

        /// <summary> Defines the method that determines whether the command can execute in its current state. </summary>
        /// <param name="parameter"> Data used by the command. An <see cref="IMainWindowPresenter"/> is expected. </param>
        /// <returns></returns>
        public override bool CanExecute(object parameter)
        {
            if (!base.CanExecute(parameter))
                return false;

            if (!(parameter is IMainWindowPresenter presenter))
                return false;

            return presenter.EnabledBranches.Any() && presenter.EnabledNations.Any() && presenter.EnabledRanks.Any() && presenter.EnabledVehicleGaijinIds.Any();
        }

        /// <summary> Defines the method to be called when the command is invoked. </summary>
        /// <param name="parameter"> Data used by the command. An <see cref="IMainWindowPresenter"/> is expected. </param>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            if (parameter is IMainWindowPresenter presenter)
            {
                presenter.ToggleLongOperationIndicator(true);
                presenter.Owner.BeginInit();
                {
                    presenter.ExecuteCommand(ECommandName.DeletePresets);

                    var gameMode = presenter.CurrentGameMode;
                    var emptyBranches = presenter.GetEmptyBranches();
                    var vehicleClasses = presenter.EnabledVehicleClassesByBranches;
                    var branchSpecifications = presenter.EnabledBranches.ToDictionary(branch => branch, branch => new BranchSpecification(branch, vehicleClasses[branch]));
                    var nationSpecifications = presenter
                        .EnabledNations
                        .ToDictionary
                        (
                            nation => nation,
                            nation => new NationSpecification
                            (
                                nation,
                                presenter.EnabledCountries.Where(nationCountryPair => nationCountryPair.Nation == nation).Select(nationCountryPair => nationCountryPair.Country),
                                presenter.EnabledBranches.Except(emptyBranches[nation]),
                                EInteger.Number.Ten
                            )
                        );
                    var specification = new Specification
                    (
                        presenter.Randomisation,
                        gameMode,
                        nationSpecifications,
                        branchSpecifications,
                        presenter.EnabledVehicleBranchTags,
                        presenter.EnabledVehicleSubclasses,
                        presenter.EnabledRanks,
                        presenter.EnabledEconomicRankIntervals,
                        presenter.EnabledVehicleGaijinIds
                    );

                    presenter.GeneratedPresets.Clear();
                    presenter.GeneratedPresets.AddRange(ApplicationHelpers.Manager.GeneratePrimaryAndFallbackPresets(specification));

                    if (presenter.GeneratedPresets.IsEmpty())
                    {
                        presenter.ShowNoResults();
                        presenter.ToggleLongOperationIndicator(false);
                        return;
                    }

                    var primaryPreset = presenter.GeneratedPresets[EPreset.Primary];
                    var selectedNation = primaryPreset.Nation;

                    if (primaryPreset.IsEmpty())
                    {
                        presenter.ShowNoVehicles(primaryPreset.Nation, primaryPreset.MainBranch);
                        presenter.ToggleLongOperationIndicator(false);
                        return;
                    }

                    var selectedBranches = primaryPreset.Select(vehicle => vehicle.Branch.AsEnumerationItem).Distinct();
                    var firstVehicle = primaryPreset.First();

                    presenter.LoadPresets();
                    presenter.DisplayPreset(EPreset.Primary);
                    presenter.EnableOnly(selectedNation, selectedBranches);
                    presenter.FocusResearchTree(selectedNation, selectedBranches.First());
                    presenter.BringIntoView(firstVehicle);
                }
                presenter.Owner.EndInit();

                presenter.ToggleLongOperationIndicator(false);
                presenter.RaiseSwitchToResearchTreeCommandCanExecuteChanged();

                GC.Collect();
            }
        }
    }
}