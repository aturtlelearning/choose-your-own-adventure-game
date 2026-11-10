using System.Text;

[TestClass]
[DoNotParallelize]
public class WatchtowerTests
{
    private const string Arrival = "A ruined watchtower rises beside the western trail. Ivy grips its cracked stone walls, and wind whistles through the broken roof. Inside, worn stairs climb above a ground floor strewn with fallen timber.";
    private const string StairsOutcome = "You carefully climb the worn stairs. Deep claw marks scar the stone near the top. Through the broken roof, you glimpse the dragon’s mountain against the sky. You return to the trail, wondering what once landed here.";
    private const string SearchOutcome = "Beneath a fallen beam, you discover an old guard’s journal. Its final entry reads: ‘The wingbeats returned after sunset. We leave before dawn.’ The remaining pages are blank. You replace the journal and return to the trail.";

    private TextReader _originalIn = null!;
    private TextWriter _originalOut = null!;
    private StringWriter _output = null!;

    [TestInitialize]
    public void Setup()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;
        _output = new StringWriter();
        Console.SetOut(_output);
    }

    [TestCleanup]
    public void Cleanup()
    {
        Console.SetIn(_originalIn);
        Console.SetOut(_originalOut);
        _output.Dispose();
    }

    [DataTestMethod]
    [DataRow("w")]
    [DataRow("west")]
    [DataRow("W")]
    [DataRow("  WeSt  ")]
    public void WestTravel_ClimbStairsDisplaysApprovedStoryThenReturnsToTravel(string route)
    {
        var messages = LoadMessages("English");
        string output = RunGame(messages, "1", $"{route}\n1\ne\n");

        StringAssert.Contains(output, Arrival);
        StringAssert.Contains(output, "1. Climb the stairs");
        StringAssert.Contains(output, "2. Search the ground floor");
        StringAssert.Contains(output, StairsOutcome);
        Assert.IsFalse(output.Contains("guard’s journal"));
        AssertReturnsToTravel(output, messages, StairsOutcome);
    }

    [TestMethod]
    public void WestTravel_SearchGroundFloorDisplaysApprovedStoryThenReturnsToTravel()
    {
        var messages = LoadMessages("English");
        string output = RunGame(messages, "1", "west\n2\ne\n");

        StringAssert.Contains(output, Arrival);
        StringAssert.Contains(output, SearchOutcome);
        Assert.IsFalse(output.Contains(StairsOutcome));
        AssertReturnsToTravel(output, messages, SearchOutcome);
    }

    [DataTestMethod]
    [DataRow("English", "ruined watchtower")]
    [DataRow("Spanish", "torre de vigilancia en ruinas")]
    [DataRow("French", "tour de guet en ruines")]
    [DataRow("Italian", "torre di guardia in rovina")]
    public void TravelGuidance_ExplainsWestInSelectedLanguage(string language, string location)
    {
        string prompt = LoadMessages(language).GetMessage("path_prompt_full");
        StringAssert.Contains(prompt, "'w'");
        StringAssert.Contains(prompt, "'west'");
        StringAssert.Contains(prompt, location);
    }

    [DataTestMethod]
    [DataRow("Spanish", "2", "1", "Una torre de vigilancia en ruinas", "1. Subir las escaleras", "2. Registrar la planta baja", "Profundas marcas de garras", "el diario de un viejo guardia")]
    [DataRow("Spanish", "2", "2", "Una torre de vigilancia en ruinas", "1. Subir las escaleras", "2. Registrar la planta baja", "el diario de un viejo guardia", "Profundas marcas de garras")]
    [DataRow("French", "3", "1", "Une tour de guet en ruines", "1. Monter l’escalier", "2. Fouiller le rez-de-chaussée", "De profondes marques de griffes", "le journal d’un vieux garde")]
    [DataRow("French", "3", "2", "Une tour de guet en ruines", "1. Monter l’escalier", "2. Fouiller le rez-de-chaussée", "le journal d’un vieux garde", "De profondes marques de griffes")]
    [DataRow("Italian", "4", "1", "Una torre di guardia in rovina", "1. Salire le scale", "2. Esplorare il piano terra", "Profonde tracce di artigli", "il diario di una vecchia guardia")]
    [DataRow("Italian", "4", "2", "Una torre di guardia in rovina", "1. Salire le scale", "2. Esplorare il piano terra", "il diario di una vecchia guardia", "Profonde tracce di artigli")]
    public void WestTravel_LocalizesBothStoriesAndReturnsToTravel(
        string language, string selection, string choice, string arrival,
        string stairsOption, string searchOption, string discovery, string otherDiscovery)
    {
        var messages = LoadMessages(language);
        string output = RunGame(messages, selection, $"  WeSt  \n{choice}\ne\n");

        StringAssert.Contains(output, arrival);
        StringAssert.Contains(output, stairsOption);
        StringAssert.Contains(output, searchOption);
        StringAssert.Contains(output, discovery);
        Assert.IsFalse(output.Contains(otherDiscovery));
        Assert.IsFalse(output.Contains("[watchtower_"));
        AssertReturnsToTravel(output, messages, discovery);
    }

    [DataTestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void WatchtowerMessages_HaveCompleteKeysAndCompatibleFormatting(string language)
    {
        var english = LoadMessages("English");
        var messages = LoadMessages(language);
        string[] keys = ["watchtower_arrival", "watchtower_stairs_option", "watchtower_search_option",
            "watchtower_stairs_outcome", "watchtower_search_outcome", "watchtower_invalid"];

        foreach (string key in keys)
        {
            string text = messages.GetMessage(key);
            Assert.AreNotEqual($"[{key}]", text, key);
            Assert.IsFalse(string.IsNullOrWhiteSpace(text), key);
            Assert.AreEqual(CompositeFormat.Parse(english.GetMessage(key)).MinimumArgumentCount,
                CompositeFormat.Parse(text).MinimumArgumentCount, key);
            if (language != "English")
                Assert.AreNotEqual(english.GetMessage(key), text, $"{key} must be translated.");
        }
    }

    [DataTestMethod]
    [DataRow("wrong\n", "1", 1)]
    [DataRow("\n", "2", 1)]
    [DataRow("   \n", "1", 1)]
    [DataRow("wrong\n\n0\n", "2", 3)]
    public void Watchtower_InvalidChoicesRepeatMenuUntilValidSelection(
        string invalidInput, string choice, int errorCount)
    {
        var messages = LoadMessages("English");
        string output = RunGame(messages, "1", $"w\n{invalidInput}{choice}\ne\n");
        const string error = "Invalid choice. Enter '1' to climb the stairs or '2' to search the ground floor.";

        Assert.AreEqual(errorCount, CountOccurrences(output, error));
        Assert.AreEqual(errorCount + 1, CountOccurrences(output, "1. Climb the stairs"));
        Assert.AreEqual(errorCount + 1, CountOccurrences(output, "2. Search the ground floor"));
        Assert.AreEqual(1, CountOccurrences(output, Arrival));
        string outcome = choice == "1" ? StairsOutcome : SearchOutcome;
        Assert.AreEqual(1, CountOccurrences(output, outcome));
        Assert.IsFalse(output.Contains(choice == "1" ? SearchOutcome : StairsOutcome));
        Assert.IsFalse(output.Contains(messages.GetMessage("path_invalid")));
        Assert.IsTrue(output.IndexOf(outcome, StringComparison.Ordinal)
            > output.LastIndexOf(error, StringComparison.Ordinal));
        AssertReturnsToTravel(output, messages, outcome);
    }

    [DataTestMethod]
    [DataRow("English", "1", "Invalid choice. Enter '1' to climb the stairs or '2' to search the ground floor.")]
    [DataRow("Spanish", "2", "Opción no válida. Escribe '1' para subir las escaleras o '2' para registrar la planta baja.")]
    [DataRow("French", "3", "Choix invalide. Tapez '1' pour monter l’escalier ou '2' pour fouiller le rez-de-chaussée.")]
    [DataRow("Italian", "4", "Scelta non valida. Digita '1' per salire le scale o '2' per esplorare il piano terra.")]
    public void Watchtower_RetriesUseSelectedLanguage(string language, string selection, string error)
    {
        var messages = LoadMessages(language);
        Assert.AreEqual(error, messages.GetMessage("watchtower_invalid"));
        string output = RunGame(messages, selection, "west\nwrong\n\n 2 \ne\n");

        Assert.AreEqual(2, CountOccurrences(output, error));
        Assert.AreEqual(3, CountOccurrences(output, messages.GetMessage("watchtower_stairs_option")));
        Assert.AreEqual(3, CountOccurrences(output, messages.GetMessage("watchtower_search_option")));
        Assert.AreEqual(1, CountOccurrences(output, messages.GetMessage("watchtower_search_outcome")));
        Assert.IsFalse(output.Contains(messages.GetMessage("watchtower_stairs_outcome")));
        Assert.IsFalse(output.Contains("[watchtower_"));
        AssertReturnsToTravel(output, messages, messages.GetMessage("watchtower_search_outcome"));
    }

    [DataTestMethod]
    [DataRow("English", "1", "", 0)]
    [DataRow("English", "1", "wrong\n", 1)]
    [DataRow("English", "1", "wrong\n\n", 2)]
    [DataRow("Spanish", "2", "", 0)]
    [DataRow("Spanish", "2", "wrong\n", 1)]
    [DataRow("Spanish", "2", "wrong\n\n", 2)]
    [DataRow("French", "3", "", 0)]
    [DataRow("French", "3", "wrong\n", 1)]
    [DataRow("French", "3", "wrong\n\n", 2)]
    [DataRow("Italian", "4", "", 0)]
    [DataRow("Italian", "4", "wrong\n", 1)]
    [DataRow("Italian", "4", "wrong\n\n", 2)]
    public void Watchtower_EofExitsWithoutOutcomeOrReturningToTravel(
        string language, string selection, string invalidInput, int errorCount)
    {
        var messages = LoadMessages(language);
        using var input = new ScriptedReader(CreateGameInput(messages, selection, "w\n" + invalidInput), allowedEofReads: 1);
        Console.SetIn(input);

        new Game().StartGame();

        string output = _output.ToString();
        Assert.AreEqual(errorCount, CountOccurrences(output, messages.GetMessage("watchtower_invalid")));
        Assert.AreEqual(errorCount + 1, CountOccurrences(output, messages.GetMessage("watchtower_stairs_option")));
        Assert.AreEqual(errorCount + 1, CountOccurrences(output, messages.GetMessage("watchtower_search_option")));
        Assert.AreEqual(1, CountOccurrences(output, messages.GetMessage("watchtower_arrival")));
        Assert.AreEqual(1, CountOccurrences(output, messages.GetMessage("path_prompt_full")));
        Assert.AreEqual(1, CountOccurrences(output, messages.GetMessage("adventure_ends")));
        Assert.IsFalse(output.Contains(messages.GetMessage("watchtower_stairs_outcome")));
        Assert.IsFalse(output.Contains(messages.GetMessage("watchtower_search_outcome")));
    }

    [DataTestMethod]
    [DataRow("English", "1")]
    [DataRow("Spanish", "2")]
    [DataRow("French", "3")]
    [DataRow("Italian", "4")]
    public void Watchtower_RepeatVisitsKeepBothChoicesAndIdenticalStories(string language, string selection)
    {
        var messages = LoadMessages(language);
        string output = RunGame(messages, selection, "w\nwrong\n1\nwest\n1\nw\n2\nw\n2\nwest\n1\ne\n");
        string[] visits = output.Split(messages.GetMessage("watchtower_arrival"));
        string[] expectedOutcomes = ["watchtower_stairs_outcome", "watchtower_stairs_outcome",
            "watchtower_search_outcome", "watchtower_search_outcome", "watchtower_stairs_outcome"];

        Assert.AreEqual(6, visits.Length, "All five visits must display the same arrival text.");
        Assert.AreEqual(1, CountOccurrences(output, messages.GetMessage("watchtower_invalid")));
        Assert.AreEqual(6, CountOccurrences(output, messages.GetMessage("path_prompt_full")));
        for (int visitIndex = 0; visitIndex < expectedOutcomes.Length; visitIndex++)
        {
            string visit = visits[visitIndex + 1];
            StringAssert.Contains(visit, messages.GetMessage("watchtower_stairs_option"));
            StringAssert.Contains(visit, messages.GetMessage("watchtower_search_option"));
            string outcome = messages.GetMessage(expectedOutcomes[visitIndex]);
            Assert.AreEqual(1, CountOccurrences(visit, outcome));
            string otherOutcome = expectedOutcomes[visitIndex] == "watchtower_stairs_outcome"
                ? "watchtower_search_outcome" : "watchtower_stairs_outcome";
            Assert.IsFalse(visit.Contains(messages.GetMessage(otherOutcome)));
            Assert.IsTrue(visit.IndexOf(messages.GetMessage("path_prompt_full"), StringComparison.Ordinal)
                > visit.IndexOf(outcome, StringComparison.Ordinal));
        }
    }

    [DataTestMethod]
    [DataRow("English", "1", "1")]
    [DataRow("English", "1", "2")]
    [DataRow("English", "1", "3")]
    [DataRow("Spanish", "2", "1")]
    [DataRow("Spanish", "2", "2")]
    [DataRow("Spanish", "2", "3")]
    [DataRow("French", "3", "1")]
    [DataRow("French", "3", "2")]
    [DataRow("French", "3", "3")]
    [DataRow("Italian", "4", "1")]
    [DataRow("Italian", "4", "2")]
    [DataRow("Italian", "4", "3")]
    public void Watchtower_RetriesAndVisitsPreserveCharacterAndMerchantUpgradesBeforeCombat(
        string language, string selection, string merchantChoice)
    {
        var messages = LoadMessages(language);
        string secondMerchantChoice = merchantChoice == "3" ? "3\n" : "";
        string adventure = $"s\n{merchantChoice}\nw\nwrong\n\n1\nwest\n2\ns\n{secondMerchantChoice}n\na\nr\n";
        using var input = new ScriptedReader(CreateGameInput(messages, selection, adventure, "1\n2\n"));
        Console.SetIn(input);

        new Game().StartGame();

        string output = _output.ToString();
        int characterStart = output.IndexOf(messages.GetMessage("stats_header"), StringComparison.Ordinal);
        int adventureStart = output.IndexOf(messages.GetMessage("path_prompt_full"), characterStart, StringComparison.Ordinal);
        string[] originalStats = output[characterStart..adventureStart].Split(Environment.NewLine);
        int playerStart = output.IndexOf(messages.GetMessage("combat_player_header"), StringComparison.Ordinal);
        int dragonStart = output.IndexOf(messages.GetMessage("combat_dragon_header"), playerStart, StringComparison.Ordinal);
        string[] combatStats = output[playerStart..dragonStart].Split(Environment.NewLine);
        string[] unchangedKeys = ["stats_name", "stats_strength", "stats_agility", "stats_health", "stats_weapon", "stats_damage"];
        foreach (string key in unchangedKeys)
        {
            string label = messages.GetMessage(key);
            string originalStat = originalStats.Single(line => line.StartsWith(label, StringComparison.Ordinal));
            CollectionAssert.Contains(combatStats, originalStat, $"{key} must survive watchtower exploration unchanged.");
        }

        CollectionAssert.Contains(combatStats, messages.GetMessage("stats_gold") + (merchantChoice == "3" ? "50" : "10"));
        Assert.AreEqual(merchantChoice == "1", combatStats.Contains(messages.GetMessage("stats_damage_reduction") + "2"));
        Assert.AreEqual(merchantChoice == "2", combatStats.Contains(string.Format(messages.GetMessage("stats_magic_damage_bonus"), 2)));
        if (merchantChoice != "3")
        {
            string purchasedKey = merchantChoice == "1" ? "merchant_armor_purchased" : "merchant_weapon_purchased";
            string acknowledgedKey = merchantChoice == "1" ? "merchant_already_purchased" : "merchant_weapon_already_purchased";
            Assert.AreEqual(1, CountOccurrences(output, string.Format(messages.GetMessage(purchasedKey), 10)));
            Assert.AreEqual(1, CountOccurrences(output, messages.GetMessage(acknowledgedKey)));
        }
        Assert.AreEqual(2, CountOccurrences(output, messages.GetMessage("watchtower_arrival")));
        Assert.AreEqual(2, CountOccurrences(output, messages.GetMessage("watchtower_invalid")));
        StringAssert.Contains(output, messages.GetMessage("north_path_narrative"));
        Assert.IsFalse(output.Contains(messages.GetMessage("path_invalid")));
    }

    private static int CountOccurrences(string output, string text) => output.Split(text).Length - 1;

    private string RunGame(Messages messages, string languageSelection, string adventureInput)
    {
        using var input = new ScriptedReader(CreateGameInput(messages, languageSelection, adventureInput) + "unused\n");
        Console.SetIn(input);
        new Game().StartGame();
        Assert.AreEqual("unused", Console.ReadLine(), "The adventure must stop at the exit command.");
        return _output.ToString();
    }

    private static string CreateGameInput(Messages messages, string languageSelection, string adventureInput,
        string characterActions = "2\n")
    {
        string race = messages.TranslateRaceForDisplay("Human");
        string occupation = messages.TranslateOccupationForDisplay("Fighter");
        return $"{languageSelection}\n1\n{race}\nHero\n{occupation}\nroll\nroll\nroll\n{characterActions}{adventureInput}";
    }

    private static void AssertReturnsToTravel(string output, Messages messages, string outcome)
    {
        int outcomeIndex = output.IndexOf(outcome, StringComparison.Ordinal);
        int travelIndex = output.IndexOf(messages.GetMessage("path_prompt_full"), outcomeIndex, StringComparison.Ordinal);
        Assert.IsTrue(travelIndex > outcomeIndex, "The travel menu must follow the selected outcome.");
        StringAssert.Contains(output[travelIndex..], messages.GetMessage("adventure_ends"));
    }

    private static Messages LoadMessages(string language)
    {
        var messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    // Fail an incorrect console flow promptly rather than retrying exhausted input forever.
    private sealed class ScriptedReader(string input, int allowedEofReads = 0) : StringReader(input)
    {
        private int _remainingEofReads = allowedEofReads;

        public override string? ReadLine()
        {
            string? line = base.ReadLine();
            if (line != null || _remainingEofReads-- > 0)
                return line;

            throw new AssertFailedException("The game consumed more input than the scripted journey supplies.");
        }
    }
}
