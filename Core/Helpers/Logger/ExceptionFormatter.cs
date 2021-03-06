﻿using Core.Enumerations;
using Core.Enumerations.Logger;
using Core.Extensions;
using Core.Helpers.Logger.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Helpers.Logger
{
    /// <summary> Handles formatting of exception information into a form adequate for logging. </summary>
    public class ExceptionFormatter : IExceptionFormatter
    {
        #region Constants

        /// <summary> A format string supposed to contain an extension type and a message. </summary>
        private const string _formattedExceptionMessage = "\t[{0}]\n\t\t{1}";

        #endregion Constants
        #region Methods

        /// <summary> Re-formats an exception into a more readable form. </summary>
        /// <param name="exception"> An exception to re-format.</param>
        /// <returns></returns>
        public string GetFormattedException(Exception exception)
        {
            if (exception == null)
                return ECoreLogMessage.ExceptionIsNull;

            var lines = new List<string>()
            {
                $"{ECharacter.NewLine}{_formattedExceptionMessage.FormatFluently(exception.GetType(), exception.Message.Flatten())}",
                GetFormattedStackTrace(exception),
                GetFormattedInnerExceptions(exception),
            };

            return lines
                .Where(line => line.Any())
                .StringJoin(ECharacter.NewLine)
            ;
        }

        /// <summary> Formats inner exceptions of a specified exception into a more readable form. </summary>
        /// <param name="exception"> An exception whose inner exceptions to format. </param>
        /// <returns></returns>
        private string GetFormattedInnerExceptions(Exception exception)
        {
            var lines = new List<string>();
            var innerException = exception.InnerException;

            while (innerException != null)
            {
                lines.Add(_formattedExceptionMessage.FormatFluently(innerException.GetType(), innerException.Message.Flatten()));
                lines.Add(GetFormattedStackTrace(innerException));

                innerException = innerException.InnerException;
            }

            return lines
                .Where(line => line.Any())
                .StringJoin(ECharacter.NewLine)
            ;
        }

        /// <summary> Formats the given exception's stack trace into a fitting form. </summary>
        /// <param name="exception"> An exception whose stack trace to format. </param>
        /// <returns></returns>
        private string GetFormattedStackTrace(Exception exception)
        {
            if (exception.StackTrace is null) return string.Empty;

            var lines = new List<string>();
            var stackFrames = exception.StackTrace.Split(ECharacter.NewLine, StringSplitOptions.RemoveEmptyEntries);

            foreach (var stackFrame in stackFrames)
                lines.Add(stackFrame.Replace("   ", $"\t\t\t"));

            return lines.StringJoin(ECharacter.NewLine);
        }

        #endregion Methods
    }
}