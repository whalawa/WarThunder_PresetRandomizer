﻿using Core.Organization.Enumerations.Logger;

namespace Client.Console.Enumerations.Logger
{
    /// <summary> Log message strings related to the "<see cref="Console"/>" assembly. </summary>
    public class EConsoleUiLogMessage : EOrganizationLogMessage
    {
        private static readonly string _gameMode = $"{_game} {_mode}";
        private static readonly string _separatedBySpaces = $"{_separated} {_by} {_spaces}";

        /// <summary> 
        /// A message with formatting placeholders.
        /// <para> 1: application whose location to select. </para>
        /// </summary>
        public static readonly string SelectValidLocation = $"{_Select} {_valid} " + _FMT + $"{_location}: ";

        public static readonly string InputSpecification = $"{_Specification} ({_gameMode}, {_nation}, {_branch}, {_BR} - {_separatedBySpaces}) > ";

        public static readonly string IncorrectInput = $"{_Incorrect} {_input} ({_need} {_4} {_arguments} {_separatedBySpaces}).";
        public static readonly string IncorrectGameMode = $"{_Incorrect} {_gameMode}.";
        public static readonly string IncorrectNation = $"{_Incorrect} {_nation}.";
        public static readonly string IncorrectBranch = $"{_Incorrect} {_branch}.";
        public static readonly string IncorrectBattleRating = $"{_Incorrect} {_battle} {_rating} ({_must} {_be} {_a} {_number}).";

        public static readonly string PressAnyKeyToExit = $"{_Press} {_any} {_key} {_to} {_exit}.";
    }
}