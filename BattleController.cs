using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public static BattleController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public int startingMana = 4, maxMana = 12;
    public int playerMana;

    public int startingCardsAmount = 5;
    
    public enum TurnOrder
    {
        PlayerActive,
        PlayerCardAttacks,
        EnemyActive,
        EnemyCardAttacks
    }
    public TurnOrder currentPhase;
    
    // Start is called before the first frame update
    void Start()
    {
        playerMana = startingMana;
        UIController.Instance.SetPlayerManaText(playerMana);
        
        DeckController.Instance.DrawMultipleCards(startingCardsAmount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpendPlayerMana(int amountToSpend)
    {
        playerMana -= amountToSpend;

        if (playerMana < 0)
        {
            playerMana = 0;
        }
        
        UIController.Instance.SetPlayerManaText(playerMana);
    }

    public void AdvanceTurn()
    {
        currentPhase++;
        
        if ((int)currentPhase >= System.Enum.GetValues(typeof(TurnOrder)).Length)
        {
            currentPhase = 0;
        }

        switch (currentPhase)
        {
            case TurnOrder.PlayerActive:
                break;
            case TurnOrder.PlayerCardAttacks:
                AdvanceTurn();
                break;
            case TurnOrder.EnemyActive:
                AdvanceTurn();
                break;
            case TurnOrder.EnemyCardAttacks:
                AdvanceTurn();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
