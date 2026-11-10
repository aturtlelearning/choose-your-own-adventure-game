using System.Text;

[TestClass]
[DoNotParallelize]
public class InventoryTests
{
    [TestMethod]
    public void Inventory_NoWeaponOrUpgradeShowsExplicitNoneWithoutDamage()
    {
        var player = new Player();

        string display = player.GetInventoryDisplay(LoadMessages("English"));

        Assert.AreEqual(string.Join(Environment.NewLine,
            "Inventory", "Weapon: None", "Magical upgrade: None"), display);
    }

    [DataTestMethod]
    [DataRow("long sword", 12)]
    [DataRow("lightning bolt spell", 12)]
    [DataRow("dagger", 6)]
    [DataRow("long bow", 8)]
    [DataRow("dagger", 9)]
    public void Inventory_UsesCurrentWeaponNameAndBaseDamage(string type, int damage)
    {
        var player = new Player { Weapon = new Weapon(type, damage, "WEAPON ART") };

        string display = player.GetInventoryDisplay(LoadMessages("English"));

        Assert.AreEqual(string.Join(Environment.NewLine,
            "Inventory", "Weapon: " + type, "Base maximum damage: " + damage,
            "Magical upgrade: None"), display);
    }

    [DataTestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Inventory_ArmorShowsNameAndReductionIndependentlyOfWeapon(bool removeWeapon)
    {
        var player = new Player { Weapon = new Weapon("dagger", 6, "ART") };
        Assert.IsTrue(player.TryPurchaseMagicalArmor());
        if (removeWeapon)
            player.Weapon = null;

        string display = player.GetInventoryDisplay(LoadMessages("English"));

        string[] expected = removeWeapon
            ? ["Inventory", "Weapon: None", "Magical upgrade: Magical armor", "Damage reduction: 2"]
            : ["Inventory", "Weapon: dagger", "Base maximum damage: 6", "Magical upgrade: Magical armor", "Damage reduction: 2"];
        Assert.AreEqual(string.Join(Environment.NewLine, expected), display);
    }

    [DataTestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Inventory_EnchantmentShowsSeparateBonusIndependentlyOfWeapon(bool removeWeapon)
    {
        var player = new Player { Weapon = new Weapon("long bow", 8, "ART") };
        Assert.IsTrue(player.TryPurchaseWeaponEnchantment());
        if (removeWeapon)
            player.Weapon = null;

        string display = player.GetInventoryDisplay(LoadMessages("English"));

        string[] expected = removeWeapon
            ? ["Inventory", "Weapon: None", "Magical upgrade: Weapon enchantment", "Magic damage bonus: +2"]
            : ["Inventory", "Weapon: long bow", "Base maximum damage: 8", "Magical upgrade: Weapon enchantment", "Magic damage bonus: +2"];
        Assert.AreEqual(string.Join(Environment.NewLine, expected), display);
    }

    [DataTestMethod]
    [DataRow("Spanish", "Inventario\nArma: arco largo\nDaño máximo base: 8\nMejora mágica: Ninguna",
        "Mejora mágica: Armadura mágica\nReducción de daño: 2",
        "Mejora mágica: Encantamiento del arma\nBonificación de daño mágico: +2", "Arma: Ninguna")]
    [DataRow("French", "Inventaire\nArme: arc long\nDégâts de base maximaux : 8\nAmélioration magique : Aucune",
        "Amélioration magique : Armure magique\nRéduction des dégâts : 2",
        "Amélioration magique : Enchantement de l’arme\nBonus de dégâts magiques : +2", "Arme: Aucune")]
    [DataRow("Italian", "Inventario\nArma: arco lungo\nDanno massimo di base: 8\nMiglioria magica: Nessuna",
        "Miglioria magica: Armatura magica\nRiduzione dei danni: 2",
        "Miglioria magica: Incantamento dell’arma\nBonus ai danni magici: +2", "Arma: Nessuna")]
    public void Inventory_LocalizesAllEquipmentAndAbsenceStates(string language, string noUpgrade,
        string armorEnding, string enchantmentEnding, string noWeapon)
    {
        var messages = LoadMessages(language);
        var plain = new Player { Weapon = new Weapon("long bow", 8, "ART") };
        Assert.AreEqual(Lines(noUpgrade), plain.GetInventoryDisplay(messages));
        var armored = new Player { Weapon = new Weapon("long bow", 8, "ART") };
        armored.TryPurchaseMagicalArmor();
        Assert.IsTrue(armored.GetInventoryDisplay(messages).EndsWith(Lines(armorEnding), StringComparison.Ordinal));
        var enchanted = new Player { Weapon = new Weapon("long bow", 8, "ART") };
        enchanted.TryPurchaseWeaponEnchantment();
        Assert.IsTrue(enchanted.GetInventoryDisplay(messages).EndsWith(Lines(enchantmentEnding), StringComparison.Ordinal));

        var players = new[] { plain, armored, enchanted };
        string[] upgradeEndings = [noUpgrade.Split('\n')[^1], armorEnding, enchantmentEnding];
        for (int index = 0; index < players.Length; index++)
        {
            var player = players[index];
            player.Weapon = null;
            string display = player.GetInventoryDisplay(messages);
            string expected = noUpgrade.Split('\n')[0] + "\n" + noWeapon + "\n" + upgradeEndings[index];
            Assert.AreEqual(Lines(expected), display);
        }
    }

    [DataTestMethod]
    [DataRow(0, false)]
    [DataRow(0, true)]
    [DataRow(1, false)]
    [DataRow(1, true)]
    [DataRow(2, false)]
    [DataRow(2, true)]
    public void Inventory_IsReadOnlyAndDoesNotReadOrWriteConsole(int upgrade, bool removeWeapon)
    {
        var weapon = new Weapon("dagger", 6, "UNIQUE_WEAPON_ART");
        var player = new Player
        {
            Name = "UNIQUE_HERO", Race = "Human", Occupation = "Thief",
            Strength = 14, Agility = 9, HealthPoints = 17, Weapon = weapon
        };
        if (upgrade == 1)
            player.TryPurchaseMagicalArmor();
        if (upgrade == 2)
            player.TryPurchaseWeaponEnchantment();
        if (removeWeapon)
            player.Weapon = null;
        var before = (player.Name, player.Race, player.Occupation, player.Strength, player.Agility,
            player.HealthPoints, player.Gold, player.Weapon, player.HasMagicalArmor, player.HasEnchantedWeapon,
            player.HasPurchasedUpgrade, player.DamageReduction, player.MagicDamageBonus);
        var weaponBefore = (weapon.Type, weapon.MaxDamage, weapon.AsciiArt);
        var messages = LoadMessages("English");
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        using var noInput = new FailOnReadReader();
        using var consoleOutput = new StringWriter();
        try
        {
            Console.SetIn(noInput);
            Console.SetOut(consoleOutput);
            string display = player.GetInventoryDisplay(messages);
            Assert.AreEqual(display, player.GetInventoryDisplay(messages));
            Assert.AreEqual("", consoleOutput.ToString());
            foreach (string key in new[] { "stats_name", "stats_race", "stats_occupation", "stats_health",
                "stats_gold", "stats_strength", "stats_agility", "combat_dragon_header" })
                Assert.IsFalse(display.Contains(messages.GetMessage(key)), key);
            Assert.IsFalse(display.Contains("UNIQUE_HERO"));
            Assert.IsFalse(display.Contains(weapon.AsciiArt));
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
        Assert.AreEqual(before, (player.Name, player.Race, player.Occupation, player.Strength, player.Agility,
            player.HealthPoints, player.Gold, player.Weapon, player.HasMagicalArmor, player.HasEnchantedWeapon,
            player.HasPurchasedUpgrade, player.DamageReduction, player.MagicDamageBonus));
        Assert.AreEqual(weaponBefore, (weapon.Type, weapon.MaxDamage, weapon.AsciiArt));
    }

    [DataTestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void InventoryMessages_HaveCompleteKeysAndMatchingFormatArguments(string language)
    {
        var english = LoadMessages("English");
        var messages = LoadMessages(language);
        foreach (string key in new[] { "inventory_header", "inventory_base_damage", "inventory_none",
            "inventory_upgrade", "inventory_armor", "inventory_enchantment" })
        {
            string text = messages.GetMessage(key);
            Assert.AreNotEqual($"[{key}]", text, key);
            Assert.IsFalse(string.IsNullOrWhiteSpace(text), key);
            Assert.AreEqual(CompositeFormat.Parse(english.GetMessage(key)).MinimumArgumentCount,
                CompositeFormat.Parse(text).MinimumArgumentCount, key);
        }
        Assert.AreEqual("[missing_inventory_key]", messages.GetMessage("missing_inventory_key"));
    }

    private sealed class FailOnReadReader : TextReader
    {
        public override string? ReadLine() => throw new AssertFailedException("Inventory must not read input.");
        public override int Read() => throw new AssertFailedException("Inventory must not read input.");
    }

    private static string Lines(string text) => text.Replace("\n", Environment.NewLine);

    private static Messages LoadMessages(string language)
    {
        var messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }
}
