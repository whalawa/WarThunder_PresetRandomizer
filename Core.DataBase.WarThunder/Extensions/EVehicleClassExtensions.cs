﻿using Core.DataBase.WarThunder.Enumerations;
using Core.Enumerations;
using Core.Extensions;

namespace Core.DataBase.WarThunder.Extensions
{
    /// <summary> Methods extending the <see cref="EVehicleClass"/> enumeration. </summary>
    public static class EVehicleClassExtensions
    {
        /// <summary> Checks whether the vehicle class is valid. </summary>
        /// <param name="vehicleClass"> The vehicle class to check. </param>
        /// <returns></returns>
        public static bool IsValid(this EVehicleClass vehicleClass) =>
            vehicleClass.ToString().HasSeveral();

        /// <summary> Returns the vehicle branch to which the class belongs to. </summary>
        /// <param name="vehicleClass"> The vehicle class whose branch to get. </param>
        /// <returns></returns>
        public static EBranch GetBranch(this EVehicleClass vehicleClass)
        {
            var vehicleClassEnumerationValue = vehicleClass.CastTo<int>();

            if (vehicleClassEnumerationValue.ToString().HasSingle()) // For all classes in one branch.
                return vehicleClassEnumerationValue.CastTo<EBranch>();

            else // For specific classes.
                return vehicleClassEnumerationValue.Do(value => value / EInteger.Number.Ten).CastTo<EBranch>();
        }
    }
}