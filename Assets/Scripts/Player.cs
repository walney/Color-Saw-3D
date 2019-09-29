using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public Color color;
	public float speed = 10f;
	public float minSpeed = 1.5f;
	public float sensitivity = 0.035f;
	public float turnSensitivity = 10f;
	public float stopTreshold = 0.4f;
	public Dictionary<Vector3, Block> blocks = new Dictionary<Vector3, Block>();

	private bool movementReset;
	private int originBlockCount;

	private Rigidbody rb;
	private Vector3 playerPos;
	private Vector3 playerStartPos;
	private Vector3 mousePos;
	private Vector3 mouseStartPos;
	private Vector3 direction;
	private Vector3 targetPos;

	private void Awake()
	{
		GameManager.player = this;
		rb = GetComponent<Rigidbody>();
		ResetMovement();
	}

	private void Start()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			var child = transform.GetChild(i);
			var childBlock = child.GetComponent<Block>();
			blocks.Add(child.localPosition, childBlock);

			if (!childBlock.origin)
			{
				child.GetComponent<MeshRenderer>().material.color = color;
			}
			else
			{
				originBlockCount++;
			}
		}

		RefreshBlocks();
	}

	private void Update()
	{
		if (GameManager.gm.gameOver)
		{
			return;
		}

		playerPos = rb.position;
		mousePos = Input.mousePosition;

		if (Input.GetMouseButtonDown(0))
		{
			ResetMovement();
		}
		else if (Input.GetMouseButton(0))
		{
			var mouseDir = mousePos - mouseStartPos;
			if (direction == Vector3.zero)
			{
				if (mouseDir.magnitude < turnSensitivity)
				{
					return;
				}
				direction = mouseDir.normalized;
				direction = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? new Vector3(1, 0, 0) : new Vector3(0, 0, 1);
			}

			var mouseDistance = mousePos - mouseStartPos;
			if (direction.x != 0 && mousePos.x != mouseStartPos.x)
			{
				var distance = new Vector3(Mathf.RoundToInt(mouseDistance.x * sensitivity), 0, mouseDistance.y);
				targetPos = playerStartPos + direction * distance.x;
			}
			else if (direction.z != 0 && mousePos.y != mouseStartPos.y)
			{
				var distance = new Vector3(mouseDistance.x, 0, Mathf.RoundToInt(mouseDistance.y * sensitivity));
				targetPos = playerStartPos + direction * distance.z;
			}
			else
			{
				targetPos = playerStartPos;
			}
		}

		if ((targetPos - playerPos).magnitude < stopTreshold)
		{
			if (!movementReset)
			{
				ResetMovement();
			}
		}
		else
		{
			movementReset = false;
		}
	}

	private void FixedUpdate()
	{
		if (playerPos != targetPos)
		{
			targetPos = new Vector3(Mathf.Clamp(targetPos.x, -12f, 12f), 0, Mathf.Clamp(targetPos.z, -25f, 25f));
			rb.MovePosition(Vector3.MoveTowards(playerPos, targetPos, speed * Mathf.Max(minSpeed, (playerPos - targetPos).magnitude) * Time.fixedDeltaTime));
		}
	}

	private void ResetMovement()
	{
		movementReset = true;
		direction = Vector3.zero;
		mouseStartPos = Input.mousePosition;
		playerStartPos = new Vector3(Mathf.RoundToInt(playerPos.x), 0, Mathf.RoundToInt(playerPos.z));
	}

	public void RefreshBlocks()
	{
		// Make all blocks not connected
		foreach (var block in blocks.Values)
		{
			block.connected = false;
		}

		// Find the connected blocks
		foreach (var block in blocks.Values)
		{
			if (block.origin)
			{
				block.CheckConnection();
			}
		}

		// Find the disconnected blocks
		List<Block> notConnectedBlocks = new List<Block>();
		foreach (var block in blocks.Values)
		{
			if (!block.connected)
			{
				notConnectedBlocks.Add(block);
			}
		}

		// Destroy the disconnected blocks
		foreach (var block in notConnectedBlocks)
		{
			block.DestroyBlock(false);
		}

		if (originBlockCount == blocks.Count)
		{
			GameManager.gm.Advance();
		}
	}

	public Block BlockByKey(Vector3 key)
	{
		return blocks.ContainsKey(key) ? blocks[key] : null;
	}
}