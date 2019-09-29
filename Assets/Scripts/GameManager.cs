using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager gm;
	public static Player player;

	public bool advancing;
	public bool gameOver;

	public Text levelText;
	public Text stageText;
	public GameObject gameOverUI;
	public GameObject blockDestructionParticles;

	public Level[] levels;
	public int curLevelNr;
	public int curStageNr;
	private GameObject curStage;

	private void Awake()
	{
		if (!gm)
		{
			gm = this;
		}
	}

	private void Start()
	{
		NextLevel();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			RestartScene();
		}
	}

	public void RestartScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void Advance()
	{
		if (advancing)
		{
			return;
		}
		advancing = true;

		if (curStageNr < levels[curLevelNr - 1].stages.Length)
		{
			NextStage();
		}
		else if (curLevelNr < levels.Length)
		{
				
			NextLevel();
		}
	}

	public void NextLevel()
	{
		curLevelNr++;
		
		curStageNr = 0;
		NextStage();
		RefreshUI();
	}

	public void NextStage()
	{
		curStageNr++;
		if (curStage)
		{
			Destroy(curStage);
		}
		StartCoroutine(SpawnNextStage());
		RefreshUI();
	}

	public void RefreshUI()
	{
		levelText.text = "LEVEL " + curLevelNr.ToString();
		stageText.text = curStageNr.ToString() + "/" + levels[curLevelNr - 1].stages.Length;
	}

	private IEnumerator SpawnNextStage()
	{
		yield return new WaitForSeconds(1f);
		curStage = Instantiate(levels[curLevelNr - 1].stages[curStageNr - 1]);
		advancing = false;
	}

	public void GameOver()
	{
		gameOver = true;
		gameOverUI.SetActive(true);
	}
}

[System.Serializable]
public class Level
{
	public GameObject[] stages;
}