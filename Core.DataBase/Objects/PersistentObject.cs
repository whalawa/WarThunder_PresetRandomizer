﻿using Core.DataBase.Enumerations.Logger;
using Core.DataBase.Helpers.Interfaces;
using Core.DataBase.Objects.Interfaces;
using Core.Extensions;
using Core.Helpers.Logger.Enumerations;
using System;

namespace Core.DataBase.Objects
{
    /// <summary> A persistent (stored in a database) object. </summary>
    public class PersistentObject : IPersistentObject
    {
        #region Fields

        /// <summary> The category of logs generated by this object. </summary>
        protected string _logCategory;

        /// <summary> The data repository the object belongs to. </summary>
        protected IDataRepository _dataRepository;

        #endregion Fields
        #region Constructors

        /// <summary>
        /// Creates a new transient object that can be persisted later.
        /// This constructor is used to maintain inheritance of class composition required for NHibernate mapping.
        /// </summary>
        protected PersistentObject()
        {
        }

        /// <summary> Creates a new transient object that can be persisted later. </summary>
        /// <param name="dataRepository"> A data repository to persist the object with. </param>
        protected PersistentObject(IDataRepository dataRepository)
        {
            SetLogCategory();
            _dataRepository = dataRepository;
            _dataRepository.NewObjects.Add(this);

            LogCreation();
        }

        #endregion Constructors
        #region Methods: Initialization

        protected void LogCreation() => LogTrace(ECoreLogMessage.Created.FormatFluently(ToString()));

        /// <summary> Sets the <see cref="_logCategory"/> for the object. </summary>
        protected void SetLogCategory() => _logCategory = GetType().ToStringLikeCode();

        /// <summary> Initializes non-persistent fields of the object. Use this method to finalize reading from a database. </summary>
        /// <param name="dataRepository"> A data repository to assign the object to. </param>
        public virtual void InitializeNonPersistentFields(IDataRepository dataRepository)
        {
            SetLogCategory();
            _dataRepository = dataRepository;
        }

        #endregion Methods: Initialization
        #region Methods: Logging

        /// <summary> Creates a log entry of the "Trace" level for the current <see cref="_logCategory"/>. </summary>
        /// <param name="category"> The category of the event being logged. </param>
        /// <param name="message"> A message to supplement the log with. </param>
        protected void LogTrace(string message) =>
            _dataRepository.Logger.LogTrace(_logCategory, message);

        /// <summary> Creates a log entry of the "Debug" level for the current <see cref="_logCategory"/>. </summary>
        /// <param name="category"> The category of the event being logged. </param>
        /// <param name="message"> A message to supplement the log with. </param>
        protected void LogDebug(string message) =>
            _dataRepository.Logger.LogDebug(_logCategory, message);

        /// <summary> Creates a log entry of the "Info" level for the current <see cref="_logCategory"/>. </summary>
        /// <param name="category"> The category of the event being logged. </param>
        /// <param name="message"> A message to supplement the log with. </param>
        protected void LogInfo(string message) =>
            _dataRepository.Logger.LogInfo(_logCategory, message);

        /// <summary> Creates a log entry of the "Warn" level for the current <see cref="_logCategory"/>. </summary>
        /// <param name="category"> The category of the event being logged. </param>
        /// <param name="message"> A message to supplement the log with. </param>
        protected void LogWarn(string message) =>
            _dataRepository.Logger.LogWarn(_logCategory, message);

        /// <summary> Creates a log entry of the "Error" level for the current <see cref="_logCategory"/>. </summary>
        /// <param name="category"> The category of the event being logged. </param>
        /// <param name="message"> A message to supplement the log with. </param>
        /// <param name="exception"> An exception whose data to log. </param>
        protected void LogError(string message, Exception exception) =>
            _dataRepository.Logger.LogError(_logCategory, message, exception);

        /// <summary> Creates a log entry of the "Fatal" level for the current <see cref="_logCategory"/>. </summary>
        /// <param name="category"> The category of the event being logged. </param>
        /// <param name="message"> A message to supplement the log with. </param>
        /// <param name="exception"> An exception whose data to log. </param>
        protected void LogFatal(string message, Exception exception) =>
            _dataRepository.Logger.LogFatal(_logCategory, message, exception);

        #endregion Methods: Logging

        /// <summary> Returns a string that represents the instance. </summary>
        /// <returns></returns>
        public override string ToString() => $"{this.GetTypeString()}";

        /// <summary> Commit changes to the current persistent object (persist if the object is transient) using the <see cref="IDataRepository"/> provided with the object's constructor. </summary>
        public virtual void CommitChanges()
        {
            LogDebug(EDataBaseLogMessage.PreparingToCommitChangesTo.FormatFluently(ToString()));

            if (_dataRepository.IsClosed)
            {
                LogDebug(EDataBaseLogMessage.DataRepositoryClosed_CommittingAborted);
                return;
            }

            _dataRepository.CommitChanges(this);

            if (_dataRepository.NewObjects.Contains(this))
                _dataRepository.NewObjects.Remove(this);
        }
    }
}
