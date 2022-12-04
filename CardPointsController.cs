using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPointsController : MonoBehaviour
{
    public static CardPointsController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public CardPlacePoint[] playerCardPoints, enemyCardPoints;

    public float timeBetweenAttack = .25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerAttack()
    {
        StartCoroutine(PlayerAttackCoroutine());
    }

    IEnumerator PlayerAttackCoroutine()
    {
        yield return new WaitForSeconds(timeBetweenAttack);

        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            if (playerCardPoints[i].activeCard != null)
            {
                if (enemyCardPoints[i].activeCard != null)
                {
                    // Attack the enemy card
                    enemyCardPoints[i].activeCard.DamageCard(playerCardPoints[i].activeCard.attackPower);
                }
                else
                {
                    BattleController.Instance.DamageEnemy(playerCardPoints[i].activeCard.attackPower);
                }
                playerCardPoints[i].activeCard.animator.SetTrigger("Attack");

                yield return new WaitForSeconds(timeBetweenAttack);
            }
            
            // Breaks out of loop if game has ended
            if (BattleController.Instance.hasBattleEnded) i = playerCardPoints.Length;
        }
        
        CheckAssignedCards();
        
        BattleController.Instance.AdvanceTurn();
    }

    public void EnemyAttack()
    {
        StartCoroutine(EnemyAttackCoroutine());
    }

    IEnumerator EnemyAttackCoroutine()
    {
        yield return new WaitForSeconds(timeBetweenAttack);

        for (int i = 0; i < enemyCardPoints.Length; i++)
        {
            if (enemyCardPoints[i].activeCard != null)
            {
                if (playerCardPoints[i].activeCard != null)
                {
                    // Attack the player card
                    playerCardPoints[i].activeCard.DamageCard(enemyCardPoints[i].activeCard.attackPower);
                }
                else
                {
                    BattleController.Instance.DamagePlayer(enemyCardPoints[i].activeCard.attackPower);
                }
                enemyCardPoints[i].activeCard.animator.SetTrigger("Attack");
                
                yield return new WaitForSeconds(timeBetweenAttack);
            }

            // Breaks out of loop if game has ended
            if (BattleController.Instance.hasBattleEnded) i = enemyCardPoints.Length;
        }
        
        CheckAssignedCards();
        
        BattleController.Instance.AdvanceTurn();
    }

    public void CheckAssignedCards()
    {
        foreach (var playerCardPoint in playerCardPoints)
        {
            if (playerCardPoint.activeCard != null)
            {
                if (playerCardPoint.activeCard.currentHealth <= 0)
                {
                    playerCardPoint.activeCard = null;
                }
            }
        }
        foreach (var enemyCardPoint in enemyCardPoints)
        {
            if (enemyCardPoint.activeCard != null)
            {
                if (enemyCardPoint.activeCard.currentHealth <= 0)
                {
                    enemyCardPoint.activeCard = null;
                }
            }
        }
    }
}
