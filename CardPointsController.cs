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
            if (!playerCardPoints[i].activeCard)
            {
                if (!enemyCardPoints[i].activeCard)
                {
                    // Attack the enemy card
                }
                else
                {
                    // Attack the enemy's overall health
                }

                yield return new WaitForSeconds(timeBetweenAttack);
            }
        }
        
        BattleController.Instance.AdvanceTurn();
    }
}
