using UnityEngine;

public class Switch : MonoBehaviour
{
    [HideInInspector] public bool isClear;

    void Awake()
    {
        isClear = AreaClear(Physics.OverlapSphere(transform.position, 4));
    }

    void Update()
    {
        if (isClear != AreaClear(Physics.OverlapSphere(transform.position, 4)))
        {
            isClear = AreaClear(Physics.OverlapSphere(transform.position, 4));
        }
    }

    bool AreaClear(Collider[] colliders)
    {
        foreach (var c in colliders)
        {
            if (c.CompareTag("Player"))
            {
                return false;
            }
        }

        return true;
    }
}
