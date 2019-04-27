﻿using Core.DataBase.Helpers;
using Core.DataBase.Tests;
using Core.DataBase.Tests.Enumerations;
using Core.DataBase.WarThunder.Objects;
using Core.DataBase.WarThunder.Objects.Interfaces;
using Core.Enumerations;
using Core.Extensions;
using Core.Helpers;
using Core.Helpers.Interfaces;
using Core.Helpers.Logger.Enumerations;
using Core.Json.Helpers;
using Core.Json.WarThunder.Helpers.Interfaces;
using Core.UnpackingToolsIntegration.Enumerations;
using Core.UnpackingToolsIntegration.Helpers;
using Core.UnpackingToolsIntegration.Helpers.Interfaces;
using Core.WarThunderExtractionToolsIntegration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Core.IntegrationTests
{
    [TestClass]
    public class IntegrationTests
    {
        private IFileManager _fileManager;
        private IFileReader _fileReader;
        private IUnpacker _unpacker;
        private IJsonHelperWarThunder _jsonHelper;
        private string _rootDirectory;
        private string _defaultTempDirectory;

        #region Internal Methods

        [TestInitialize]
        public void Initialize()
        {
            _fileManager = new FileManager(Presets.Logger);
            _fileReader = new FileReader(Presets.Logger);
            _unpacker = new Unpacker(Presets.Logger, _fileManager);
            _jsonHelper = new JsonHelperWarThunder(Presets.Logger);
            _rootDirectory = $"{Directory.GetCurrentDirectory()}\\TestFiles";
            _defaultTempDirectory = Settings.TempLocation;

            if (!Directory.Exists(_rootDirectory))
                Directory.CreateDirectory(_rootDirectory);
            else
                _fileManager.EmptyDirectory(_rootDirectory);

            Settings.TempLocation = _rootDirectory;
        }

        [TestCleanup]
        public void CleanUp()
        {
            Presets.Logger.LogInfo(ECoreLogCategory.IntegrationTests, ECoreLogMessage.CleanUpAfterIntegrationTestStartsHere);
            Presets.CleanUp();
            _fileManager.DeleteDirectory(_rootDirectory);

            Settings.TempLocation = _defaultTempDirectory;
        }

        public override string ToString() => nameof(IntegrationTests);

        #endregion Internal Methods

        [TestMethod]
        public void DeserializeAndPersistVehicleList()
        {
            // arrange
            var sourceFiles = new List<FileInfo>
            {
                _fileManager.GetFileInfo(Settings.WarThunderLocation, EFile.StatAndBalanceParameters)
            };

            var outputDirectories = new List<DirectoryInfo>();

            foreach (var sourceFile in sourceFiles)
            {
                var outputDirectory = new DirectoryInfo(_unpacker.Unpack(sourceFile));
                outputDirectories.Add(outputDirectory);
                _unpacker.Unpack(outputDirectory, ETool.BlkUnpacker);
            }

            var blkxFiles = new List<FileInfo>();

            foreach (var outputDirectory in outputDirectories)
                blkxFiles.AddRange(outputDirectory.GetFiles($"{ECharacter.Asterisk}{ECharacter.Period}{EFileExtension.Blkx}", SearchOption.AllDirectories));

            // act
            var wpCostJson = _fileReader.Read(blkxFiles.First(file => file.Name.Contains(EFile.GeneralVehicleData)));
            var cachedVehicles = _jsonHelper.DeserializeVehicleList(wpCostJson);

            cachedVehicles.Count().Should().BeGreaterThan(1300);
            cachedVehicles.All(vehicle => !vehicle.Value.GaijinId.IsNullOrWhiteSpaceFluently()).Should().BeTrue();

            var fileName = $"{ToString()}.{MethodBase.GetCurrentMethod().Name}()";

            using (var dataRepository = new DataRepository(fileName, true, Assembly.Load(EAssemblies.WarThunderMappingAssembly), Presets.Logger))
            {
                void assert(IEnumerable<IVehicle> vehicleCollection)
                {
                    vehicleCollection.Count().Should().BeGreaterThan(1300);
                    vehicleCollection.All(vehicle => vehicle.Id != null).Should().BeTrue();
                    vehicleCollection.All(vehicle => !vehicle.GaijinId.IsNullOrWhiteSpaceFluently()).Should().BeTrue();
                    vehicleCollection.All(vehicle => !vehicle.Nation.IsNullOrWhiteSpaceFluently()).Should().BeTrue();
                    vehicleCollection.All(vehicle => !vehicle.MoveType.IsNullOrWhiteSpaceFluently()).Should().BeTrue();
                    vehicleCollection.Any(vehicle => vehicle.PurchaseCostInSilver == 0).Should().BeTrue();
                    vehicleCollection.Any(vehicle => vehicle.PurchaseCostInSilver > 0).Should().BeTrue();
                    vehicleCollection.Any(vehicle => vehicle.NumberOfSpawnsInSimulation == null).Should().BeTrue();
                    vehicleCollection.Any(vehicle => vehicle.NumberOfSpawnsInSimulation == 1).Should().BeTrue();
                    vehicleCollection.Any(vehicle => vehicle.NumberOfSpawnsInSimulation == 2).Should().BeTrue();
                }

                foreach (var cachedVehicle in cachedVehicles)
                    new Vehicle(dataRepository, cachedVehicle.Value);

                var vehiclesBeforePersistence = new Vehicle[dataRepository.NewObjects.Count()];
                dataRepository.NewObjects.CopyTo(vehiclesBeforePersistence, 0);

                assert(dataRepository.NewObjects.OfType<IVehicle>());

                dataRepository.PersistNewObjects();

                var vehiclesAfterPersistence = dataRepository.Query<IVehicle>();

                // assert
                assert(vehiclesAfterPersistence);
                vehiclesAfterPersistence.IsEquivalentTo(vehiclesBeforePersistence).Should().BeTrue();
            }
        }
    }
}
