[TestClass]
[DoNotParallelize]
public class InventoryFlowTests
{
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
    [DataRow("i")]
    [DataRow("inventory")]
    [DataRow("I")]
    [DataRow("INVENTORY")]
    [DataRow("  InVeNtOrY  ")]
    [DataRow("  i  ")]
    public void InventoryCommand_PrintsEquipmentAndReturnsDirectlyToTravel(string command)
    {
        var messages = LoadMessages("English");
        string output = RunGame(messages, "1", $"{command}\ne\n");
        string expected = string.Join(Environment.NewLine,
            "Inventory", "Weapon: long sword", "Base maximum damage: 12", "Magical upgrade: None");

        StringAssert.Contains(output, expected + Environment.NewLine + messages.GetMessage("path_prompt_full"));
        Assert.AreEqual(1, Count(output, expected));
        Assert.AreEqual(2, Count(output, messages.GetMessage("path_prompt_full")));
        Assert.IsFalse(output.Contains(messages.GetMessage("path_invalid")));
        Assert.IsTrue(new Game().IsValidPathChoice(command));
    }

    [DataTestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void TravelGuidance_ExplainsInventoryAndAllValidShortCommands(string language)
    {
        var messages = LoadMessages(language);
        string prompt = messages.GetMessage("path_prompt_full");
        StringAssert.Contains(prompt, "'i'");
        StringAssert.Contains(prompt, "'inventory'");
        foreach (string token in new[] { "n", "s", "w", "i", "e" })
            StringAssert.Contains(messages.GetMessage("path_invalid"), $"'{token}'");
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
    public void InventoryFlow_ReflectsPurchasesAndPreservesRepeatedViewingAndEncounters(
        string language, string selection, string merchantChoice)
    {
        var messages = LoadMessages(language);
        string secondMerchantChoice = merchantChoice == "3" ? "3\n" : "";
        string output = RunGame(messages, selection,
            $"wrong\n\ni\ninventory\nsouth\n{merchantChoice}\n INVENTORY \nwest\n1\nw\n2\ns\n{secondMerchantChoice}i\nnorth\nr\n");
        string[] views = output.Split(messages.GetMessage("inventory_header") + Environment.NewLine);
        Assert.AreEqual(5, views.Length, "Four inventory views must be available throughout the journey.");
        for (int view = 1; view <= 4; view++)
        {
            string upgrade = view <= 2 || merchantChoice == "3" ? "inventory_none"
                : merchantChoice == "1" ? "inventory_armor" : "inventory_enchantment";
            var expectedLines = new List<string>
            {
                messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay("long sword"),
                string.Format(messages.GetMessage("inventory_base_damage"), 12),
                string.Format(messages.GetMessage("inventory_upgrade"), messages.GetMessage(upgrade))
            };
            if (view > 2 && merchantChoice == "1")
                expectedLines.Add(messages.GetMessage("stats_damage_reduction") + "2");
            if (view > 2 && merchantChoice == "2")
                expectedLines.Add(string.Format(messages.GetMessage("stats_magic_damage_bonus"), 2));
            string actualView = views[view].Split(messages.GetMessage("path_prompt_full"))[0].TrimEnd('\r', '\n');
            Assert.AreEqual(string.Join(Environment.NewLine, expectedLines), actualView);
        }
        Assert.AreEqual(2, Count(output, messages.GetMessage("path_invalid")));
        Assert.AreEqual(2, Count(output, messages.GetMessage("watchtower_arrival")));
        StringAssert.Contains(output, messages.GetMessage("watchtower_stairs_outcome"));
        StringAssert.Contains(output, messages.GetMessage("watchtower_search_outcome"));
        StringAssert.Contains(output, messages.GetMessage("north_path_narrative"));
        StringAssert.Contains(output, messages.GetMessage("retreat"));
        if (merchantChoice != "3")
        {
            string purchaseKey = merchantChoice == "1" ? "merchant_armor_purchased" : "merchant_weapon_purchased";
            string repeatKey = merchantChoice == "1" ? "merchant_already_purchased" : "merchant_weapon_already_purchased";
            Assert.AreEqual(1, Count(output, string.Format(messages.GetMessage(purchaseKey), 10)));
            Assert.AreEqual(1, Count(output, messages.GetMessage(repeatKey)));
        }
        Assert.IsFalse(output.Contains("[inventory_"));
    }

    [DataTestMethod]
    [DataRow("English", "1")]
    [DataRow("Spanish", "2")]
    [DataRow("French", "3")]
    [DataRow("Italian", "4")]
    public void InventoryFlow_RepeatedViewingReturnsToTravelAndAcceptsFullWordExit(string language, string selection)
    {
        var messages = LoadMessages(language);
        string output = RunGame(messages, selection, "i\ninventory\nexit\n");
        Assert.AreEqual(2, Count(output, messages.GetMessage("inventory_header") + Environment.NewLine));
        Assert.AreEqual(3, Count(output, messages.GetMessage("path_prompt_full")));
        Assert.AreEqual(1, Count(output, messages.GetMessage("adventure_ends")));
    }

    [DataTestMethod]
    [DataRow("English", "1")]
    [DataRow("Spanish", "2")]
    [DataRow("French", "3")]
    [DataRow("Italian", "4")]
    public void InventoryFlow_ViewingAndInvalidCommandsPreserveCharacterBeforeCombat(string language, string selection)
    {
        var messages = LoadMessages(language);
        using var input = new ScriptedReader(CreateGameInput(messages, selection,
            "inventory\nwrong\n\ni\nw\n2\ni\nnorth\na\nr\n", "1\n2\n"));
        Console.SetIn(input);

        new Game().StartGame();

        string output = _output.ToString();
        int characterStart = output.IndexOf(messages.GetMessage("stats_header"), StringComparison.Ordinal);
        int travelStart = output.IndexOf(messages.GetMessage("path_prompt_full"), characterStart, StringComparison.Ordinal);
        string[] before = output[characterStart..travelStart].Split(Environment.NewLine);
        int playerStart = output.IndexOf(messages.GetMessage("combat_player_header"), StringComparison.Ordinal);
        int dragonStart = output.IndexOf(messages.GetMessage("combat_dragon_header"), playerStart, StringComparison.Ordinal);
        string[] after = output[playerStart..dragonStart].Split(Environment.NewLine);
        foreach (string key in new[] { "stats_name", "stats_strength", "stats_agility", "stats_health",
            "stats_gold", "stats_weapon", "stats_damage" })
        {
            string label = messages.GetMessage(key);
            string originalStat = before.Single(line => line.StartsWith(label, StringComparison.Ordinal));
            CollectionAssert.Contains(after, originalStat, key);
        }
        Assert.AreEqual(2, Count(output, messages.GetMessage("path_invalid")));
        Assert.AreEqual(3, Count(output, messages.GetMessage("inventory_header") + Environment.NewLine));
    }

    [DataTestMethod]
    [DataRow("English", "1", "", 0)]
    [DataRow("English", "1", "i\n", 1)]
    [DataRow("English", "1", "inventory\n", 1)]
    [DataRow("Spanish", "2", "", 0)]
    [DataRow("Spanish", "2", "i\n", 1)]
    [DataRow("Spanish", "2", "inventory\n", 1)]
    [DataRow("French", "3", "", 0)]
    [DataRow("French", "3", "i\n", 1)]
    [DataRow("French", "3", "inventory\n", 1)]
    [DataRow("Italian", "4", "", 0)]
    [DataRow("Italian", "4", "i\n", 1)]
    [DataRow("Italian", "4", "inventory\n", 1)]
    public void TravelEof_EndsAdventureOnceWithoutRetryOrInventedAction(
        string language, string selection, string adventureInput, int inventoryCount)
    {
        var messages = LoadMessages(language);
        string output = RunToTravelEof(messages, selection, adventureInput);
        AssertTravelEofOutput(output, messages, inventoryCount, 0);
    }

    [DataTestMethod]
    [DataRow("English", "1", "wrong\n", 0, 1)]
    [DataRow("English", "1", "\n", 0, 1)]
    [DataRow("English", "1", "i\ninventory\n", 2, 0)]
    [DataRow("English", "1", "wrong\n\n I \ninventory\nwrong\n", 2, 3)]
    [DataRow("Spanish", "2", "wrong\n", 0, 1)]
    [DataRow("Spanish", "2", "\n", 0, 1)]
    [DataRow("Spanish", "2", "i\ninventory\n", 2, 0)]
    [DataRow("Spanish", "2", "wrong\n\n I \ninventory\nwrong\n", 2, 3)]
    [DataRow("French", "3", "wrong\n", 0, 1)]
    [DataRow("French", "3", "\n", 0, 1)]
    [DataRow("French", "3", "i\ninventory\n", 2, 0)]
    [DataRow("French", "3", "wrong\n\n I \ninventory\nwrong\n", 2, 3)]
    [DataRow("Italian", "4", "wrong\n", 0, 1)]
    [DataRow("Italian", "4", "\n", 0, 1)]
    [DataRow("Italian", "4", "i\ninventory\n", 2, 0)]
    [DataRow("Italian", "4", "wrong\n\n I \ninventory\nwrong\n", 2, 3)]
    public void TravelEof_AfterRetriesAndRepeatedInventoryStillEndsWithoutExtraError(
        string language, string selection, string adventureInput, int inventoryCount, int errorCount)
    {
        var messages = LoadMessages(language);
        string output = RunToTravelEof(messages, selection, adventureInput);
        AssertTravelEofOutput(output, messages, inventoryCount, errorCount);
    }

    private static void AssertTravelEofOutput(string output, Messages messages, int inventoryCount, int errorCount)
    {
        Assert.AreEqual(inventoryCount, Count(output, messages.GetMessage("inventory_header") + Environment.NewLine));
        Assert.AreEqual(errorCount, Count(output, messages.GetMessage("path_invalid")));
        Assert.AreEqual(inventoryCount + errorCount + 1, Count(output, messages.GetMessage("path_prompt_full")));
        Assert.AreEqual(1, Count(output, messages.GetMessage("adventure_ends")));
        Assert.IsTrue(output.EndsWith(messages.GetMessage("enter_choice")
            + messages.GetMessage("adventure_ends") + Environment.NewLine, StringComparison.Ordinal));
        foreach (string key in new[] { "south_path_narrative", "watchtower_arrival", "north_path_narrative" })
            Assert.IsFalse(output.Contains(messages.GetMessage(key)), key);
    }

    private string RunToTravelEof(Messages messages, string selection, string adventureInput)
    {
        using var input = new ScriptedReader(CreateGameInput(messages, selection, adventureInput), allowedEofReads: 1);
        Console.SetIn(input);
        new Game().StartGame();
        return _output.ToString();
    }

    private string RunGame(Messages messages, string selection, string adventureInput)
    {
        using var input = new ScriptedReader(CreateGameInput(messages, selection, adventureInput) + "unused\n");
        Console.SetIn(input);
        new Game().StartGame();
        Assert.AreEqual("unused", Console.ReadLine(), "Inventory must not consume acknowledgement or onward input.");
        return _output.ToString();
    }

    private static string CreateGameInput(Messages messages, string selection, string adventureInput,
        string characterActions = "2\n")
    {
        string race = messages.TranslateRaceForDisplay("Human");
        string occupation = messages.TranslateOccupationForDisplay("Fighter");
        return $"{selection}\n1\n{race}\nHero\n{occupation}\nroll\nroll\nroll\n{characterActions}{adventureInput}";
    }

    private static int Count(string output, string text) => output.Split(text).Length - 1;

    private static Messages LoadMessages(string language)
    {
        var messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    // Allow a real EOF for termination tests, but fail promptly if the game retries it.
    private sealed class ScriptedReader(string input, int allowedEofReads = 0) : StringReader(input)
    {
        private int _remainingEofReads = allowedEofReads;

        public override string? ReadLine()
        {
            string? line = base.ReadLine();
            if (line != null || _remainingEofReads-- > 0)
                return line;

            throw new AssertFailedException("The game overran the scripted inventory journey or retried exhausted input.");
        }
    }
}
