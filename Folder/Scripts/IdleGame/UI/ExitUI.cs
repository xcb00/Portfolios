using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExitUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI stageTxt;
    [SerializeField] GameObject exitWindow;

    private void OnEnable()
    {
        exitWindow.SetActive(false);
        EventHandler.ChangeStageTextEvent += ChangeStage;
    }
    private void OnDisable() => EventHandler.ChangeStageTextEvent -= ChangeStage;

    public void ActiveExitWindow(bool active) => exitWindow.SetActive(active); //{ Debug.Log("Click Pause "+active); exitWindow.SetActive(active); }// => exitWindow.SetActive(active);

    void ChangeStage(int i) => stageTxt.SetText(System.String.Format("Stage {0}", i));

    public void OnPointerClick(PointerEventData eventData) => ActiveExitWindow(false);
}
