using UnityEngine;
using UnityEngine.UI;

public class UiPhone : MonoBehaviour
{
    [SerializeField] private Button shoot;
    [SerializeField] private Button reload;
    [SerializeField] private Button crouch;
    [SerializeField] private Button jump;
    [SerializeField] private Button settings;
    [SerializeField] private Button sprint;

    public UiButtonLogic[] weapons;


    public UiButtonLogic Shoot => shoot.GetComponent<UiButtonLogic>();
    public UiButtonLogic Reload => reload.GetComponent<UiButtonLogic>();
    public UiButtonLogic Crouch => crouch.GetComponent<UiButtonLogic>();
    public UiButtonLogic Jump => jump.GetComponent<UiButtonLogic>();
    public UiButtonLogic Settings => settings.GetComponent<UiButtonLogic>();
    public UiButtonLogic Sprint => sprint.GetComponent<UiButtonLogic>();
}