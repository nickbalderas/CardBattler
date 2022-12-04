using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public TMP_Text playerManaText, playerHealthText, enemyHealthText, enemyManaText;

    public GameObject manaWarning;
    public float manaWarningTime;
    private float _manaWarningCounter;

    public GameObject drawCardButton;

    public GameObject endTurnButton;

    public UIDamageIndicator playerDamage, enemyDamage;

    public GameObject battleEndScreen;
    public TMP_Text battleResultText;

    public string mainMenuScene, battleSelectScene;

    public GameObject pauseScreen;

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
    }

    public void SetPlayerManaText(int manaAmount)
    {
        playerManaText.text = "Mana: " + manaAmount;
    }
    
    public void SetEnemyManaText(int manaAmount)
    {
        enemyManaText.text = "Mana: " + manaAmount;
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

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void ChooseNewBattle()
    {
        SceneManager.LoadScene(battleSelectScene);
        Time.timeScale = 1f;
    }

    public void PauseUnpause()
    {
        pauseScreen.SetActive(!pauseScreen.activeSelf);
        Time.timeScale = pauseScreen.activeSelf ? 0f : 1f;
    }
}
