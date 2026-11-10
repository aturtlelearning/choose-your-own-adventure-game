public class Merchant
{
    // True returns to the path menu; false ends the game on input exhaustion.
    public bool Encounter(Player player, Messages messages)
    {
        if (player.HasPurchasedUpgrade)
        {
            Console.WriteLine(messages.GetMessage(player.HasMagicalArmor
                ? "merchant_already_purchased" : "merchant_weapon_already_purchased"));
            return true;
        }

        while (true)
        {
            Console.WriteLine(string.Format(messages.GetMessage("merchant_gold"), player.Gold));
            Console.WriteLine(string.Format(messages.GetMessage("merchant_armor_option"), Player.MagicalArmorCost));
            Console.WriteLine(string.Format(messages.GetMessage("merchant_weapon_option"), Player.WeaponEnchantmentCost));
            Console.WriteLine(messages.GetMessage("merchant_decline_option"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine();

            if (input == null)
                return false;

            if (input.Trim() == "3")
            {
                Console.WriteLine(messages.GetMessage("merchant_declined"));
                return true;
            }

            if (input.Trim() == "1" && player.TryPurchaseMagicalArmor())
            {
                Console.WriteLine(string.Format(messages.GetMessage("merchant_armor_purchased"), player.Gold));
                return true;
            }

            if (input.Trim() == "2" && player.TryPurchaseWeaponEnchantment())
            {
                Console.WriteLine(string.Format(messages.GetMessage("merchant_weapon_purchased"), player.Gold));
                return true;
            }

            Console.WriteLine(messages.GetMessage("merchant_invalid"));
        }
    }
}
