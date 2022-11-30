using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardScriptableObject cardSO;
    
    public int currentHealth, attackPower, manaCost;

    public TMP_Text healthText, attackText, costText, nameText, actionDescription, loreText;

    public Image characterArt, bgArt;

    private Vector3 _targetPoint;
    private Quaternion _targetRotation;
    public float moveSpeed = 5f, rotateSpeed = 540f;

    // Start is called before the first frame update
    void Start()
    {
        SetupCard();
    }

    private void SetupCard()
    {
        currentHealth = cardSO.currentHealth;
        attackPower = cardSO.attackPower;
        manaCost = cardSO.manaCost;
        
        healthText.text = currentHealth.ToString();
        attackText.text = attackPower.ToString();
        costText.text = manaCost.ToString();

        nameText.text = cardSO.cardName;
        actionDescription.text = cardSO.actionDescription;
        loreText.text = cardSO.cardLore;

        characterArt.sprite = cardSO.characterSprite;
        bgArt.sprite = cardSO.bgSprite;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _targetPoint, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, rotateSpeed * Time.deltaTime);
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotationToMatch)
    {
        _targetPoint = pointToMoveTo;
        _targetRotation = rotationToMatch;
    }
}
