using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIToast : MonoBehaviour
{
    public void Toast(string text)
    {
        StartCoroutine("ShowText", text);
    }

    IEnumerator ShowText(string text)
    {
        GetComponent<Text>().text = text;
        yield return new WaitForSeconds(3);
        GetComponent<Text>().text = "";
    }
}
