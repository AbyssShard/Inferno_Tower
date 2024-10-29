using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingDemon : EnemyBase, EnemyAI
{
    public void RunEnemyAI()
    {
        if (posIndex.x == player.posIndex.x && player.posIndex.y == posIndex.y - 1)
            Attack();
        else
        {
            canMove = true;
            if (posIndex.y - 1 != player.posIndex.y && posIndex.y > player.posIndex.y)
                Move(0, -1);

            if (posIndex.x != player.posIndex.x)
            {
                int direction = player.posIndex.x < posIndex.x ? -1 : 1;
                Move(direction, 0);
            }
        }
    }

    void Attack()
    {
        canMove = true;
        player.ReciveDamage(damage);
    }

    void Move(int x, int y)
    {
        foreach (Grid grid in FindObjectOfType<GridGenerator>().curentTowerGrids)
        {
            if (posIndex + new Vector2(x, y) == grid.gridIndex && grid.occupant == null)
            {
                posIndex += new Vector2(x, y);
            }
        }
    }
}
