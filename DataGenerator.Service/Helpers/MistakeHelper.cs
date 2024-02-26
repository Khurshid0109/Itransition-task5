using Bogus;
using System.Text;
using DataGenerator.Domain.Entities;

namespace DataGenerator.Service.Helpers;

public static class MistakeHelper
{
    private const string Latin = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private const string Cyrillic = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";
    private const string Numbers = "0123456789";

    private static string _regionChars = String.Empty;

    private delegate string Mistake(string text, Randomizer randomizer, string symbols);

    private static readonly Mistake[] Mistakes =
    {
        ChangeSymbol,
        AddSymbol,
        RemoveSymbol
    };

    public static User MakeMistakes(this User data, double mistakeRate, Randomizer randomizer, string locale)
    {
        SetUpLocale(locale);

        for (int i = 0; i < (int)mistakeRate; i++)
        {
            MakeMistake(data, randomizer);
        }

        if (randomizer.Double() < mistakeRate - (int)mistakeRate)
        {
            MakeMistake(data, randomizer);
        }

        return data;
    }

    private static string ChangeSymbol(string text, Randomizer randomizer, string symbols)
    {
        var builder = new StringBuilder(text);

        if (builder.Length > 0)
        {
            builder[randomizer.Number(0, text.Length - 1)] = randomizer.ArrayElement(symbols.ToArray());
        }

        return builder.ToString();
    }

    private static string AddSymbol(string text, Randomizer randomizer, string symbols)
    {
        var builder = new StringBuilder(text);

        return builder.Insert(randomizer.Number(0, text.Length - 1), randomizer.ArrayElement(symbols.ToArray())).ToString();
    }

    private static string RemoveSymbol(string text, Randomizer randomizer, string symbols)
    {
        var builder = new StringBuilder(text);

        if (text.Length > 0)
        {
            builder.Remove(randomizer.Number(0, text.Length - 1), 1);
            return builder.ToString();
        }

        return builder.ToString();
    }

    private static void MakeMistake(this User data, Randomizer randomizer)
    {
        var properties = data.GetType().GetProperties().Where(x => x.PropertyType == typeof(string) && x.CanWrite).ToArray();
        var currentProperty = randomizer.ArrayElement(properties);
        var symbols = _regionChars;
        if (currentProperty.Name is "PhoneNumber" or "Id")
        {
            symbols = Numbers;
        }
        var value = currentProperty.GetValue(data)?.ToString();
        currentProperty.SetValue(data, randomizer.ArrayElement(Mistakes)(value, randomizer, symbols));
    }

    private static void SetUpLocale(string locale)
    {
        switch (locale)
        {
            case "ru":
                _regionChars = Cyrillic;
                break;
            case "en_US":
                _regionChars = Latin;
                break;
            case "de":
                _regionChars = Latin;
                break;
            case "fr":
                _regionChars = Latin;
                break;
        }
    }
}