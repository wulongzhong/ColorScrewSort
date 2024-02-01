using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayProcessor : MonoBehaviour
{
    public static DelayProcessor Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void DelayProcessFunc(float time, UnityAction action = null)
    {
        StartCoroutine(IDelayProcess(time, action));
    }

    public IEnumerator IDelayProcess(float time = 1f, UnityAction action = null)
    {
        yield return new WaitForSeconds(time);

        if (action != null)
        {
            action.Invoke();
        }
    }
}
