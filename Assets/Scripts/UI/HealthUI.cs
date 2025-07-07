using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public List<Image> lives;
    public void UpdateLives(int life)
    {
        lives[life].enabled = false;
    }
}
