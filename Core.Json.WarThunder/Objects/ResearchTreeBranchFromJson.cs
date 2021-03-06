﻿using Core.DataBase.WarThunder.Objects.Json;
using System.Collections;
using System.Collections.Generic;

namespace Core.Json.WarThunder.Objects
{
    /// <summary> A research tree branch. </summary>
    public class ResearchTreeBranchFromJson : IEnumerable<ResearchTreeColumnFromJson>
    {
        #region Properties

        /// <summary> The Gaijin ID of the branch. </summary>
        public string GaijinId { get; }

        /// <summary> Research tree columns comprising the branch. </summary>
        public IList<ResearchTreeColumnFromJson> Columns { get; }

        /// <summary> All vehicles postioned in the branch. </summary>
        public IEnumerable<ResearchTreeVehicleFromJson> Vehicles
        {
            get
            {
                var vehicles = new List<ResearchTreeVehicleFromJson>();

                foreach (var column in Columns)
                    vehicles.AddRange(column.Vehicles);

                return vehicles;
            }
        }

        #endregion Properties
        #region Constructors

        /// <summary> Creates a new research tree branch. </summary>
        /// <param name="gaijinId"> The Gaijin ID of the branch. </param>
        public ResearchTreeBranchFromJson(string gaijinId)
        {
            GaijinId = gaijinId;
            Columns = new List<ResearchTreeColumnFromJson>();
        }

        #endregion Constructors

        public IEnumerator<ResearchTreeColumnFromJson> GetEnumerator() => Columns.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}