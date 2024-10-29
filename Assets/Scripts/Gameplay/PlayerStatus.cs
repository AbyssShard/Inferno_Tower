using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStatus : MonoBehaviour
{
    [Header("EssentialComponents")]

    public GameplayManager gameplayManager;
    public GridGenerator gridManager;
    public Animator anim;

    [Header("PlayerValues")]
    public int maxLife, maxShild, maxEnergy;
    
    [HideInInspector] public bool climbing;
    [HideInInspector] public int life, shild, energy;

    public Vector2 posIndex;
    [HideInInspector] public Vector2 currentGridPos;
    private Vector2 vel;
    public bool reciveDamage;

    public List<GameObject> grids = new List<GameObject>();

    public void PlayerStart()
    {
        anim = GetComponent<Animator>();

        life = maxLife;
        shild = maxShild;
        energy = maxEnergy;
    }
    private void Update()
    {
        life = Mathf.Clamp(life, 0, maxLife);
        shild = Mathf.Clamp(life, 0, maxShild);
        energy = Mathf.Clamp(energy, 0, maxEnergy);

        posIndex.y = Mathf.Clamp(posIndex.y, 1, gridManager.gridSize.y - 1);
        posIndex.x = Mathf.Clamp(posIndex.x, 0, gridManager.gridSize.x);

        grids = gridManager.curentTowerChunk;
        foreach (GameObject grid in grids)
        {
            Grid towerGrid = grid.GetComponent<Grid>();

            if (posIndex == towerGrid.gridIndex)
            {
                if (towerGrid.occupant == null)
                    towerGrid.occupant = this.gameObject;
                currentGridPos = grid.transform.position;
            }
            if (posIndex != towerGrid.gridIndex && towerGrid.occupant == this.gameObject)
                towerGrid.occupant = null;

        }

        if(climbing)
            transform.position = Vector2.SmoothDamp(transform.position, currentGridPos, ref vel, 0.2f);

        climbing = Vector2.Distance(transform.position, currentGridPos) > 0.1f;
        anim.SetBool("Move", climbing);
    }

    public void ReciveDamage(int damage)
    {
        Camera.main.GetComponent<Animator>().SetTrigger("Shake");
        Debug.Log("Dano levado = " + damage);
        life -= damage;
        reciveDamage = false;
    }
}
