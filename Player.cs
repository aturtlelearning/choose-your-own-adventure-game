public class Player
{
    private readonly Die _die;

    public Player(Die? die = null)
    {
        _die = die ?? new Die();
    }

    public string Name { get; set; } = "";
    public string Race { get; set; } = "";
    public string Occupation { get; set; } = "";

    public int Strength { get; set; }
    public int Agility { get; set; }
    public int HealthPoints { get; set; }

    public Weapon? Weapon { get; set; }

    public const int MagicalArmorCost = 40;
    public const int WeaponEnchantmentCost = 40;
    public int Gold { get; private set; } = 50;
    public bool HasMagicalArmor { get; private set; }
    public bool HasEnchantedWeapon { get; private set; }
    public bool HasPurchasedUpgrade => HasMagicalArmor || HasEnchantedWeapon;
    public int DamageReduction => HasMagicalArmor ? 2 : 0;
    public int MagicDamageBonus => HasEnchantedWeapon ? 2 : 0;

    public bool TryPurchaseMagicalArmor()
    {
        if (HasPurchasedUpgrade || Gold < MagicalArmorCost)
            return false;

        Gold -= MagicalArmorCost;
        HasMagicalArmor = true;
        return true;
    }

    public bool TryPurchaseWeaponEnchantment()
    {
        if (HasPurchasedUpgrade || Gold < WeaponEnchantmentCost || Weapon == null)
            return false;

        Gold -= WeaponEnchantmentCost;
        HasEnchantedWeapon = true;
        return true;
    }

    public void CreateCharacter(Messages messages)
    {
        Race = PromptForRace(messages);
        Name = PromptForName(messages);
        Occupation = PromptForOccupation(messages);

        AssignWeaponByOccupation();

        int strengthModifier = RollStrength(messages);
        RollAgility(messages);
        RollHealthPoints(messages, strengthModifier);

        PromptForNextAction(messages);
    }

    private string PromptForRace(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("race_prompt"));
            string? input = Console.ReadLine()?.Trim();

            if (messages.IsValidRace(input))
                return messages.NormalizeRace(input!);

            Console.WriteLine(messages.GetMessage("race_invalid"));
        }
    }

    private string PromptForName(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("name_prompt"));
            string? input = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(input))
                return input;

            Console.WriteLine(messages.GetMessage("name_invalid"));
        }
    }

    private string PromptForOccupation(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("occupation_prompt"));
            string? input = Console.ReadLine()?.Trim();

            if (messages.IsValidOccupation(input))
                return messages.NormalizeOccupation(input!);

            Console.WriteLine(messages.GetMessage("occupation_invalid"));
        }
    }

    private void AssignWeaponByOccupation()
    {
        switch (Occupation.ToLower())
        {
            case "fighter":
                Weapon = new Weapon("long sword", 12, "-)=====>");
                break;
            case "magician":
                Weapon = new Weapon("lightning bolt spell", 12, "zap~~~~~~");
                break;
            case "thief":
                Weapon = new Weapon("dagger", 6, "-)==>");
                break;
            case "archer":
                Weapon = new Weapon("long bow", 8, "}    -->");
                break;
        }
    }

    private int RollStrength(Messages messages)
    {
        PromptRoll(messages.GetMessage("roll_strength"), messages);
        Strength = _die.Roll(20);

        int modifier = _die.Roll(4);

        if (Race == "Halfling")
        {
            Strength -= modifier;
            return -modifier;
        }

        Strength += modifier;
        return modifier;
    }

    private void RollAgility(Messages messages)
    {
        PromptRoll(messages.GetMessage("roll_agility"), messages);
        Agility = _die.Roll(20);

        if (Race == "Halfling" || Race == "Elf")
            Agility += _die.Roll(4);
    }

    private void RollHealthPoints(Messages messages, int strengthModifier)
    {
        PromptRoll(messages.GetMessage("roll_health"), messages);
        HealthPoints = _die.Roll(20) + strengthModifier;
    }

    private void PromptForNextAction(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("next_action"));
            string? input = Console.ReadLine()?.Trim();

            if (input == "1")
                DisplayStats(messages);
            else if (input == "2")
                return;
            else
                Console.WriteLine(messages.GetMessage("invalid"));
        }
    }

    public string GetInventoryDisplay(Messages messages)
    {
        var output = new List<string> { messages.GetMessage("inventory_header") };
        output.Add(messages.GetMessage("stats_weapon") + (Weapon == null
            ? messages.GetMessage("inventory_none") : messages.TranslateWeaponForDisplay(Weapon.Type)));
        if (Weapon != null)
            output.Add(string.Format(messages.GetMessage("inventory_base_damage"), Weapon.MaxDamage));

        string upgradeKey = HasMagicalArmor ? "inventory_armor"
            : HasEnchantedWeapon ? "inventory_enchantment" : "inventory_none";
        output.Add(string.Format(messages.GetMessage("inventory_upgrade"), messages.GetMessage(upgradeKey)));
        if (HasMagicalArmor)
            output.Add(messages.GetMessage("stats_damage_reduction") + DamageReduction);
        else if (HasEnchantedWeapon)
            output.Add(string.Format(messages.GetMessage("stats_magic_damage_bonus"), MagicDamageBonus));
        return string.Join(Environment.NewLine, output);
    }

    public void DisplayStats(Messages messages)
    {
        Console.WriteLine(messages.GetMessage("stats_header"));
        Console.WriteLine(messages.GetMessage("stats_name") + Name);
        Console.WriteLine(messages.GetMessage("stats_race") + messages.TranslateRaceForDisplay(Race));
        Console.WriteLine(messages.GetMessage("stats_occupation") + messages.TranslateOccupationForDisplay(Occupation));
        Console.WriteLine(messages.GetMessage("stats_strength") + Strength);
        Console.WriteLine(messages.GetMessage("stats_agility") + Agility);
        Console.WriteLine(messages.GetMessage("stats_health") + HealthPoints);
        Console.WriteLine(messages.GetMessage("stats_gold") + Gold);
        if (HasMagicalArmor)
            Console.WriteLine(messages.GetMessage("stats_damage_reduction") + DamageReduction);
        if (HasEnchantedWeapon)
            Console.WriteLine(string.Format(messages.GetMessage("stats_magic_damage_bonus"), MagicDamageBonus));

        if (Weapon != null)
        {
            Console.WriteLine(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(Weapon.Type));
            Console.WriteLine(messages.GetMessage("stats_damage") + Weapon.MaxDamage);
        }
    }

    public int RollAttack()
    {
        return _die.Roll(20);
    }

    public bool AttackHits(int attackRoll)
    {
        return attackRoll <= Strength;
    }

    public int RollDamage()
    {
        if (Weapon == null)
            return 0;

        return _die.Roll(Weapon.MaxDamage);
    }

    public string Attack(Dragon dragon, Messages messages)
    {
        if (Weapon == null)
            return "You have no weapon to attack with.";

        var output = new List<string>();

        output.Add(string.Format(
            messages.GetMessage("player_attack_intro"),
            messages.TranslateOccupationForDisplay(Occupation),
            messages.TranslateWeaponForDisplay(Weapon.Type)
        ));

        // If this fails to compile, change AsciiArt to whatever exists in Weapon.cs
        output.Add(Weapon.AsciiArt);

        int attackRoll = RollAttack();

        // MISS
        if (!AttackHits(attackRoll))
        {
            output.Add(string.Format(messages.GetMessage("player_missed_dragon"), dragon.Name));
            return string.Join(Environment.NewLine, output);
        }

        output.Add(string.Format(messages.GetMessage("player_hit_dragon"), dragon.Name));

        int defenseRoll = _die.Roll(20);

        // DEFENSE
        if (defenseRoll <= dragon.Agility)
        {
            output.Add(string.Format(messages.GetMessage("dragon_defended_player_attack"), dragon.Name));
            output.Add(string.Format(
                dragon.GetRandomDefenseTaunt(messages),
                messages.TranslateRaceForDisplay(Race)
            ));
            return string.Join(Environment.NewLine, output);
        }

        // DAMAGE
        int damage = RollDamage() + MagicDamageBonus;
        dragon.HealthPoints -= damage;

        output.Add(string.Format(messages.GetMessage("player_damage_dealt"), damage));
        output.Add(string.Format(messages.GetMessage("dragon_health_now"), dragon.Name, dragon.HealthPoints));

        // END
        if (dragon.HealthPoints <= 0)
            output.Add(messages.GetMessage("dragon_defeated_narrative"));
        else
            output.Add(dragon.GetRandomDamageReply(messages));

        return string.Join(Environment.NewLine, output);
    }

    private void PromptRoll(string prompt, Messages messages)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "roll")
                return;

            Console.WriteLine(messages.GetMessage("roll_invalid"));
        }
    }
}
