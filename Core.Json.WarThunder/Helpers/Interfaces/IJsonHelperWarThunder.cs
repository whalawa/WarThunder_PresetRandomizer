﻿using Core.DataBase.Helpers.Interfaces;
using Core.DataBase.WarThunder.Objects;
using Core.DataBase.WarThunder.Objects.Json;
using Core.Json.Helpers.Interfaces;
using System.Collections.Generic;

namespace Core.Json.WarThunder.Helpers.Interfaces
{
    /// <summary> Provide methods to work with JSON data specific to War Thunder. </summary>
    public interface IJsonHelperWarThunder : IJsonHelper
    {
        #region Methods: Deserialization

        /// <summary> Deserializes JSON text and creates an object instance from it. </summary>
        /// <typeparam name="T"> The object time into which to deserialize. </typeparam>
        /// <param name="jsonText"> The JSON text to deserialize. </param>
        /// <returns></returns>
        T DeserializeObject<T>(string jsonText);

        /// <summary> Deserializes JSON text and creates a collection of object instances from it. </summary>
        /// <typeparam name="T"> The object time into which to deserialize. </typeparam>
        /// <param name="jsonText"> The JSON text to deserialize. </param>
        /// <returns> A collection of object instances. </returns>
        IDictionary<string, T> DeserializeDictionary<T>(string jsonText);

        /// <summary> Deserializes given JSON text into instances of interim non-persistent objects. </summary>
        /// <typeparam name="T"> A generic type of JSON mapping classes. </typeparam>
        /// <param name="jsonText"> JSON text to deserialize. </param>
        /// <returns></returns>
        IEnumerable<T> DeserializeList<T>(string jsonText) where T : DeserializedFromJson;

        /// <summary> Deserializes given JSON text into instances persistent objects. </summary>
        /// <typeparam name="T"> A generic type of persistent objects. </typeparam>
        /// <param name="dataRepository"> The data repository to assign new instances to. </param>
        /// <param name="jsonText"> JSON text to deserialize. </param>
        /// <returns></returns>
        IEnumerable<T> DeserializeList<T>(IDataRepository dataRepository, string jsonText) where T : PersistentObjectWithIdAndGaijinId;

        #endregion Methods: Deserialization
    }
}
