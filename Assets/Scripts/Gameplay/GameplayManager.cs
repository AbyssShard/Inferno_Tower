using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("EssentialComponents")]
    public PlayerStatus player;
    public CardDeckManager cardManager;
    public GridGenerator grid;
    public EnemyInstanceManager enemyInstance;
    [Space]
    public Vector2[] cardPositions; // Posições pré-definidas para até 4 cartas

    [HideInInspector] public bool startTurn;

    public int level, floor;

    private void Start()
    {
        player.PlayerStart();
        cardManager.DeckManagerStart();
        grid.GenerateGrids();
    }

    private void Update()
    {
        ManageLevelIndex();
        //Iniciar turno.
        if (Input.GetKeyDown(KeyCode.Space) && !startTurn && cardManager.selectedCards.Count > 0)
        {
            startTurn = true;
            StartCoroutine(Turn());
        }
    }

    void ManageLevelIndex()
    {
        //Mudança de andar e level.
        if (floor == 4)
        {
            level += 1;
            floor = 0;
        }
    }

    //Metodo que realoca elementos do jogo ao acabar um turno.
    public void StopTurn()
    {
        StopAllCoroutines();
        DeselectCards();
        ChangeCards();
        startTurn = false;
    }

    //Metodo para desselecionar as cartas.
    private void DeselectCards()
    {
        //Desselecionar cartas e remover da lista de selecionados.
        List<CardBase> cardsToRemove = new List<CardBase>();
        foreach (CardBase card in cardManager.selectedCards)
        {
            card.selected = false;
            cardsToRemove.Add(card);
        }

        foreach (CardBase card in cardsToRemove)
        {
            cardManager.selectedCards.Remove(card);
        }
    }
    //Metodo para embaralhar as cartas.
    private void ChangeCards()
    {
        //Iniciar o efeito de criar trocar cartas e criar novas em DeckManager.
        CardBase[] inGameCards = FindObjectsOfType<CardBase>();
        foreach (CardBase card in inGameCards)
        {
            StartCoroutine(ChangeTurnCards(card));
        }
        cardManager.CreateNewCards();
    }

    //Corountine do turno.
    private IEnumerator Turn()
    {
        //Repetir ações escolhidas pelo jogador em ordem.
        for (int i = 0; i < cardManager.selectedCards.Count; i++)
        {
            if (cardManager.selectedCards[i] != null)
            {
                CardBase card = cardManager.selectedCards[i].GetComponent<CardBase>();
                card.GetComponent<CardAction>().Action();
                card.used = true;
                yield return new WaitUntil(() => card.endAction);
                yield return new WaitForSeconds(0.6f);
            }
            else
                continue;
        }

        yield return new WaitForSeconds(1f);

        //realizar ações dos inimigos do mais proximo para o mais distante.
        int enemiesCount = enemyInstance.enemiesInScene.Count;
        if(enemiesCount > 0)
        {
            for (int i = 0; i < enemiesCount; i++)
            {
                enemyInstance.enemiesInScene[i].GetComponent<EnemyAI>().RunEnemyAI();
                if (!enemyInstance.enemiesInScene[i].canMove)
                {
                    continue;
                }
                yield return new WaitForSeconds(1f);
            }
        }
        //Parar o turno.
        StopTurn();
    }

    //Coroutine do efeito da mudança de cartas e destruição delas.
    private IEnumerator ChangeTurnCards(CardBase card)
    {
        card.used = true;
        yield return new WaitForSeconds(1f);
        Destroy(card.gameObject);
    }

    // Método para ajustar as posições dinamicamente com base na quantidade de cartas possíveis.
    public Vector2[] GetAdjustedCardPositions(int cardCount, float[] rotations)
    {
        Vector2[] adjustedPositions = new Vector2[cardCount];

        float offset = 2f; // Espaçamento entre as cartas
        float startX = -(offset * (cardCount - 1)) / 2f; // Ponto inicial centralizado
        float baseY = cardPositions[0].y; // Valor base para a altura das cartas

        for (int i = 0; i < cardCount; i++)
        {
            // Ajusta a posição Y com base na rotação Z
            float yOffset = Mathf.Abs(rotations[i]) / 15f; // Quanto mais perto de -15 ou 15, menor o valor
            adjustedPositions[i] = new Vector2(startX + i * offset, baseY - yOffset); // Reduz a altura conforme a rotação se afasta do centro
        }

        return adjustedPositions;
    }

    // Método para calcular a rotação em arco das cartas
    public float[] GetCardRotations(int cardCount)
    {
        float[] rotations = new float[cardCount];

        // Se houver apenas uma carta, sua rotação deve ser zero.
        if (cardCount == 1)
        {
            rotations[0] = 0f;
            return rotations;
        }

        float startRotation = 15f; // Rotação para a primeira carta
        float endRotation = -15f;     // Rotação para a última carta

        for (int i = 0; i < cardCount; i++)
        {
            // Calcula a rotação interpolando entre o início (-15) e o fim (15)
            rotations[i] = Mathf.Lerp(startRotation, endRotation, (float)i / (cardCount - 1));
        }

        return rotations;
    }
}
