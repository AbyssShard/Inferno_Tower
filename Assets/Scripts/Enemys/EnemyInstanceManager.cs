using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstanceManager : MonoBehaviour
{
    public GameplayManager gameplayManager;
    public EnemiesToAdd[] enemiesToAdd;
    public List<EnemyBase> enemiesInScene = new List<EnemyBase>();

    // Obt�m um inimigo aleat�rio baseado em pesos
    GameObject GetRandomEnemy()
    {
        var possibleEnemies = enemiesToAdd[gameplayManager.level].possibleEnemies;
        int totalWeight = CalculateTotalWeight(possibleEnemies);

        int randomWeight = Random.Range(0, totalWeight);
        return SelectEnemyBasedOnWeight(possibleEnemies, randomWeight);
    }

    // Calcula o peso total de todos os inimigos poss�veis
    int CalculateTotalWeight(PossibleEnemies[] possibleEnemies)
    {
        int totalWeight = 0;
        foreach (var enemy in possibleEnemies)
        {
            totalWeight += enemy.chanceWeight;
        }
        return totalWeight;
    }

    // Seleciona um inimigo com base no peso sorteado
    GameObject SelectEnemyBasedOnWeight(PossibleEnemies[] possibleEnemies, int randomWeight)
    {
        int currentWeight = 0;
        foreach (var enemy in possibleEnemies)
        {
            currentWeight += enemy.chanceWeight;
            if (randomWeight < currentWeight)
            {
                return enemy.enemyObject;
            }
        }
        return null;
    }

    // M�todo para limpar todos os inimigos na cena
    void ClearExistingEnemies()
    {
        foreach (var enemy in enemiesInScene)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        enemiesInScene.Clear();
    }

    // Spawna inimigos nas grids fornecidas respeitando o n�mero m�ximo de inimigos
    public void SpawnEnemies(List<Grid> towerGrids)
    {
        ClearExistingEnemies(); // Limpa inimigos existentes antes de gerar novos

        int maxEnemies = enemiesToAdd[gameplayManager.level].maxEnemies; // Obt�m o n�mero m�ximo de inimigos
        int enemiesSpawned = 0; // Inicia contagem para novos inimigos

        // Sorteia um n�mero de inimigos a serem spawnados entre 1 e maxEnemies
        int enemiesToSpawn = Random.Range(2, maxEnemies + 1);

        // Embaralha as grids para sele��o aleat�ria
        ShuffleGrids(towerGrids);

        foreach (var grid in towerGrids)
        {
            if (enemiesSpawned >= maxEnemies) break; // Interrompe se atingir o m�ximo de inimigos

            if (IsValidGridForSpawn(grid) && enemiesToSpawn > 0)
            {
                GameObject enemyPrefab = GetRandomEnemy();

                if (enemyPrefab != null)
                {
                    SpawnEnemyAtGrid(grid, enemyPrefab);
                    enemiesSpawned++; // Conta o inimigo instanciado
                    enemiesToSpawn--; // Decrementa a quantidade de inimigos a serem spawnados
                }
            }
        }
    }

    // Embaralha a lista de grids
    void ShuffleGrids(List<Grid> grids)
    {
        for (int i = 0; i < grids.Count; i++)
        {
            Grid temp = grids[i];
            int randomIndex = Random.Range(i, grids.Count);
            grids[i] = grids[randomIndex];
            grids[randomIndex] = temp;
        }
    }

    // Verifica se a grid � v�lida para spawnar inimigos
    bool IsValidGridForSpawn(Grid grid)
    {
        int playerPosIndex = (int)FindObjectOfType<PlayerStatus>().posIndex.y;
        return grid.occupant == null
            && grid.gridIndex.y > playerPosIndex + 1
            && grid.gridIndex.y < gameplayManager.grid.gridSize.y - 1;
    }

    // Spawna o inimigo na grid especificada
    void SpawnEnemyAtGrid(Grid grid, GameObject enemyPrefab)
    {
        Vector2 spawnPosition = (Vector2)grid.transform.position + enemyPrefab.GetComponent<EnemyBase>().posOffset;

        EnemyBase newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity).GetComponent<EnemyBase>();
        grid.occupant = newEnemy.gameObject;

        newEnemy.posIndex = grid.gridIndex;
        newEnemy.worldPos = grid.transform.position;

        enemiesInScene.Add(newEnemy); // Adiciona o novo inimigo � lista de inimigos na cena
    }
}

[System.Serializable]
public class PossibleEnemies
{
    public GameObject enemyObject;
    public int chanceWeight;
}

[System.Serializable]
public class EnemiesToAdd
{
    public PossibleEnemies[] possibleEnemies;
    public int maxEnemies; // Nova vari�vel para definir o m�ximo de inimigos que podem existir
}
