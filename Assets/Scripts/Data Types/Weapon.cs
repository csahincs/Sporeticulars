public class Weapon
{
    private double Damage { get; set; }
    private double StunPower { get; set; }
    
    private double Tier { get; set; }
    private double Level { get; set; }
    Weapon()
    {

    }
    Weapon(double Health, double Damage, double StunPower, int Tier)
    {
        this.Damage = Damage;
        this.StunPower = StunPower;

        this.Tier = Tier;
        this.Level = 1;
    }

    public double GetDamage()
    {
        return Damage;
    }
    public double GetStunPower()
    {
        return StunPower;
    }

    public double GetTier()
    {
        return Tier;
    }
    public double GetLevel()
    {
        return Level;
    }

    public void Upgrade()
    {
        Level = Level + 1;
    }

    public double GetUpgradeCost()
    {
        return Tier * Tier * Level + 10;
    }

    public double GetInfluenceMultiplier()
    {
        return Damage / (StunPower + 1);
    }

}
