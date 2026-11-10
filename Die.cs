using System;

public class Die
{
    private readonly Random _random;

    public Die(Random? random = null)
    {
        _random = random ?? new Random();
    }

    public int Roll(int sides)
    {
        return _random.Next(1, sides + 1);
    }
}
