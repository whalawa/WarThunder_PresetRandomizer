﻿using Core.DataBase.WarThunder.Enumerations;
using Core.Enumerations;
using Core.Extensions;
using System.Linq;

namespace Core.DataBase.WarThunder.Objects.Connectors
{
    public class NationTagPair
    {
        #region Properties

        public ENation Nation { get; }

        public EVehicleBranchTag Tag { get; }

        #endregion Properties
        #region Constructors

        public NationTagPair(ENation nation, EVehicleBranchTag tag)
        {
            Nation = nation;
            Tag = tag;
        }

        #endregion Constructors
        #region Methods: Equality Comparison

        public override bool Equals(object obj)
        {
            if (!(obj is NationTagPair otherPair))
                return false;

            return Nation.Equals(otherPair.Nation)
                && Tag.Equals(otherPair.Tag);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = EInteger.Number.PrimesAboveHundred.First();

                hash = hash * EInteger.Number.PrimesAboveHundred.Second() + Nation.GetHashCode();
                hash = hash * EInteger.Number.PrimesAboveHundred.Third() + Tag.GetHashCode();

                return hash;
            }
        }

        #endregion Methods: Equality Comparison

        public override string ToString() => $"{Nation}{ECharacter.Underscore}{Tag}";
    }
}