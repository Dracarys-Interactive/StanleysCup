using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    public class Platform : Moveable
    {
        protected override void Update()
        {
            Player player = GetComponentInChildren<Player>();

            if (player)
            {
                Enemy enemy = GetComponentInChildren<Enemy>();

                if (enemy)
                {
                    GameManager.Instance.ResetGame();
                }
                else
                {
                    base.Update();
                }
            }
            else
            {
                base.Update();
            }
        }
    }
}
