using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarStatusController : MonoBehaviour
{
    public StatusController statusControllerPrefab;
    public Transform statusStartPosition;
    public float statusOffset;
    public float textScale;

    NeederController star;
    List<StatusController> statusList;

    public void Spawn()
    {
        star = GetComponent<NeederController>();

        Debug.Log("Got " + star + " for star");

        statusList = new List<StatusController>();

        // setup the main fulfilled/decay timer text
        StatusController starStatusController = Instantiate(statusControllerPrefab.gameObject, statusStartPosition).GetComponent<StatusController>();
        starStatusController.transform.localScale = new Vector3(textScale, textScale, textScale);
        starStatusController.SetAlignment(TextAnchor.MiddleLeft, TextAlignment.Left);
        statusList.Add(starStatusController);
        
        // start this offset at 1 so we move the text down
        int statuses = 1;

        foreach (var need in star.needs.Values)
        {
            statuses++;
            GameObject statusController = Instantiate(statusControllerPrefab.gameObject, statusStartPosition);
            statusController.transform.localScale = new Vector3(textScale, textScale, textScale);
            statusController.transform.Translate(Vector2.down * (statusOffset * statuses));

            StatusController status = statusController.GetComponent<StatusController>();
            status.SetAlignment(TextAnchor.MiddleLeft, TextAlignment.Left);
            statusList.Add(status);
        }
    }

    public void Die()
    {
        foreach(var status in statusList)
        {
            status.Die();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (star.IsComplete())
        {
            statusList[0].SetText("Fulfilled!");
        } 
        else
        {
            statusList[0].SetText($"Decay: {(int)(star.timePerDecay - star.decayTimer)}s");
        }

        int i = 1;
        foreach (var need in star.needs.Values)
        {
            StatusController status = statusList[i];
            status.SetColor(need.name.MaterialColor());
            status.SetText($"{need.name}: {string.Format("{0:P2}", need.Percent())}");
            i++;
        }
    }
}
