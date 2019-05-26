public class Weapon
{
    private double Damage { get; set; }
    private double StunPower { get; set; }
    
    private int Tier { get; set; }
    private int Level { get; set; }

    public Weapon()
    {
        Level = 1;
    }
    public Weapon(double Damage, double StunPower, int Tier)
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
    public void SetDamage(double Damage)
    {
        this.Damage = Damage;
    }
    public void SetStunPower(double StunPower)
    {
        this.StunPower = StunPower;
    }
    public void SetTier(int Tier)
    {
        this.Tier = Tier;
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
