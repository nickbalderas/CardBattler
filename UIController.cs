using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public TMP_Text playerManaText, playerHealthText, enemyHealthText;

    public GameObject manaWarning;
    public float manaWarningTime;
    private float _manaWarningCounter;

    public GameObject drawCardButton;

    public GameObject endTurnButton;

    public UIDamageIndicator playerDamage, enemyDamage;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_manaWarningCounter > 0)
        {
            _manaWarningCounter -= Time.deltaTime;

            if (_manaWarningCounter <= 0)
            {
                manaWarning.SetActive(false);
            }
        }
    }

    public void SetPlayerManaText(int manaAmount)
    {
        playerManaText.text = "Mana: " + manaAmount;
    }

    public void SetPlayerHealthText(int healthAmount)
    {
        playerHealthText.text = "Player Health: " + healthAmount;
    }
    
    public void SetEnemyHealthText(int healthAmount)
    {
        enemyHealthText.text = "Enemy Health: " + healthAmount;
    }

    public void ShowManaWarning()
    {
        manaWarning.SetActive(true);
        _manaWarningCounter = manaWarningTime;
    }

    public void DrawCard()
    {
        DeckController.Instance.DrawCardForMana();
    }

    public void EndPlayerTurn()
    {
        BattleController.Instance.EndPlayerTurn();
    }
}
