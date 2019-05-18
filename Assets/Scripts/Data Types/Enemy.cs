public class Enemy
{
    private double Health{ get; set; }
    private double Damage{ get; set; }


    private int Tier { get; set; }
    private int Level { get; set; }

    Enemy()
    {

    }

    Enemy(double Health, double Damage, double StunPower, int Tier)
    {
        this.Health = Health;
        this.Damage= Damage;
        this.Tier = Tier;
        this.Level = 1;
    }

    public double GetHealth()
    {
        return Health;
    }
    public double GetDamage()
    {
        return Damage;
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

    public void TakeDamage(double Damage)
    {
        Health = Health - Damage;
    }
}
