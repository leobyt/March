using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RenameChildren : MonoBehaviour
{
	public string renameTo;
	public bool rename = false;
	Transform [ ] children;

	void Update()
	{
		if (rename) {
			children = new Transform [transform.childCount];

			for (int i = 0; i < transform.childCount; i++) {
				children [i] = transform.GetChild(i);
			}

			for (int i = 0; i < children.Length; i++) {
				children [i].gameObject.name = renameTo + " (" + i + ")";
			}

			rename = false;
		}
	}
}
