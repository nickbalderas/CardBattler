using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    public int currentHealth, attackPower, manaCost;

    public TMP_Text healthText, attackText, costText;

    // Start is called before the first frame update
    void Start()
    {
        healthText = currentHelath;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
