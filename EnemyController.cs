using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    private List<CardScriptableObject> _activeCards = new List<CardScriptableObject>();

    public Card cardToSpawn;
    public Transform cardSpawnPoint;
    
    public enum AIType
    {
        placeFromDeck,
        handRandomPlace,
        handDefensive,
        handAggressive
    }
    public AIType enemyAIType;

    private List<CardScriptableObject> _cardsInHand = new List<CardScriptableObject>();
    public int startHandSize;

    // Start is called before the first frame update
    void Start()
    {
        SetupDeck();

        if (enemyAIType != AIType.placeFromDeck)
        {
            SetupHand();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void SetupDeck()
    {
        _activeCards.Clear();

        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>();
        tempDeck.AddRange(deckToUse);

        int iterations = 0;
        while (tempDeck.Count > 0 && iterations < 500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            _activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);

            iterations++;
        }
    }
    
    public void StartAction()
    {
        StartCoroutine(EnemyActionCoroutine());
    }

    IEnumerator EnemyActionCoroutine()
    {
        if (_activeCards.Count == 0)
        {
            SetupDeck();
        }
        
        yield return new WaitForSeconds(.5f);

        if (enemyAIType != AIType.placeFromDeck)
        {
            for (int i = 0; i < BattleController.Instance.cardsToDrawPerTurn; i++)
            {
                _cardsInHand.Add(_activeCards[0]);
                _activeCards.RemoveAt(0);

                if (_activeCards.Count == 0)
                {
                    SetupDeck();
                }
            }
        }

        List<CardPlacePoint> cardPlacePoints = new List<CardPlacePoint>();
        cardPlacePoints.AddRange(CardPointsController.Instance.enemyCardPoints);

        int randomPoint = Random.Range(0, cardPlacePoints.Count);
        CardPlacePoint selectedPoint = cardPlacePoints[randomPoint];

        if (enemyAIType == AIType.placeFromDeck || enemyAIType == AIType.handRandomPlace)
        {
            cardPlacePoints.Remove(selectedPoint);
            
            while (selectedPoint.activeCard != null && cardPlacePoints.Count > 0)
            {
                randomPoint = Random.Range(0, cardPlacePoints.Count);
                selectedPoint = cardPlacePoints[randomPoint];
                cardPlacePoints.RemoveAt(randomPoint);
            }
        }

        CardScriptableObject selectedCard = null;
        int iterations = 0;
        List<CardPlacePoint> preferredPoints = new List<CardPlacePoint>();
        List<CardPlacePoint> secondaryPoints = new List<CardPlacePoint>();

        switch (enemyAIType)
        {
            case AIType.placeFromDeck:
                if (selectedPoint.activeCard == null)
                {
                    Card newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);
                    newCard.cardSO = _activeCards[0];
                    _activeCards.RemoveAt(0);
                    newCard.SetupCard();
                    newCard.MoveToPoint(selectedPoint.transform.position, selectedPoint.transform.rotation);

                    selectedPoint.activeCard = newCard;
                    newCard.assignedPlace = selectedPoint;
                }
                break;
            case AIType.handRandomPlace:
                selectedCard = SelectedCardToPlay();

                iterations = 50;
                while (selectedCard != null && iterations > 0 && selectedPoint.activeCard == null)
                {
                    PlayCard(selectedCard, selectedPoint);

                    // check if we should try to play another card
                    selectedCard = SelectedCardToPlay();
                    
                    iterations--;

                    yield return new WaitForSeconds(CardPointsController.Instance.timeBetweenAttack);
                    
                    while (selectedPoint.activeCard != null && cardPlacePoints.Count > 0)
                    {
                        randomPoint = Random.Range(0, cardPlacePoints.Count);
                        selectedPoint = cardPlacePoints[randomPoint];
                        cardPlacePoints.RemoveAt(randomPoint);
                    }
                }
                break;
            case AIType.handDefensive:
                selectedCard = SelectedCardToPlay();
                
                preferredPoints.Clear();
                secondaryPoints.Clear();

                for (int i = 0; i < cardPlacePoints.Count; i++)
                {
                    if (cardPlacePoints[i].activeCard == null)
                    {
                        if (CardPointsController.Instance.playerCardPoints[i].activeCard != null)
                        {
                            preferredPoints.Add(cardPlacePoints[i]);
                        }
                        else
                        {
                            secondaryPoints.Add(cardPlacePoints[i]);
                        }
                    }
                }

                iterations = 50;
                while (selectedCard != null && iterations > 0 && preferredPoints.Count + secondaryPoints.Count > 0)
                {
                    // pick a point to use
                    if (preferredPoints.Count > 0)
                    {
                        int selectPoint = Random.Range(0, preferredPoints.Count);
                        selectedPoint = preferredPoints[selectPoint];
                        
                        preferredPoints.RemoveAt(selectPoint);
                    }
                    else
                    {
                        int selectPoint = Random.Range(0, secondaryPoints.Count);
                        selectedPoint = secondaryPoints[selectPoint];
                        
                        secondaryPoints.RemoveAt(selectPoint);
                    }
                    
                    PlayCard(selectedCard, selectedPoint);
                    
                    // check if we should try and play another card

                    selectedCard = SelectedCardToPlay();

                    iterations--;
                    yield return new WaitForSeconds(CardPointsController.Instance.timeBetweenAttack);
                }
                
                break;
            case AIType.handAggressive:
                selectedCard = SelectedCardToPlay();
                
                preferredPoints.Clear();
                secondaryPoints.Clear();

                for (int i = 0; i < cardPlacePoints.Count; i++)
                {
                    if (cardPlacePoints[i].activeCard == null)
                    {
                        if (CardPointsController.Instance.playerCardPoints[i].activeCard == null)
                        {
                            preferredPoints.Add(cardPlacePoints[i]);
                        }
                        else
                        {
                            secondaryPoints.Add(cardPlacePoints[i]);
                        }
                    }
                }

                iterations = 50;
                while (selectedCard != null && iterations > 0 && preferredPoints.Count + secondaryPoints.Count > 0)
                {
                    // pick a point to use
                    if (preferredPoints.Count > 0)
                    {
                        int selectPoint = Random.Range(0, preferredPoints.Count);
                        selectedPoint = preferredPoints[selectPoint];
                        
                        preferredPoints.RemoveAt(selectPoint);
                    }
                    else
                    {
                        int selectPoint = Random.Range(0, secondaryPoints.Count);
                        selectedPoint = secondaryPoints[selectPoint];
                        
                        secondaryPoints.RemoveAt(selectPoint);
                    }
                    
                    PlayCard(selectedCard, selectedPoint);
                    
                    // check if we should try and play another card

                    selectedCard = SelectedCardToPlay();

                    iterations--;
                    yield return new WaitForSeconds(CardPointsController.Instance.timeBetweenAttack);
                }
                break;
        }
        yield return new WaitForSeconds(.5f);

        BattleController.Instance.AdvanceTurn();
    }

    private void SetupHand()
    {
        for (int i = 0; i < startHandSize; i++)
        {
            if (_activeCards.Count == 0)
            {
                SetupDeck();
            }
            
            _cardsInHand.Add(_activeCards[0]);
            _activeCards.RemoveAt(0);
        }
    }

    public void PlayCard(CardScriptableObject cardScriptableObject, CardPlacePoint cardPlacePoint)
    {
        Card newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);
        newCard.cardSO = cardScriptableObject;
        
        newCard.SetupCard();
        newCard.MoveToPoint(cardPlacePoint.transform.position, cardPlacePoint.transform.rotation);

        cardPlacePoint.activeCard = newCard;
        newCard.assignedPlace = cardPlacePoint;

        _cardsInHand.Remove(cardScriptableObject);
        
        BattleController.Instance.SpendEnemyMana(cardScriptableObject.manaCost);
    }

    private CardScriptableObject SelectedCardToPlay()
    {
        CardScriptableObject cardToPlay = null;

        List<CardScriptableObject> cardsToPlay = new List<CardScriptableObject>();
        foreach (var card in _cardsInHand)
        {
            if (card.manaCost <= BattleController.Instance.enemyMana)
            {
                cardsToPlay.Add(card);
            }

            if (cardsToPlay.Count > 0)
            {
                int selected = Random.Range(0, cardsToPlay.Count);
                cardToPlay = cardsToPlay[selected];
            }
        }

        return cardToPlay;
    }
}
