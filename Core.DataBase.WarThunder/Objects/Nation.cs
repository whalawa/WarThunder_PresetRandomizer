﻿using Core.DataBase.Helpers.Interfaces;
using Core.DataBase.Objects;
using Core.Enumerations.DataBase;
using Core.Objects.Interfaces;
using NHibernate.Mapping.Attributes;
using System;
using System.Collections.Generic;

namespace Core.Objects
{
    /// <summary> A nation in the game. </summary>
    [Class(Table = ETable.Nation)]
    public class Nation : PersistentObjectWithIdAndName, INation
    {
        #region Persistent Properties

        /// <summary> The nation's ID. </summary>
        [Id(Column = EColumn.Id, TypeType = typeof(Guid), Name = nameof(Id))] // The type and name of the identificator column have to be explicitly specified.
        public override Guid Id // All persistent properties have to be public/protected virtual and have public/protected setters.
        {
            get { return _id; }
            // Even though we don't want to change IDs after creation,
            // a setter is required by NHibernate to initialize persistent properties of an object after reading data from a database and instantiating the object with the parameterless constructor.
            protected set { _id = value; }
        }

        /// <summary> The nation's Name. </summary>
        [Property(NotNull = true)]
        public override string Name
        {
            get { return _name; }
            protected set { _name = value; }
        }

        #endregion Persistent Properties
        #region Association Properties

        /// <summary> The nation's military branches. </summary>
        [OneToMany(ClassType = typeof(Branch))]
        public virtual IEnumerable<IBranch> Branches { get; protected set; }

        #endregion Association Properties
        #region Constructors

        /// <summary>
        /// Creates a new transient object that can be persisted later.
        /// This constructor is used by NHibernate to instantiate deserialized data read from a database.
        /// </summary>
        protected Nation()
        {
        }

        /// <summary> Creates a new nation. </summary>
        /// <param name="dataRepository"> A data repository to persist the object with. </param>
        /// <param name="name"> The nation's name. </param>
        public Nation(IDataRepository dataRepository, string name)
            : base(dataRepository, name)
        {
        }

        #endregion Constructors
    }
}