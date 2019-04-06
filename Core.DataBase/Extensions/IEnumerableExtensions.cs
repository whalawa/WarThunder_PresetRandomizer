﻿using Core.DataBase.Objects.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Core.Extensions
{
    /// <summary> Methods extending the <see cref="IEnumerable{T}"/> class. </summary>
    public static class IEnumerableExtensions
    {
        #region Methods: Equivalence

        public static bool IsEquivalentTo(this IEnumerable<IPersistentObject> sourceCollection, IEnumerable<IPersistentObject> comparedCollection, bool includeNestedObjects, int recursionLevel = 0)
        {
            if (sourceCollection.Count() == comparedCollection.Count())
            {
                return sourceCollection
                    .Zip(comparedCollection, (source, target) => source.IsEquivalentTo(target, recursionLevel.IsPositive(), recursionLevel - 1))
                    .All(isEquivalent => isEquivalent)
                ;
            }
            return false;
        }

        #endregion Methods: Equivalence
    }
}