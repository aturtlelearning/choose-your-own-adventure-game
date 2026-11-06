using System.Text;
using System.Text.Json;

[TestClass]
[DoNotParallelize]
public class ItalianLanguageTests
{
    private Messages ItalianMessages()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Italian");
        messages.ReadDictionary();
        return messages;
    }

    [TestMethod]
    public void Italian_LoadsWelcomeAndGoodbye()
    {
        Messages messages = ItalianMessages();
        Assert.AreEqual("Italian", messages.CurrentLanguage);
        Assert.AreEqual("Benvenuto nel gioco Scegli la tua avventura", messages.GetMessage("welcome"));
        Assert.AreEqual("L'avventura finisce. Arrivederci.", messages.GetMessage("goodbye"));
        Assert.AreEqual("[unknown_key]", messages.GetMessage("unknown_key"));
    }

    [TestMethod]
    public void Italian_HasAllEnglishKeysAndMatchingFormatArguments()
    {
        using JsonDocument document = JsonDocument.Parse(
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "language_data.json")));
        JsonElement english = document.RootElement.GetProperty("English");
        JsonElement italian = document.RootElement.GetProperty("Italian");
        Messages messages = ItalianMessages();

        foreach (JsonProperty section in english.EnumerateObject())
        {
            JsonElement translatedSection = italian.GetProperty(section.Name);
            foreach (JsonProperty entry in section.Value.EnumerateObject())
            {
                // Input maps use localized keys; their canonical values must match instead.
                if (section.Name == "raceMap" || section.Name == "occupationMap")
                {
                    Assert.IsTrue(translatedSection.EnumerateObject()
                        .Any(candidate => candidate.Value.GetString() == entry.Value.GetString()));
                    continue;
                }

                string? translated = translatedSection.GetProperty(entry.Name).GetString();
                Assert.IsFalse(string.IsNullOrWhiteSpace(translated), $"Missing {section.Name}.{entry.Name}");
                if (section.Name == "dictionary")
                {
                    Assert.AreEqual(translated, messages.GetMessage(entry.Name));
                    Assert.AreEqual(
                        CompositeFormat.Parse(entry.Value.GetString()!).MinimumArgumentCount,
                        CompositeFormat.Parse(translated!).MinimumArgumentCount,
                        $"Format arguments differ for {entry.Name}");
                }
            }
        }
    }

    [DataTestMethod]
    [DataRow("Umano", "Human")]
    [DataRow("Elfo", "Elf")]
    [DataRow("Nano", "Dwarf")]
    [DataRow("Halfling", "Halfling")]
    public void Italian_RacesValidateNormalizeAndDisplay(string italian, string canonical)
    {
        Messages messages = ItalianMessages();
        string input = " " + italian.ToUpperInvariant() + " ";
        Assert.IsTrue(messages.IsValidRace(input));
        Assert.AreEqual(canonical, messages.NormalizeRace(input));
        Assert.AreEqual(italian, messages.TranslateRaceForDisplay(canonical));
        Assert.IsFalse(messages.IsValidRace("orco"));
        Assert.IsFalse(messages.IsValidRace(null));
    }

    [DataTestMethod]
    [DataRow("Guerriero", "Fighter")]
    [DataRow("Mago", "Magician")]
    [DataRow("Ladro", "Thief")]
    [DataRow("Arciere", "Archer")]
    public void Italian_OccupationsValidateNormalizeAndDisplay(string italian, string canonical)
    {
        Messages messages = ItalianMessages();
        string input = " " + italian.ToUpperInvariant() + " ";
        Assert.IsTrue(messages.IsValidOccupation(input));
        Assert.AreEqual(canonical, messages.NormalizeOccupation(input));
        Assert.AreEqual(italian, messages.TranslateOccupationForDisplay(canonical));
        Assert.IsFalse(messages.IsValidOccupation("pirata"));
        Assert.IsFalse(messages.IsValidOccupation(null));
    }

    [DataTestMethod]
    [DataRow("long sword", "spada lunga")]
    [DataRow("lightning bolt spell", "incantesimo del fulmine")]
    [DataRow("dagger", "pugnale")]
    [DataRow("long bow", "arco lungo")]
    [DataRow("claws", "artigli")]
    public void Italian_TranslatesWeapons(string canonical, string italian)
    {
        Assert.AreEqual(italian, ItalianMessages().TranslateWeaponForDisplay(canonical));
    }

    [DataTestMethod]
    [DataRow("English", "Welcome to Choose Your Adventure Game", "Human", "Fighter", "long sword")]
    [DataRow("Spanish", "Bienvenido al Juego de Elige Tu Aventura", "Humano", "Guerrero", "espada larga")]
    [DataRow("French", "Bienvenue dans Choisissez Votre Aventure", "Humain", "Guerrier", "epee longue")]
    public void SwitchingFromItalian_ReloadsExistingLanguages(
        string language, string welcome, string race, string occupation, string weapon)
    {
        Messages messages = ItalianMessages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        Assert.AreEqual(welcome, messages.GetMessage("welcome"));
        Assert.AreEqual(race, messages.TranslateRaceForDisplay("Human"));
        Assert.AreEqual(occupation, messages.TranslateOccupationForDisplay("Fighter"));
        Assert.AreEqual(weapon, messages.TranslateWeaponForDisplay("long sword"));
    }

    [DataTestMethod]
    [DataRow("1", "English")]
    [DataRow("2", "Spanish")]
    [DataRow("3", "French")]
    [DataRow(" 4 ", "Italian")]
    [DataRow("invalid", "English")]
    public void StartGame_SelectsLanguageAndExits(string choice, string language)
    {
        Messages expected = new Messages();
        expected.SetCurrentLanguage(language);
        expected.ReadDictionary();
        string output = RunGame(choice + "\n2\n");
        StringAssert.Contains(output, "4. Italiano");
        StringAssert.Contains(output, expected.GetMessage("welcome"));
        StringAssert.Contains(output, expected.GetMessage("menu"));
        StringAssert.Contains(output, expected.GetMessage("goodbye"));
    }

    [TestMethod]
    public void StartGame_ItalianCharacterDisplaysTranslatedStatsAndTakesSouthPath()
    {
        string output = RunGame("4\n1\nUmano\nEroe\nGuerriero\nroll\nroll\nroll\n1\n2\ns\ne\n");
        StringAssert.Contains(output, "Razza: Umano");
        StringAssert.Contains(output, "Professione: Guerriero");
        StringAssert.Contains(output, "Arma: spada lunga");
        StringAssert.Contains(output, ItalianMessages().GetMessage("south_path_narrative"));
        StringAssert.Contains(output, ItalianMessages().GetMessage("adventure_ends"));
    }

    private string RunGame(string input)
    {
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        using StringReader reader = new StringReader(input);
        using StringWriter writer = new StringWriter();
        try
        {
            Console.SetIn(reader);
            Console.SetOut(writer);
            new Game().StartGame();
            return writer.ToString();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }
}
