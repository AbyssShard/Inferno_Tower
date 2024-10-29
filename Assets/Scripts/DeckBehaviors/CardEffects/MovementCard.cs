using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCard : CardBase, CardAction
{
    public Vector2 direction;
    public void Action()
    {

        PlayerStatus player = FindObjectOfType<PlayerStatus>();
        GameObject enemy = null;
        foreach (GameObject grid in player.grids)
        {
            Grid towerGrid = grid.GetComponent<Grid>();
            if(towerGrid.gridIndex == player.posIndex + direction)
            {
                if (towerGrid.occupant != null)
                {
                    enemy = towerGrid.occupant;
                }
            }
        }
        if(enemy == null)
        {
            player.posIndex += direction;
            endAction = true;
        }
        else
            StartCoroutine(OnCollideWithObstacle(player, enemy.GetComponent<EnemyBase>()));
    }
    IEnumerator OnCollideWithObstacle(PlayerStatus player, EnemyBase enemy)
    {
        player.reciveDamage = true;
        Vector2 originalPosIndex = player.posIndex;
        yield return new WaitForSeconds(0.3f);
        player.posIndex += direction;
        yield return new WaitForSeconds(0.7f);
        player.ReciveDamage(enemy.damage);
        player.posIndex = originalPosIndex;
        endAction = true;
    }
}
