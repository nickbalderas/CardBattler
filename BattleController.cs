using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BattleController : MonoBehaviour
{
    public static BattleController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public int startingMana = 4, maxMana = 12;
    public int playerMana, enemyMana;
    private int _currentPlayerMaxMana, _currentEnemyMaxMana;

    public int startingCardsAmount = 5;
    public int cardsToDrawPerTurn = 2;

    [Range(0f, 1f)]
    public float playerFirstChance = 0.5f;
    
    public enum TurnOrder
    {
        PlayerActive,
        PlayerCardAttacks,
        EnemyActive,
        EnemyCardAttacks
    }
    public TurnOrder currentPhase;

    public Transform discardPoint;

    public int playerHealth, enemyHealth;

    public bool hasBattleEnded;

    public float resultScreenDelayTime = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        _currentPlayerMaxMana = startingMana;
        _currentEnemyMaxMana = startingMana;
        FillPlayerMana();
        FillEnemyMana();
        
        DeckController.Instance.DrawMultipleCards(startingCardsAmount);
        
        UIController.Instance.SetPlayerHealthText(playerHealth);
        UIController.Instance.SetEnemyHealthText(enemyHealth);

        if (!(Random.value > playerFirstChance)) return;
        currentPhase = TurnOrder.PlayerCardAttacks;
        AdvanceTurn();
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

    public void FillPlayerMana()
    {
        playerMana = _currentPlayerMaxMana;
        UIController.Instance.SetPlayerManaText(playerMana);
    }
    
    public void SpendEnemyMana(int amountToSpend)
    {
        enemyMana -= amountToSpend;

        if (enemyMana < 0)
        {
            enemyMana = 0;
        }
        
        UIController.Instance.SetEnemyManaText(enemyMana);
    }

    public void FillEnemyMana()
    {
        enemyMana = _currentEnemyMaxMana;
        UIController.Instance.SetEnemyManaText(enemyMana);
    }

    public void AdvanceTurn()
    {
        if (hasBattleEnded) return;
        
        currentPhase++;
        
        if ((int)currentPhase >= System.Enum.GetValues(typeof(TurnOrder)).Length)
        {
            currentPhase = 0;
        }

        switch (currentPhase)
        {
            case TurnOrder.PlayerActive:
                UIController.Instance.endTurnButton.SetActive(true);
                UIController.Instance.drawCardButton.SetActive(true);

                if (_currentPlayerMaxMana < maxMana)
                {
                    _currentPlayerMaxMana++;
                }
                
                FillPlayerMana();
                
                DeckController.Instance.DrawMultipleCards(cardsToDrawPerTurn);
                
                break;
            case TurnOrder.PlayerCardAttacks:
                CardPointsController.Instance.PlayerAttack();
                break;
            case TurnOrder.EnemyActive:
                if (_currentEnemyMaxMana < maxMana)
                {
                    _currentEnemyMaxMana++;
                }
                
                FillEnemyMana();
                
                EnemyController.Instance.StartAction();
                break;
            case TurnOrder.EnemyCardAttacks:
                CardPointsController.Instance.EnemyAttack();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void EndPlayerTurn()
    {
        UIController.Instance.endTurnButton.SetActive(false);
        UIController.Instance.drawCardButton.SetActive(false);
        
        AdvanceTurn();
    }

    public void DamagePlayer(int damageAmount)
    {
        if (playerHealth > 0 || !hasBattleEnded)
        {
            playerHealth -= damageAmount;

            if (playerHealth <= 0)
            {
                playerHealth = 0;
                
                EndBattle();
            }
            
            UIController.Instance.SetPlayerHealthText(playerHealth);

            UIDamageIndicator damageClone = Instantiate(UIController.Instance.playerDamage, UIController.Instance.playerDamage.transform.parent);
            damageClone.damageText.text = damageAmount.ToString();
            damageClone.gameObject.SetActive(true);
        }
    }
    
    public void DamageEnemy(int damageAmount)
    {
        if (enemyHealth > 0 || !hasBattleEnded)
        {
            enemyHealth -= damageAmount;

            if (enemyHealth <= 0)
            {
                enemyHealth = 0;
                
                EndBattle();
            }
            
            UIController.Instance.SetEnemyHealthText(enemyHealth);
            
            UIDamageIndicator damageClone = Instantiate(UIController.Instance.enemyDamage, UIController.Instance.enemyDamage.transform.parent);
            damageClone.damageText.text = damageAmount.ToString();
            damageClone.gameObject.SetActive(true);
        }
    }

    private void EndBattle()
    {
        hasBattleEnded = true;
        
        HandController.Instance.EmptyHand();

        if (enemyHealth <= 0)
        {
            UIController.Instance.battleResultText.text = "VICTORY!";

            foreach (var enemyCardPoint in CardPointsController.Instance.enemyCardPoints)
            {
                if (enemyCardPoint.activeCard != null)
                {
                    enemyCardPoint.activeCard.MoveToPoint(discardPoint.position, enemyCardPoint.activeCard.transform.rotation);
                }
            }
        }
        else
        {
            UIController.Instance.battleResultText.text = "DEFEATED!";
            
            foreach (var playerCardPoint in CardPointsController.Instance.playerCardPoints)
            {
                if (playerCardPoint.activeCard != null)
                {
                    playerCardPoint.activeCard.MoveToPoint(discardPoint.position, playerCardPoint.activeCard.transform.rotation);
                }
            }
        }
        
        StartCoroutine(ShowResultsCoroutine());
    }

    IEnumerator ShowResultsCoroutine()
    {
        yield return new WaitForSeconds(resultScreenDelayTime);
        
        UIController.Instance.battleEndScreen.SetActive(true);
    }
}
