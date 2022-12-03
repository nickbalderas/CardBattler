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

        List<CardPlacePoint> cardPlacePoints = new List<CardPlacePoint>();
        cardPlacePoints.AddRange(CardPointsController.Instance.enemyCardPoints);

        int randomPoint = Random.Range(0, cardPlacePoints.Count);
        CardPlacePoint selectedPoint = cardPlacePoints[randomPoint];

        while (selectedPoint.activeCard != null && cardPlacePoints.Count > 0)
        {
            randomPoint = Random.Range(0, cardPlacePoints.Count);
            selectedPoint = cardPlacePoints[randomPoint];
            cardPlacePoints.RemoveAt(randomPoint);
        }

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

        yield return new WaitForSeconds(.5f);

        BattleController.Instance.AdvanceTurn();
    }
}
