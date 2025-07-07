using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelNumberUI : MonoBehaviour
{
    public TMP_Text levelNumberText;
   
    public void UpdateLevel(int level)
    {
        levelNumberText.text = level.ToString();
    }
}
