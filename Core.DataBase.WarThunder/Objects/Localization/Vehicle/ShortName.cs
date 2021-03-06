﻿using Core.DataBase.Enumerations;
using Core.DataBase.Helpers.Interfaces;
using Core.DataBase.WarThunder.Enumerations.DataBase;
using Core.DataBase.WarThunder.Objects.Interfaces;
using NHibernate.Mapping.Attributes;
using System.Collections.Generic;

namespace Core.DataBase.WarThunder.Objects.Localization.Vehicle
{
    [Class(Table = ETable.LocalisationVehicleShortName)]
    public class ShortName : VehicleLocalisation
    {
        #region Persistent Properties

        /// <summary> The localization set's ID. </summary>
        [Id(Column = EColumn.Id, TypeType = typeof(long), Name = nameof(Id), Generator = EIdGenerator.HiLo)]
        public override long Id { get; protected set; }

        /// <summary> The vehicle this localisation belongs to. </summary>
        [ManyToOne(0, Column = ETable.Vehicle + "_" + EColumn.Id, ClassType = typeof(Objects.Vehicle), NotNull = true, Lazy = Laziness.Proxy)]
        [Key(1, Unique = true, Column = ETable.Vehicle + "_" + EColumn.Id)]
        public override IVehicle Vehicle { get; protected set; }

        [Property] public override string English { get; protected set; }

        [Property] public override string French { get; protected set; }

        [Property] public override string Italian { get; protected set; }

        [Property] public override string German { get; protected set; }

        [Property] public override string Spanish { get; protected set; }

        [Property] public override string Russian { get; protected set; }

        [Property] public override string Polish { get; protected set; }

        [Property] public override string Czech { get; protected set; }

        [Property] public override string Turkish { get; protected set; }

        [Property] public override string Chinese { get; protected set; }

        [Property] public override string Japanese { get; protected set; }

        [Property] public override string Portuguese { get; protected set; }

        [Property] public override string Vietnamese { get; protected set; }

        [Property] public override string Ukrainian { get; protected set; }

        [Property] public override string Serbian { get; protected set; }

        [Property] public override string Hungarian { get; protected set; }

        [Property] public override string Korean { get; protected set; }

        [Property] public override string Belarusian { get; protected set; }

        [Property] public override string Romanian { get; protected set; }

        [Property] public override string TChinese { get; protected set; }

        [Property] public override string HChinese { get; protected set; }

        #endregion Persistent Properties
        #region Constructors

        /// <summary> This constructor is used by NHibernate to instantiate an entity read from a database. </summary>
        protected ShortName()
        {
        }

        /// <summary> Creates a new transient object that can be persisted later. </summary>
        /// <param name="dataRepository"> A data repository to persist the object with. </param>
        /// <param name="vehicle"> The vehicle this localisation belongs to. </param>
        /// <param name="localisationRecord"> A collection of localisation values read from CSV files. </param>
        public ShortName(IDataRepository dataRepository, IVehicle vehicle, IList<string> localisationRecord)
            : base(dataRepository, vehicle, localisationRecord)
        {
        }

        #endregion Constructors
    }
}