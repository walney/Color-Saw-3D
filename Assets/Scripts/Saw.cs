using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
	public Vector3 rotation;
	public Transform model;

	private void Update()
	{
		model.Rotate(rotation, rotation.magnitude * Time.deltaTime);
	}

	private void OnTriggerEnter(Collider other)
	{
		var block = other.GetComponent<Block>();
		if (block)
		{
			block.DestroyBlock();
		}
	}
}