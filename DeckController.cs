using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeckController : MonoBehaviour
{
    public static DeckController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();

    public  List<CardScriptableObject> activeCards = new List<CardScriptableObject>();

    public Card cardToSpawn;

    public int drawCardCost = 2;

    public float waitBetweenDrawingCards = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        SetupDeck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetupDeck()
    {
        activeCards.Clear();

        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>();
        tempDeck.AddRange(deckToUse);

        int iterations = 0;
        while (tempDeck.Count > 0 && iterations < 500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);

            iterations++;
        }
    }

    public void DrawCardToHand()
    {
        if (activeCards.Count == 0)
        {
            SetupDeck();
        }

        Card newCard = Instantiate(cardToSpawn, transform.position, transform.rotation);
        newCard.cardSO = activeCards[0];
        newCard.SetupCard();
        
        activeCards.RemoveAt(0);
        
        HandController.Instance.AddCardToHand(newCard);
    }

    public void DrawCardForMana()
    {
        if (BattleController.Instance.playerMana >= drawCardCost)
        {
            DrawCardToHand();
            BattleController.Instance.SpendPlayerMana(drawCardCost);
        }
        else
        {
            UIController.Instance.ShowManaWarning();
            UIController.Instance.drawCardButton.SetActive(false);
        }
    }

    public void DrawMultipleCards(int amountToDraw)
    {
        StartCoroutine(DrawMultipleCoroutine(amountToDraw));
    }

    IEnumerator DrawMultipleCoroutine(int amountToDraw)
    {
        for (int i = 0; i < amountToDraw; i++)
        {
            DrawCardToHand();

            yield return new WaitForSeconds(waitBetweenDrawingCards);
        }
    }
}
