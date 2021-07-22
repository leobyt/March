using UnityEngine;
using System.Collections.Generic;

public class SwitchManager : MonoBehaviour
{
    public GameObject battery;
    public Switch[] switches;

    public void SpawnBattery()
    {
        var list = GetEmptySwitches();
        int random = Random.Range(0, list.Length);
        battery.transform.position = list[random].transform.position;
    }

    public void HideBattery()
    {
        battery.transform.position = new Vector3(0, -5f, 0);
    }

    Switch[] GetEmptySwitches()
    {
        List<Switch> list = new List<Switch>();

        foreach (var s in switches)
        {
            if (s.isClear)
            {
                list.Add(s);
            }
        }

        return list.ToArray();
    }
}
