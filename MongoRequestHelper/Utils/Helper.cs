using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MongoRequestHelper.Utils
{
    /// <summary>
    /// Класс вспомогательных инструментов
    /// </summary>
    internal static class Helper
    {
        /// <summary>
        /// Получить все сообщения исключения в т.ч. и внутренних
        /// </summary>
        /// <param name="exception">Исключение</param>
        /// <returns>Список сообщений</returns>
        public static IEnumerable<string> GetAllErrorMessage(Exception? exception)
        {
            var messages = new List<string>();

            if (exception == null) return messages;

            if (!string.IsNullOrEmpty(exception.Message)) messages.Add(exception.Message);

            var innerException = exception.InnerException;

            while (true)
            {
                if (innerException == null) return messages;

                if (!string.IsNullOrEmpty(innerException.Message)) messages.Add(innerException.Message);

                innerException = innerException.InnerException;
            }
        }

        /// <summary>
        /// Получить имя коллекции по типу данных документов
        /// </summary>
        /// <param name="documentType">Тип данных документов</param>
        /// <returns>Название коллекции</returns>
        public static string GetCollectionName(Type documentType)
        {
            return Pluralize(SplitCamelCase(documentType.Name).Last());
        }

        /// <summary>
        /// Возвращает множественное число слова
        /// </summary>
        /// <param name="word">Слово в единственном числе на английском</param>
        /// <returns>Множественное число слова</returns>
        public static string Pluralize(string word)
        {
            Dictionary<string, string> exceptions = new Dictionary<string, string>() {
                { "man", "men" },
                { "woman", "women" },
                { "child", "children" },
                { "tooth", "teeth" },
                { "foot", "feet" },
                { "mouse", "mice" },
                { "belief", "beliefs" } };

            if (exceptions.ContainsKey(word.ToLowerInvariant()))
            {
                return exceptions[word.ToLowerInvariant()];
            }
            else if (word.EndsWith("y", StringComparison.OrdinalIgnoreCase) &&
                     !word.EndsWith("ay", StringComparison.OrdinalIgnoreCase) &&
                     !word.EndsWith("ey", StringComparison.OrdinalIgnoreCase) &&
                     !word.EndsWith("iy", StringComparison.OrdinalIgnoreCase) &&
                     !word.EndsWith("oy", StringComparison.OrdinalIgnoreCase) &&
                     !word.EndsWith("uy", StringComparison.OrdinalIgnoreCase))
            {
                return word.Substring(0, word.Length - 1) + "ies";
            }
            else if (word.EndsWith("us", StringComparison.InvariantCultureIgnoreCase))
            {
                // http://en.wikipedia.org/wiki/Plural_form_of_words_ending_in_-us
                return word + "es";
            }
            else if (word.EndsWith("ss", StringComparison.InvariantCultureIgnoreCase))
            {
                return word + "es";
            }
            else if (word.EndsWith("s", StringComparison.InvariantCultureIgnoreCase))
            {
                return word;
            }
            else if (word.EndsWith("x", StringComparison.InvariantCultureIgnoreCase) ||
                     word.EndsWith("ch", StringComparison.InvariantCultureIgnoreCase) ||
                     word.EndsWith("sh", StringComparison.InvariantCultureIgnoreCase))
            {
                return word + "es";
            }
            else if (word.EndsWith("f", StringComparison.InvariantCultureIgnoreCase) && word.Length > 1)
            {
                return word.Substring(0, word.Length - 1) + "ves";
            }
            else if (word.EndsWith("fe", StringComparison.InvariantCultureIgnoreCase) && word.Length > 2)
            {
                return word.Substring(0, word.Length - 2) + "ves";
            }
            else
            {
                return word + "s";
            }
        }

        /// <summary>
        /// Возвращает список подстрок строки в нотации CamelCase
        /// </summary>
        /// <param name="source">Строка в нотации CamelCase</param>
        /// <returns>Список подстрок</returns>
        public static IEnumerable<string> SplitCamelCase(this string source)
        {
            const string pattern = @"[A-Z][a-z]*|[a-z]+|\d+";
            var matches = Regex.Matches(source, pattern);
            foreach (Match match in matches)
            {
                yield return match.Value;
            }
        }
    }
}