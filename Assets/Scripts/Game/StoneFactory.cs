using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;

public class StoneFactory : MonoBehaviour
{

    #region singleton

    public static StoneFactory Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    #endregion
    
    public GameSetting gameSetting;
    
    private Dictionary<string, AsyncOperationHandle<GameObject>> loadingDictionary;
    private Dictionary<string, AsyncOperationHandle<Sprite>> spriteDictionary;
    public UnityEvent Ready;
    public UnityEvent StoneBuilt;
    
    [SerializeField] Material spriteMaterial;
    
    private List<Transform>[] stoneLists;
    private List<Transform> horizontalLists;
    private List<Transform> verticalLists;
    private List<Transform> bombLists;

    private bool isReady = false;

    public float GetLoadPercentage()
    {
        return (float) loadingDictionary.Count / gameSetting.stoneTypes.Length;
    }
    
    private void Start() {
        StartCoroutine(LoadAndAssociateResultWithKey());
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            Ready.AddListener(OnFinishLoadingInGame);
        }
    }

    private void OnFinishLoadingInGame()
    {
        BuildStones();
    }
    
    IEnumerator LoadAndAssociateResultWithKey() {
        if (loadingDictionary == null)
            loadingDictionary = new Dictionary<string, AsyncOperationHandle<GameObject>>();

        AsyncOperationHandle<IList<IResourceLocation>> locations
            = Addressables.LoadResourceLocationsAsync(gameSetting.stoneTypes,
                Addressables.MergeMode.Union, typeof(GameObject));

        yield return locations;

        var loadOps = new List<AsyncOperationHandle>(locations.Result.Count);

        foreach (IResourceLocation location in locations.Result) {
            AsyncOperationHandle<GameObject> handle =
                Addressables.LoadAssetAsync<GameObject>(location);
            handle.Completed += obj => loadingDictionary.Add(location.PrimaryKey, obj);
            loadOps.Add(handle);
        }

        yield return Addressables.ResourceManager.CreateGenericGroupOperation(loadOps, true);
        
        //Sprite dictionary
        
        if (spriteDictionary == null)
            spriteDictionary = new Dictionary<string, AsyncOperationHandle<Sprite>>();

        foreach (string location in gameSetting.spriteTypes) {
            AsyncOperationHandle<Sprite> handle =
                Addressables.LoadAssetAsync<Sprite>(location);
            handle.Completed += obj => spriteDictionary.Add(location, obj);
            loadOps.Add(handle);
        }

        yield return Addressables.ResourceManager.CreateGenericGroupOperation(loadOps, true);

        Debug.Log("Finish loading");

        isReady = true;
        Ready.Invoke();
    }
    
    public void BuildStones()
    {
        if (!isReady) return;
        
        Debug.Log("Building Stones");
        
        int total = CommonVars.GRID_HEIGHT * CommonVars.GRID_WIDTH;
        stoneLists = new List<Transform>[gameSetting.pickTypes.Length];
        for (int stoneIndex = 0; stoneIndex < gameSetting.pickTypes.Length; stoneIndex++)
        {
            stoneLists[stoneIndex] = new List<Transform>();
            for (int i = 0; i < total; i++)
            {
                GameObject stone = Instantiate(loadingDictionary[gameSetting.pickTypes[stoneIndex]].Result);
                stone.GetComponent<SpriteRenderer>().material = spriteMaterial;
                stone.SetActive(false);
                stoneLists[stoneIndex].Add(stone.transform);
            }
        }

        horizontalLists = new List<Transform>();
        for (int i = 0; i < total; i++)
        {
            GameObject stone = Instantiate(loadingDictionary["Horizontal"].Result);
            stone.GetComponent<SpriteRenderer>().material = spriteMaterial;
            stone.SetActive(false);
            horizontalLists.Add(stone.transform);
        }

        verticalLists = new List<Transform>();
        for (int i = 0; i < total; i++)
        {
            GameObject stone = Instantiate(loadingDictionary["Vertical"].Result);
            stone.GetComponent<SpriteRenderer>().material = spriteMaterial;
            stone.SetActive(false);
            verticalLists.Add(stone.transform);
        }

        bombLists = new List<Transform>();
        for (int i = 0; i < total; i++)
        {
            GameObject stone = Instantiate(loadingDictionary["Bomb"].Result);
            stone.GetComponent<SpriteRenderer>().material = spriteMaterial;
            stone.SetActive(false);
            bombLists.Add(stone.transform);
        }

        StoneBuilt.Invoke();
    }

    public Transform GetStone(int type)
    {
        Transform stone = stoneLists[type][0];
        stoneLists[type].RemoveAt(0);
        return stone;
    }

    public void ReturnStone(int type, Transform stone)
    {
        stone.gameObject.SetActive(false);
        stoneLists[type].Add(stone);
    }

    public Transform GetHorizontal()
    {
        Transform stone = horizontalLists[0];
        horizontalLists.RemoveAt(0);
        return stone;
    }

    public void ReturnHorizontal(Transform stone)
    {
        stone.gameObject.SetActive(false);
        horizontalLists.Add(stone);
    }

    public Transform GetVertical()
    {
        Transform stone = verticalLists[0];
        verticalLists.RemoveAt(0);
        return stone;
    }

    public void ReturnVertical(Transform stone)
    {
        stone.gameObject.SetActive(false);
        verticalLists.Add(stone);
    }

    public Transform GetBomb()
    {
        Transform stone = bombLists[0];
        bombLists.RemoveAt(0);
        return stone;
    }

    public void ReturnBomb(Transform stone)
    {
        stone.gameObject.SetActive(false);
        bombLists.Add(stone);
    }

    public Sprite GetCatSprite(int catType)
    {
        return spriteDictionary[gameSetting.spriteTypes[catType]].Result;
    }

    public Sprite GetCatSprite(string spriteRef)
    {
        return spriteDictionary[spriteRef].Result;
    }
}