using UnityEngine;

public class Enemy
{
    public enum StatusEnum { Normal, Stunned };

    private double Health;
    private double Damage;
    private double StunMeter;
    private StatusEnum Status;
    private bool OnCollision;

    public Enemy()
    {
        StunMeter = 0f;
        Status = StatusEnum.Normal;
    }

    public Enemy(double Health, double Damage)
    {
        this.Health = Health;
        this.Damage= Damage;
        StunMeter = 0f;
        Status = StatusEnum.Normal;
        OnCollision = false;
    }

    public double GetHealth()
    {
        return Health;
    }
    public double GetDamage()
    {
        return Damage;
    }
    public double GetStunMeter()
    {
        return StunMeter;
    }
    public StatusEnum GetStatus()
    {
        return Status;
    }
    public void SetStatus(StatusEnum Status)
    {
        this.Status = Status;
    }
    public void TakeDamage(double Damage)
    {
        Health = Health - Damage;
    }

    public void SetOnCollision(bool Col)
    {
        OnCollision = Col;
    }

    public bool GetOnCollision()
    {
        return OnCollision;
    }

    public void TakeStun(double Stun)
    {
        if(Status == StatusEnum.Stunned)
        {
            return;
        }
        StunMeter = StunMeter + Stun; 
        if(StunMeter >= 100)
        {
            Status = StatusEnum.Stunned;
        }
    }

    public void ResetStunMeter()
    {
        StunMeter = 0;
    }
}
