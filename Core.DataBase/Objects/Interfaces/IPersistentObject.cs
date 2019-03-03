﻿using Core.DataBase.Helpers.Interfaces;
using System.Collections.Generic;

namespace Core.DataBase.Objects.Interfaces
{
    /// <summary> A persistent (stored in a database) object. </summary>
    public interface IPersistentObject
    {
        /// <summary> Initializes non-persistent fields of the instance. Use this method to finalize reading from a database. </summary>
        /// <param name="dataRepository"> A data repository to assign the object to. </param>
        void InitializeNonPersistentFields(IDataRepository dataRepository);

        /// <summary> Returns all persistent objects nested in the instance. This method requires overriding implementation to function. </summary>
        /// <returns></returns>
        IEnumerable<IPersistentObject> GetAllNestedObjects();

        /// <summary>
        /// Checks whether the specified instance can be considered equivalent to the current one.
        /// </summary>
        /// <param name="comparedPersistentObject"> An instance of a compared object. </param>
        /// <param name="includeNestedObjects"> Whether to use nested objects for comparison. </param>
        /// <param name="recursionLevel"> The level of recursion up to which to compare nested objects. Use with CAUTION in case of cyclic links. Set to zero to disable recursion. </param>
        /// <returns></returns>
        bool IsEquivalentTo(IPersistentObject comparedPersistentObject, bool includeNestedObjects, int recursionLevel = 0);

        /// <summary> Commit changes to the current persistent object (persist if the object is transient) using the <see cref="IDataRepository"/> provided with the object's constructor. </summary>
        void CommitChanges();
    }
}
