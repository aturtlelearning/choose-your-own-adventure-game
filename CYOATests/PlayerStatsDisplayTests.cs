using System.Text.Json;

[TestClass]
[DoNotParallelize]
public class PlayerStatsDisplayTests
{
    [TestMethod]
    public void SharedStats_PlainPlayerReturnsExactRowsWithoutHeadingOrFinalNewline()
    {
        var player = new Player
        {
            Strength = 14, Agility = 9, HealthPoints = 17,
            Weapon = new Weapon("dagger", 6, "ART")
        };

        string output = player.GetStatsDisplay(LoadMessages("English"));

        Assert.AreEqual(string.Join(Environment.NewLine,
            "Strength: 14", "Agility: 9", "Health Points: 17", "Gold: 50",
            "Weapon: dagger", "Weapon's Maximum Damage Points: 6"), output);
    }

    [DataTestMethod]
    [DataRow(false, "Strength: 14\nAgility: 9\nHealth Points: 17\nGold: 10\nDamage reduction: 2\nWeapon: long sword\nWeapon's Maximum Damage Points: 12")]
    [DataRow(true, "Strength: 14\nAgility: 9\nHealth Points: 17\nGold: 10\nDamage reduction: 2")]
    public void SharedStats_ArmorRowIsIndependentOfWeaponPresence(bool removeWeapon, string expected)
    {
        var player = new Player
        {
            Strength = 14, Agility = 9, HealthPoints = 17,
            Weapon = new Weapon("long sword", 12, "ART")
        };
        Assert.IsTrue(player.TryPurchaseMagicalArmor());
        if (removeWeapon)
            player.Weapon = null;

        Assert.AreEqual(expected.Replace("\n", Environment.NewLine), player.GetStatsDisplay(LoadMessages("English")));
    }

    [DataTestMethod]
    [DataRow(false, "Strength: 14\nAgility: 9\nHealth Points: 17\nGold: 10\nMagic damage bonus: +2\nWeapon: lightning bolt spell\nWeapon's Maximum Damage Points: 12")]
    [DataRow(true, "Strength: 14\nAgility: 9\nHealth Points: 17\nGold: 10\nMagic damage bonus: +2")]
    public void SharedStats_EnchantmentKeepsBonusSeparateFromWeaponDamage(bool removeWeapon, string expected)
    {
        var player = new Player
        {
            Strength = 14, Agility = 9, HealthPoints = 17,
            Weapon = new Weapon("lightning bolt spell", 12, "ART")
        };
        Assert.IsTrue(player.TryPurchaseWeaponEnchantment());
        if (removeWeapon)
            player.Weapon = null;

        Assert.AreEqual(expected.Replace("\n", Environment.NewLine), player.GetStatsDisplay(LoadMessages("English")));
    }

    public static IEnumerable<object[]> DisplayCases() => StatsDisplayCharacterizationTests.DisplayCases();

    [DataTestMethod]
    [DynamicData(nameof(DisplayCases), DynamicDataSourceType.Method)]
    public void SharedStats_MatchFrozenRowsInEveryLanguageWithoutStateOrConsoleEffects(
        string language, string upgrade, bool hasWeapon)
    {
        var snapshots = JsonSerializer.Deserialize<List<StatsDisplayCharacterizationTests.DisplaySnapshot>>(
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "StatsDisplaySnapshots.json")))!;
        var snapshot = snapshots.Single(item => item.Language == language
            && item.Upgrade == upgrade && item.HasWeapon == hasWeapon);
        var player = new Player(new Die(new Random(0)))
        {
            Name = "SnapshotHero", Race = snapshot.Race, Occupation = snapshot.Occupation,
            Strength = 14, Agility = 9, HealthPoints = 17,
            Weapon = new Weapon(snapshot.WeaponType, snapshot.WeaponDamage, "PLAYER ART")
        };
        if (upgrade == "armor")
            Assert.IsTrue(player.TryPurchaseMagicalArmor());
        if (upgrade == "enchantment")
            Assert.IsTrue(player.TryPurchaseWeaponEnchantment());
        if (!hasWeapon)
            player.Weapon = null;
        var messages = LoadMessages(language);
        var stateBefore = (player.Name, player.Race, player.Occupation, player.Strength, player.Agility,
            player.HealthPoints, player.Gold, player.Weapon, player.HasMagicalArmor, player.HasEnchantedWeapon,
            player.HasPurchasedUpgrade, player.DamageReduction, player.MagicDamageBonus);
        var weaponBefore = (player.Weapon?.Type, player.Weapon?.MaxDamage, player.Weapon?.AsciiArt);
        string inventoryBefore = player.GetInventoryDisplay(messages);
        // Remove only the frozen character prefix (four rows) and final newline.
        // Expected rows do not come from either current production formatter.
        string expected = string.Join(Environment.NewLine, snapshot.Character.Split('\n')[4..^1]);
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        using var noInput = new FailOnReadReader();
        using var consoleOutput = new StringWriter();
        try
        {
            Console.SetIn(noInput);
            Console.SetOut(consoleOutput);
            string output = player.GetStatsDisplay(messages);
            Assert.AreEqual(expected, output);
            Assert.AreEqual(output, player.GetStatsDisplay(messages));
            Assert.AreEqual("", consoleOutput.ToString());
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
        Assert.AreEqual(stateBefore, (player.Name, player.Race, player.Occupation, player.Strength, player.Agility,
            player.HealthPoints, player.Gold, player.Weapon, player.HasMagicalArmor, player.HasEnchantedWeapon,
            player.HasPurchasedUpgrade, player.DamageReduction, player.MagicDamageBonus));
        Assert.AreEqual(weaponBefore, (player.Weapon?.Type, player.Weapon?.MaxDamage, player.Weapon?.AsciiArt));
        Assert.AreEqual(inventoryBefore, player.GetInventoryDisplay(messages));
    }

    private sealed class FailOnReadReader : TextReader
    {
        public override string? ReadLine() => throw new AssertFailedException("Shared stats must not read console input.");
        public override int Read() => throw new AssertFailedException("Shared stats must not read console input.");
    }

    private static Messages LoadMessages(string language)
    {
        var messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }
}
