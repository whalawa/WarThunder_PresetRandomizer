﻿using Core.DataBase.Tests.Enumerations;
using Core.DataBase.WarThunder.Helpers;
using Core.DataBase.WarThunder.Objects;
using Core.DataBase.WarThunder.Objects.Interfaces;
using Core.Enumerations.Logger;
using Core.Tests;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.DataBase.WarThunder.Tests.Objects
{
    /// <summary> See <see cref="Branch"/>. </summary>
    [TestClass]
    public class BranchTests
    {
        #region Internal Methods

        public override string ToString() => nameof(NationTests);

        [TestCleanup]
        public void CleanUp()
        {
            Presets.Logger.LogInfo(ECoreLogCategory.UnitTests, ECoreLogMessage.CleanUpAfterUnitTestStartsHere);
            Presets.CleanUp();
        }

        #endregion Internal Methods
        #region Tests: CommitChanges() and Query()

        [TestMethod]
        public void CommitChanges_Query_ReturnsObject()
        {
            // arrange
            var fileName = $"{ToString()}.{MethodBase.GetCurrentMethod().Name}()";

            using (var dataRepository = new DataRepositoryWarThunder(fileName, true, Assembly.Load(EAssemblies.WarThunderMappingAssembly), Presets.Logger))
            {
                var zimbabwe = new Nation(dataRepository, "Zimbabwe");
                var bycicleCorps = new Branch(dataRepository, "Bycicle Corps", zimbabwe);
                zimbabwe.Branches = new List<IBranch> { bycicleCorps };

                // act
                dataRepository.PersistNewObjects();

                var query = dataRepository.Query<IBranch>();
                query.Count().Should().Be(1);

                var branch = query.First();

                // assert
                branch.IsEquivalentTo(bycicleCorps, 2);
            }
        }

        #endregion Tests: CommitChanges() and Query()
        #region Tests: IsEquivalentTo()

        [TestMethod]
        public void IsEquivalentTo_Self_ShouldBeTrue()
        {
            // arrange
            var zimbabwe = new Nation(Presets.MockDataRepository.Object, "Zimbabwe");
            var bycicleCorps = new Branch(Presets.MockDataRepository.Object, "BycicleCorps", zimbabwe);
            zimbabwe.Branches = new List<IBranch> { bycicleCorps };

            // act
            var isEquivalent = bycicleCorps.IsEquivalentTo(bycicleCorps, 2);

            // assert
            isEquivalent.Should().BeTrue();
        }

        [TestMethod]
        public void IsEquivalentTo_AnotherInstance_SameNation_ShouldBeTrue()
        {
            // arrange
            var zimbabwe = new Nation(Presets.MockDataRepository.Object, "Zimbabwe");

            var bycicleCorps = new Branch(Presets.MockDataRepository.Object, "BycicleCorps", zimbabwe);
            var bycicleCorpsClone = new Branch(Presets.MockDataRepository.Object, bycicleCorps.Id, bycicleCorps.GaijinId, bycicleCorps.Nation);
            zimbabwe.Branches = new List<IBranch> { bycicleCorps, bycicleCorpsClone };

            // act
            var isEquivalent = bycicleCorps.IsEquivalentTo(bycicleCorpsClone, 2);

            // assert
            isEquivalent.Should().BeTrue();
        }

        [TestMethod]
        public void IsEquivalentTo_AnotherInstance_DifferentNations_ShouldBeFalse()
        {
            // arrange
            var zimbabwe = new Nation(Presets.MockDataRepository.Object, "Zimbabwe");
            var bycicleCorps = new Branch(Presets.MockDataRepository.Object, "BycicleCorps", zimbabwe);
            zimbabwe.Branches = new List<IBranch> { bycicleCorps };

            var estonia = new Nation(Presets.MockDataRepository.Object, "Estonia");
            var bycicleCorpsCloneFlawed = new Branch(Presets.MockDataRepository.Object, bycicleCorps.GaijinId, estonia);
            estonia.Branches = new List<IBranch> { bycicleCorpsCloneFlawed };

            // act
            var isEquivalent = bycicleCorps.IsEquivalentTo(bycicleCorpsCloneFlawed, 2);

            // assert
            isEquivalent.Should().BeFalse();
        }

        #endregion Tests: IsEquivalentTo()
    }
}