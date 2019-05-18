public class Weapon
{
    private double Damage { get; set; }
    private double StunPower { get; set; }
    

    Weapon()
    {

    }
    Weapon(double Health, double Damage, double StunPower, int Tier)
    {
        this.Damage = Damage;
        this.StunPower = StunPower;
    }
    
    public double GetDamage()
    {
        return Damage;
    }
    public double GetStunPower()
    {
        return StunPower;
    }

    public double GetInfluenceMultiplier()
    {
        return Damage / (StunPower + 1);
    }

}
