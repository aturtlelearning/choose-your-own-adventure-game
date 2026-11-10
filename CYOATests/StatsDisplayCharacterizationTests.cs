using System.Text.Json;

[TestClass]
[DoNotParallelize]
public class StatsDisplayCharacterizationTests
{
    // Frozen complete strings from the pre-refactor layout. Never build expected
    // text from a production display method or the live localization dictionary.
    private static readonly List<DisplaySnapshot> Snapshots = JsonSerializer.Deserialize<List<DisplaySnapshot>>(
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "StatsDisplaySnapshots.json")))
        ?? throw new InvalidOperationException("Missing frozen display snapshots.");

    public static IEnumerable<object[]> DisplayCases()
    {
        foreach (string language in new[] { "English", "Spanish", "French", "Italian" })
        foreach (string upgrade in new[] { "none", "armor", "enchantment" })
        foreach (bool hasWeapon in new[] { true, false })
            yield return [language, upgrade, hasWeapon];
    }

    [DataTestMethod]
    [DynamicData(nameof(DisplayCases), DynamicDataSourceType.Method)]
    public void CharacterSummary_MatchesCompletePreRefactorOutput(string language, string upgrade, bool hasWeapon)
    {
        var snapshot = FindSnapshot(language, upgrade, hasWeapon);
        var player = CreatePlayer(snapshot);
        var messages = LoadMessages(language);
        TextWriter originalOut = Console.Out;
        using var output = new StringWriter();
        try
        {
            Console.SetOut(output);
            player.DisplayStats(messages);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        Assert.AreEqual(PlatformNewlines(snapshot.Character), output.ToString());
    }

    [DataTestMethod]
    [DynamicData(nameof(DisplayCases), DynamicDataSourceType.Method)]
    public void CombatStats_MatchCompletePreRefactorOutput(string language, string upgrade, bool hasWeapon)
    {
        var snapshot = FindSnapshot(language, upgrade, hasWeapon);
        var player = CreatePlayer(snapshot);
        var dragon = new Dragon(new Die(new Random(0)))
        {
            Name = "SnapshotDragon", Strength = 18, Agility = 10, HealthPoints = 30,
            Weapon = new Weapon("claws", 12, "DRAGON ART")
        };
        var combat = new Combat(player, dragon, LoadMessages(language));

        string output = combat.GetCombatStatsDisplay();

        Assert.AreEqual(PlatformNewlines(snapshot.Combat), output);
    }

    private static DisplaySnapshot FindSnapshot(string language, string upgrade, bool hasWeapon) =>
        Snapshots.Single(snapshot => snapshot.Language == language
            && snapshot.Upgrade == upgrade && snapshot.HasWeapon == hasWeapon);

    private static Player CreatePlayer(DisplaySnapshot snapshot)
    {
        var player = new Player(new Die(new Random(0)))
        {
            Name = "SnapshotHero", Race = snapshot.Race, Occupation = snapshot.Occupation,
            Strength = 14, Agility = 9, HealthPoints = 17,
            Weapon = new Weapon(snapshot.WeaponType, snapshot.WeaponDamage, "PLAYER ART")
        };
        if (snapshot.Upgrade == "armor")
            Assert.IsTrue(player.TryPurchaseMagicalArmor());
        if (snapshot.Upgrade == "enchantment")
            Assert.IsTrue(player.TryPurchaseWeaponEnchantment());
        // Purchase first so the no-weapon case preserves independently owned effects.
        if (!snapshot.HasWeapon)
            player.Weapon = null;
        return player;
    }

    private static Messages LoadMessages(string language)
    {
        var messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    // Convert only the snapshot's newline convention; never trim or normalize spaces.
    private static string PlatformNewlines(string text) => text.Replace("\n", Environment.NewLine);

    public sealed class DisplaySnapshot
    {
        public required string Language { get; init; }
        public required string Upgrade { get; init; }
        public bool HasWeapon { get; init; }
        public required string Race { get; init; }
        public required string Occupation { get; init; }
        public required string WeaponType { get; init; }
        public int WeaponDamage { get; init; }
        public required string Character { get; init; }
        public required string Combat { get; init; }
    }
}
