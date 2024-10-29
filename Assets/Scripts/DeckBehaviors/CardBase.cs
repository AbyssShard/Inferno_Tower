using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBase : MonoBehaviour
{
    GameplayManager gameplayManager;
    CardDeckManager cardManager;

    [Header("CardPreferences")]
    public int cardCoust;

    [HideInInspector] public bool selected, used;

    [HideInInspector] public List<CardBase> selectedCards;
    [HideInInspector] public Vector3 originalPos;

    public SpriteRenderer cardCoustRenderer;
    private Sprite[] numbers;

    public bool endAction;
    bool MouseIsAboveMe()
    {
        // Verifica se há colisão na posição do cursor.
        Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldMousePos, Vector2.zero);
        GameObject cursorObject = hit.collider != null ? hit.collider.gameObject : null;

        // Retorna o valor caso o mouse esteja acima do objeto.
        return cursorObject == this.gameObject ? true : false;
    }

    private void Start()
    {
        gameplayManager = FindObjectOfType<GameplayManager>();
        cardManager = FindObjectOfType<CardDeckManager>();

        used = false;

        numbers = cardManager.numbers;
        cardCoustRenderer.sprite = numbers[cardCoust - 1];

        selectedCards = cardManager.selectedCards;
    }

    private void Update()
    {     
        ManageSelection();
        ManagePosition();
    }

    private void ManageSelection()
    {
        //Alternar entre selecionado ou não caso o jogador clique encima da carta.
        if(MouseIsAboveMe() && Input.GetMouseButtonDown(0) && !gameplayManager.startTurn)
        {
            selected = !selected;
        }

        if (selected)
        {
            if (!selectedCards.Contains(this))
            {
                selectedCards.Add(this);
            }
        }
        else
        {
            selectedCards.Remove(this);
        }
    }

    private void ManagePosition()
    {
        //Mudara a posição da carta caso o mouse esteja acima ou ela esteja selecionada ou ao terminar um turno.
        Vector3 goPos;
        if (!used)
        {
            if (!selected)
                goPos = MouseIsAboveMe() ? originalPos + Vector3.up * 0.8f : originalPos;
            else
                goPos = originalPos - Vector3.up * 0.9f;
        }
        else
            goPos = originalPos - Vector3.up * 4f;

        transform.position = Vector3.Lerp((Vector2)transform.position,Camera.main.transform.position + goPos, Time.deltaTime * 5f);
    }
}

interface CardAction
{
    void Action();
}
