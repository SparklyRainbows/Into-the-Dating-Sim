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

    private float volume = 1;

    [HideInInspector] public AudioManager audio;
    private AudioSource audioSource;
    public AudioClip menu;
    public AudioClip battle;
    public AudioClip love;

    public TMP_FontAsset defaultFont;
    public TMP_FontAsset edgyFont;

    private List<ItemData> inventory = new List<ItemData>();

    public List<EnemyUnitInfo> enemyInfo;
    public List<PlayerUnitInfo> playerInfo;
    public List<TerrainUnitInfo> terrainInfo;
    public List<int> levelDifficulties;

    [HideInInspector] public bool viewedTutorial = false;

    [HideInInspector] public int totalLevels;
    public int currentLevel = 1;

    [Tooltip("PhynneVhall, MinkeeVhall, PhynneMinkee")]
    public List<ItemData> comboInfo;

    private int flour = 0;

    private void Start() {
        InitUnits();
        InitInventory();

        totalLevels = levelDifficulties.Count;

        audio = GetComponentInChildren<AudioManager>();
        audioSource = GetComponent<AudioSource>();

        PlayMenu();
    }

    #region Inventory
    public void RemoveFromInventory(ItemData item) {
        if (inventory.Contains(item)) {
            item.Init();
            inventory.Remove(item);
        } else {
            Debug.LogError("Trying to remove an item from the inventory that you don't have: " + item.name);
        }
    }

    public void AddToInventory(ItemData item) {
        if (item.consumed) {
            inventory.Add(item);
        } else {
            inventory.Insert(0, item);
        }
    }

    public List<ItemData> GetInventory() {
        return inventory;
    }

    private void InitInventory() {
        foreach (PlayerUnitInfo player in playerInfo) {
            if (player.WeaponMain != null && !inventory.Contains(player.WeaponMain)) {
                player.WeaponMain.OnStart();
                AddToInventory(player.WeaponMain);
            }

            if (player.WeaponSecondary != null && !inventory.Contains(player.WeaponSecondary)) {
                player.WeaponSecondary.OnStart();
                AddToInventory(player.WeaponSecondary);
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

        ItemData data = comboInfo[index];
        data.OnStart();
        return data;
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
        return levelDifficulties[currentLevel];
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
        StartCoroutine(ChangeBGMVolume(volume));
    }

    public void ScaleBGMVolume(float amount) {
        float volume = audioSource.volume;
        volume *= amount;
        volume = Mathf.Clamp(volume, 0, 1);

        StartCoroutine(ChangeBGMVolume(volume));
    }

    private IEnumerator ChangeBGMVolume(float volume) {
        float change = (volume - audioSource.volume) / 10f;

        for (int i = 0; i < 10; i++) { 
            audioSource.volume += change;
            yield return new WaitForSeconds(.1f);
        }
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
}
