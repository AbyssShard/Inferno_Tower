using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CardDeckManager : MonoBehaviour
{
    [Header("EssentialComponents")]

    public GameplayManager gameplayManager;
    [Space(3)]

    [Header("CardsByLevel")]

    public CardsToAdd[] possibleCards;
    [Space(3)]

    [Header("CardVisualConfig")]

    public Sprite[] numbers;

    [HideInInspector] public List<CardBase> selectedCards = new List<CardBase>();

    public void DeckManagerStart()
    {
        CreateNewCards();//Criar novas cartas no inicio do jogo
    }

    //Variavel que verifica se existem cartas no baralho
    bool AllChanceWeightsAreZero()
    {
        foreach (CardsToAdd cardsToAdd in possibleCards)
        {
            foreach (PossibleCards card in cardsToAdd.possibleCards)
            {
                if (card.chanceWeight > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    //Metodo que reseta o baralho caso não existam mais cartas nele
    private void ResetWeights()
    {
        if (AllChanceWeightsAreZero())
        {
            foreach (PossibleCards cards in possibleCards[gameplayManager.level].possibleCards)
            {
                cards.chanceWeight = cards.initialChanceWeight;
            }
        }
    }

    //Metodo de cirar novas cartas
    public void CreateNewCards()
    {
        ResetWeights();//Verificar se existem cartas no baralho

        List<PossibleCards> availableCards = GetAvailableCards();//Lista que recebera a quantidade de cada carta no baralho para ser sorteada
        int cardCount = availableCards.Count;//Quantidade de cartas no baralho

        //Caso não seja possivel fazer umas instancia, sera avisado
        if (cardCount == 0)
        {
            Debug.LogWarning("Nenhuma carta disponível para criação.");
            return; // Não cria cartas se não houver cartas disponíveis
        }

        // Ajusta as rotações das cartas de acordo com a quantidade de cartas disponíveis
        float[] rotations = gameplayManager.GetCardRotations(cardCount);

        // Ajusta as posições das cartas de acordo com as rotações
        Vector2[] adjustedPositions = gameplayManager.GetAdjustedCardPositions(cardCount, rotations);

        // Criar novas cartas nas posições e rotações ajustadas
        for (int i = 0; i < cardCount; i++)
        {
            Vector2 cardPos = adjustedPositions[i];
            GameObject selectedCard = GetRandomCard(availableCards);

            if (selectedCard != null)
            {
                // A posição Z deve ser 0, pois estamos lidando com 2D
                CardBase newCard = Instantiate(selectedCard, cardPos - Vector2.up * 10f, Quaternion.Euler(0, 0, rotations[i]), Camera.main.transform).GetComponent<CardBase>();
                newCard.originalPos = cardPos;
            }
        }
    }

    // Função para obter as cartas disponíveis com chanceWeight > 0
    List<PossibleCards> GetAvailableCards()
    {
        List<PossibleCards> availableCards = new List<PossibleCards>();

        foreach (PossibleCards card in possibleCards[gameplayManager.level].possibleCards)
        {
            if (card.chanceWeight > 0)
            {
                availableCards.Add(card);
            }
        }

        return availableCards;
    }

    GameObject GetRandomCard(List<PossibleCards> availableCards)
    {
        if (availableCards.Count == 0)
            return null;

        int totalWeight = 0;
        foreach (PossibleCards card in availableCards)
        {
            totalWeight += card.chanceWeight;
        }

        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (PossibleCards card in availableCards)
        {
            currentWeight += card.chanceWeight;

            if (randomWeight < currentWeight)
            {
                card.chanceWeight--; // Subtrai 1 do chanceWeight da carta selecionada
                return card.card;
            }
        }
        return null;
    }
}


[System.Serializable]
public class PossibleCards
{
    public GameObject card;
    public int chanceWeight;
    public int initialChanceWeight;
}

[System.Serializable]
public class CardsToAdd
{
    public PossibleCards[] possibleCards;
}
