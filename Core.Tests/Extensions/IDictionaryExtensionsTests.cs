﻿using Core.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Core.Tests.Extensions
{
    /// <summary> See <see cref="IDictionaryExtensions"/>. </summary>
    [TestClass]
    public class IDictionaryExtensionsTests
    {
        #region Tests: AddRange()

        [TestMethod]
        public void AddRange()
        {
            // arrange
            var _0 = 0;
            var _1 = 1;
            var sourceDictionary = new Dictionary<int, int> { { _0, _0 } };
            var donorDictionary = new Dictionary<int, int> { { _1, _1 } };

            // act
            sourceDictionary.AddRange(donorDictionary);

            // assert
            sourceDictionary[_0].Should().Be(_0);
            sourceDictionary[_1].Should().Be(_1);
        }

        #endregion Tests: AddRange()
        #region Tests: AddSafely()

        [TestMethod]
        public void AddSafely_KeyAbsent_AddsKeyAndValue()
        {
            // arrange
            var actualDictionary = new Dictionary<int, int>();
            var expectedDictionary = new Dictionary<int, int> { { 1, 1 } };

            // act
            actualDictionary.AddSafely(1, 1);

            // assert
            actualDictionary.Should().BeEquivalentTo(expectedDictionary);
        }

        [TestMethod]
        public void AddSafely_KeyPresent_ReplacesValueOfKey()
        {
            // arrange
            var actualDictionary = new Dictionary<int, int> { { 1, 1 } };
            var expectedDictionary = new Dictionary<int, int> { { 1, 2 } };

            // act
            actualDictionary.AddSafely(1, 2);

            // assert
            actualDictionary.Should().BeEquivalentTo(expectedDictionary);
        }

        #endregion Tests: AddSafely()
        #region Tests: Append()

        [TestMethod]
        public void Append_NotUnique()
        {
            // arrange
            var actualDictionary = new Dictionary<int, IEnumerable<int>>();
            var expectedDictionary = new Dictionary<int, IEnumerable<int>>
            {
                { 0, new List<int> { 0, 1 } },
                { 1, new List<int> { 0 } },
                { 2, new List<int> { 0, 1, 1 } },
            };

            // act
            actualDictionary.Append(0, 0);
            actualDictionary.Append(0, 1);
            actualDictionary.Append(1, 0);
            actualDictionary.Append(2, 0);
            actualDictionary.Append(2, 1);
            actualDictionary.Append(2, 1);

            // assert
            actualDictionary.Should().BeEquivalentTo(expectedDictionary);
        }

        [TestMethod]
        public void Append_Unique()
        {
            // arrange
            var actualDictionary = new Dictionary<int, IEnumerable<int>>();
            var expectedDictionary = new Dictionary<int, IEnumerable<int>>
            {
                { 0, new List<int> { 0 } },
            };

            // act
            actualDictionary.Append(0, 0, true);
            actualDictionary.Append(0, 0, true);
            actualDictionary.Append(0, 0, true);

            // assert
            actualDictionary.Should().BeEquivalentTo(expectedDictionary);
        }

        #endregion Tests: Append()
        #region Tests: GetWithInstantiation()

        [TestMethod]
        public void GetWithInstantiation()
        {
            // arrange
            var dictionary = new Dictionary<int, object>();

            // act
            var newObject = dictionary.GetWithInstantiation(0);

            // assert
            newObject.Should().NotBeNull();
            dictionary.Count().Should().Be(1);
        }

        #endregion Tests: GetWithInstantiation()
        #region Tests: GetWithEnumerableInstantiation()

        [TestMethod]
        public void GetWithEnumerableInstantiation()
        {
            // arrange
            var dictionary = new Dictionary<int, IEnumerable<object>>();

            // act
            var newCollection = dictionary.GetWithEnumerableInstantiation(0);

            // assert
            newCollection.Should().NotBeNull();
            dictionary.Count().Should().Be(1);
        }

        #endregion Tests: GetWithEnumerableInstantiation()
        #region Tests: Increment()

        [TestMethod]
        public void Increment()
        {
            // arrange
            var key = 0;
            var dictionary = new Dictionary<int, int> { { key, 0 } };

            // act
            dictionary.Increment(key);

            // assert
            dictionary[key].Should().Be(1);
        }

        #endregion Tests: Increment()
    }
}