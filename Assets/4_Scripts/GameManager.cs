using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	const int NUM_OF_TYPE = 6;
	const int NUM_OF_TYPE_WITH_BABY = 7;
	const int NUM_OF_UPGRADE = 5;
	const int NUM_OF_REBIRTH = 4;
	const int NUM_OF_SKILL = 2;
	const float ANIM_SPEED = 4;
	const float BASIC_RANGE = 1.5f;
	const float GRAVITY_SCALE = 9.8f;
	const float KNOCKBACK_CHECK_ZSCALE = 1.01f;
	const float FIRST_KNOCKBACK_VELOCITY = 2.5f;
	const float SECOND_KNOCKBACK_VELOCITY = 1.8f;
	const float BOTTOM_UI_SPEED = 10;

	// GameBasicInfo

	public Unit cTowerBasicStatus;
	public Unit hTowerBasicStatus;
	public Unit[] carnivoreBasicStatus;
	public Unit[] herbivoreBasicStatus;

	Unit cTowerStatus = new Unit();
	Unit hTowerStatus = new Unit();
	[SerializeField]
	Unit[] carnivoreStatus = new Unit[NUM_OF_TYPE_WITH_BABY];
	[SerializeField]
	Unit[] herbivoreStatus = new Unit[NUM_OF_TYPE];

	Unit cTowerInfo = new Unit();
	Unit hTowerInfo = new Unit();
	List<Unit> carnivoreInfo = new List<Unit>();
	List<Unit> herbivoreInfo = new List<Unit>();

	int[] carnivoreUpgrade = new int[NUM_OF_TYPE];
	int[] rebirthUpgrade = new int[NUM_OF_TYPE];

	int[] herbivoreUpgrade = new int[NUM_OF_TYPE];
	int herbRebirthUpgrade = 0;

	int stage = 0;
	float meat = 0;
	int gold = 0;
	int dia = 0;

	int rebirthTime = 0;

	public float meatIncreasePerSecond;
	public float maxMeatReserves;

	// ~GameBasicInfo

	// GameInnerInfo

	[System.NonSerialized]
	public int assignment = 1;
	
	int[] popUpEver = new int[NUM_OF_TYPE + NUM_OF_SKILL + 1]; 

	int productionLimit = 0;
	int productionLimitTemp = 0;

	int carnivoreNumber = 1;
	float babyTiger = 0;

	float hProductionTimer = 0;
	int hProductionCount = 0;
	int herbivoreNumber = 1;

	int skillLimit = 0;
	int skillLimitTemp = 0;

	bool skill1 = false;
	int skill1Cool = 0;
	float skill1Timer = 0;
	bool skill1Able = false;
	float skill1AbleTimer = 0;

	int skill1CoolSave = 0;

	bool skill2 = false;
	int skill2Cool = 0;
	float skill2Timer = 0;

	int skill2CoolSave = 0;

	int diaTemp = 0;

	bool isBottomUI = false;

	bool pause = false;

	// ~GameInnerInfo

	// UserInterface

	public GameObject pauseObj;

	public GameObject[] tutorialObj;
	public GameObject[] tutorialScrollRect;
	public Text tutorialPageText;
	public Image tutorialNextImage;

	public Image[] towerHpImage;
	public GameObject[] towerParent;
	public Animation[] towerAnimation;

	public Text meatReservesText;
	public Image topMeatImage;
	public Text currentStageText;
	public Text goldReservesText;
	public Text diaReservesText;
	public Text rebirthText;

	public Image assignmentImage;
	public Sprite[] assignmentSprite;

	public SpriteRenderer fieldSpriteRenderer;
	public Image bottomBackGroundImage;

	public Sprite[] thema;
	public Sprite[] bottomThema;

	public GameObject[] skill1Obj;
	public GameObject[] skill2Obj;

	public GameObject bottomUI;

	public GameObject[] scrollArea;

	public Image[] uIButtonImage;
	public Sprite[] uIButtonSprite;
	public Sprite[] uIButtonSpriteSelected;

	public Image[] productionButtonImage1;
	public Image[] productionButtonImage2;
	public Sprite[] productionSprite;
	public Sprite[] productionUnableSprite;
	public GameObject[] productionCostText;
	public GameObject[] meatImage;

	public Button[] upgradeButton;
	public Text[] upgradeLevelText;
	public Text[] upgradeCostText;
	public Image[] goldImage;
	public Image[] upgradeBBDImage;
	public Sprite[] upgradeBBDSprite;

	public Button[] rebirthUpgradeButton;
	public Text[] rebirthUpgradeLevelText;
	public Text[] rebirthUpgradeCostText;
	public Image[] rebirthBBDImage;
	public Sprite[] rebirthBBDSprite;

	public Animation rebirthAnimation;
	public Text rebirthDiaText;

	public GameObject[] gameInfoPopUp;
	public Animation[] gameInfoAnimation;
	public Button[] gameInfoButton;
	public GameObject[] rebirthInfoPopUp;
	public Animation[] rebirthInfoAnimation;

	public GameObject defeatPopUp;
	public Animator defeatAnimation;

	public GameObject[] noticePopUp;
	public Animation[] noticePopUpAnimation;

	// ~UserInterface

	// Effect

	public GameObject[] attackEffect;
	public GameObject[] dieEffect;
	public GameObject[] skill1Effect;
	public GameObject[] skill2Effect;
	public GameObject[] rebirth3Effect;

	public GameObject[] productionAlready;
	public GameObject clearAnim;
	public GameObject rebirthAnim;

	// ~Effect

	void Start ()
	{
		LoadData();
		Initialization();
		PauseUpdate();
	}

	void Update ()
	{
		if (!pause)
		{
			HerbivoreAutoProduction();
			CarnivoreAttack();
			HerbivoreAttack();
			CarnivoreAttacked();
			HerbivoreAttacked();
			CarnivoreMove();
			HerbivoreMove();
			MeatManage();
			SkillManage();
			RebirthManage();
		}

		BottomUIManage();
	}

	void LoadData ()
	{
		stage = PlayerPrefs.GetInt("stageData");
		gold = PlayerPrefs.GetInt("goldData");
		dia = PlayerPrefs.GetInt("diaData");
		rebirthTime = PlayerPrefs.GetInt("rebirthTimeData");

		currentStageText.text = stage + "";

		for (int i = 0; i < NUM_OF_TYPE; ++i)
		{
			carnivoreUpgrade[i] = PlayerPrefs.GetInt("CarnivoreUpgrade " + i + "Data");
		}

		for (int i = 0; i < NUM_OF_TYPE; ++i)
		{
			rebirthUpgrade[i] = PlayerPrefs.GetInt("rebirthUpgrade " + i + "Data");
		}

		skill1CoolSave = PlayerPrefs.GetInt("skill1CoolSaveData");
		skill2CoolSave = PlayerPrefs.GetInt("skill2CoolSaveData");

		diaTemp = PlayerPrefs.GetInt("diaTempData");

		for (int i = 0; i < NUM_OF_TYPE + NUM_OF_SKILL + 1; ++i)
		{
			popUpEver[i] = PlayerPrefs.GetInt("popUpEver " + i + "Data");
		}

		if (stage <= 8)
			skillLimit = 0;
		else if (stage <= 14)
			skillLimit = 1;
		else
			skillLimit = 2;
		
		SkillUIUpdate();
		
		BottomUIUpdate();
	}

	void Initialization ()
	{
		if (stage <= 15)
		{
			fieldSpriteRenderer.sprite = thema[0];
			bottomBackGroundImage.sprite = bottomThema[0];
		}
		else if (stage <= 30)
		{
			fieldSpriteRenderer.sprite = thema[1];
			bottomBackGroundImage.sprite = bottomThema[1];
		}
		else if (stage <= 45)
		{
			fieldSpriteRenderer.sprite = thema[2];
			bottomBackGroundImage.sprite = bottomThema[2];
		}
		else if (stage <= 60)
		{
			fieldSpriteRenderer.sprite = thema[3];
			bottomBackGroundImage.sprite = bottomThema[3];
		}

		Unit cTemp = new Unit();
		cTemp.Copy(cTowerBasicStatus);
		Unit hTemp = new Unit();
		hTemp.Copy(hTowerBasicStatus);

		if (stage <= 15)
		{
			
		}
		else if (stage <= 30)
		{
			cTemp.hitPoint = 45;
			hTemp.hitPoint = 45;
		}
		else if (stage <= 45)
		{
			cTemp.hitPoint = 50;
			hTemp.hitPoint = 50;
		}
		else if (stage <= 60)
		{
			cTemp.hitPoint = 60;
			hTemp.hitPoint = 60;
		}

		cTowerStatus.Copy(cTemp);
		hTowerStatus.Copy(hTemp);

		for (int i = 0; i < NUM_OF_TYPE_WITH_BABY; ++i)
		{
			carnivoreStatus[i] = new Unit();
			carnivoreStatus[i].Copy(carnivoreBasicStatus[i]);
		}

		for (int i = 0; i < NUM_OF_TYPE; ++i)
		{
			herbivoreStatus[i] = new Unit();
			herbivoreStatus[i].Copy(herbivoreBasicStatus[i]);
		}

		hProductionTimer = 0;
		hProductionCount = 0;

		carnivoreNumber = 1;
		herbivoreNumber = 1;

		productionLimit = 0;

		skill1 = false;
		skill1Timer = 0;
		skill1Able = false;
		skill1AbleTimer = 0;

		skillLimit = 0;

		skill2 = false;
		skill2Timer = 0;

		babyTiger = 0;
	}

	void ClearGame (bool toNextStage)
	{
		Destroy(cTowerInfo.character);
		Destroy(hTowerInfo.character);
		for (int i = carnivoreInfo.Count - 1; i >= 0; --i)
		{
			Unit tmp = carnivoreInfo[i];
			carnivoreInfo.Remove(tmp);
			Destroy(tmp.character);
		}
		for (int i = herbivoreInfo.Count - 1; i >= 0; --i)
		{
			Unit tmp = herbivoreInfo[i];
			herbivoreInfo.Remove(tmp);
			Destroy(tmp.character);
		}

		meat = 0;
		meatReservesText.text = meat + "";

		for (int i = 0; i < NUM_OF_TYPE; ++i)
		{
			carnivoreStatus[i].productionTimer = 0;
		}

		if (toNextStage)
		{
			if (stage >= 1 && stage <= 15 && stage % 5 == 0 || stage >= 16 && stage <= 30 && stage % 3 == 0
			   || stage >= 31 && stage <= 45 && stage % 2 == 0 || stage >= 46 && stage <= 60)
				++diaTemp;

			++stage;
			++gold;

			if (skill1Cool > 0)
			{
				skill1CoolSave = --skill1Cool;
			}
			if (skill2Cool > 0)
			{
				skill2CoolSave = --skill2Cool;
			}
		}

		Initialization();

		UnitStatUpdate();

		ProductionLimitUpdate();
		SkillLimitUpdate();
		SkillUIUpdate();

		BottomUIUpdate();

		currentStageText.text = stage + "";
		goldReservesText.text = gold + "";
		diaReservesText.text = dia + "";

		cTowerInfo.character = Instantiate(cTowerStatus.character);
		cTowerInfo.character.transform.parent = towerParent[0].transform;
		cTowerInfo.character.tag = "Carnivore Tower";
		cTowerInfo.hitPoint = cTowerStatus.hitPoint;
		hTowerInfo.character = Instantiate(hTowerStatus.character);
		hTowerInfo.character.transform.parent = towerParent[1].transform;
		hTowerInfo.character.tag = "Herbivore Tower";
		hTowerInfo.hitPoint = hTowerStatus.hitPoint;
		TowerUpdate();

		SaveData();

		if (stage == 45 && popUpEver[8] == 0)
		{
			noticePopUp[0].SetActive(true);
			noticePopUpAnimation[0].Play();
			popUpEver[8] = 1;
			pause = true;
		}
	}

	void SaveData ()
	{
		PlayerPrefs.SetInt("stageData", stage);
		PlayerPrefs.SetInt("goldData", gold);
		PlayerPrefs.SetInt("diaData", dia);
		PlayerPrefs.SetInt("rebirthTimeData", rebirthTime);

		for (int i = 0; i < NUM_OF_TYPE; ++i)
		{
			PlayerPrefs.SetInt("CarnivoreUpgrade " + i + "Data", carnivoreUpgrade[i]);
		}

		for (int i = 0; i < NUM_OF_TYPE; ++i)
		{
			PlayerPrefs.SetInt("rebirthUpgrade " + i + "Data", rebirthUpgrade[i]);
		}

		PlayerPrefs.SetInt("skill1CoolSaveData", skill1CoolSave);
		PlayerPrefs.SetInt("skill2CoolSaveData", skill2CoolSave);

		PlayerPrefs.SetInt("diaTempData", diaTemp);

		for (int i = 0; i < NUM_OF_TYPE + NUM_OF_SKILL + 1; ++i)
		{
			PlayerPrefs.SetInt("popUpEver " + i + "Data", popUpEver[i]);
		}

		PlayerPrefs.Save();
	}

	void HerbivoreAutoProduction ()
	{
		hProductionTimer += Time.deltaTime * assignment;

		switch (stage)
		{
			case 1: // STAGE 1
				if (hProductionTimer >= 4 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 6 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					HerbivoreProduction(0);
				}
				break;
			case 2: // STAGE 2
				if (hProductionTimer >= 5 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 5 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 3: // STAGE 3
				if (hProductionTimer >= 4 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 4: // STAGE 4
				if (hProductionTimer >= 3 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 5: // STAGE 5
				if (hProductionTimer >= 3 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 7 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 6 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 6: // STAGE 6
				if (hProductionTimer >= 3 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 8 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 7: // STAGE 7
				if (hProductionTimer >= 4 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 7 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 8: // STAGE 8
				if (hProductionTimer >= 3 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 9:
				if (hProductionTimer >= 3 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 8 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 6 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 10:
				if (hProductionTimer >= 3 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 8 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 11:
				if (hProductionTimer >= 4 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 5 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(0);
				}
				break;
			case 12:
				if (hProductionTimer >= 3 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 8 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 6 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 13:
				if (hProductionTimer >= 3 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 6 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 14:
				if (hProductionTimer >= 3 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 7 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 15:
				if (hProductionTimer >= 3 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				break;
			case 16:
				if (hProductionTimer >= 7 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 8 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 5 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 17:
				if (hProductionTimer >= 7.5 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 5 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 6 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 7 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 6 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 3.5 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 18:
				if (hProductionTimer >= 6 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 7 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 19:
				if (hProductionTimer >= 6.5 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 5 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 6 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 5 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 2.5 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 20:
				if (hProductionTimer >= 7 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 2.5 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 2.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 2.5 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 21:
				if (hProductionTimer >= 5 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 1.5 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 7 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 22:
				if (hProductionTimer >= 6 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 5 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 23:
				if (hProductionTimer >= 4.5 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 2.7 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2.7 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 3.7 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 4.5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 1.7 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3.7 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 1.5 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 24:
				if (hProductionTimer >= 5 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 2.5 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3.5 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1.5 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 25:
				if (hProductionTimer >= 5 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 4 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 1.5 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 10;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1.5 && hProductionCount == 10)
				{
					hProductionTimer = 0;
					hProductionCount = 11;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 5 && hProductionCount == 11)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 26:
				if (hProductionTimer >= 5 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 2.5 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 6 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 2.5 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 2.5 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 10;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 5 && hProductionCount == 10)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 27:
				if (hProductionTimer >= 5 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 28:
				if (hProductionTimer >= 5 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 29:
				if (hProductionTimer >= 3 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.3 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.3 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.3 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 10;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 10)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 30:
				if (hProductionTimer >= 2 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.3 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 31:
				if (hProductionTimer >= 2 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 1.5 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 32:
				if (hProductionTimer >= 2 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 1.5 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 33:
				if (hProductionTimer >= 2 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 1.5 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 34:
				if (hProductionTimer >= 2 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 1.5 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 35:
				if (hProductionTimer >= 2 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 1.5 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 10;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 10)
				{
					hProductionTimer = 0;
					hProductionCount = 11;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 3 && hProductionCount == 11)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 36:
				if (hProductionTimer >= 2 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 37:
				if (hProductionTimer >= 2 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 10;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 10)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 38:
				if (hProductionTimer >= 2 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 1.5 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 39:
				if (hProductionTimer >= 2 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 10;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 10)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 40:
				if (hProductionTimer >= 2 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 41:
				if (hProductionTimer >= 2 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 1.5 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 10;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 10)
				{
					hProductionTimer = 0;
					hProductionCount = 11;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 11)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
				}
				break;
			case 42:
				if (hProductionTimer >= 2 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 43:
				if (hProductionTimer >= 2 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 44:
				if (hProductionTimer >= 2 && hProductionCount == 0)
				{
					hProductionTimer = 0;
					hProductionCount = 1;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 1)
				{
					hProductionTimer = 0;
					hProductionCount = 2;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 2 && hProductionCount == 2)
				{
					hProductionTimer = 0;
					hProductionCount = 3;
					HerbivoreProduction(3);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 3)
				{
					hProductionTimer = 0;
					hProductionCount = 4;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 4)
				{
					hProductionTimer = 0;
					hProductionCount = 5;
					HerbivoreProduction(4);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 5)
				{
					hProductionTimer = 0;
					hProductionCount = 6;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 6)
				{
					hProductionTimer = 0;
					hProductionCount = 7;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 7)
				{
					hProductionTimer = 0;
					hProductionCount = 8;
					HerbivoreProduction(2);
				}
				else if (hProductionTimer >= 0.5 && hProductionCount == 8)
				{
					hProductionTimer = 0;
					hProductionCount = 9;
					HerbivoreProduction(0);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 9)
				{
					hProductionTimer = 0;
					hProductionCount = 10;
					HerbivoreProduction(1);
				}
				else if (hProductionTimer >= 1 && hProductionCount == 10)
				{
					hProductionTimer = 0;
					hProductionCount = 0;
					HerbivoreProduction(1);
				}
				break;
			case 45:
				if (hProductionCount == 0)
				{
					hProductionCount = 1;
					HerbivoreProduction(1);
				}
				break;
		}
	}

	void CarnivoreAttack ()
	{
		for (int i = 0; i < NUM_OF_TYPE; ++i)
		{
			if (carnivoreStatus[i].productionTimer > 0)
			{
				carnivoreStatus[i].productionTimer -= Time.deltaTime * assignment;

				if (carnivoreStatus[i].productionTimer <= 0)
				{
					productionAlready[i].SetActive(true);
				}
			}
			productionButtonImage2[i].fillAmount = carnivoreStatus[i].productionTimer / carnivoreStatus[i].delay;
		}

		for (int i = 0; i < carnivoreInfo.Count; ++i)
		{
			if (carnivoreInfo[i].character.CompareTag("Carnivore 5"))
			{
				if (carnivoreInfo[i].overHeatTimer > 0)
				{
					carnivoreInfo[i].overHeatTimer -= Time.deltaTime * assignment;

					if (carnivoreInfo[i].overHeatTimer <= 0)
					{
						carnivoreInfo[i].overHeatTimer = 0;
					}
					
					carnivoreInfo[i].componentSpriteRenderer.sprite = carnivoreInfo[i].anim.overHeat
						[(int)(((7 - 0.5f * carnivoreUpgrade[4]) - carnivoreInfo[i].overHeatTimer) / 0.15f) % carnivoreInfo[i].anim.overHeat.Length];
				}
			}

			if (!carnivoreInfo[i].character.CompareTag("Carnivore 5")
				|| carnivoreInfo[i].character.CompareTag("Carnivore 5") && carnivoreInfo[i].overHeatTimer <= 0)
			{
				if (carnivoreInfo[i].knockBack.knockStack == 0)
				{
					if (carnivoreInfo[i].target >= 0)
					{
						for (int j = 0; j < herbivoreInfo.Count; ++j)
						{
							if (carnivoreInfo[i].target == herbivoreInfo[j].number && herbivoreInfo[j].knockBack.knockStack == 0)
							{
								if (carnivoreInfo[i].character.transform.position.x - 1.75f * BASIC_RANGE <= herbivoreInfo[j].character.transform.position.x && carnivoreInfo[i].character.transform.position.x - 1 * BASIC_RANGE > herbivoreInfo[j].character.transform.position.x && carnivoreInfo[i].character.CompareTag("Carnivore 5"))
								{
									carnivoreInfo[i].target = 0;
									goto CSkip;
								}

								if (carnivoreInfo[i].attackTimer < carnivoreInfo[i].attackSpeed)
								{
									if (carnivoreInfo[i].attackTimer < carnivoreInfo[i].attackSpeed / 2)
									{
										carnivoreInfo[i].componentSpriteRenderer.sprite = carnivoreInfo[i].anim.stand;
									}
									else
									{
										if (carnivoreInfo[i].character.CompareTag("Carnivore 5") && carnivoreInfo[i].range == 1)
										{
											carnivoreInfo[i].componentSpriteRenderer.sprite = carnivoreInfo[i].anim.attack2[(int)((carnivoreInfo[i].attackTimer - carnivoreInfo[i].attackSpeed / 2) / ((carnivoreInfo[i].attackSpeed / 2) / carnivoreInfo[i].anim.attack2.Length))];
										}
										else
										{
											carnivoreInfo[i].componentSpriteRenderer.sprite = carnivoreInfo[i].anim.attack1[(int)((carnivoreInfo[i].attackTimer - carnivoreInfo[i].attackSpeed / 2) / ((carnivoreInfo[i].attackSpeed / 2) / carnivoreInfo[i].anim.attack1.Length))];
										}
									}
									carnivoreInfo[i].attackTimer += Time.deltaTime * assignment;
									if (skill2)
										carnivoreInfo[i].attackTimer += Time.deltaTime * assignment;
								}
								if (carnivoreInfo[i].AttackChance)
								{
									herbivoreInfo[j].hitPoint -= carnivoreInfo[i].attackDamage;
									GameObject obj = Instantiate(attackEffect[0]);
									obj.transform.parent = gameObject.transform;
									obj.transform.position = herbivoreInfo[j].character.transform.position;
									obj.transform.position += new Vector3(Random.Range(-50, 51) * 0.01f, Random.Range(-50, 51) * 0.01f, 0);
									obj.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
									if (carnivoreInfo[i].character.CompareTag("Carnivore 6"))
									{
										GameObject obj2 = Instantiate(attackEffect[2]);
										obj2.transform.parent = gameObject.transform;
										obj2.transform.position = carnivoreInfo[i].character.transform.position + new Vector3(-3, 0, 0);
										for (int k = 0; k < herbivoreInfo.Count; ++k)
										{
											if (carnivoreInfo[i].character.transform.position.x - 1.5f * BASIC_RANGE <= herbivoreInfo[k].character.transform.position.x && j != k)
											{
												herbivoreInfo[k].hitPoint -= carnivoreInfo[i].attackDamage;
												GameObject obj3 = Instantiate(attackEffect[0]);
												obj3.transform.parent = gameObject.transform;
												obj3.transform.position = herbivoreInfo[k].character.transform.position;
												obj3.transform.position += new Vector3(Random.Range(-50, 51) * 0.01f, Random.Range(-50, 51) * 0.01f, 0);
												obj3.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
												break;
											}
										}
									}
									carnivoreInfo[i].attackTimer = 0;

									if (carnivoreInfo[i].character.CompareTag("Carnivore 5") && carnivoreInfo[i].range == 2.5f)
									{
										++carnivoreInfo[i].overHeatCount;
										if (carnivoreInfo[i].overHeatCount >= 10 + carnivoreUpgrade[4])
										{
											carnivoreInfo[i].overHeatTimer = 7 - 0.5f * carnivoreUpgrade[4];
											carnivoreInfo[i].overHeatCount = 0;
											carnivoreInfo[i].target = -1;
										}
									}
								}
								continue;
							}
						}
						carnivoreInfo[i].target = 0;
					}

					if (carnivoreInfo[i].character.CompareTag("Carnivore 5"))
					{
						float herbPos = -31.3f;

						for (int j = 0; j < herbivoreInfo.Count; ++j)
						{
							if (herbPos < herbivoreInfo[j].character.transform.position.x)
								herbPos = herbivoreInfo[j].character.transform.position.x;
						}

						if (carnivoreInfo[i].character.transform.position.x - 2.5f * BASIC_RANGE <= herbPos
							&& carnivoreInfo[i].character.transform.position.x - 1.75f * BASIC_RANGE > herbPos)
						{
							carnivoreInfo[i].range = 2.5f;
						}
						else if (carnivoreInfo[i].character.transform.position.x - 1.75f * BASIC_RANGE <= herbPos)
						{
							carnivoreInfo[i].range = 1;
						}
					}

					bool isTank = false;

					for (int j = 0; j < herbivoreInfo.Count; ++j)
					{
						if (carnivoreInfo[i].character.transform.position.x - carnivoreInfo[i].range * BASIC_RANGE <= herbivoreInfo[j].character.transform.position.x && herbivoreInfo[j].character.CompareTag("Herbivore 4"))
						{
							isTank = true;
						}
					}

					for (int j = 0; j < herbivoreInfo.Count; ++j)
					{
						if (isTank && !herbivoreInfo[j].character.CompareTag("Herbivore 4"))
						{
							continue;
						}
						if (carnivoreInfo[i].character.transform.position.x - carnivoreInfo[i].range * BASIC_RANGE <= herbivoreInfo
							[j].character.transform.position.x && herbivoreInfo[j].knockBack.knockStack == 0)
						{
							if (carnivoreInfo[i].attackTimer < carnivoreInfo[i].attackSpeed)
							{
								if (carnivoreInfo[i].attackTimer < carnivoreInfo[i].attackSpeed / 2)
								{
									carnivoreInfo[i].componentSpriteRenderer.sprite = carnivoreInfo[i].anim.stand;
								}
								else
								{
									if (carnivoreInfo[i].character.CompareTag("Carnivore 5") && carnivoreInfo[i].range == 1)
									{
										carnivoreInfo[i].componentSpriteRenderer.sprite = carnivoreInfo[i].anim.attack2[(int)((carnivoreInfo[i].attackTimer - carnivoreInfo[i].attackSpeed / 2) / ((carnivoreInfo[i].attackSpeed / 2) / carnivoreInfo[i].anim.attack2.Length))];
									}
									else
									{
										carnivoreInfo[i].componentSpriteRenderer.sprite = carnivoreInfo[i].anim.attack1[(int)((carnivoreInfo[i].attackTimer - carnivoreInfo[i].attackSpeed / 2) / ((carnivoreInfo[i].attackSpeed / 2) / carnivoreInfo[i].anim.attack1.Length))];
									}
								}
								carnivoreInfo[i].attackTimer += Time.deltaTime * assignment * (skill2 ? 2 : 1);
								carnivoreInfo[i].target = -1;
							}
							if (carnivoreInfo[i].AttackChance)
							{
								herbivoreInfo[j].hitPoint -= carnivoreInfo[i].attackDamage;
								GameObject obj = Instantiate(attackEffect[0]);
								obj.transform.parent = gameObject.transform;
								obj.transform.position = herbivoreInfo[j].character.transform.position;
								obj.transform.position += new Vector3(Random.Range(-50, 51) * 0.01f, Random.Range(-50, 51) * 0.01f, 0);
								obj.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
								if (carnivoreInfo[i].character.CompareTag("Carnivore 6"))
								{
									GameObject obj2 = Instantiate(attackEffect[2]);
									obj2.transform.parent = gameObject.transform;
									obj2.transform.position = carnivoreInfo[i].character.transform.position + new Vector3 (-3, 0, 0);
									for (int k = 0; k < herbivoreInfo.Count; ++k)
									{
										if (carnivoreInfo[i].character.transform.position.x - 1.5f * BASIC_RANGE <= herbivoreInfo[k].character.transform.position.x && j != k)
										{
											herbivoreInfo[k].hitPoint -= carnivoreInfo[i].attackDamage;
											GameObject obj3 = Instantiate(attackEffect[0]);
											obj3.transform.parent = gameObject.transform;
											obj3.transform.position = herbivoreInfo[k].character.transform.position;
											obj3.transform.position += new Vector3(Random.Range(-50, 51) * 0.01f, Random.Range(-50, 51) * 0.01f, 0);
											obj3.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
											break;
										}
									}
								}
								carnivoreInfo[i].target = herbivoreInfo[j].number;
								carnivoreInfo[i].attackTimer = 0;

								if (carnivoreInfo[i].character.CompareTag("Carnivore 5") && carnivoreInfo[i].range == 2.5f)
								{
									++carnivoreInfo[i].overHeatCount;
									if (carnivoreInfo[i].overHeatCount >= 10 + carnivoreUpgrade[4])
									{
										carnivoreInfo[i].overHeatTimer = 7 - 0.5f * carnivoreUpgrade[4];
										carnivoreInfo[i].overHeatCount = 0;
										carnivoreInfo[i].target = -1;
									}
								}
							}
							goto CSkip;
						}
					}

					if (carnivoreInfo[i].character.transform.position.x - carnivoreInfo[i].range * BASIC_RANGE <= -31.3f)
					{
						if (carnivoreInfo[i].attackTimer < carnivoreInfo[i].attackSpeed)
						{
							if (carnivoreInfo[i].attackTimer < carnivoreInfo[i].attackSpeed / 2)
							{
								carnivoreInfo[i].componentSpriteRenderer.sprite = carnivoreInfo[i].anim.stand;
							}
							else
							{
								if (carnivoreInfo[i].character.CompareTag("Carnivore 5") && carnivoreInfo[i].range == 1)
								{
									carnivoreInfo[i].componentSpriteRenderer.sprite = carnivoreInfo[i].anim.attack2[(int)((carnivoreInfo[i].attackTimer - carnivoreInfo[i].attackSpeed / 2) / ((carnivoreInfo[i].attackSpeed / 2) / carnivoreInfo[i].anim.attack2.Length))];
								}
								else
								{
									carnivoreInfo[i].componentSpriteRenderer.sprite = carnivoreInfo[i].anim.attack1[(int)((carnivoreInfo[i].attackTimer - carnivoreInfo[i].attackSpeed / 2) / ((carnivoreInfo[i].attackSpeed / 2) / carnivoreInfo[i].anim.attack1.Length))];
								}
							}
							carnivoreInfo[i].attackTimer += Time.deltaTime * assignment * (skill2 ? 2 : 1);
							carnivoreInfo[i].target = -1;
						}
						if (carnivoreInfo[i].AttackChance)
						{
							hTowerInfo.hitPoint -= carnivoreInfo[i].attackDamage;

							GameObject obj = Instantiate(attackEffect[1]);
							obj.transform.parent = hTowerInfo.character.transform;

							carnivoreInfo[i].target = -1;
							carnivoreInfo[i].attackTimer = 0;

							TowerUpdate();
							towerAnimation[1].Play();

							if (carnivoreInfo[i].character.CompareTag("Carnivore 5") && carnivoreInfo[i].range == 2.5f)
							{
								++carnivoreInfo[i].overHeatCount;
								if (carnivoreInfo[i].overHeatCount >= 10 + carnivoreUpgrade[4])
								{
									carnivoreInfo[i].overHeatTimer = 7 - 0.5f * carnivoreUpgrade[4];
									carnivoreInfo[i].overHeatCount = 0;
									carnivoreInfo[i].target = -1;
								}
							}
						}
						goto CSkip;
					}

					carnivoreInfo[i].target = 0;
					CSkip:;
				}
			}
		}
	}

	void HerbivoreAttack ()
	{
		for (int i = 0; i < NUM_OF_TYPE; ++i)
		{
			if (herbivoreStatus[i].productionTimer > 0)
			{
				herbivoreStatus[i].productionTimer -= Time.deltaTime * assignment;
			}
		}

		for (int i = 0; i < herbivoreInfo.Count; ++i)
		{
			if (herbivoreInfo[i].character.CompareTag("Herbivore 5"))
			{
				if (herbivoreInfo[i].overHeatTimer > 0)
				{
					herbivoreInfo[i].overHeatTimer -= Time.deltaTime * assignment;

					if (herbivoreInfo[i].overHeatTimer <= 0)
					{
						herbivoreInfo[i].overHeatTimer = 0;
					}

					herbivoreInfo[i].componentSpriteRenderer.sprite = herbivoreInfo[i].anim.overHeat
						[(int)(((7 - 0.5f * carnivoreUpgrade[4]) - herbivoreInfo[i].overHeatTimer) / 0.1f) % herbivoreInfo[i].anim.overHeat.Length];
				}
			}

			if (!herbivoreInfo[i].character.CompareTag("Herbivore 5")
				|| herbivoreInfo[i].character.CompareTag("Herbivore 5") && herbivoreInfo[i].overHeatTimer <= 0)
			{
				if (herbivoreInfo[i].knockBack.knockStack == 0)
				{
					if (herbivoreInfo[i].target >= 0)
					{
						for (int j = 0; j < carnivoreInfo.Count; ++j)
						{
							if (herbivoreInfo[i].character.transform.position.x - 1.75f * BASIC_RANGE <= carnivoreInfo[j].character.transform.position.x && herbivoreInfo[i].character.transform.position.x - 1 * BASIC_RANGE > carnivoreInfo[j].character.transform.position.x && herbivoreInfo[i].character.CompareTag("Herbivore 5"))
							{
								herbivoreInfo[i].target = 0;
								goto HSkip;
							}

							if (herbivoreInfo[i].target == carnivoreInfo[j].number && carnivoreInfo[j].knockBack.knockStack == 0)
							{
								if (herbivoreInfo[i].attackTimer < herbivoreInfo[i].attackSpeed)
								{
									if (herbivoreInfo[i].attackTimer < herbivoreInfo[i].attackSpeed / 2)
									{
										herbivoreInfo[i].componentSpriteRenderer.sprite = herbivoreInfo[i].anim.stand;
									}
									else
									{
										if (herbivoreInfo[i].character.CompareTag("Herbivore 5") && herbivoreInfo[i].range == 1)
										{
											herbivoreInfo[i].componentSpriteRenderer.sprite = herbivoreInfo[i].anim.attack2[(int)((herbivoreInfo[i].attackTimer - herbivoreInfo[i].attackSpeed / 2) / ((herbivoreInfo[i].attackSpeed / 2) / herbivoreInfo[i].anim.attack2.Length))];
										}
										else
										{
											herbivoreInfo[i].componentSpriteRenderer.sprite = herbivoreInfo[i].anim.attack1[(int)((herbivoreInfo[i].attackTimer - herbivoreInfo[i].attackSpeed / 2) / ((herbivoreInfo[i].attackSpeed / 2) / herbivoreInfo[i].anim.attack1.Length))];
										}
									}
									herbivoreInfo[i].attackTimer += Time.deltaTime * assignment;
								}
								if (herbivoreInfo[i].AttackChance)
								{
									carnivoreInfo[j].hitPoint -= herbivoreInfo[i].attackDamage;
									GameObject obj = Instantiate(attackEffect[0]);
									obj.transform.parent = gameObject.transform;
									obj.transform.position = carnivoreInfo[j].character.transform.position;
									obj.transform.position += new Vector3(Random.Range(-50, 51) * 0.01f, Random.Range(-50, 51) * 0.01f, 0);
									obj.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
									herbivoreInfo[i].attackTimer = 0;

									if (herbivoreInfo[i].character.CompareTag("Herbivore 5") && herbivoreInfo[i].range == 2.5f)
									{
										++herbivoreInfo[i].overHeatCount;
										if (herbivoreInfo[i].overHeatCount >= 10 + herbivoreUpgrade[4])
										{
											herbivoreInfo[i].overHeatTimer = 7 - 0.5f * herbivoreUpgrade[4];
											herbivoreInfo[i].overHeatCount = 0;
											herbivoreInfo[i].target = -1;
										}
									}
								}
								goto HSkip;
							}
						}
						herbivoreInfo[i].target = 0;
					}

					if (herbivoreInfo[i].character.CompareTag("Herbivore 5"))
					{
						float carnPos = -1.1f;

						for (int j = 0; j < carnivoreInfo.Count; ++j)
						{
							if (carnPos > carnivoreInfo[j].character.transform.position.x)
								carnPos = carnivoreInfo[j].character.transform.position.x;
						}
						
						if (herbivoreInfo[i].character.transform.position.x + 2.5f * BASIC_RANGE >= carnPos
							&& herbivoreInfo[i].character.transform.position.x + 1.75f * BASIC_RANGE < carnPos)
						{
							herbivoreInfo[i].range = 2.5f;
						}
						else if (herbivoreInfo[i].character.transform.position.x + 1.75f * BASIC_RANGE >= carnPos)
						{
							herbivoreInfo[i].range = 1;
						}
					}

					bool isTank = false;

					for (int j = 0; j < carnivoreInfo.Count; ++j)
					{
						if (herbivoreInfo[i].character.transform.position.x + herbivoreInfo[i].range * BASIC_RANGE >= carnivoreInfo[j].character.transform.position.x && carnivoreInfo[j].character.CompareTag("Carnivore 4"))
						{
							isTank = true;
						}
					}

					for (int j = 0; j < carnivoreInfo.Count; ++j)
					{
						if (isTank && !carnivoreInfo[j].character.CompareTag("Carnivore 4"))
						{
							continue;
						}
						if (herbivoreInfo[i].character.transform.position.x + herbivoreInfo[i].range * BASIC_RANGE >= carnivoreInfo[j].character.transform.position.x && carnivoreInfo[j].knockBack.knockStack == 0)
						{
							if (herbivoreInfo[i].attackTimer < herbivoreInfo[i].attackSpeed)
							{
								if (herbivoreInfo[i].attackTimer < herbivoreInfo[i].attackSpeed / 2)
								{
									herbivoreInfo[i].componentSpriteRenderer.sprite = herbivoreInfo[i].anim.stand;
								}
								else
								{
									if (herbivoreInfo[i].character.CompareTag("Herbivore 5") && herbivoreInfo[i].range == 1)
									{
										herbivoreInfo[i].componentSpriteRenderer.sprite = herbivoreInfo[i].anim.attack2[(int)((herbivoreInfo[i].attackTimer - herbivoreInfo[i].attackSpeed / 2) / ((herbivoreInfo[i].attackSpeed / 2) / herbivoreInfo[i].anim.attack2.Length))];
									}
									else
									{
										herbivoreInfo[i].componentSpriteRenderer.sprite = herbivoreInfo[i].anim.attack1[(int)((herbivoreInfo[i].attackTimer - herbivoreInfo[i].attackSpeed / 2) / ((herbivoreInfo[i].attackSpeed / 2) / herbivoreInfo[i].anim.attack1.Length))];
									}
								}
								herbivoreInfo[i].attackTimer += Time.deltaTime * assignment;
								herbivoreInfo[i].target = -1;
							}
							if (herbivoreInfo[i].AttackChance)
							{
								carnivoreInfo[j].hitPoint -= herbivoreInfo[i].attackDamage;
								GameObject obj = Instantiate(attackEffect[0]);
								obj.transform.parent = gameObject.transform;
								obj.transform.position = carnivoreInfo[j].character.transform.position;
								obj.transform.position += new Vector3(Random.Range(-50, 51) * 0.01f, Random.Range(-50, 51) * 0.01f, 0);
								obj.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
								herbivoreInfo[i].target = carnivoreInfo[j].number;
								herbivoreInfo[i].attackTimer = 0;

								if (herbivoreInfo[i].character.CompareTag("Herbivore 5") && herbivoreInfo[i].range == 2.5f)
								{
									++herbivoreInfo[i].overHeatCount;
									if (herbivoreInfo[i].overHeatCount >= 10 + herbivoreUpgrade[4])
									{
										herbivoreInfo[i].overHeatTimer = 7 - 0.5f * herbivoreUpgrade[4];
										herbivoreInfo[i].overHeatCount = 0;
										herbivoreInfo[i].target = -1;
									}
								}
							}
							goto HSkip;
						}
					}

					if (herbivoreInfo[i].character.transform.position.x + herbivoreInfo[i].range * BASIC_RANGE >= -1.1f)
					{
						if (herbivoreInfo[i].attackTimer < herbivoreInfo[i].attackSpeed)
						{
							if (herbivoreInfo[i].attackTimer < herbivoreInfo[i].attackSpeed / 2)
							{
								herbivoreInfo[i].componentSpriteRenderer.sprite = herbivoreInfo[i].anim.stand;
							}
							else
							{
								if (herbivoreInfo[i].character.CompareTag("Herbivore 5") && herbivoreInfo[i].range == 1)
								{
									herbivoreInfo[i].componentSpriteRenderer.sprite = herbivoreInfo[i].anim.attack2[(int)((herbivoreInfo[i].attackTimer - herbivoreInfo[i].attackSpeed / 2) / ((herbivoreInfo[i].attackSpeed / 2) / herbivoreInfo[i].anim.attack2.Length))];
								}
								else
								{
									herbivoreInfo[i].componentSpriteRenderer.sprite = herbivoreInfo[i].anim.attack1[(int)((herbivoreInfo[i].attackTimer - herbivoreInfo[i].attackSpeed / 2) / ((herbivoreInfo[i].attackSpeed / 2) / herbivoreInfo[i].anim.attack1.Length))];
								}
							}
							herbivoreInfo[i].attackTimer += Time.deltaTime * assignment;
							herbivoreInfo[i].target = -1;
						}
						if (herbivoreInfo[i].AttackChance)
						{
							cTowerInfo.hitPoint -= herbivoreInfo[i].attackDamage;

							GameObject obj = Instantiate(attackEffect[1]);
							obj.transform.parent = cTowerInfo.character.transform;

							herbivoreInfo[i].target = -1;
							herbivoreInfo[i].attackTimer = 0;

							TowerUpdate();
							towerAnimation[0].Play();

							if (herbivoreInfo[i].character.CompareTag("Herbivore 5") && herbivoreInfo[i].range == 2.5f)
							{
								++herbivoreInfo[i].overHeatCount;
								if (herbivoreInfo[i].overHeatCount >= 10 + herbivoreUpgrade[4])
								{
									herbivoreInfo[i].overHeatTimer = 7 - 0.5f * herbivoreUpgrade[4];
									herbivoreInfo[i].overHeatCount = 0;
									herbivoreInfo[i].target = -1;
								}
							}
						}
						goto HSkip;
					}

					herbivoreInfo[i].target = 0;
					HSkip:;
				}
			}
		}
	}

	void CarnivoreMove ()
	{
		for (int i = 0; i < carnivoreInfo.Count; ++i)
		{
			if (carnivoreInfo[i].target == 0 && carnivoreInfo[i].knockBack.knockStack == 0)
			{
				if (carnivoreInfo[i].attackTimer < carnivoreInfo[i].attackSpeed / 2)
				{
					carnivoreInfo[i].attackTimer += Time.deltaTime * assignment * (skill2 ? 2 : 1);
				}
				else
				{
					carnivoreInfo[i].attackTimer = carnivoreInfo[i].attackSpeed / 2;
				}
				carnivoreInfo[i].character.transform.position = carnivoreInfo[i].character.transform.position -=
					new Vector3(0.7f, 0, 0) * Time.deltaTime * assignment * carnivoreInfo[i].moveSpeed * (skill2 ? 2 : 1);
				if (carnivoreInfo[i].knockBack.knockStack == 0)
				{
					carnivoreInfo[i].componentSpriteRenderer.sprite = carnivoreInfo[i].anim.move[(int)carnivoreInfo[i].moveAnimOrder];
					carnivoreInfo[i].moveAnimOrder += carnivoreInfo[i].moveSpeed * carnivoreInfo[i].anim.moveSpeed * Time.deltaTime * assignment * (skill2 ? 2 : 1);
					carnivoreInfo[i].moveAnimOrder = carnivoreInfo[i].moveAnimOrder >= carnivoreInfo[i].anim.move.Length ? 0 : carnivoreInfo[i].moveAnimOrder;
				}
			}
		}
	}

	void HerbivoreMove ()
	{
		for (int i = 0; i < herbivoreInfo.Count; ++i)
		{
			if (herbivoreInfo[i].target == 0 && herbivoreInfo[i].knockBack.knockStack == 0)
			{
				if (herbivoreInfo[i].attackTimer < herbivoreInfo[i].attackSpeed / 2)
				{
					herbivoreInfo[i].attackTimer += Time.deltaTime * assignment;
				}
				else
				{
					herbivoreInfo[i].attackTimer = herbivoreInfo[i].attackSpeed / 2;
				}
				herbivoreInfo[i].character.transform.position = herbivoreInfo[i].character.transform.position += new Vector3(0.7f, 0, 0) * Time.deltaTime * assignment * herbivoreInfo[i].moveSpeed;
				if (herbivoreInfo[i].knockBack.knockStack == 0)
				{
					herbivoreInfo[i].componentSpriteRenderer.sprite = herbivoreInfo[i].anim.move[(int)herbivoreInfo[i].moveAnimOrder];
				}
				herbivoreInfo[i].moveAnimOrder += herbivoreInfo[i].moveSpeed * herbivoreInfo[i].anim.moveSpeed * Time.deltaTime * assignment;
				herbivoreInfo[i].moveAnimOrder = herbivoreInfo[i].moveAnimOrder >= herbivoreInfo[i].anim.move.Length ? 0 : herbivoreInfo[i].moveAnimOrder;
			}
		}
	}

	void CarnivoreAttacked ()
	{
		for (int i = 0; i < carnivoreInfo.Count; ++i)
		{
			if (carnivoreInfo[i].hitPoint <= carnivoreInfo[i].maxHitPoint / 2 && carnivoreInfo[i].knockBack.knockCount == 2 || carnivoreInfo[i].hitPoint <= 0 && carnivoreInfo[i].knockBack.knockCount == 1)
			{
				--carnivoreInfo[i].knockBack.knockCount;
				carnivoreInfo[i].knockBack.knockStack = 2;
				carnivoreInfo[i].knockBack.knockTimer = 0;
				carnivoreInfo[i].knockBack.startPos = carnivoreInfo[i].character.transform.position;
				carnivoreInfo[i].knockBack.velocity = new Vector2(FIRST_KNOCKBACK_VELOCITY, FIRST_KNOCKBACK_VELOCITY);

				carnivoreInfo[i].componentSpriteRenderer.sprite = carnivoreInfo[i].anim.knock;

				carnivoreInfo[i].attackTimer = 0;

				for (int j = 0; j < herbivoreInfo.Count; ++j)
				{
					if (carnivoreInfo[i].number == herbivoreInfo[j].target)
					{
						herbivoreInfo[j].target = 0;
					}
				}
				carnivoreInfo[i].target = 0;
			}

			if (carnivoreInfo[i].knockBack.knockStack > 0)
			{
				carnivoreInfo[i].knockBack.knockTimer += Time.deltaTime * assignment;

				Vector2 curPos;
				curPos.x = carnivoreInfo[i].knockBack.startPos.x + carnivoreInfo[i].knockBack.velocity.x * carnivoreInfo[i].knockBack.knockTimer;
				curPos.y = carnivoreInfo[i].knockBack.startPos.y + (carnivoreInfo[i].knockBack.velocity.y * carnivoreInfo[i].knockBack.knockTimer) - (0.5f * GRAVITY_SCALE * carnivoreInfo[i].knockBack.knockTimer * carnivoreInfo[i].knockBack.knockTimer);

				if (carnivoreInfo[i].knockBack.startPos.y > curPos.y)
				{
					curPos.y = carnivoreInfo[i].knockBack.startPos.y;
					carnivoreInfo[i].character.transform.position = curPos;
					--carnivoreInfo[i].knockBack.knockStack;

					if (carnivoreInfo[i].knockBack.knockStack > 0)
					{
						carnivoreInfo[i].knockBack.knockTimer = 0;
						carnivoreInfo[i].knockBack.startPos = carnivoreInfo[i].character.transform.position;
						carnivoreInfo[i].knockBack.velocity = new Vector2(SECOND_KNOCKBACK_VELOCITY, SECOND_KNOCKBACK_VELOCITY);

						carnivoreInfo[i].componentSpriteRenderer.sprite = carnivoreInfo[i].anim.knock;

						carnivoreInfo[i].attackTimer = 0;

						for (int j = 0; j < herbivoreInfo.Count; ++j)
						{
							if (carnivoreInfo[i].number == herbivoreInfo[j].target)
							{
								herbivoreInfo[j].target = 0;
							}
						}
						carnivoreInfo[i].target = 0;
					}
					else if (carnivoreInfo[i].hitPoint <= 0)
					{
						Unit tmp = carnivoreInfo[i];
						carnivoreInfo.Remove(tmp);
						Destroy(tmp.character);

						Carnivore6ADUpdate();

						GameObject obj = Instantiate(dieEffect[0]);
						obj.transform.parent = gameObject.transform;
						obj.transform.position = tmp.character.transform.position;
					}
				}
				else
				{
					carnivoreInfo[i].character.transform.position = curPos;
				}
			}
		}
	}

	void HerbivoreAttacked ()
	{
		for (int i = 0; i < herbivoreInfo.Count; ++i)
		{
			if (herbivoreInfo[i].hitPoint <= herbivoreInfo[i].maxHitPoint / 2 && herbivoreInfo[i].knockBack.knockCount == 2 || herbivoreInfo[i].hitPoint <= 0 && herbivoreInfo[i].knockBack.knockCount == 1
				|| herbivoreInfo[i].character.transform.localScale.z == KNOCKBACK_CHECK_ZSCALE)
			{
				if (herbivoreInfo[i].character.transform.localScale.z == KNOCKBACK_CHECK_ZSCALE)
				{
					if (stage == 45)
						goto boss;

					if (herbivoreInfo[i].knockBack.knockStack == 0)
					{
						GameObject obj = Instantiate(skill1Effect[0]);
						obj.transform.parent = gameObject.transform;
						obj.transform.position = herbivoreInfo[i].character.transform.position;

						Vector3 scale = herbivoreInfo[i].character.transform.localScale;
						scale.z = 1;
						herbivoreInfo[i].character.transform.localScale = scale;
						herbivoreInfo[i].hitPoint -= 3;
					}
					else
					{
						Vector3 scale = herbivoreInfo[i].character.transform.localScale;
						scale.z = 1;
						herbivoreInfo[i].character.transform.localScale = scale;

						break;
					}
				}
				else
				{
					--herbivoreInfo[i].knockBack.knockCount;
				}
				
				herbivoreInfo[i].knockBack.knockStack = 2;
				herbivoreInfo[i].knockBack.knockTimer = 0;
				float temp = herbivoreInfo[i].knockBack.startPos.y;
				herbivoreInfo[i].knockBack.startPos = herbivoreInfo[i].character.transform.position;
				if (temp < herbivoreInfo[i].character.transform.position.y)
				{
					herbivoreInfo[i].knockBack.startPos.y = temp;
				}
				herbivoreInfo[i].knockBack.velocity = new Vector2(-FIRST_KNOCKBACK_VELOCITY, FIRST_KNOCKBACK_VELOCITY);

				herbivoreInfo[i].componentSpriteRenderer.sprite = herbivoreInfo[i].anim.knock;

				herbivoreInfo[i].attackTimer = 0;

				for (int j = 0; j < carnivoreInfo.Count; ++j)
				{
					if (herbivoreInfo[i].number == carnivoreInfo[j].target)
					{
						carnivoreInfo[j].target = 0;
					}
				}
				herbivoreInfo[i].target = 0;
				boss:;
			}

			if (herbivoreInfo[i].knockBack.knockStack > 0)
			{
				herbivoreInfo[i].knockBack.knockTimer += Time.deltaTime * assignment;

				Vector2 curPos;
				curPos.x = herbivoreInfo[i].knockBack.startPos.x + herbivoreInfo[i].knockBack.velocity.x * herbivoreInfo[i].knockBack.knockTimer;
				curPos.y = herbivoreInfo[i].knockBack.startPos.y + (herbivoreInfo[i].knockBack.velocity.y * herbivoreInfo[i].knockBack.knockTimer) - (0.5f * GRAVITY_SCALE * herbivoreInfo[i].knockBack.knockTimer * herbivoreInfo[i].knockBack.knockTimer);

				if (herbivoreInfo[i].knockBack.startPos.y > curPos.y)
				{
					curPos.y = herbivoreInfo[i].knockBack.startPos.y;
					herbivoreInfo[i].character.transform.position = curPos;
					--herbivoreInfo[i].knockBack.knockStack;

					if (herbivoreInfo[i].knockBack.knockStack > 0)
					{
						herbivoreInfo[i].knockBack.knockTimer = 0;
						herbivoreInfo[i].knockBack.startPos = herbivoreInfo[i].character.transform.position;
						herbivoreInfo[i].knockBack.velocity = new Vector2(-SECOND_KNOCKBACK_VELOCITY, SECOND_KNOCKBACK_VELOCITY);

						herbivoreInfo[i].componentSpriteRenderer.sprite = herbivoreInfo[i].anim.knock;

						herbivoreInfo[i].attackTimer = 0;

						for (int j = 0; j < carnivoreInfo.Count; ++j)
						{
							if (herbivoreInfo[i].number == carnivoreInfo[j].target)
							{
								carnivoreInfo[j].target = 0;
							}
						}
						herbivoreInfo[i].target = 0;
					}
					else if (herbivoreInfo[i].hitPoint <= 0)
					{
						Unit tmp = herbivoreInfo[i];
						herbivoreInfo.Remove(tmp);
						Destroy(tmp.character);

						meat += tmp.price * 0.3f;

						meat += tmp.price * rebirthUpgrade[0] * 0.05f;

						GameObject obj = Instantiate(dieEffect[0]);
						obj.transform.parent = gameObject.transform;
						obj.transform.position = tmp.character.transform.position;
					}
				}
				else
				{
					herbivoreInfo[i].character.transform.position = curPos;
				}
			}
		}
	}

	void Carnivore6ADUpdate ()
	{
		float count = 0;
		for (int i = 0; i < carnivoreInfo.Count; ++i)
		{
			if (carnivoreInfo[i].character.CompareTag("Carnivore 7"))
				count += 0.5f;
			else
				++count;
		}

		for (int i = 0; i < carnivoreInfo.Count; ++i)
		{
			if (carnivoreInfo[i].character.CompareTag("Carnivore 6"))
				carnivoreInfo[i].attackDamage = carnivoreStatus[5].attackDamage + 0.1f * count;
		}
	}

	void TowerUpdate ()
	{
		if (!pause)
		{
			if (cTowerInfo.hitPoint <= 0)
			{
				GameObject obj = Instantiate(dieEffect[1]);
				obj.transform.parent = gameObject.transform;
				obj.transform.position = cTowerInfo.character.transform.position + new Vector3(0, 1, 0);

				towerHpImage[0].fillAmount = cTowerInfo.hitPoint / cTowerStatus.hitPoint;
				Destroy(cTowerInfo.character);

				for (int i = 0; i < NUM_OF_TYPE; ++i)
				{
					gameInfoPopUp[i].SetActive(false);
					rebirthInfoPopUp[i].SetActive(false);
				}

				pause = true;
				defeatPopUp.SetActive(true);
				defeatAnimation.SetTrigger("Replay");
				rebirthDiaText.text = "+ " + diaTemp;

				gameObject.GetComponent<AudioSource>().mute = true;
			}
			else
			{
				towerHpImage[0].fillAmount = cTowerInfo.hitPoint / cTowerStatus.hitPoint;
			}

			if (hTowerInfo.hitPoint <= 0)
			{
				GameObject obj = Instantiate(dieEffect[1]);
				obj.transform.parent = gameObject.transform;
				obj.transform.position = hTowerInfo.character.transform.position + new Vector3(0, 1, 0);

				pause = true;
				if (stage < 45)
					StartCoroutine(Cloud());
				else
					StartCoroutine(BossClear());

				towerHpImage[1].fillAmount = hTowerInfo.hitPoint / hTowerStatus.hitPoint;
				Destroy(hTowerInfo.character);
			}
			else
			{
				towerHpImage[1].fillAmount = hTowerInfo.hitPoint / hTowerStatus.hitPoint;
			}
		}
	}

	IEnumerator Tutorial ()
	{
		yield return new WaitForSeconds(0.24f);
		pause = true;
		stage = 0;
		currentStageText.text = stage + "";
		tutorialObj[0].SetActive(true);
		tutorialObj[1].SetActive(true);
		ClearGame(false);
	}

	IEnumerator Cloud ()
	{
		yield return new WaitForSeconds(0.4f);
		clearAnim.SetActive(true);
		StartCoroutine(Clear(true));
	}

	IEnumerator Clear (bool toNextStage)
	{
		yield return new WaitForSeconds(0.24f);
		pause = false;
		defeatPopUp.SetActive(false);
		ClearGame(toNextStage);
	}

	IEnumerator BossClear ()
	{
		yield return new WaitForSeconds(0.4f);
		Rebirth();
	}

	IEnumerator AllClear ()
	{
		yield return new WaitForSeconds(0.68f);
		defeatPopUp.SetActive(false);
		pause = false;

		rebirthText.text = ++rebirthTime + " 번째 환생";

		int stageTemp = stage;
		if (stage == 45)
			stage = 1;

		while (stage % 15 != 1 || stageTemp - stage < 15 && stage > 1)
			--stage;
		gold = stage - 1;

		dia += diaTemp;
		diaTemp = 0;

		skill1Cool = 0;
		skill2Cool = 0;

		for (int i = 0; i < NUM_OF_TYPE; ++i)
		{
			carnivoreUpgrade[i] = 0;
		}
		BottomUIUpdate();

		gameObject.GetComponent<AudioSource>().mute = false;

		ClearGame(false);
	}

	void MeatManage ()
	{
		meat += meatIncreasePerSecond * Time.deltaTime * assignment;

		meat += meatIncreasePerSecond / 2 * rebirthUpgrade[0] * Time.deltaTime * assignment;

		if (meat < 0)
		{
			meat = 0;
		}
		if (meat > maxMeatReserves)
		{
			meat = maxMeatReserves;
		}

		meatReservesText.text = (int)meat + "";
	}

	void SkillManage ()
	{
		if (skill1)
		{
			if (!skill1Able)
			{
				skill1AbleTimer += Time.deltaTime * assignment;
				if (skill1AbleTimer >= 1)
				{
					skill1Able = true;
					skill1AbleTimer = 0;
				}
			}

			skill1Timer += Time.deltaTime * assignment;
			if (skill1Timer >= 15)
			{
				skill1 = false;
				skill1Timer = 0;
				skill1Able = false;
				skill1AbleTimer = 0;
			}

			Skill1AutoAttack();
		}

		if (skill2)
		{
			skill2Timer += Time.deltaTime * assignment;
			if (skill2Timer >= 20)
			{
				skill2 = false;
				skill2Timer = 0;
			}
		}
	}

	void Skill1AutoAttack ()
	{
		if (skill1Able)
		{
			int importance = 0;

			for (int i = 0; i < herbivoreInfo.Count; ++i)
			{
				if (herbivoreInfo[i].character.CompareTag("Herbivore 5") && importance < 5)
					importance = 5;
				else if (herbivoreInfo[i].character.CompareTag("Herbivore 4") && importance < 4)
					importance = 4;
				else if (herbivoreInfo[i].character.CompareTag("Herbivore 3") && importance < 3)
					importance = 3;
				else if (herbivoreInfo[i].character.CompareTag("Herbivore 2") && importance < 2)
					importance = 2;
				else if (herbivoreInfo[i].character.CompareTag("Herbivore 1") && importance < 1)
					importance = 1;
			}

			for (int i = 0; i < herbivoreInfo.Count; ++i)
			{
				if (herbivoreInfo[i].character.CompareTag("Herbivore " + importance))
				{
					Vector3 temp = herbivoreInfo[i].character.transform.localScale;
					temp.z = KNOCKBACK_CHECK_ZSCALE;
					herbivoreInfo[i].character.transform.localScale = temp;
					skill1Able = false;
					break;
				}
				else
				{
					continue;
				}
			}
		}
	}

	void RebirthManage ()
	{
		if (rebirthUpgrade[2] > 0)
		{
			for (int i = 0; i < carnivoreInfo.Count; ++i)
			{
				float maxPosX = -30;
				for (int j = 0; j < herbivoreInfo.Count; ++j)
				{
					if (maxPosX < herbivoreInfo[j].character.transform.position.x + herbivoreInfo[j].range * BASIC_RANGE)
					{
						maxPosX = herbivoreInfo[j].character.transform.position.x + herbivoreInfo[j].range * BASIC_RANGE;
					}
				}

				if (carnivoreInfo[i].character.transform.position.x - 10.8f > maxPosX)
				{
					int random = Random.Range(0, 1000 / (int)Mathf.Pow(2, rebirthUpgrade[2] - 1) - (rebirthUpgrade[2] == 4 ? 50 : 0) + 1);

					if (random == 0)
					{
						Rebirth3EffectUpdate(carnivoreInfo[i].character, 0);

						carnivoreInfo[i].character.transform.position += new Vector3(-10.8f, 0, 0);

						Rebirth3EffectUpdate(carnivoreInfo[i].character, 1);
					}
				}
			}
		}

		if (rebirthUpgrade[3] > 0)
		{
			if (babyTiger >= 1 + NUM_OF_REBIRTH - rebirthUpgrade[3])
			{
				CarnivoreProduction(6);
				babyTiger -= 1 + NUM_OF_REBIRTH - rebirthUpgrade[3];
			}
		}
	}

	void BottomUIManage ()
	{
		if (isBottomUI)
		{
			if (bottomUI.transform.localPosition.y < -130)
			{
				Vector3 temp = bottomUI.transform.localPosition;
				temp.y += BOTTOM_UI_SPEED * -(temp.y + 130) * Time.deltaTime;
				bottomUI.transform.localPosition = temp;

				if (bottomUI.transform.localPosition.y > -130)
				{
					Vector3 temp2 = bottomUI.transform.localPosition;
					temp2.y = -130;
					bottomUI.transform.localPosition = temp2;
				}
			}
		}
		else
		{
			if (bottomUI.transform.localPosition.y > -310)
			{
				Vector3 temp = bottomUI.transform.localPosition;
				temp.y += BOTTOM_UI_SPEED * -(temp.y + 310) * Time.deltaTime;
				bottomUI.transform.localPosition = temp;

				if (bottomUI.transform.localPosition.y < -310)
				{
					Vector3 temp2 = bottomUI.transform.localPosition;
					temp2.y = -310;
					bottomUI.transform.localPosition = temp2;
				}
			}
		}

		if (!isBottomUI)
		{
			scrollArea[0].transform.localPosition = new Vector3(0, -99, 0);
			scrollArea[1].transform.localPosition = new Vector3(0, -71, 0);
		}
	}

	void UnitStatUpdate ()
	{
		for (int i = 0; i < NUM_OF_TYPE_WITH_BABY; ++i)
		{
			carnivoreStatus[i] = new Unit();

			Unit cTemp = new Unit();
			cTemp.Copy(carnivoreBasicStatus[i]);

			if (i >= 0 && i <= 5)
			{
				switch (i)
				{
					case 0:
					case 1:
						cTemp.hitPoint += carnivoreUpgrade[i];
						break;
					case 2:
						cTemp.hitPoint += carnivoreUpgrade[i];
						cTemp.attackDamage += carnivoreUpgrade[i] * 0.5f;
						break;
					case 3:
						cTemp.hitPoint += carnivoreUpgrade[i] * 2;
						break;
					case 4:
						cTemp.attackDamage += carnivoreUpgrade[i] * 0.5f;
						break;
					case 5:
						cTemp.hitPoint += carnivoreUpgrade[i];
						cTemp.attackDamage += carnivoreUpgrade[i];
						break;
				}
			}

			cTemp.hitPoint *= 1 + 0.2f * rebirthUpgrade[1];

			carnivoreStatus[i].Copy(cTemp);
		}

		for (int i = 0; i < NUM_OF_TYPE; ++i)
		{
			herbivoreStatus[i] = new Unit();
			Unit hTemp = new Unit();
			hTemp.Copy(herbivoreBasicStatus[i]);

			if (stage <= 2)
			{
				herbivoreUpgrade[i] = 0;
				herbRebirthUpgrade = 0;
				hTemp.hitPoint -= 1;
			}
			else if (stage <= 7)
			{
				herbivoreUpgrade[i] = 0;
				herbRebirthUpgrade = 0;
			}
			else if (stage <= 15)
			{
				herbivoreUpgrade[i] = 1;
				herbRebirthUpgrade = 0;
			}
			else if (stage <= 19)
			{
				herbivoreUpgrade[i] = 2;
				herbRebirthUpgrade = 0;
			}
			else if (stage <= 24)
			{
				herbivoreUpgrade[i] = 2;
				herbRebirthUpgrade = 1;
			}
			else if (stage <= 30)
			{
				herbivoreUpgrade[i] = 3;
				herbRebirthUpgrade = 1;
			}
			else if (stage <= 35)
			{
				herbivoreUpgrade[i] = 3;
				herbRebirthUpgrade = 2;
			}
			else if (stage <= 40)
			{
				herbivoreUpgrade[i] = 4;
				herbRebirthUpgrade = 2;
			}
			else if (stage <= 44)
			{
				herbivoreUpgrade[i] = 4;
				herbRebirthUpgrade = 3;
			}
			else if (stage <= 45)
			{
				hTemp.hitPoint = 100000;
				hTemp.attackDamage = 2;
				hTemp.moveSpeed = 0.45f;
				hTemp.attackSpeed = 5;
				hTemp.anim.moveSpeed *= 4f;
			}

			switch (i)
			{
				case 0:
				case 1:
					hTemp.hitPoint += herbivoreUpgrade[i];
					break;
				case 2:
					hTemp.hitPoint += herbivoreUpgrade[i];
					hTemp.attackDamage += herbivoreUpgrade[i] * 0.5f;
					break;
				case 3:
					hTemp.hitPoint += herbivoreUpgrade[i] * 2;
					break;
				case 4:
					hTemp.attackDamage += herbivoreUpgrade[i] * 0.5f;
					break;
				case 5:
					hTemp.hitPoint += herbivoreUpgrade[i];
					hTemp.attackDamage += herbivoreUpgrade[i];
					break;
			}

			hTemp.hitPoint *= 1 + 0.2f * herbRebirthUpgrade;

			herbivoreStatus[i].Copy(hTemp);
		}
	}
	void Skill1EffectUpdate ()
	{
		for (int i = 0; i < herbivoreInfo.Count; ++i)
		{
			GameObject effect = Instantiate(skill1Effect[1]);
			effect.transform.parent = herbivoreInfo[i].character.transform;

			if (herbivoreInfo[i].character.CompareTag("Herbivore 1"))
			{
				effect.transform.localPosition = new Vector3(0, 1.8f, 0);
				effect.transform.localScale = new Vector3(0.8f, 0.8f, 1);
			}
			else if (herbivoreInfo[i].character.CompareTag("Herbivore 2"))
			{
				effect.transform.localPosition = new Vector3(0, 2.3f, 0);
				effect.transform.localScale = new Vector3(0.8f, 0.8f, 1);
			}
			else if (herbivoreInfo[i].character.CompareTag("Herbivore 3"))
			{
				effect.transform.localPosition = new Vector3(0, 1.8f, 0);
				effect.transform.localScale = new Vector3(0.8f, 0.8f, 1);
			}
			else if (herbivoreInfo[i].character.CompareTag("Herbivore 4"))
			{
				effect.transform.localPosition = new Vector3(0, 2, 0);
				effect.transform.localScale = new Vector3(0.8f, 0.8f, 1);
			}
			else if (herbivoreInfo[i].character.CompareTag("Herbivore 5"))
			{
				effect.transform.localPosition = new Vector3(0.6f, 1.2f, 0);
				effect.transform.localScale = new Vector3(0.5f, 0.5f, 1);
			}
			else if (herbivoreInfo[i].character.CompareTag("Herbivore 6"))
			{
				effect.transform.localPosition = new Vector3(0, 0, 0);
				effect.transform.localScale = new Vector3(1, 1, 1);
			}
		}
	}

	void Skill1EffectUpdate (GameObject obj)
	{
		GameObject effect = Instantiate(skill1Effect[1]);
		effect.transform.parent = obj.transform;

		if (obj.CompareTag("Herbivore 1"))
		{
			effect.transform.localPosition = new Vector3(0, 1.8f, 0);
			effect.transform.localScale = new Vector3(0.8f, 0.8f, 1);
		}
		else if (obj.CompareTag("Herbivore 2"))
		{
			effect.transform.localPosition = new Vector3(0, 2.3f, 0);
			effect.transform.localScale = new Vector3(0.8f, 0.8f, 1);
		}
		else if (obj.CompareTag("Herbivore 3"))
		{
			effect.transform.localPosition = new Vector3(0, 1.8f, 0);
			effect.transform.localScale = new Vector3(0.8f, 0.8f, 1);
		}
		else if (obj.CompareTag("Herbivore 4"))
		{
			effect.transform.localPosition = new Vector3(0, 2, 0);
			effect.transform.localScale = new Vector3(0.8f, 0.8f, 1);
		}
		else if (obj.CompareTag("Herbivore 5"))
		{
			effect.transform.localPosition = new Vector3(0.6f, 1.2f, 0);
			effect.transform.localScale = new Vector3(0.5f, 0.5f, 1);
		}
		else if (obj.CompareTag("Herbivore 6"))
		{
			effect.transform.localPosition = new Vector3(0, 0, 0);
			effect.transform.localScale = new Vector3(1, 1, 1);
		}

		AnimationScript anim = effect.GetComponent<AnimationScript>();
		anim.anim[0].timeCycle = 15;
		anim.anim[0].timer = skill1Timer;
	}

	void Skill2EffectUpdate ()
	{
		for (int i = 0; i < carnivoreInfo.Count; ++i)
		{
			for (int j = 0; j < 2; ++j)
			{
				GameObject effect = Instantiate(skill2Effect[j]);
				effect.transform.parent = carnivoreInfo[i].character.transform;

				if (carnivoreInfo[i].character.CompareTag("Carnivore 1"))
				{
					effect.transform.localPosition = new Vector3(0.25f, 0.3f, 0);
					effect.transform.localScale = new Vector3(0.8f, 0.8f, 1);
				}
				else if (carnivoreInfo[i].character.CompareTag("Carnivore 2"))
				{
					effect.transform.localPosition = new Vector3(0.1f, 0.18f, 0);
					effect.transform.localScale = new Vector3(0.7f, 0.7f, 1);
				}
				else if (carnivoreInfo[i].character.CompareTag("Carnivore 3"))
				{
					effect.transform.localPosition = new Vector3(0, 0.1f, 0);
					effect.transform.localScale = new Vector3(0.8f, 0.8f, 1);
				}
				else if (carnivoreInfo[i].character.CompareTag("Carnivore 4"))
				{
					effect.transform.localPosition = new Vector3(0.2f, -0.2f, 0);
					effect.transform.localScale = new Vector3(1, 1, 1);
				}
				else if (carnivoreInfo[i].character.CompareTag("Carnivore 5"))
				{
					effect.transform.localPosition = new Vector3(-0.1f, -1.1f, 0);
					effect.transform.localScale = new Vector3(0.8f, 0.8f, 1);
				}
				else if (carnivoreInfo[i].character.CompareTag("Carnivore 6"))
				{
					effect.transform.localPosition = new Vector3(1.15f, -0.5f, 0);
					effect.transform.localScale = new Vector3(0.9f, 0.9f, 1);
				}
				else if (carnivoreInfo[i].character.CompareTag("Carnivore 7"))
				{
					effect.transform.localPosition = new Vector3(0.2f, 0, 0);
					effect.transform.localScale = new Vector3(0.6f, 0.6f, 1);
				}
			}
		}
	}

	void Skill2EffectUpdate (GameObject obj)
	{
		for (int i = 0; i < 2; ++i)
		{
			GameObject effect = Instantiate(skill2Effect[i]);
			effect.transform.parent = obj.transform;

			if (obj.CompareTag("Carnivore 1"))
			{
				effect.transform.localPosition = new Vector3(0.25f, 0.3f, 0);
				effect.transform.localScale = new Vector3(0.8f, 0.8f, 1);
			}
			else if (obj.CompareTag("Carnivore 2"))
			{
				effect.transform.localPosition = new Vector3(0.1f, 0.18f, 0);
				effect.transform.localScale = new Vector3(0.7f, 0.7f, 1);
			}
			else if (obj.CompareTag("Carnivore 3"))
			{
				effect.transform.localPosition = new Vector3(0, 0.1f, 0);
				effect.transform.localScale = new Vector3(0.8f, 0.8f, 1);
			}
			else if (obj.CompareTag("Carnivore 4"))
			{
				effect.transform.localPosition = new Vector3(0.2f, -0.2f, 0);
				effect.transform.localScale = new Vector3(1, 1, 1);
			}
			else if (obj.CompareTag("Carnivore 5"))
			{
				effect.transform.localPosition = new Vector3(-0.1f, -1.1f, 0);
				effect.transform.localScale = new Vector3(0.8f, 0.8f, 1);
			}
			else if (obj.CompareTag("Carnivore 6"))
			{
				effect.transform.localPosition = new Vector3(1.15f, -0.5f, 0);
				effect.transform.localScale = new Vector3(0.9f, 0.9f, 1);
			}
			else if (obj.CompareTag("Carnivore 7"))
			{
				effect.transform.localPosition = new Vector3(0.2f, 0, 0);
				effect.transform.localScale = new Vector3(0.6f, 0.6f, 1);
			}

			if (i == 1)
			{
				AnimationScript anim = effect.GetComponent<AnimationScript>();
				anim.order = 0;
				anim.anim[0].timer = skill2Timer % 0.32f;
				anim.anim[0].loopTime = 62 - (int)(skill2Timer / 0.32f);
				if (62 - (int)(skill2Timer / 0.32f) <= 0)
					anim.order = 1;
			}
		}
	}

	void Rebirth3EffectUpdate (GameObject obj, int n)
	{
		GameObject effect = Instantiate(rebirth3Effect[n]);
		effect.transform.parent = gameObject.transform;

		if (n == 0)
		{
			if (obj.CompareTag("Carnivore 1"))
			{
				effect.transform.position = obj.transform.position + new Vector3(0.2f, 0.35f, 0);
				effect.transform.localScale = new Vector3(0.88f, 0.88f, 1);
			}
			else if (obj.CompareTag("Carnivore 2"))
			{
				effect.transform.position = obj.transform.position + new Vector3(0.1f, 0.05f, 0);
				effect.transform.localScale = new Vector3(0.75f, 0.75f, 1);
			}
			else if (obj.CompareTag("Carnivore 3"))
			{
				effect.transform.position = obj.transform.position + new Vector3(0.1f, 0.35f, 0);
				effect.transform.localScale = new Vector3(1, 1, 1);
			}
			else if (obj.CompareTag("Carnivore 4"))
			{
				effect.transform.position = obj.transform.position + new Vector3(0.2f, -0.05f, 0);
				effect.transform.localScale = new Vector3(1.2f, 1.2f, 1);
			}
			else if (obj.CompareTag("Carnivore 5"))
			{
				effect.transform.position = obj.transform.position + new Vector3(0, -1.055f, 0);
				effect.transform.localScale = new Vector3(0.88f, 0.88f, 1);
			}
			else if (obj.CompareTag("Carnivore 6"))
			{
				effect.transform.position = obj.transform.position + new Vector3(1, 0.65f, 0);
				effect.transform.localScale = new Vector3(2.2f, 2.2f, 1);
			}
			else if (obj.CompareTag("Carnivore 7"))
			{
				effect.transform.position = obj.transform.position + new Vector3(0.2f, 0.25f, 0);
				effect.transform.localScale = new Vector3(0.7f, 0.7f, 1);
			}
		}
		else if (n == 1)
		{
			if (obj.CompareTag("Carnivore 1"))
			{
				effect.transform.position = obj.transform.position + new Vector3(0.2f, -0.55f, 0);
				effect.transform.localScale = new Vector3(0.88f, 0.88f, 1);
			}
			else if (obj.CompareTag("Carnivore 2"))
			{
				effect.transform.position = obj.transform.position + new Vector3(0.1f, -0.8f, 0);
				effect.transform.localScale = new Vector3(0.75f, 0.75f, 1);
			}
			else if (obj.CompareTag("Carnivore 3"))
			{
				effect.transform.position = obj.transform.position + new Vector3(0.1f, -0.7f, 0);
				effect.transform.localScale = new Vector3(1, 1, 1);
			}
			else if (obj.CompareTag("Carnivore 4"))
			{
				effect.transform.position = obj.transform.position + new Vector3(0.2f, -1.3f, 0);
				effect.transform.localScale = new Vector3(1.2f, 1.2f, 1);
			}
			else if (obj.CompareTag("Carnivore 5"))
			{
				effect.transform.position = obj.transform.position + new Vector3(0, -1.925f, 0);
				effect.transform.localScale = new Vector3(0.88f, 0.88f, 1);
			}
			else if (obj.CompareTag("Carnivore 6"))
			{
				effect.transform.position = obj.transform.position + new Vector3(1, -1.25f, 0);
				effect.transform.localScale = new Vector3(2.2f, 2.2f, 1);
			}
			else if (obj.CompareTag("Carnivore 7"))
			{
				effect.transform.position = obj.transform.position + new Vector3(0.2f, -0.65f, 0);
				effect.transform.localScale = new Vector3(0.7f, 0.7f, 1);
			}
		}
	}

	void ProductionLimitUpdate ()
	{
		if (stage <= 1)
			productionLimit = 1;
		else if (stage <= 2)
			productionLimit = 2;
		else if (stage <= 3)
			productionLimit = 3;
		else if (stage <= 4)
			productionLimit = 4;
		else if (stage <= 5)
			productionLimit = 5;
		else
			productionLimit = 6;

		if (productionLimit > productionLimitTemp && popUpEver[productionLimit - 1] == 0)
		{
			gameInfoPopUp[productionLimit - 1].SetActive(true);
		}

		productionLimitTemp = productionLimit;

		for (int i = 0; i < NUM_OF_TYPE; ++i)
		{
			if (i < productionLimit)
			{
				productionButtonImage1[i].sprite = productionSprite[i];
				productionCostText[i].SetActive(true);
				meatImage[i].SetActive(true);
				gameInfoButton[i].interactable = true;
				if (carnivoreUpgrade[i] < NUM_OF_UPGRADE)
					upgradeButton[i].interactable = true;
				upgradeLevelText[i].enabled = true;
				upgradeCostText[i].enabled = true;
				goldImage[i].enabled = true;
			}
			else
			{
				productionButtonImage1[i].sprite = productionUnableSprite[i];
				productionCostText[i].SetActive(false);
				meatImage[i].SetActive(false);
				gameInfoButton[i].interactable = false;
				if (carnivoreUpgrade[i] < NUM_OF_UPGRADE)
					upgradeButton[i].interactable = false;
				upgradeLevelText[i].enabled = false;
				upgradeCostText[i].enabled = false;
				goldImage[i].enabled = false;
			}
		}
	}

	void SkillLimitUpdate ()
	{
		if (stage <= 9)
			skillLimit = 0;
		else if (stage <= 14)
			skillLimit = 1;
		else
			skillLimit = 2;

		if (skillLimit > skillLimitTemp && popUpEver[5 + skillLimit] == 0)
		{
			rebirthInfoPopUp[3 + skillLimit].SetActive(true);
		}

		skillLimitTemp = skillLimit;
	}

	void SkillUIUpdate ()
	{
		if (1 > skillLimit)
		{
			skill1Obj[0].SetActive(false);
			skill1Obj[1].SetActive(false);
			skill1Obj[2].SetActive(false);
			skill1Obj[3].SetActive(false);
			skill1Obj[4].SetActive(true);
		}
		else
		{
			switch (skill1Cool)
			{
				case 3:
					skill1Obj[0].SetActive(false);
					skill1Obj[1].SetActive(false);
					skill1Obj[2].SetActive(false);
					skill1Obj[3].SetActive(false);
					skill1Obj[4].SetActive(false);
					break;
				case 2:
					skill1Obj[0].SetActive(false);
					skill1Obj[1].SetActive(false);
					skill1Obj[2].SetActive(true);
					skill1Obj[3].SetActive(false);
					skill1Obj[4].SetActive(false);
					break;
				case 1:
					skill1Obj[0].SetActive(false);
					skill1Obj[1].SetActive(true);
					skill1Obj[2].SetActive(true);
					skill1Obj[3].SetActive(false);
					skill1Obj[4].SetActive(false);
					break;
				case 0:
					skill1Obj[0].SetActive(true);
					skill1Obj[1].SetActive(false);
					skill1Obj[2].SetActive(false);
					skill1Obj[3].SetActive(true);
					skill1Obj[4].SetActive(false);
					break;
			}
		}

		if (2 > skillLimit)
		{
			skill2Obj[0].SetActive(false);
			skill2Obj[1].SetActive(false);
			skill2Obj[2].SetActive(false);
			skill2Obj[3].SetActive(false);
			skill2Obj[4].SetActive(false);
			skill2Obj[5].SetActive(false);
			skill2Obj[6].SetActive(true);
		}
		else
		{
			switch (skill2Cool)
			{
				case 5:
					skill2Obj[0].SetActive(false);
					skill2Obj[1].SetActive(false);
					skill2Obj[2].SetActive(false);
					skill2Obj[3].SetActive(false);
					skill2Obj[4].SetActive(false);
					skill2Obj[5].SetActive(false);
					skill2Obj[6].SetActive(false);
					break;
				case 4:
					skill2Obj[0].SetActive(false);
					skill2Obj[1].SetActive(true);
					skill2Obj[2].SetActive(false);
					skill2Obj[3].SetActive(false);
					skill2Obj[4].SetActive(false);
					skill2Obj[5].SetActive(false);
					skill2Obj[6].SetActive(false);
					break;
				case 3:
					skill2Obj[0].SetActive(false);
					skill2Obj[1].SetActive(true);
					skill2Obj[2].SetActive(true);
					skill2Obj[3].SetActive(false);
					skill2Obj[4].SetActive(false);
					skill2Obj[5].SetActive(false);
					skill2Obj[6].SetActive(false);
					break;
				case 2:
					skill2Obj[0].SetActive(false);
					skill2Obj[1].SetActive(true);
					skill2Obj[2].SetActive(true);
					skill2Obj[3].SetActive(true);
					skill2Obj[4].SetActive(false);
					skill2Obj[5].SetActive(false);
					skill2Obj[6].SetActive(false);
					break;
				case 1:
					skill2Obj[0].SetActive(false);
					skill2Obj[1].SetActive(true);
					skill2Obj[2].SetActive(true);
					skill2Obj[3].SetActive(true);
					skill2Obj[4].SetActive(true);
					skill2Obj[5].SetActive(false);
					skill2Obj[6].SetActive(false);
					break;
				case 0:
					skill2Obj[0].SetActive(true);
					skill2Obj[1].SetActive(false);
					skill2Obj[2].SetActive(false);
					skill2Obj[3].SetActive(false);
					skill2Obj[4].SetActive(false);
					skill2Obj[5].SetActive(true);
					skill2Obj[6].SetActive(false);
					break;
			}
		}
	}

	void BottomUIUpdate ()
	{
		for (int i = 0; i < productionLimit; ++i)
		{
			if (carnivoreUpgrade[i] < NUM_OF_UPGRADE)
			{
				upgradeLevelText[i].text = "Level Up";
				upgradeCostText[i].text = (carnivoreUpgrade[i] + 1) + "";

				upgradeButton[i].interactable = true;
			}
			else
			{
				upgradeLevelText[i].text = "Max Level";
				upgradeCostText[i].text = "";

				upgradeButton[i].interactable = false;
			}

			upgradeBBDImage[i].sprite = upgradeBBDSprite[carnivoreUpgrade[i]];
		}

		for (int i = 0; i < NUM_OF_TYPE; ++i)
		{
			int cost = 0;

			switch (rebirthUpgrade[i])
			{
				case 0:
					cost = 2;
					break;
				case 1:
					cost = 6;
					break;
				case 2:
					cost = 12;
					break;
				case 3:
					cost = 18;
					break;
			}

			rebirthBBDImage[i].sprite = rebirthBBDSprite[rebirthUpgrade[i]];

			if (rebirthUpgrade[i] < NUM_OF_REBIRTH)
			{
				rebirthUpgradeLevelText[i].text = "Level Up";
				rebirthUpgradeCostText[i].text = cost + "";

				rebirthUpgradeButton[i].interactable = true;
			}
			else
			{
				rebirthUpgradeLevelText[i].text = "Max Level";
				rebirthUpgradeCostText[i].text = "";

				rebirthUpgradeButton[i].interactable = false;
			}
		}
	}

	public void PauseUpdate ()
	{
		if (pause)
		{
			pauseObj.SetActive(false);
			clearAnim.SetActive(true);
			if (stage == 0)
				StartCoroutine(Tutorial());
			else
				StartCoroutine(Clear(false));
			towerHpImage[0].enabled = true;
			towerHpImage[1].enabled = true;
			meatReservesText.enabled = true;
			topMeatImage.enabled = true;
			assignmentImage.enabled = true;
		}
		else
		{
			pause = true;
			pauseObj.SetActive(true);
			cTowerInfo.character = Instantiate(cTowerStatus.character);
			hTowerInfo.character = Instantiate(hTowerStatus.character);
			towerHpImage[0].enabled = false;
			towerHpImage[1].enabled = false;
			meatReservesText.enabled = false;
			topMeatImage.enabled = false;
			assignmentImage.enabled = false;
		}
	}

	public void TutorialPrev ()
	{
		for (int i = 2; i < tutorialObj.Length; ++i)
		{
			if (tutorialObj[i].activeSelf == true)
			{
				tutorialObj[i].SetActive(false);
				Vector3 temp;
				switch (i)
				{
					case 1:
						temp = tutorialScrollRect[i - 1].transform.localPosition;
						temp.y = -9.2f;
						tutorialScrollRect[i - 1].transform.localPosition = temp;
						break;
					case 2:
						temp = tutorialScrollRect[i - 1].transform.localPosition;
						temp.y = -40;
						tutorialScrollRect[i - 1].transform.localPosition = temp;
						break;
					case 3:
						temp = tutorialScrollRect[i - 1].transform.localPosition;
						temp.y = 2.3f;
						tutorialScrollRect[i - 1].transform.localPosition = temp;
						break;
					case 4:
						temp = tutorialScrollRect[i - 1].transform.localPosition;
						temp.y = -9.2f;
						tutorialScrollRect[i - 1].transform.localPosition = temp;
						break;
					case 5:
						temp = tutorialScrollRect[i - 1].transform.localPosition;
						temp.y = -22.2f;
						tutorialScrollRect[i - 1].transform.localPosition = temp;
						break;
					case 6:
						temp = tutorialScrollRect[i - 1].transform.localPosition;
						temp.y = -38.5f;
						tutorialScrollRect[i - 1].transform.localPosition = temp;
						break;
				}

				tutorialObj[i - 1].SetActive(true);
				tutorialPageText.text = (i - 1) + " /  6";
				if (i - 1 == 5)
					tutorialNextImage.color = new Color32(200, 205, 219, 255);
				break;
			}
		}
	}

	public void TutorialNext ()
	{
		if (tutorialObj[6].activeSelf)
		{
			++stage;
			for (int i = 0; i < tutorialObj.Length; ++i)
			{
				tutorialObj[i].SetActive(false);
			}
			clearAnim.SetActive(true);
			StartCoroutine(Clear(false));
		}
		else
		{
			for (int i = 1; i < tutorialObj.Length - 1; ++i)
			{
				if (tutorialObj[i].activeSelf == true)
				{
					tutorialObj[i].SetActive(false);
					Vector3 temp;
					switch (i)
					{

						case 1:
							temp = tutorialScrollRect[i - 1].transform.localPosition;
							temp.y = -9.2f;
							tutorialScrollRect[i - 1].transform.localPosition = temp;
							break;
						case 2:
							temp = tutorialScrollRect[i - 1].transform.localPosition;
							temp.y = -40;
							tutorialScrollRect[i - 1].transform.localPosition = temp;
							break;
						case 3:
							temp = tutorialScrollRect[i - 1].transform.localPosition;
							temp.y = 2.3f;
							tutorialScrollRect[i - 1].transform.localPosition = temp;
							break;
						case 4:
							temp = tutorialScrollRect[i - 1].transform.localPosition;
							temp.y = -9.2f;
							tutorialScrollRect[i - 1].transform.localPosition = temp;
							break;
						case 5:
							temp = tutorialScrollRect[i - 1].transform.localPosition;
							temp.y = -22.2f;
							tutorialScrollRect[i - 1].transform.localPosition = temp;
							break;
						case 6:
							temp = tutorialScrollRect[i - 1].transform.localPosition;
							temp.y = -38.5f;
							tutorialScrollRect[i - 1].transform.localPosition = temp;
							break;
					}

					tutorialObj[i + 1].SetActive(true);
					tutorialPageText.text = (i + 1) + " /  6";
					if (i + 1 == 6)
						tutorialNextImage.color = new Color32(191, 38, 38, 255);
					break;
				}
			}
		}
	}

	public void Assignment ()
	{
		if (assignment == 1)
		{
			assignment = 2;
			assignmentImage.sprite = assignmentSprite[1];
		}
		else if (assignment == 2)
		{
			pause = true;
			assignment = 0;
			assignmentImage.sprite = assignmentSprite[2];

			gameObject.GetComponent<AudioSource>().mute = true;

		}
		else
		{
			pause = false;
			assignment = 1;
			assignmentImage.sprite = assignmentSprite[0];

			gameObject.GetComponent<AudioSource>().mute = false;
		}
	}

	public void CarnivoreProduction (int n)
	{
		if (!pause)
		{
			if (n < productionLimit || n == 6)
			{
				if (carnivoreStatus[n].ProductionChance && meat - carnivoreStatus[n].price >= 0)
				{
					if (rebirthUpgrade[3] > 0 && babyTiger < 1 + NUM_OF_REBIRTH - rebirthUpgrade[3])
					{
						switch (n)
						{
							case 0:
							case 1:
								babyTiger += 0.5f;
								break;
							case 2:
								babyTiger += 1;
								break;
							case 3:
							case 4:
								babyTiger += 1.5f;
								break;
							case 5:
								babyTiger += 2.5f;
								break;
						}
					}

					GameObject obj = Instantiate(carnivoreStatus[n].character);
					obj.transform.parent = gameObject.transform;
					obj.tag = "Carnivore " + (n + 1);

					if (skill2)
					{
						Skill2EffectUpdate(obj);
					}

					Unit unit = new Unit();
					unit.Copy(carnivoreStatus[n]);
					unit.character = obj;
					int random = Random.Range(0, 61);
					unit.attackSpeed = 1 / unit.attackSpeed;
					unit.character.transform.localPosition += new Vector3(0, 0.005f * random, 0);
					unit.maxHitPoint = unit.hitPoint;
					unit.knockBack.knockCount = 2;
					unit.componentSpriteRenderer = unit.character.GetComponent<SpriteRenderer>();
					unit.componentSpriteRenderer.sortingOrder = 60 - random;

					unit.number = carnivoreNumber++;
					unit.target = 0;

					carnivoreInfo.Add(unit);

					carnivoreStatus[n].productionTimer = carnivoreStatus[n].delay;
					meat -= carnivoreStatus[n].price;

					Carnivore6ADUpdate();
				}
			}
		}
	}

	void HerbivoreProduction (int n)
	{
		GameObject obj = Instantiate(herbivoreStatus[n].character);
		obj.transform.parent = gameObject.transform;
		obj.tag = "Herbivore " + (n + 1);

		//if (skill1)
		//{
		//	Skill1EffectUpdate(obj);
		//}

		Unit unit = new Unit();
		unit.Copy(herbivoreStatus[n]);
		unit.character = obj;
		int random = Random.Range(0, 61);
		unit.character.transform.localPosition += new Vector3(0, 0.005f * random, 0);
		unit.attackSpeed = 1 / unit.attackSpeed;
		unit.maxHitPoint = unit.hitPoint;
		unit.knockBack.knockCount = 2;
		unit.componentSpriteRenderer = unit.character.GetComponent<SpriteRenderer>();
		unit.componentSpriteRenderer.sortingOrder = 60 - random;

		unit.number = herbivoreNumber++;
		unit.target = 0;

		herbivoreInfo.Add(unit);
	}

	public void CarnivoreUpgrade (int n)
	{
		if (carnivoreUpgrade[n] < NUM_OF_UPGRADE)
		{
			if (gold - (carnivoreUpgrade[n] + 1) >= 0)
			{
				gold -= carnivoreUpgrade[n] + 1;
				++carnivoreUpgrade[n];

				goldReservesText.text = gold + "";

				carnivoreStatus[n] = new Unit();

				Unit cTemp = new Unit();
				cTemp.Copy(carnivoreBasicStatus[n]);

				switch (n)
				{
					case 0:
					case 1:
						cTemp.hitPoint += carnivoreUpgrade[n];
						break;
					case 2:
						cTemp.hitPoint += carnivoreUpgrade[n];
						cTemp.attackDamage += carnivoreUpgrade[n] * 0.5f;
						break;
					case 3:
						cTemp.hitPoint += carnivoreUpgrade[n] * 2;
						break;
					case 4:
						cTemp.attackDamage += carnivoreUpgrade[n] * 0.5f;
						break;
					case 5:
						cTemp.hitPoint += carnivoreUpgrade[n];
						cTemp.attackDamage += carnivoreUpgrade[n];
						break;
				}

				cTemp.hitPoint *= 1 + 0.2f * rebirthUpgrade[1];

				carnivoreStatus[n].Copy(cTemp);

				BottomUIUpdate();

				SaveData();
			}
		}
	}

	public void RebirthUpgrade (int n)
	{
		if (rebirthUpgrade[n] < NUM_OF_REBIRTH)
		{
			int cost = 0;

			switch (rebirthUpgrade[n])
			{
				case 0:
					cost = 2;
					break;
				case 1:
					cost = 6;
					break;
				case 2:
					cost = 12;
					break;
				case 3:
					cost = 18;
					break;
			}

			if (dia - cost >= 0)
			{
				++rebirthUpgrade[n];
				dia -= cost;

				diaReservesText.text = dia + "";

				BottomUIUpdate();

				SaveData();

				if (babyTiger >= NUM_OF_UPGRADE - rebirthUpgrade[3])
				{
					--babyTiger;
				}
			}
		}
	}

	public void Skill (int n)
	{
		if (!pause)
		{
			if (n <= skillLimit)
			{
				switch (n)
				{
					case 1:
						if (skill1Cool == 0)
						{
							skill1 = true;
							skill2Timer = 0;
							skill1Able = true;
							skill1AbleTimer = 0;
							skill1Cool = 3;

							//Skill1EffectUpdate();
						}

						if (skill2Cool > 0 || skillLimit < 2)
							OpenBottomUI(1);
						break;
					case 2:
						if (skill2Cool == 0)
						{
							skill2 = true;
							skill2Timer = 0;
							skill2Cool = 5;

							Skill2EffectUpdate();
						}

						if (skill1Cool > 0)
							OpenBottomUI(1);
						break;
				}

				SkillUIUpdate();
			}
		}
	}

	public void OpenBottomUI (int n)
	{
		Vector3 temp;

		switch (n)
		{
			case 1:
				temp = bottomUI.transform.localPosition;
				temp.x = 244;
				bottomUI.transform.localPosition = temp;
				break;
			case 2:
				temp = bottomUI.transform.localPosition;
				temp.x = 0;
				bottomUI.transform.localPosition = temp;
				break;
			case 3:
				temp = bottomUI.transform.localPosition;
				temp.x = -244;
				bottomUI.transform.localPosition = temp;
				break;
		}

		for (int i = 0; i < uIButtonImage.Length; ++i)
		{
			if (i == n - 1)
				uIButtonImage[i].sprite = uIButtonSpriteSelected[i];
			else
				uIButtonImage[i].sprite = uIButtonSprite[i];
		}

		isBottomUI = true;
	}

	public void CloseBottomUI ()
	{
		isBottomUI = false;

		for (int i = 0; i < uIButtonImage.Length; ++i)
		{
			uIButtonImage[i].sprite = uIButtonSprite[i];
		}
	}

	public void OpenGamePopUp (int n)
	{
		for (int i = 0; i < NUM_OF_TYPE; ++i)
		{
			gameInfoPopUp[i].SetActive(false);
		}

		gameInfoPopUp[n].SetActive(true);
		gameInfoAnimation[n].Play();
	}

	public void CloseGamePopUp (int n)
	{
		gameInfoPopUp[n].SetActive(false);
		popUpEver[n] = 1;

		SaveData();
	}

	public void OpenRebirthPopUp (int n)
	{
		for (int i = 0; i < NUM_OF_TYPE; ++i)
		{
			rebirthInfoPopUp[i].SetActive(false);
		}

		rebirthInfoPopUp[n].SetActive(true);
		rebirthInfoAnimation[n].Play();
	}

	public void CloseRebirthPopUp (int n)
	{
		rebirthInfoPopUp[n].SetActive(false);
		if (n == 4 || n == 5)
			popUpEver[2 + n] = 1;

		SaveData();
	}

	public void Replay ()
	{
		clearAnim.SetActive(true);
		skill1Cool = skill1CoolSave;
		skill2Cool = skill2CoolSave;
		StartCoroutine(Clear(false));

		gameObject.GetComponent<AudioSource>().mute = false;
	}

	public void Rebirth ()
	{
		rebirthText.text = rebirthTime + " 번째 환생";
		rebirthAnim.SetActive(true);
		rebirthAnimation.Play();
		StartCoroutine(AllClear());
	}

	public void CloseNotice (int n)
	{
		noticePopUp[n].SetActive(false);
		pause = false;
	}
}
