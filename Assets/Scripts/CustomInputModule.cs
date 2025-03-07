using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CustomInputModule : StandaloneInputModule
{
    public LayerMask uiLayerMask;

    protected override void ProcessDrag(PointerEventData pointerEvent)
    {
        var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

        // Process drag only if over the correct layer
        if (currentOverGo != null && ((1 << currentOverGo.layer) & uiLayerMask) != 0)
        {
            base.ProcessDrag(pointerEvent);
        }
    }

    public override void Process()
    {
        base.Process();

        // Filter raycast results
        foreach (var pointerEvent in m_PointerData.Values)
        {
            if (pointerEvent.pointerCurrentRaycast.gameObject != null)
            {
                var filteredRaycasts = FilterRaycasts(pointerEvent.pointerCurrentRaycast);
                if (filteredRaycasts.Count > 0)
                {
                    pointerEvent.pointerCurrentRaycast = FindFirstRaycast(filteredRaycasts);
                }
                else
                {
                    // Reset the raycast if no valid results are found
                    pointerEvent.pointerCurrentRaycast = new RaycastResult();
                }
            }
            else
            {
                // Reset the raycast if gameObject is null
                pointerEvent.pointerCurrentRaycast = new RaycastResult();
            }
        }
    }

    private List<RaycastResult> FilterRaycasts(RaycastResult raycast)
    {
        List<RaycastResult> filteredResults = new List<RaycastResult>();

        if (raycast.gameObject != null)
        {
            if (((1 << raycast.gameObject.layer) & uiLayerMask.value) != 0)
            {
                filteredResults.Add(raycast);
            }
        }

        return filteredResults;
    }
}
