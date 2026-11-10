using System.Text;

[TestClass]
[DoNotParallelize]
public class MerchantTests
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

    private static Messages LoadMessages(string language = "English")
    {
        var messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    private static void SetInput(string input) => Console.SetIn(new BoundedReader(input));

    [TestMethod]
    public void PurchaseArmor_ChargesOnceAndPreservesCharacterAndWeapon()
    {
        var weapon = new Weapon("long sword", 12, "-)=====>");
        var player = new Player
        {
            Name = "Hero", Race = "Human", Occupation = "Fighter",
            Strength = 15, Agility = 8, HealthPoints = 20, Weapon = weapon
        };
        var messages = LoadMessages();
        Assert.AreEqual(50, player.Gold);
        Assert.IsFalse(player.HasMagicalArmor);
        SetInput("1\n");

        Assert.IsTrue(new Merchant().Encounter(player, messages));

        Assert.AreEqual(10, player.Gold);
        Assert.IsTrue(player.HasMagicalArmor);
        Assert.AreSame(weapon, player.Weapon);
        Assert.AreEqual("long sword", weapon.Type);
        Assert.AreEqual(12, weapon.MaxDamage);
        Assert.AreEqual("-)=====>", weapon.AsciiArt);
        Assert.AreEqual("Hero", player.Name);
        Assert.AreEqual("Human", player.Race);
        Assert.AreEqual("Fighter", player.Occupation);
        Assert.AreEqual(15, player.Strength);
        Assert.AreEqual(8, player.Agility);
        Assert.AreEqual(20, player.HealthPoints);
        StringAssert.Contains(_output.ToString(), "You purchased magical armor. Gold remaining: 10.");
        Assert.AreEqual(50, new Player().Gold);
        Assert.IsFalse(new Player().HasMagicalArmor);
        Assert.IsFalse(new Player().HasEnchantedWeapon);
    }

    [DataTestMethod]
    [DataRow("Fighter", "long sword", 12, "-)=====>")]
    [DataRow("Magician", "lightning bolt spell", 12, "zap~~~~~~")]
    [DataRow("Thief", "dagger", 6, "-)==>")]
    [DataRow("Archer", "long bow", 8, "}    -->")]
    public void PurchaseEnchantment_PreservesOccupationWeaponAndPreventsFurtherPurchases(
        string occupation, string weaponType, int maxDamage, string art)
    {
        var weapon = new Weapon(weaponType, maxDamage, art);
        var player = new Player
        {
            Name = "Hero", Race = "Human", Occupation = occupation,
            Strength = 15, Agility = 8, HealthPoints = 20, Weapon = weapon
        };
        var messages = LoadMessages();
        SetInput("3\n");
        Assert.IsTrue(new Merchant().Encounter(player, messages));
        Assert.AreEqual(50, player.Gold);
        Assert.IsFalse(player.HasEnchantedWeapon);

        SetInput("wrong\n\n 2 \n");
        Assert.IsTrue(new Merchant().Encounter(player, messages));
        Assert.AreEqual(10, player.Gold);
        Assert.IsTrue(player.HasEnchantedWeapon);
        Assert.IsFalse(player.HasMagicalArmor);
        Assert.AreEqual(2, player.MagicDamageBonus);
        Assert.AreSame(weapon, player.Weapon);
        Assert.AreEqual(weaponType, weapon.Type);
        Assert.AreEqual(maxDamage, weapon.MaxDamage);
        Assert.AreEqual(art, weapon.AsciiArt);
        Assert.AreEqual(occupation, player.Occupation);
        Assert.AreEqual("Human", player.Race);
        Assert.AreEqual("Hero", player.Name);
        Assert.AreEqual(15, player.Strength);
        Assert.AreEqual(8, player.Agility);
        Assert.AreEqual(20, player.HealthPoints);
        StringAssert.Contains(_output.ToString(), "You purchased a magical weapon enchantment. Gold remaining: 10.");

        _output.GetStringBuilder().Clear();
        SetInput("1\n");
        Assert.IsTrue(new Merchant().Encounter(player, messages));
        Assert.AreEqual("1", Console.ReadLine(), "Returning after enchantment must not reopen purchasing.");
        StringAssert.Contains(_output.ToString(), messages.GetMessage("merchant_weapon_already_purchased"));
        Assert.IsFalse(player.TryPurchaseMagicalArmor());
        Assert.IsFalse(player.TryPurchaseWeaponEnchantment());
        Assert.AreEqual(10, player.Gold);
        Assert.IsFalse(player.HasMagicalArmor);
        Assert.AreEqual(2, player.MagicDamageBonus);

        var armored = new Player { Weapon = new Weapon(weaponType, maxDamage, art) };
        SetInput("1\n");
        new Merchant().Encounter(armored, messages);
        SetInput("2\n");
        new Merchant().Encounter(armored, messages);
        Assert.AreEqual("2", Console.ReadLine());
        Assert.IsFalse(armored.TryPurchaseWeaponEnchantment());
        Assert.AreEqual(10, armored.Gold);
        Assert.IsTrue(armored.HasMagicalArmor);
        Assert.IsFalse(armored.HasEnchantedWeapon);
        Assert.AreEqual(0, armored.MagicDamageBonus);
    }

    [DataTestMethod]
    [DataRow("English", "Gold: 50", "1. Magical armor", "You purchased magical armor. Gold remaining: 10.")]
    [DataRow("Spanish", "Oro: 50", "1. Armadura mágica", "Has comprado armadura mágica. Oro restante: 10.")]
    [DataRow("French", "Or : 50", "1. Armure magique", "Vous avez acheté une armure magique. Or restant : 10.")]
    [DataRow("Italian", "Oro: 50", "1. Armatura magica", "Hai acquistato un'armatura magica. Oro rimanente: 10.")]
    public void MerchantDialogue_IsLocalizedAndSupportsRecoveryAndRepeatVisits(
        string language, string gold, string option, string confirmation)
    {
        var messages = LoadMessages(language);
        var player = new Player();
        SetInput("0\n\nwrong\n3\n");
        Assert.IsTrue(new Merchant().Encounter(player, messages));
        Assert.AreEqual(50, player.Gold);
        Assert.IsFalse(player.HasMagicalArmor);
        string declined = _output.ToString();
        StringAssert.Contains(declined, gold);
        StringAssert.Contains(declined, option);
        StringAssert.Contains(declined, string.Format(messages.GetMessage("merchant_armor_option"), 40));
        StringAssert.Contains(declined, messages.GetMessage("merchant_decline_option"));
        StringAssert.Contains(declined, messages.GetMessage("merchant_declined"));
        Assert.AreEqual(3, declined.Split(messages.GetMessage("merchant_invalid")).Length - 1);
        StringAssert.Contains(declined, string.Format(messages.GetMessage("merchant_weapon_option"), 40));

        _output.GetStringBuilder().Clear();
        SetInput(" 1 \n");
        Assert.IsTrue(new Merchant().Encounter(player, messages));
        StringAssert.Contains(_output.ToString(), confirmation);
        Assert.AreEqual(10, player.Gold);
        Assert.IsTrue(player.HasMagicalArmor);

        _output.GetStringBuilder().Clear();
        SetInput("1\n");
        Assert.IsTrue(new Merchant().Encounter(player, messages));
        Assert.AreEqual(messages.GetMessage("merchant_already_purchased") + Environment.NewLine,
            _output.ToString());
        Assert.AreEqual("1", Console.ReadLine(), "Repeat visits must not read another purchase choice.");
        Assert.AreEqual(10, player.Gold);
        Assert.IsTrue(player.HasMagicalArmor);
        Assert.IsFalse(player.TryPurchaseMagicalArmor());
        Assert.AreEqual(10, player.Gold);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("\n")]
    [DataRow("wrong\n")]
    [DataRow("0\n\n")]
    public void MerchantEof_ReturnsExitWithoutChangingState(string input)
    {
        var weapon = new Weapon("dagger", 6, "-)==>");
        var player = new Player { HealthPoints = 20, Strength = 12, Agility = 8, Weapon = weapon };
        SetInput(input);
        Assert.IsFalse(new Merchant().Encounter(player, LoadMessages()));
        Assert.AreEqual(50, player.Gold);
        Assert.IsFalse(player.HasMagicalArmor);
        Assert.IsFalse(player.HasEnchantedWeapon);
        Assert.AreEqual(0, player.MagicDamageBonus);
        Assert.AreEqual(20, player.HealthPoints);
        Assert.AreEqual(12, player.Strength);
        Assert.AreEqual(8, player.Agility);
        Assert.AreSame(weapon, player.Weapon);
        Assert.AreEqual(6, weapon.MaxDamage);
    }

    [DataTestMethod]
    [DataRow(true, 5, 17, 3)]
    [DataRow(true, 2, 20, 0)]
    [DataRow(true, 1, 20, 0)]
    [DataRow(false, 5, 15, 5)]
    [DataRow(false, 2, 18, 2)]
    [DataRow(false, 1, 19, 1)]
    public void DragonAttack_AppliesArmorToActualDamageAndReportedHealth(
        bool armored, int rolledDamage, int expectedHealth, int expectedDamage)
    {
        var player = new Player { Name = "Hero", HealthPoints = 20, Agility = 0 };
        if (armored)
        {
            SetInput("1\n");
            new Merchant().Encounter(player, LoadMessages());
        }
        var dragon = new Dragon(new Die(new ScriptedRandom(10, 10, 10, 1, 20, rolledDamage)))
        {
            Strength = 20
        };
        var messages = LoadMessages();

        string attack = dragon.Attack(player, messages);

        Assert.AreEqual(expectedHealth, player.HealthPoints);
        StringAssert.Contains(attack, string.Format(messages.GetMessage("dragon_damage_dealt"), dragon.Name, expectedDamage));
        StringAssert.Contains(attack, string.Format(messages.GetMessage("player_health_now"), player.Name, expectedHealth));
    }

    [DataTestMethod]
    [DataRow(true, false)]
    [DataRow(true, true)]
    [DataRow(false, false)]
    [DataRow(false, true)]
    public void DragonAttack_MissOrDefenseDealsNoDamage(bool armored, bool defended)
    {
        var player = new Player { HealthPoints = 20, Agility = defended ? 20 : 0 };
        if (armored)
            player.TryPurchaseMagicalArmor();
        var dragon = new Dragon(new Die(new ScriptedRandom(10, 10, 10, 10, 10)))
        {
            Strength = defended ? 20 : 0
        };
        var messages = LoadMessages();
        string attack = dragon.Attack(player, messages);
        Assert.AreEqual(20, player.HealthPoints);
        string key = defended ? "player_defended_dragon_attack" : "dragon_missed_player";
        StringAssert.Contains(attack, defended
            ? string.Format(messages.GetMessage(key), player.Name)
            : string.Format(messages.GetMessage(key), dragon.Name, player.Name));
    }

    [DataTestMethod]
    [DataRow("English", "Gold: ", "Damage reduction: ", "Magic damage bonus: +2")]
    [DataRow("Spanish", "Oro: ", "Reducción de daño: ", "Bonificación de daño mágico: +2")]
    [DataRow("French", "Or : ", "Réduction des dégâts : ", "Bonus de dégâts magiques : +2")]
    [DataRow("Italian", "Oro: ", "Riduzione dei danni: ", "Bonus ai danni magici: +2")]
    public void Stats_ShowLocalizedGoldAndOnlyPurchasedUpgradeWithoutChangingExistingStats(
        string language, string goldLabel, string reductionLabel, string bonusLabel)
    {
        var messages = LoadMessages(language);
        var player = new Player
        {
            Name = "Hero", Race = "Human", Occupation = "Fighter", Strength = 12,
            Agility = 8, HealthPoints = 20, Weapon = new Weapon("long sword", 12, "sword")
        };
        var combat = new Combat(player, new Dragon(new Die()), messages);
        player.DisplayStats(messages);
        string beforeCharacter = _output.ToString();
        string beforeCombat = combat.GetCombatStatsDisplay();
        StringAssert.Contains(beforeCharacter, goldLabel + "50");
        StringAssert.Contains(beforeCombat, goldLabel + "50");
        Assert.IsFalse(beforeCharacter.Contains(reductionLabel));
        Assert.IsFalse(beforeCombat.Contains(reductionLabel));
        Assert.IsFalse(beforeCharacter.Contains(bonusLabel));
        Assert.IsFalse(beforeCombat.Contains(bonusLabel));

        player.TryPurchaseMagicalArmor();
        _output.GetStringBuilder().Clear();
        player.DisplayStats(messages);
        string afterCharacter = _output.ToString();
        string afterCombat = combat.GetCombatStatsDisplay();
        foreach (string display in new[] { afterCharacter, afterCombat })
        {
            StringAssert.Contains(display, goldLabel + "10");
            StringAssert.Contains(display, reductionLabel + "2");
            Assert.IsFalse(display.Contains(bonusLabel));
            StringAssert.Contains(display, messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay("long sword"));
            StringAssert.Contains(display, messages.GetMessage("stats_damage") + "12");
            StringAssert.Contains(display, messages.GetMessage("stats_health") + "20");
            StringAssert.Contains(display, messages.GetMessage("stats_strength") + "12");
            StringAssert.Contains(display, messages.GetMessage("stats_agility") + "8");
            StringAssert.Contains(display, messages.GetMessage("stats_name") + "Hero");
        }
        StringAssert.Contains(afterCharacter, messages.GetMessage("stats_race") + messages.TranslateRaceForDisplay("Human"));
        StringAssert.Contains(afterCharacter, messages.GetMessage("stats_occupation") + messages.TranslateOccupationForDisplay("Fighter"));

        var enchanted = new Player
        {
            Name = "Hero", Race = "Human", Occupation = "Fighter", Strength = 12,
            Agility = 8, HealthPoints = 20, Weapon = new Weapon("long sword", 12, "sword")
        };
        enchanted.TryPurchaseWeaponEnchantment();
        _output.GetStringBuilder().Clear();
        enchanted.DisplayStats(messages);
        foreach (string display in new[] { _output.ToString(), new Combat(enchanted, combat.dragon, messages).GetCombatStatsDisplay() })
        {
            StringAssert.Contains(display, goldLabel + "10");
            StringAssert.Contains(display, bonusLabel);
            Assert.IsFalse(display.Contains(reductionLabel));
            StringAssert.Contains(display, messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay("long sword"));
            StringAssert.Contains(display, messages.GetMessage("stats_damage") + "12");
            StringAssert.Contains(display, messages.GetMessage("stats_health") + "20");
            StringAssert.Contains(display, messages.GetMessage("stats_strength") + "12");
            StringAssert.Contains(display, messages.GetMessage("stats_agility") + "8");
            StringAssert.Contains(display, messages.GetMessage("stats_name") + "Hero");
        }
    }

    [DataTestMethod]
    [DataRow("English", "1", "2. Magical weapon enchantment", "You purchased a magical weapon enchantment. Gold remaining: 10.")]
    [DataRow("Spanish", "2", "2. Encantamiento mágico para tu arma", "Has comprado un encantamiento mágico para tu arma. Oro restante: 10.")]
    [DataRow("French", "3", "2. Enchantement magique pour votre arme", "Vous avez acheté un enchantement magique pour votre arme. Or restant : 10.")]
    [DataRow("Italian", "4", "2. Incantamento magico per la tua arma", "Hai acquistato un incantamento magico per la tua arma. Oro rimanente: 10.")]
    public void SouthPath_EnchantmentFlowUsesSelectedLanguageAndAcknowledgesRepeatVisit(
        string language, string selection, string option, string confirmation)
    {
        var messages = LoadMessages(language);
        string race = messages.TranslateRaceForDisplay("Human");
        string occupation = messages.TranslateOccupationForDisplay("Fighter");
        SetInput($"{selection}\n1\n{race}\nHero\n{occupation}\nroll\nroll\nroll\n2\ns\n3\ns\nwrong\n\n2\ns\ne\n");
        new Game().StartGame();
        string output = _output.ToString();
        StringAssert.Contains(output, option);
        StringAssert.Contains(output, string.Format(messages.GetMessage("merchant_armor_option"), 40));
        StringAssert.Contains(output, string.Format(messages.GetMessage("merchant_weapon_option"), 40));
        StringAssert.Contains(output, messages.GetMessage("merchant_declined"));
        StringAssert.Contains(output, confirmation);
        StringAssert.Contains(output, messages.GetMessage("merchant_weapon_already_purchased"));
        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
        Assert.AreEqual(2, output.Split(messages.GetMessage("merchant_invalid")).Length - 1);
        Assert.AreEqual(1, output.Split(confirmation).Length - 1);
        Assert.IsFalse(output.Contains("[merchant_"));
    }

    [DataTestMethod]
    [DataRow("1\ne\n", true, false)]
    [DataRow("3\ne\n", false, false)]
    [DataRow("3\ns\n1\ns\ne\n", true, true)]
    [DataRow("", false, false)]
    [DataRow("\nwrong\n0\n", false, false)]
    public void SouthPath_HandlesPurchaseDeclineReturnAndEofWithoutLooping(
        string merchantInput, bool purchased, bool revisited)
    {
        var messages = LoadMessages();
        SetInput("1\n1\nHuman\nHero\nFighter\nroll\nroll\nroll\n2\ns\n" + merchantInput);
        new Game().StartGame();
        string output = _output.ToString();
        StringAssert.Contains(output, messages.GetMessage("south_path_narrative"));
        StringAssert.Contains(output, "1. Magical armor");
        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
        Assert.AreEqual(purchased, output.Contains("You purchased magical armor. Gold remaining: 10."));
        Assert.AreEqual(revisited, output.Contains(messages.GetMessage("merchant_already_purchased")));
        Assert.IsFalse(output.Contains("[merchant_"));
    }

    [DataTestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void MerchantLocalization_HasCompleteKeysAndMatchingFormatArguments(string language)
    {
        var english = LoadMessages();
        var translated = LoadMessages(language);
        string[] keys = ["merchant_gold", "merchant_armor_option", "merchant_decline_option",
            "merchant_declined", "merchant_armor_purchased", "merchant_already_purchased",
            "merchant_invalid", "stats_gold", "stats_damage_reduction", "south_path_narrative",
            "merchant_weapon_option", "merchant_weapon_purchased", "merchant_weapon_already_purchased",
            "stats_magic_damage_bonus"];
        foreach (string key in keys)
        {
            string text = translated.GetMessage(key);
            Assert.AreNotEqual("[" + key + "]", text);
            Assert.IsFalse(string.IsNullOrWhiteSpace(text));
            Assert.AreEqual(CompositeFormat.Parse(english.GetMessage(key)).MinimumArgumentCount,
                CompositeFormat.Parse(text).MinimumArgumentCount, key);
        }
    }

    [DataTestMethod]
    [DataRow("long sword", 12, true, 7, 13)]
    [DataRow("lightning bolt spell", 12, true, 7, 13)]
    [DataRow("dagger", 6, true, 7, 13)]
    [DataRow("long bow", 8, true, 7, 13)]
    [DataRow("long sword", 12, false, 5, 15)]
    [DataRow("lightning bolt spell", 12, false, 5, 15)]
    [DataRow("dagger", 6, false, 5, 15)]
    [DataRow("long bow", 8, false, 5, 15)]
    public void PlayerAttack_EnchantmentAddsTwoToUnblockedDamageAndPreservesBaseWeapon(
        string type, int maxDamage, bool enchanted, int expectedDamage, int expectedHealth)
    {
        var player = new Player(new Die(new ScriptedRandom(1, 20, 5)))
        {
            Strength = 20, Weapon = new Weapon(type, maxDamage, "original art")
        };
        if (enchanted)
        {
            SetInput("2\n");
            new Merchant().Encounter(player, LoadMessages());
        }
        var dragon = new Dragon(new Die(new ScriptedRandom(10, 10, 10, 1)))
        {
            HealthPoints = 20, Agility = 0
        };
        var messages = LoadMessages();
        string attack = player.Attack(dragon, messages);
        Assert.AreEqual(expectedHealth, dragon.HealthPoints);
        StringAssert.Contains(attack, string.Format(messages.GetMessage("player_damage_dealt"), expectedDamage));
        StringAssert.Contains(attack, string.Format(messages.GetMessage("dragon_health_now"), dragon.Name, expectedHealth));
        StringAssert.Contains(attack, "original art");
        Assert.AreEqual(maxDamage, player.Weapon.MaxDamage);
        Assert.AreEqual(type, player.Weapon.Type);
    }

    [DataTestMethod]
    [DataRow(true, true)]
    [DataRow(true, false)]
    [DataRow(false, true)]
    [DataRow(false, false)]
    public void PlayerAttack_EnchantmentDoesNotBypassMissesOrDefense(bool enchanted, bool defended)
    {
        var player = new Player(new Die(new ScriptedRandom(10, 10)))
        {
            Strength = defended ? 20 : 0, Weapon = new Weapon("dagger", 6, "dagger")
        };
        if (enchanted)
            player.TryPurchaseWeaponEnchantment();
        var dragon = new Dragon(new Die(new ScriptedRandom(10, 10, 10, 1)))
        {
            HealthPoints = 20, Agility = 20
        };
        var messages = LoadMessages();
        string attack = player.Attack(dragon, messages);
        Assert.AreEqual(20, dragon.HealthPoints);
        string key = defended ? "dragon_defended_player_attack" : "player_missed_dragon";
        StringAssert.Contains(attack, string.Format(messages.GetMessage(key), dragon.Name));
    }

    private sealed class ScriptedRandom(params int[] rolls) : Random
    {
        private readonly Queue<int> _rolls = new(rolls);

        public override int Next(int minValue, int maxValue)
        {
            int result = _rolls.Dequeue();
            Assert.IsTrue(result >= minValue && result < maxValue, "Scripted roll is outside the die's range.");
            return result;
        }
    }

    // A retry loop after EOF fails synchronously instead of leaving a hung test.
    private sealed class BoundedReader(string input) : StringReader(input)
    {
        private int _remainingReads = input.Count(character => character == '\n') + 2;

        public override string? ReadLine()
        {
            if (--_remainingReads < 0)
                throw new AssertFailedException("The game kept reading after input was exhausted.");
            return base.ReadLine();
        }
    }
}
