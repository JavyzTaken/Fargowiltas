using Terraria.Localization;

namespace Fargowiltas.Localization
{
    /// <summary>
    ///     An object capable of holding a key and objects.
    /// </summary>
    public abstract record LangEntry(string Key, params object[] Objects)
    {
        public LocalizedText GetText() => Language.GetText(Key);

        public string GetTextValue() => Language.GetTextValue(Key, Objects);

        public override string ToString() => GetTextValue();

        public static implicit operator string(LangEntry entry) => entry.GetTextValue();

        public static implicit operator LocalizedText(LangEntry entry) => entry.GetText();
    }

    public record VanillaLangEntry(string Key, params object[] Objects) : LangEntry(Key, Objects);

    public record MutantLangEntry(string Key, params object[] Objects) : LangEntry("Mods.Fargowiltas." + Key, Objects)
    {
        public static MutantLangEntry NPCs(string key, params object[] objects) => new("NPCs." + key, objects);
    }

    public record SoulsLangEntry(string Key, params object[] Objects) : LangEntry("Mods.FargowiltasSouls." + Key, Objects);
}