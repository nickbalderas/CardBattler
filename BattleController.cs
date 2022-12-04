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
    public int playerMana, enemyMana;
    private int _currentPlayerMaxMana, _currentEnemyMaxMana;

    public int startingCardsAmount = 5;
    public int cardsToDrawPerTurn = 2;
    
    public enum TurnOrder
    {
        PlayerActive,
        PlayerCardAttacks,
        EnemyActive,
        EnemyCardAttacks
    }
    public TurnOrder currentPhase;

    public Transform discardPoint;

    public int playerHealth;
    public int enemyHealth;
    
    // Start is called before the first frame update
    void Start()
    {
        _currentPlayerMaxMana = startingMana;
        FillPlayerMana();
        
        DeckController.Instance.DrawMultipleCards(startingCardsAmount);
        
        UIController.Instance.SetPlayerHealthText(playerHealth);
        UIController.Instance.SetEnemyHealthText(enemyHealth);
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
        if (playerHealth > 0 )
        {
            playerHealth -= damageAmount;

            if (playerHealth <= 0)
            {
                playerHealth = 0;
                
                // End Battle
            }
            
            UIController.Instance.SetPlayerHealthText(playerHealth);

            UIDamageIndicator damageClone = Instantiate(UIController.Instance.playerDamage, UIController.Instance.playerDamage.transform.parent);
            damageClone.damageText.text = damageAmount.ToString();
            damageClone.gameObject.SetActive(true);
        }
    }
    
    public void DamageEnemy(int damageAmount)
    {
        if (enemyHealth > 0 )
        {
            enemyHealth -= damageAmount;

            if (enemyHealth <= 0)
            {
                enemyHealth = 0;
                
                // End Battle
            }
            
            UIController.Instance.SetEnemyHealthText(enemyHealth);
            
            UIDamageIndicator damageClone = Instantiate(UIController.Instance.enemyDamage, UIController.Instance.enemyDamage.transform.parent);
            damageClone.damageText.text = damageAmount.ToString();
            damageClone.gameObject.SetActive(true);
        }
    }
}
