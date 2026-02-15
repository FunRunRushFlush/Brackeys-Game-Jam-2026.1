using UnityEngine;

using UnityEngine;

internal class RewardPopUpCanvasUI : MonoBehaviour
{
    [SerializeField] private GameObject RewardPopUpCanvas;

    public void ShowRewardPopUpCanvas()
    {
        RewardPopUpCanvas.SetActive(true);
    }

    public void CloseRewardPopUpCanvas()
    {
        RewardPopUpCanvas.SetActive(false);
    }
}
