﻿using Client.Wpf.Controls.Base;
using Client.Wpf.Enumerations;
using Core.DataBase.WarThunder.Enumerations;
using Core.DataBase.WarThunder.Objects.Interfaces;
using Core.Organization.Objects;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Client.Wpf.Controls
{
    /// <summary> Interaction logic for ResearchTreeNationControl.xaml. </summary>
    public partial class ResearchTreeNationControl : LocalizedUserControl
    {
        #region Fields

        /// <summary> The map of the branch enumeration onto corresponding controls. </summary>
        private readonly IDictionary<EBranch, ResearchTreeBranchControl> _branchControls;

        #endregion Fields
        #region Properties

        /// <summary> The embedded tab control. </summary>
        internal TabControl TabControl => _tabControl;

        /// <summary> The map of the branch enumeration onto corresponding tabs. </summary>
        internal IDictionary<EBranch, TabItem> BranchTabs { get; }

        #endregion Properties
        #region Constructors

        /// <summary> Creates a new control. </summary>
        public ResearchTreeNationControl()
        {
            InitializeComponent();

            _armyTab.Tag = EBranch.Army;
            _helicoptersTab.Tag = EBranch.Helicopters;
            _aviationTab.Tag = EBranch.Aviation;
            _fleetTab.Tag = EBranch.Fleet;

            BranchTabs = new Dictionary<EBranch, TabItem>
            {
                { EBranch.Army, _armyTab },
                { EBranch.Helicopters, _helicoptersTab },
                { EBranch.Aviation, _aviationTab },
                { EBranch.Fleet, _fleetTab },
            };
            _branchControls = new Dictionary<EBranch, ResearchTreeBranchControl>
            {
                { EBranch.Army, _armyBranch },
                { EBranch.Helicopters, _helicoptersBranch },
                { EBranch.Aviation, _aviationBranch },
                { EBranch.Fleet, _fleetBranch },
            };
        }

        #endregion Constructors

        /// <summary> Returns the string representation of the object. </summary>
        /// <returns></returns>
        public override string ToString() => $"[{base.ToString()}] {(Parent as FrameworkElement)?.Tag}";

        /// <summary> Applies localization to visible text on the control. </summary>
        public override void Localize()
        {
            base.Localize();

            _armyTab.Header = ApplicationHelpers.LocalizationManager.GetLocalizedString(ELocalizationKey.Army);
            _helicoptersTab.Header = ApplicationHelpers.LocalizationManager.GetLocalizedString(ELocalizationKey.Helicopters);
            _aviationTab.Header = ApplicationHelpers.LocalizationManager.GetLocalizedString(ELocalizationKey.Planes);
            _fleetTab.Header = ApplicationHelpers.LocalizationManager.GetLocalizedString(ELocalizationKey.Fleet);
        }

        /// <summary> Populates tabs with appropriate research trees. </summary>
        public void Populate(ResearchTree researchTree)
        {
            foreach (var branchTabKeyValuePair in BranchTabs)
            {
                if (researchTree.TryGetValue(branchTabKeyValuePair.Key, out var branch))
                {
                    _branchControls[branchTabKeyValuePair.Key].Populate(branch);
                    continue;
                }
                branchTabKeyValuePair.Value.IsEnabled = false;
            }
        }

        /// <summary> Displays <see cref="IVehicle.BattleRating"/> values for the given <paramref name="gameMode"/>. </summary>
        /// <param name="gameMode"> The game mode for which to display the battle rating. </param>
        public void DisplayBattleRatingFor(EGameMode gameMode)
        {
            foreach (var vehicleCell in _branchControls.Values)
                vehicleCell.DisplayBattleRatingFor(gameMode);
        }
    }
}