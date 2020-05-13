using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInformation : MonoBehaviour {
    #region Instance
    public static GameInformation instance = null;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(GetComponent<AudioSource>());
            Destroy(this);
        }
    }
    #endregion

    public List<Sprite> axeSprites;
    public List<Sprite> swordSprites;
    public List<Sprite> staffSprites;
    public List<Sprite> bowSprites;

    private float volume = 1;
    private bool changingVolume;

    [HideInInspector] public AudioManager audio;
    private AudioSource audioSource;
    public AudioClip menu;
    public AudioClip battle;
    public AudioClip love;

    public TMP_FontAsset defaultFont;
    public TMP_FontAsset edgyFont;

    public List<ItemData> allItems;

    public List<EnemyUnitInfo> enemyInfo;
    public List<PlayerUnitInfo> playerInfo;
    public List<TerrainUnitInfo> terrainInfo;
    public List<int> levelDifficulties;

    [HideInInspector] public bool viewedTutorial = false;

    [HideInInspector] public int totalLevels;
    public int currentLevel = 1;

    [Tooltip("PhynneVhall, MinkeeVhall, PhynneMinkee")]
    public List<ItemData> comboInfo;

    private int flour = 500;

    private void Start() {
        Init();
    }

    public void Init() {
        InitUnits();
        InitInventory();

        totalLevels = levelDifficulties.Count;

        audio = GetComponentInChildren<AudioManager>();
        audioSource = GetComponent<AudioSource>();

        PlayMenu();
    }

    #region Inventory
    public void RemoveFromInventory(ItemData item) {
        item.owned = false;
    }

    public void AddToInventory(ItemData item) {
        item.owned = true;
    }

    public List<ItemData> GetInventory() {
        List<ItemData> inventory = new List<ItemData>();
        foreach (ItemData item in allItems) {
            if (item.owned) {
                inventory.Add(item);
            }
        }

        return inventory;
    }

    public List<ItemData> GetShopItems() {
        List<ItemData> shop = new List<ItemData>();
        foreach (ItemData item in allItems) {
            if (!item.owned) {
                shop.Add(item);
            }
        }

        return shop;
    }

    private void InitInventory() {
        foreach (ItemData combo in comboInfo) {
            combo.OnStart();
        }

        foreach (ItemData item in allItems) {
            item.OnStart();
        }

        foreach (PlayerUnitInfo player in playerInfo) {
            if (player.WeaponMain != null) {
                AddToInventory(player.WeaponMain);
                player.WeaponMain.equipped = true;
            }

            if (player.WeaponSecondary != null) {
                AddToInventory(player.WeaponSecondary);
                player.WeaponSecondary.equipped = true;
            }
        }
    }
    #endregion

    #region Info
    private void InitUnits() {
        foreach (PlayerUnitInfo info in playerInfo) {
            info.Init();
        }
        foreach (EnemyUnitInfo info in enemyInfo) {
            info.Init();
        }
    }

    public EnemyUnitInfo GetInfo(UnitID enemy) {
        foreach (EnemyUnitInfo info in enemyInfo) {
            if (info.id == enemy) {
                return info;
            }
        }

        return null;
    }

    public PlayerUnitInfo GetPlayerInfo(UnitID player) {
        foreach (PlayerUnitInfo info in playerInfo) {
            if (info.id == player) {
                return info;
            }
        }
        return null;
    }

    public PlayerUnitInfo GetPlayerInfo(string name) {
        switch (name) {
            case "Minkee":
                return GetPlayerInfo(UnitID.MINKEE);
            case "Phynne":
                return GetPlayerInfo(UnitID.PHYNNE);
            case "Vhall":
                return GetPlayerInfo(UnitID.VHALL);
        }

        return null;
    }

    public TerrainUnitInfo GetTerrainInfo(Terrain terrain) {
        foreach (TerrainUnitInfo t in terrainInfo) {
            if (t.terrain == terrain) {
                return t;
            }
        }

        Debug.LogError("Couldn't find terrain: " + terrain);
        return null;
    }

    public EnemyUnitInfo GetRandomEnemy() {
        return enemyInfo[Random.Range(0, enemyInfo.Count)];
    }
    #endregion

    #region Combo Info
    public ItemData GetComboInfo(UnitPair pair) {
        int index = 0;

        switch (pair) {
            case UnitPair.VHALLPHYNNE:
                index = 0;
                break;
            case UnitPair.MINKEEVHALL:
                index = 1;
                break;
            case UnitPair.PHYNNEMINKEE:
                index = 2;
                break;
        }

        return comboInfo[index];
    }

    public List<ItemData> GetComboInfo(UnitID id) {
        List<ItemData> data = new List<ItemData>();
        List<UnitPair> pairs = new List<UnitPair>();

        switch (id) {
            case UnitID.MINKEE:
                pairs.Add(UnitPair.MINKEEVHALL);
                pairs.Add(UnitPair.PHYNNEMINKEE);
                break;
            case UnitID.PHYNNE:
                pairs.Add(UnitPair.PHYNNEMINKEE);
                pairs.Add(UnitPair.VHALLPHYNNE);
                break;
            case UnitID.VHALL:
                pairs.Add(UnitPair.VHALLPHYNNE);
                pairs.Add(UnitPair.MINKEEVHALL);
                break;
        }

        foreach (UnitPair pair in pairs) {
            data.Add(GetComboInfo(pair));
        }

        return data;
    }
    #endregion

    #region Flour
    public int GetFlour() {
        return flour;
    }

    public void AddFlour(int amount) {
        flour += amount;
    }

    public bool SubtractFlour(int amount) {
        if (amount > flour) {
            return false;
        }

        flour -= amount;
        return true;
    }
    #endregion

    public int GetLevelDifficulty() {
        return levelDifficulties[currentLevel - 1];
    }

    public bool WonGame() {
        return currentLevel > totalLevels;
    }

    #region Scenes
    public void GoToRestScene() {
        SceneManager.LoadScene("RestScene");
    }

    public void GoToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		        Application.Quit ();
#endif
    }
    #endregion

    #region BGM
    public void SetBGMVolume(float volume) {
        if (changingVolume) {
            return;
        }

        StartCoroutine(ChangeBGMVolume(volume));
    }

    public void ScaleBGMVolume(float amount) {
        float volume = audioSource.volume;
        volume *= amount;
        volume = Mathf.Clamp(volume, 0, 1);

        StartCoroutine(ChangeBGMVolume(volume));
    }

    private IEnumerator ChangeBGMVolume(float volume) {
        changingVolume = true;

        float change = (volume - audioSource.volume) / 10f;

        for (int i = 0; i < 10; i++) { 
            audioSource.volume += change;
            yield return new WaitForSeconds(.1f);
        }

        changingVolume = false;
    }

    public void PlayMenu() {
        if (!audioSource.clip.name.Equals(menu.name)) {
            StartCoroutine(ChangeClip(menu));
        }
    }

    public void PlayBattle() {
        StartCoroutine(ChangeClip(battle));
    }

    public void PlayLove() {
        if (!audioSource.clip.name.Equals(love.name)) {
            StartCoroutine(ChangeClip(love, .6f));
        }
    }

    private IEnumerator ChangeClip(AudioClip clip, float targetVolume = 1f) {
        yield return ChangeBGMVolume(0f);
        audioSource.clip = clip;
        audioSource.Play();
        yield return ChangeBGMVolume(targetVolume);
    }
    #endregion

    public Sprite GetWeaponSprite(ItemData weapon, WeaponRank rank) {
        if (weapon.isSword) {
            return swordSprites[(int)rank];
        }

        switch (weapon.type) {
            case WeaponType.MELEE:
                return axeSprites[(int)rank];
            case WeaponType.RANGED:
                return bowSprites[(int)rank];
            case WeaponType.STAFF:
                return staffSprites[(int)rank];
        }

        return null;
    }
}
