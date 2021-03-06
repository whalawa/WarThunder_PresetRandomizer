﻿using Core.DataBase.Helpers.Interfaces;
using Core.DataBase.Objects;
using Core.DataBase.WarThunder.Objects.Interfaces;

namespace Core.DataBase.WarThunder.Objects.VehicleGameModeParameterSets
{
    public class VehicleGameModeParameterSetBase : PersistentObjectWithId, IVehicleGameModeParameterSetBase
    {
        #region Properties

        /// <summary> The vehicle this set belongs to. </summary>
        public virtual IPersistentDeserialisedObjectWithId Entity { get; protected set; }

        /// <summary> An internal value used during initialization. </summary>
        protected internal virtual object InternalArcade { get; set; }

        /// <summary> An internal value used during initialization. </summary>
        protected internal virtual object InternalRealistic { get; set; }

        /// <summary> An internal value used during initialization. </summary>
        protected internal virtual object InternalSimulator { get; set; }

        /// <summary> An internal value used during initialization. </summary>
        protected internal virtual object InternalEvent { get; set; }

        #endregion Properties
        #region Constructors

        public VehicleGameModeParameterSetBase()
        {
        }

        public VehicleGameModeParameterSetBase(IDataRepository dataRepository, long id)
            : base(dataRepository, id)
        {
        }

        #endregion Constructors
    }
}