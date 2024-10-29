using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public PlayerStatus player;
    [Header("EnemyValues")]
    public int life;
    public int damage;
    [Header("SpawnConfig")]
    public Vector2 posOffset;

    [HideInInspector]public Vector2 posIndex;
    [HideInInspector]public Vector2 worldPos;

    private Vector2 vel;
    public Grid currentGrid;
    public bool canMove;

    private void Start()
    {
        player = FindObjectOfType<PlayerStatus>();
    }

    private void Update()
    {
        foreach(Grid grid in FindObjectOfType<GridGenerator>().curentTowerGrids)
        {
            if(grid.gridIndex == posIndex)
            {
                grid.occupant = this.gameObject;
                currentGrid = grid;
                worldPos = grid.transform.position;
            }
            else if (grid.occupant == this.gameObject)
            {
                grid.occupant = null;
            }
        }
        transform.position = Vector2.SmoothDamp(transform.position, worldPos, ref vel, 0.4f);
    }
}

public interface EnemyAI
{
    public void RunEnemyAI();
}
