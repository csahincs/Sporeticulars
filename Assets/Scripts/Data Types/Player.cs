using System.Collections.Generic;

public class Player
{
    private Weapon SelectedWeapon { get; set; }

    public Player()
    {

    }

    public Weapon GetSelectedWeapon()
    {
        return SelectedWeapon;
    }

    public void SetSelectedWeapon(Weapon weapon)
    {
        SelectedWeapon = weapon;
    }

}
