namespace Processos_Juridicos.Utilities.Notifications
{
    /// <summary>
    /// Provides functions to generate various standardized strings used throughout the project
    /// </summary>
    public static class TextTemplates
    {

        /// <param name="action">The action that failed (e.g., "criação").</param>
        /// <param name="entityType">The type of entity the action was attempted on (e.g., "unidade").</param>
        /// <param name="id">An optional identifier a specific entity instance.</param>
        /// <param name="suffix">The suffix to be used in the sentence ("o" or "a").</param>
        /// <returns>A formatted error message describing an action's failure.</returns>
        /// Example of generated string: "Ocorreu um erro ao remover a unidade com o ID 3"
        public static string ActionFailureMessage(string action, string suffix, string entityType, int? id)
        {
            string idPart = id != null ? $" com o ID {id}" : string.Empty;
            return $"Ocorreu um erro ao {action} {suffix} {entityType}{idPart}.";
        }

        /// <param name="action">The action that failed (e.g., "criação").</param>
        /// <param name="entityType">The type of entity the action was attempted on (e.g., "unidade").</param>
        /// <param name="id">An optional identifier a specific entity instance.</param>
        /// <param name="prefix">The prefix to be used in the sentence ("o" or "a").</param>
        /// <returns>A formatted success message describing the failure.</returns>
        /// Example of generated string: "O processo com o ID 3 foi removido com sucesso."
        public static string ActionSuccessMessage(string action, string prefix, string entityType, int? id)
        {
            string idPart = id != null ? $" com o ID {id}" : string.Empty;
            return $"{prefix} {entityType}{idPart} foi {action} com sucesso.";
        }
    }
}
