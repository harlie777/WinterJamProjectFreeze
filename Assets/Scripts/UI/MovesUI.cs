using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovesUI : MonoBehaviour
{
    public TMP_Text  movesText;

    void OnEnable()
    {
        PlayerController.OnMovesUpdated += UpdateUI;
    }

    void OnDisable()
    {
        PlayerController.OnMovesUpdated -= UpdateUI;
    }

    void UpdateUI(int movesLeft)
    {
        movesText.text = $"{movesLeft}";
    }
}
