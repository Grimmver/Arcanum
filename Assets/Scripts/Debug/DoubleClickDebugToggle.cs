using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DoubleClickDebugToggle : MonoBehaviour, IPointerClickHandler
{
    public GameObject targetToToggle;

    [SerializeField] private MatchManager mbg;

    public float maxInterval = 0.3f;

    private float lastClickTime = -1f;

    public Vector3 offset;

    public bool currState;

    public void OnPointerClick(PointerEventData eventData)
    {
        float currClickTime = Time.time;
        if (currClickTime - lastClickTime <= maxInterval)
        {
            ToggleTarget();
        }
        lastClickTime = currClickTime;
    }

    private void ToggleTarget()
    {
        if (targetToToggle == null)
            return;

        RectTransform pos = targetToToggle.GetComponent<RectTransform>();

        if (pos == null)
        {
            return;
        }

        if (currState)
        {
            pos.localPosition += offset;
            mbg.debugOverlayCount --;
        }
        else
        {
            pos.localPosition -= offset;
            mbg.debugOverlayCount ++;
        }

        currState = !currState;

        if (mbg.debugOverlayCount == 0)
        {
            mbg.PrecalcRandomWeights();
        }
    }
}
