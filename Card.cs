using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

public class Card : MonoBehaviour
{
    public CardScriptableObject cardSO;

    public bool isPlayer;
    
    public int currentHealth, attackPower, manaCost;

    public TMP_Text healthText, attackText, costText, nameText, actionDescription, loreText;

    public Image characterArt, bgArt;

    private Vector3 _targetPoint;
    private Quaternion _targetRotation;
    public float moveSpeed = 5f, rotateSpeed = 540f;

    public bool inHand;
    public int handPosition;

    private HandController _handController;

    private bool _isSelected;
    private Collider _theCollider;

    public LayerMask whatIsDesktop, whatIsPlacement;
    private bool _justPressed;

    public CardPlacePoint assignedPlace;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        if (_targetPoint == Vector3.zero)
        {
            _targetPoint = transform.position;
            _targetRotation = transform.rotation;
        }
        
        SetupCard();

        _handController = FindObjectOfType<HandController>();
        _theCollider = GetComponent<Collider>();
    }

    public void SetupCard()
    {
        currentHealth = cardSO.currentHealth;
        attackPower = cardSO.attackPower;
        manaCost = cardSO.manaCost;
        
        UpdateCardDisplay();

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

        if (_isSelected)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 100f, whatIsDesktop))
            {
                MoveToPoint(hit.point + new Vector3(0f,2f,0f), Quaternion.identity);
            }

            if (Input.GetMouseButtonDown(1) && !BattleController.Instance.hasBattleEnded)
            {
                ReturnToHand();
            }

            if (Input.GetMouseButtonDown(0) && !_justPressed && !BattleController.Instance.hasBattleEnded)
            {
                if (Physics.Raycast(ray, out hit, 100f, whatIsPlacement) && BattleController.Instance.currentPhase == BattleController.TurnOrder.PlayerActive)
                {
                    CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();

                    if (!selectedPoint.activeCard && selectedPoint.isPlayerPoint)
                    {
                        if (BattleController.Instance.playerMana >= manaCost)
                        {
                            selectedPoint.activeCard = this;
                            assignedPlace = selectedPoint;
                            MoveToPoint(selectedPoint.transform.position, Quaternion.identity);
                        
                            inHand = false;
                            _isSelected = false;
                        
                            _handController.RemoveCardFromHand(this);
                            
                            BattleController.Instance.SpendPlayerMana(manaCost);
                        }
                        else
                        {
                            ReturnToHand();
                            
                            UIController.Instance.ShowManaWarning();
                        }
                    } else ReturnToHand();
                } else ReturnToHand();
            }
        }
        _justPressed = false;
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotationToMatch)
    {
        _targetPoint = pointToMoveTo;
        _targetRotation = rotationToMatch;
    }
    
    private void OnMouseOver()
    {
        if (inHand && isPlayer && !BattleController.Instance.hasBattleEnded)
        {
            MoveToPoint(_handController.cardPositions[handPosition] + new Vector3(0f, 1f, .5f), Quaternion.identity);
        }
    }

    private void OnMouseExit()
    {
        if (inHand && isPlayer && !BattleController.Instance.hasBattleEnded)
        {
            MoveToPoint(_handController.cardPositions[handPosition], _handController.minPos.rotation);
        }
    }

    private void OnMouseDown()
    {
        if (!inHand && BattleController.Instance.currentPhase != BattleController.TurnOrder.PlayerActive && !isPlayer && !BattleController.Instance.hasBattleEnded) return;
        _isSelected = true;
        _theCollider.enabled = false;
        _justPressed = true;
    }

    private void ReturnToHand()
    {
        _isSelected = false;
        _theCollider.enabled = true;
        
        MoveToPoint(_handController.cardPositions[handPosition], _handController.minPos.rotation);
    }

    public void DamageCard(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;

            assignedPlace.activeCard = null;
            
            MoveToPoint(BattleController.Instance.discardPoint.position, BattleController.Instance.discardPoint.rotation);
            
            animator.SetTrigger("Jump");
            
            Destroy(gameObject, 5f);
        }
        
        animator.SetTrigger("Hurt");
        
        UpdateCardDisplay();
    }

    public void UpdateCardDisplay()
    {
        healthText.text = currentHealth.ToString();
        attackText.text = attackPower.ToString();
        costText.text = manaCost.ToString();
    }
}
