using UnityEngine;
using System.Collections;

public class DestroyObject : MonoBehaviour
{
    public float Delay = 0.5f;

	void Start()
	{
        Destroy(gameObject, Delay);
	}
}
