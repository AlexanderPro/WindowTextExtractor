using System;
using System.Collections.Generic;

namespace WindowTextExtractor.Extensions
{
    public static class StringExtensions
    {
        public static string TrimEnd(this string text, string value, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            var result = text;
            while (result.EndsWith(value, comparisonType))
            {
                result = result.Substring(0, result.Length - value.Length);
            }
            return result;
        }

        public static string TryFixEveryWordLetterNumberErrors(this string text)
        {
            var listOfWords = text.Split(' ');
            var fixedWords = new List<string>();

            foreach (var word in listOfWords)
            {
                var newWord = word.TryFixNumberLetterErrors();
                fixedWords.Add(newWord);
            }

            return string.Join(" ", fixedWords.ToArray())
                .Replace("\t ", "\t")
                .Replace("\r ", "\r")
                .Replace("\n ", "\n")
                .Trim();
        }

        public static string TryFixNumberLetterErrors(this string text)
        {
            if (text.Length < 5)
            {
                return text;
            }

            var totalNumbers = 0;
            var totalLetters = 0;

            foreach (char charFromString in text)
            {
                if (char.IsNumber(charFromString))
                {
                    totalNumbers++;
                }

                if (char.IsLetter(charFromString))
                {
                    totalLetters++;
                }
            }

            var fractionNumber = totalNumbers / (float)text.Length;
            var letterNumber = totalLetters / (float)text.Length;

            if (fractionNumber > 0.6)
            {
                text = text.TryFixToNumbers();
            }
            else if (letterNumber > 0.6)
            {
                text = text.TryFixToLetters();
            }

            return text;
        }

        public static string TryFixToNumbers(this string text) => text
            .Replace('o', '0')
            .Replace('O', '0')
            .Replace('Q', '0')
            .Replace('c', '0')
            .Replace('C', '0')
            .Replace('i', '1')
            .Replace('I', '1')
            .Replace('l', '1')
            .Replace('g', '9');

        public static string TryFixToLetters(this string text) => text
            .Replace('0', 'o')
            .Replace('4', 'h')
            .Replace('9', 'g')
            .Replace('1', 'l');
    }
}
