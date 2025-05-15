using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.CustomEntities
{
    public class Language
    {
        public string Name { get; set; }
        public string Code { get; set; }

        // CONSTRUCTOR

        public Language(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }

    public class LanguagesList
    {
        public static IList<Language> GetLanguages()
        {
            return new List<Language>
            {
                new Language("Español", "es"),
                new Language("English", "en"),
                new Language("Français", "fr"),
                new Language("Português", "pt"),
                new Language("Italiano", "it"),
                new Language("Deutsch", "de"),
                new Language("Nederlands", "nl"),
                new Language("Русский", "ru"),
                new Language("中文", "zh"),
                new Language("日本語", "ja"),
                new Language("한국어", "ko")
            };
        }
    }
}