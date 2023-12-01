using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiGameplay : UiMenu
{
    public TextMeshProUGUI currentAmmo;
    public TextMeshProUGUI maximumAmmo;
    public TextMeshProUGUI health;
    public Button restartButton;
    public GameObject deathScreen;
    public GameObject inGameplaySettings;
    public UiSettings uiSettings;
}