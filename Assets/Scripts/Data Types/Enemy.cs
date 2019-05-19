public class Enemy
{
    private double Health{ get; set; }
    private double Damage{ get; set; }

    public Enemy()
    {

    }

    public Enemy(double Health, double Damage)
    {
        this.Health = Health;
        this.Damage= Damage;
    }

    public double GetHealth()
    {
        return Health;
    }
    public double GetDamage()
    {
        return Damage;
    }

    public void TakeDamage(double Damage)
    {
        Health = Health - Damage;
    }
}
