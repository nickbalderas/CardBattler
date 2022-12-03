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
        
        
        BattleController.Instance.AdvanceTurn();
    }
}
