using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{

	public static GameManager instance;
	public enum GameState
	{
		Gameplay,
		Paused,
		GameOver,
		LevelUp
	}


	public GameState currentState;
	public GameState previousState;

	[Header("Damage Text Settings")]
	public Canvas damageTextCanvas;
	public float textFontSize = 20f;
	public TMP_FontAsset textFont;
	public Camera referenceCamera;

	[Header("Screens")]
	public GameObject pauseScreen;
	public GameObject resultScreen;
	public GameObject levelUpScreen;

	[Header("Current Stats to Displays")]
	public TMP_Text currentHealthDisplay;
	public TMP_Text currentRecoveryDisplay;
	public TMP_Text currentMoveSpeedDisplay;
	public TMP_Text currentMightDisplay;
	public TMP_Text currentProjectileSpeedDisplay;
	public TMP_Text currentMagnetDisplay;

	[Header("Results Screen Displays")]
	public Image chosenCharacterImage;
	public TMP_Text currentChosenCharacterName;
	public TMP_Text levelReachedDisplay;
	public TMP_Text timeSurvivedDisplay;
	public List<Image> choseWeaponUI = new List<Image>(6);
	public List<Image> chosenPassiveItemUI = new List<Image>(6);

	[Header("StopWatch")]
	public float timeLimit;
	float stopwatchTime;
	public TMP_Text stopWatchDisplay;

	public bool isGameOver = false;

	public bool chossingUpgrade;

	public GameObject playerObject;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Debug.LogWarning("Extra" + this + "Deleted");
			Destroy(gameObject);
		}
		DisableScreen();
	}

	void Update()
	{
		switch (currentState)
		{
			case GameState.Gameplay:
				CheckForPausedAndResume();
				UpdateStopWatch();
				break;

			case GameState.Paused:
				CheckForPausedAndResume();
				break;

			case GameState.GameOver:
				if (!isGameOver)
				{
					isGameOver = true;
					Time.timeScale = 0f;
					DisplayResult();
				}
				break;
			case GameState.LevelUp:
				if (!chossingUpgrade)
				{
					chossingUpgrade = true;
					Time.timeScale = 0f;
					levelUpScreen.SetActive(true);
				}
				break;

			default:
				Debug.LogWarning("STATE DOES NOT EXIST");
				break;
		}
	}

	IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 50f)
	{

		//Start generating the floating text
		GameObject textobj = new GameObject("Damage Floating Text");
		RectTransform rect = textobj.AddComponent<RectTransform>();
		TextMeshProUGUI tmPro = textobj.AddComponent<TextMeshProUGUI>();
		tmPro.text = text;
		tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
		tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
		tmPro.fontSize = textFontSize;
		if (textFont)
		{
			tmPro.font = textFont;
		}
		rect.position = referenceCamera.WorldToScreenPoint(target.position);

		//Makes sure this is destroyed after the duration finishes.
		Destroy(textobj, duration);

		//Parent the generated text object to the canvas
		textobj.transform.SetParent(instance.damageTextCanvas.transform);
		textobj.transform.SetSiblingIndex(0);
		//Pan the text upwards and fade it away over time
		WaitForEndOfFrame w = new WaitForEndOfFrame();
		float t = 0;
		float yOffset = 0;
		Vector3 lasKnownPosition= target.position;
		while (t < duration)
		{
			if (!rect)
			{
				break;
			}


			tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

			if (target)
			{
				lasKnownPosition= target.position;
			}
			
			yOffset += speed * Time.deltaTime;

			rect.position = referenceCamera.WorldToScreenPoint(lasKnownPosition + new Vector3(0, yOffset));

			yield return w;
			t += Time.deltaTime;
		}

	}

	public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
	{
		if (!instance.damageTextCanvas)
		{
			return;
		}

		if (!instance.referenceCamera)
		{
			instance.referenceCamera = Camera.main;
		}

		instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(text, target, duration, speed));

	}


	public void ChangeState(GameState newState)
	{
		currentState = newState;
	}

	public void PauseGame()
	{

		if (currentState != GameState.Paused)
		{
			previousState = currentState;
			ChangeState(GameState.Paused);
			Time.timeScale = 0f;
			pauseScreen.SetActive(true);
		}
	}

	public void ResumeGame()
	{

		if (currentState == GameState.Paused)
		{
			ChangeState(previousState);
			Time.timeScale = 1f;
			pauseScreen.SetActive(false);

		}
	}


	void CheckForPausedAndResume()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (currentState == GameState.Paused)
			{
				ResumeGame();
			}
			else
			{
				PauseGame();
			}
		}
	}

	void DisableScreen()
	{
		pauseScreen.SetActive(false);
		resultScreen.SetActive(false);
		levelUpScreen.SetActive(false);
	}

	public void GameOver()
	{
		timeSurvivedDisplay.text = stopWatchDisplay.text;
		ChangeState(GameState.GameOver);

	}

	void DisplayResult()
	{

		resultScreen.SetActive(true);
	}

	public void AssignChosenCharacterUI(CharacterData chosenCharacterData)
	{
		chosenCharacterImage.sprite = chosenCharacterData.Icon;
		currentChosenCharacterName.text = chosenCharacterData.Name;
	}

	public void AssignLevelReachedUI(int levelREachedData)
	{
		levelReachedDisplay.text = levelREachedData.ToString();
	}


	public void AssignChosenWEaponAndPassiveItemsUI(List<PlayerInventory.Slot> chosenWeaponData, List<PlayerInventory.Slot> chosenPassiveItemData)
	{
		if (chosenWeaponData.Count != choseWeaponUI.Count || chosenPassiveItemData.Count != chosenPassiveItemUI.Count)
		{
			Debug.Log("Chosen weapons and passive items data lists have different lenghts");
			return;
		}


		for (int i = 0; i < choseWeaponUI.Count; i++)
		{
			if (chosenWeaponData[i].image.sprite)
			{

				choseWeaponUI[i].enabled = true;
				choseWeaponUI[i].sprite = chosenWeaponData[i].image.sprite;
			}
			else
			{
				choseWeaponUI[i].enabled = false;

			}
		}

		for (int i = 0; i < chosenPassiveItemUI.Count; i++)
		{
			if (chosenPassiveItemData[i].image.sprite)
			{

				chosenPassiveItemUI[i].enabled = true;
				chosenPassiveItemUI[i].sprite = chosenPassiveItemData[i].image.sprite;
			}
			else
			{
				chosenPassiveItemUI[i].enabled = false;

			}
		}
	}

	void UpdateStopWatch()
	{
		stopwatchTime += Time.deltaTime;
		UpdateStopWatchDisplay();
		if (stopwatchTime >= timeLimit)
		{
			playerObject.SendMessage("Kill");
		}
	}

	void UpdateStopWatchDisplay()
	{
		int minutes = Mathf.FloorToInt(stopwatchTime / 60);
		int second = Mathf.FloorToInt(stopwatchTime % 60);

		stopWatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, second);
	}


	public void StartLevelUP()
	{
		ChangeState(GameState.LevelUp);
		playerObject.SendMessage("RemoveAndApplyUpgrades");
	}

	public void EndLevelUp()
	{
		chossingUpgrade = false;
		Time.timeScale = 1f;
		levelUpScreen.SetActive(false);
		ChangeState(GameState.Gameplay);
	}
}
