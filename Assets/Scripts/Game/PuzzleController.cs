using System;
using System.Collections;
using DG.Tweening;
using Interfaces;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class PuzzleController : MonoBehaviour
{
    #region singleton
    public static PuzzleController Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion
    
    [SerializeField] private GameObject stonePrefab;

    private int[][] _blocks;
    private Stone[][] _puzzleBlocks;
    
    private int[,] destroyedBlock;
    private int[,] filledBlock;
    
    private Vector3 originalPosition;

    private bool isLocked = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = this.transform.localPosition;

        if (!CommonVars.StartGame)
        {
            this.transform.localPosition = originalPosition + new Vector3(0f, 4f, 0f);
            this.transform.DOLocalMoveY(originalPosition.y, 0.5f).SetEase(Ease.OutBounce);
        }
        
        InitBlocks();
        
        StoneFactory.Instance.StoneBuilt.AddListener(StartCreateStoneBlocks);
        StoneFactory.Instance.BuildStones();
    }

    private void Update()
    {
        if (isShaking)
        {
            ShakePuzzle(Time.deltaTime);
        }
    }
    
    #region shaking

    private void DoStartShaking()
    {
        isShaking = true;
        shakeTimer = 0f;
        seed = Random.value;
    }

    [Header("Shaking")]
    public float shakeSpeed = 1f;
    public float shakeFactor = 1f;
    private float shakeTime = 1f;
    private float shakeTimer = 0f;
    private bool isShaking = false;
    private float seed = 0;
    private void ShakePuzzle(float deltaTime)
    {
        // Use Perlin Noise here for a quasi-random shake. Multiply or Add extra values for noise speed variations.
        // Many different ways to produc these offset values. This is just one variation.
        var xOffset = Mathf.PerlinNoise(Time.time * shakeSpeed, seed) - 0.5f;
        var yOffset = Mathf.PerlinNoise ( seed, Time.time * shakeSpeed ) - 0.5f;

        transform.position = originalPosition + new Vector3 ( xOffset, yOffset, 0 ) * shakeFactor;
        shakeTimer += deltaTime * 4f;
        if ( shakeTimer > shakeTime )
        {
            shakeTimer = 0;
            isShaking = false;
            transform.position = originalPosition;
        }
    }

    #endregion

    private void DebugBlocks(int[][] block)
    {
        string allStr = "";
        string destoryStr = "";
        for (int i = 0; i < CommonVars.GRID_HEIGHT; i++)
        {
            string line = "";
            string destroyLine = "";
            for (int j = 0; j < CommonVars.GRID_WIDTH; j++)
            {
                line += line.Length == 0 ? block[i][j].ToString() : "," + block[i][j].ToString();
                destroyLine += destroyLine.Length == 0 ? destroyedBlock[i,j].ToString() : "," + destroyedBlock[i,j].ToString();
            }
            allStr += line + "\n";
            destoryStr += destroyLine + "\n";
        }

        Debug.Log(allStr+"\n \n"+destoryStr);
    }

    #region block_creation
    private void InitBlocks()
    {
        CreateBlocks();
        while (CheckBlock(_blocks, true))
        {
            CleanMatchedBlocks(true);
            MovedDestroyedBlocks();
            FillEmptyBlocks();
        }
    }

    private void StartCreateStoneBlocks()
    {
        CreateStoneBlocks();
    }

    private void CreateBlocks()
    {
        //Debug.Log("Create Blocks");
        _blocks = new int[CommonVars.GRID_HEIGHT][];
        var i = 0;
        var j = 0;
        for (i = 0; i < CommonVars.GRID_HEIGHT; i++)
        {
            _blocks[i] = new int[CommonVars.GRID_WIDTH];
            for (j = 0; j < CommonVars.GRID_WIDTH; j++)
            {
                _blocks[i][j] = Random.Range(1, 5);
            }
        }
    }

    private bool isNormalStone(int value)
    {
        return (value >= 1 && value <= 4);
    }

    private bool playSoundWind = false;
    private bool playSoundBomb = false;
    
    private bool CheckBlock(int[][] cBlock, bool onStart = false, int curPosX = -1, int curPosY = -1, int nexPosX = -1, int nexPosY = -1)
    {
        bool result = false;
        destroyedBlock = new int[CommonVars.GRID_HEIGHT,CommonVars.GRID_WIDTH] ;
        
        //check horizontally
        for (int i = 0; i < CommonVars.GRID_HEIGHT; i++)
        {
            int fBlock = cBlock[i][0];
            
            int totalBlock = 1;
            int startBlock = 0;
            for (int j = 1; j < CommonVars.GRID_WIDTH; j++)
            {
                int nBlock = cBlock[i][j];
                if (nBlock == fBlock && isNormalStone(nBlock))
                {
                    totalBlock++;
                    if (j == CommonVars.GRID_WIDTH - 1) //last block
                    {
                        if (totalBlock >= 3)
                        {
                            if (i == curPosY && startBlock <= curPosX && curPosX <= j && !onStart)
                            {
                                for (int index = startBlock; index <= j; index++)
                                {
                                    if (totalBlock == 4 && index == curPosX && !onStart)
                                    {
                                        destroyedBlock[i, index] = CommonVars.MARK_HORIZONTAL_BOMB;
                                    }
                                    else if (totalBlock == 5 && index == curPosX && !onStart)
                                    {
                                        destroyedBlock[i, index] = CommonVars.MARK_COLOR_BOMB;
                                    }
                                    else if (destroyedBlock[i, index] == 0)
                                    {
                                        destroyedBlock[i, index] = CommonVars.MARK_DESTROYED;
                                    }
                                }
                            }
                            else if (i == nexPosY && startBlock <= nexPosX && nexPosX <= j && !onStart)
                            {
                                for (int index = startBlock; index <= j; index++)
                                {
                                    if (totalBlock == 4 && index == nexPosX && !onStart)
                                    {
                                        destroyedBlock[i, index] = CommonVars.MARK_HORIZONTAL_BOMB;
                                    }
                                    else if (totalBlock == 5 && index == nexPosX && !onStart)
                                    {
                                        destroyedBlock[i, index] = CommonVars.MARK_COLOR_BOMB;
                                    }
                                    else if (destroyedBlock[i, index] == 0)
                                    {
                                        destroyedBlock[i, index] = CommonVars.MARK_DESTROYED;
                                    }
                                }
                            }
                            else
                            {
                                for (int index = startBlock; index <= j; index++)
                                {
                                    if (totalBlock == 4 && index == startBlock + 1 && !onStart)
                                    {
                                        destroyedBlock[i, index] = CommonVars.MARK_HORIZONTAL_BOMB;
                                    }
                                    else if (totalBlock == 5 && index == startBlock + 2 && !onStart)
                                    {
                                        destroyedBlock[i, index] = CommonVars.MARK_COLOR_BOMB;
                                    }
                                    else if (destroyedBlock[i, index] == 0)
                                    {
                                        destroyedBlock[i, index] = CommonVars.MARK_DESTROYED;
                                    }
                                }
                            }
                            result = true;
                        }
                    }
                }
                else if (nBlock != fBlock)
                {
                    if (totalBlock >= 3)
                    {
                        if (i == curPosY && startBlock <= curPosX && curPosX <= j && !onStart)
                        {
                            for (int index = startBlock; index < j; index++)
                            {
                                if (totalBlock == 4 && index == curPosX && !onStart)
                                {
                                    destroyedBlock[i, index] = CommonVars.MARK_HORIZONTAL_BOMB; //mark to destroy
                                }
                                else if (totalBlock == 5 && index == curPosX && !onStart)
                                {
                                    destroyedBlock[i, index] = CommonVars.MARK_COLOR_BOMB; //mark as color bomb
                                }
                                else if (destroyedBlock[i, index] == 0)
                                {
                                    destroyedBlock[i, index] = CommonVars.MARK_DESTROYED; //mark as horizontal bomb
                                }
                            }
                        }
                        else if (i == nexPosY && startBlock <= nexPosX && nexPosX <= j && !onStart)
                        {
                            for (int index = startBlock; index < j; index++)
                            {
                                if (totalBlock == 4 && index == nexPosX && !onStart)
                                {
                                    destroyedBlock[i, index] = CommonVars.MARK_HORIZONTAL_BOMB; //mark to destroy
                                }
                                else if (totalBlock == 5 && index == nexPosX && !onStart)
                                {
                                    destroyedBlock[i, index] = CommonVars.MARK_COLOR_BOMB; //mark as color bomb
                                }
                                else if (destroyedBlock[i, index] == 0)
                                {
                                    destroyedBlock[i, index] = CommonVars.MARK_DESTROYED; //mark as horizontal bomb
                                }
                            }
                        }
                        else
                        {
                            for (int index = startBlock; index < j; index++)
                            {
                                if (totalBlock == 4 && index == startBlock + 1 && !onStart)
                                {
                                    destroyedBlock[i, index] = CommonVars.MARK_HORIZONTAL_BOMB; //mark to destroy
                                }
                                else if (totalBlock == 5 && index == startBlock + 2 && !onStart)
                                {
                                    destroyedBlock[i, index] = CommonVars.MARK_COLOR_BOMB; //mark as color bomb
                                }
                                else if (destroyedBlock[i, index] == 0)
                                {
                                    destroyedBlock[i, index] = CommonVars.MARK_DESTROYED; //mark as horizontal bomb
                                }
                            }
                        }
                        result = true;
                    }

                    fBlock = nBlock;
                    totalBlock = 1;
                    startBlock = j;
                }
            }
        }

        //check vertically
        for (int i = 0; i < CommonVars.GRID_WIDTH; i++)
        {
            int fBlock = cBlock[0][i];
            
            int totalBlock = 1;
            int startBlock = 0;
            for (int j = 1; j < CommonVars.GRID_HEIGHT; j++)
            {
                int nBlock = cBlock[j][i];
                if (nBlock == fBlock && isNormalStone(nBlock))
                {
                    totalBlock++;
                    if (j == CommonVars.GRID_HEIGHT - 1)
                    {
                        if (totalBlock >= 3)
                        {
                            if (i == curPosX && startBlock <= curPosY && curPosY <= j && !onStart)
                            {
                                for (int index = startBlock; index <= j; index++)
                                {
                                    if (totalBlock == 4 && index == curPosY && !onStart)
                                    {
                                        destroyedBlock[index, i] = CommonVars.MARK_VERTICAL_BOMB; //mark to destroy
                                    }
                                    else if (totalBlock == 5 && index == curPosY && !onStart)
                                    {
                                        destroyedBlock[index, i] = CommonVars.MARK_COLOR_BOMB; //mark as color bomb
                                    }
                                    else if (destroyedBlock[index, i] == 0)
                                    {
                                        destroyedBlock[index, i] =
                                            CommonVars.MARK_DESTROYED; //mark as horizontal bomb
                                    }
                                }
                            }
                            else if (i == nexPosX && startBlock <= nexPosY && nexPosY <= j && !onStart)
                            {
                                for (int index = startBlock; index <= j; index++)
                                {
                                    if (totalBlock == 4 && index == nexPosY && !onStart)
                                    {
                                        destroyedBlock[index, i] = CommonVars.MARK_VERTICAL_BOMB; //mark to destroy
                                    }
                                    else if (totalBlock == 5 && index == nexPosY && !onStart)
                                    {
                                        destroyedBlock[index, i] = CommonVars.MARK_COLOR_BOMB; //mark as color bomb
                                    }
                                    else if (destroyedBlock[index, i] == 0)
                                    {
                                        destroyedBlock[index, i] =
                                            CommonVars.MARK_DESTROYED; //mark as horizontal bomb
                                    }
                                }
                            }
                            else
                            {
                                for (int index = startBlock; index <= j; index++)
                                {
                                    if (totalBlock == 4 && index == startBlock + 1 && !onStart)
                                    {
                                        destroyedBlock[index, i] = CommonVars.MARK_VERTICAL_BOMB; //mark to destroy
                                    }
                                    else if (totalBlock == 5 && index == startBlock + 2 && !onStart)
                                    {
                                        destroyedBlock[index, i] = CommonVars.MARK_COLOR_BOMB; //mark as color bomb
                                    }
                                    else if (destroyedBlock[index, i] == 0)
                                    {
                                        destroyedBlock[index, i] =
                                            CommonVars.MARK_DESTROYED; //mark as horizontal bomb
                                    }
                                }
                            }

                            result = true;
                        }
                    }
                }
                else if (nBlock != fBlock)
                {
                    if (totalBlock >= 3)
                    {
                        if (i == curPosX && startBlock <= curPosY && curPosY <= j && !onStart)
                        {
                            for (int index = startBlock; index < j; index++)
                            {
                                if (totalBlock == 4 && index == curPosY && !onStart)
                                {
                                    destroyedBlock[index, i] = CommonVars.MARK_VERTICAL_BOMB; //mark to destroy
                                }
                                else if (totalBlock == 5 && index == curPosY && !onStart)
                                {
                                    destroyedBlock[index, i] = CommonVars.MARK_COLOR_BOMB; //mark as color bomb
                                }
                                else if (destroyedBlock[index, i] == 0)
                                {
                                    destroyedBlock[index, i] = CommonVars.MARK_DESTROYED; //mark as horizontal bomb
                                }
                            }
                        }
                        else if (i == nexPosX && startBlock <= nexPosY && nexPosY <= j && !onStart)
                        {
                            for (int index = startBlock; index < j; index++)
                            {
                                if (totalBlock == 4 && index == nexPosY && !onStart)
                                {
                                    destroyedBlock[index, i] = CommonVars.MARK_VERTICAL_BOMB; //mark to destroy
                                }
                                else if (totalBlock == 5 && index == nexPosY && !onStart)
                                {
                                    destroyedBlock[index, i] = CommonVars.MARK_COLOR_BOMB; //mark as color bomb
                                }
                                else if (destroyedBlock[index, i] == 0)
                                {
                                    destroyedBlock[index, i] =
                                        CommonVars.MARK_DESTROYED; //mark as horizontal bomb
                                }
                            }
                        }
                        else
                        {
                            for (int index = startBlock; index < j; index++)
                            {
                                if (totalBlock == 4 && index == startBlock + 1 && !onStart)
                                {
                                    destroyedBlock[index, i] = CommonVars.MARK_VERTICAL_BOMB; //mark to destroy
                                }
                                else if (totalBlock == 5 && index == startBlock + 2 && !onStart)
                                {
                                    destroyedBlock[index, i] = CommonVars.MARK_COLOR_BOMB; //mark as color bomb
                                }
                                else if (destroyedBlock[index, i] == 0)
                                {
                                    destroyedBlock[index, i] = CommonVars.MARK_DESTROYED; //mark as horizontal bomb
                                }
                            }
                        }

                        result = true;
                    }

                    fBlock = nBlock;
                    startBlock = j;
                    totalBlock = 1;
                }
            }
        }
        
        //DebugBlocks(cBlock);
        //Debug.Log("Check Block " + result);
        return result;
    }

    private void CleanMatchedBlocks(bool onStart = false)
    {
        float totalDestroyed = 0f;
        float totalScore = 0f;
        //Debug.Log("Clean Matched Blocks");
        for (int i = 0; i < CommonVars.GRID_HEIGHT; i++)
        {
            for (int j = 0; j < CommonVars.GRID_WIDTH; j++)
            {
                bool isDestroyed = false;
                if (destroyedBlock[i, j] == CommonVars.MARK_DESTROYED)
                {
                    _blocks[i][j] = 0;
                    isDestroyed = true;
                }
                else if (destroyedBlock[i, j] == CommonVars.MARK_HORIZONTAL_BOMB)
                {
                    _blocks[i][j] = 5;
                    isDestroyed = true;
                }
                else if (destroyedBlock[i, j] == CommonVars.MARK_VERTICAL_BOMB)
                {
                    _blocks[i][j] = 6;
                    isDestroyed = true;
                }
                else if (destroyedBlock[i, j] == CommonVars.MARK_COLOR_BOMB)
                {
                    _blocks[i][j] = 7;
                    isDestroyed = true;
                }
                
                if (!onStart && isDestroyed)
                {
                    totalDestroyed++;
                    totalScore += GameController.Instance.levelScore * 100f;
                }
            }
        }

        if (!onStart)
        {
            SoundController.Instance.PlayRandomPopSFX();
            GameController.Instance.AddStoneBreak(totalDestroyed);
            GameController.Instance.UpdateScore(totalScore);

            if (playSoundBomb)
            {
                SoundController.Instance.PlayBombClip();
                playSoundBomb = false;
            }

            if (playSoundWind)
            {
                SoundController.Instance.PlayWindClip();
                playSoundWind = false;
            }
            DoStartShaking();
        }
    }

    private void MovedDestroyedBlocks()
    {
        //Debug.Log("Moved destroyed Blocks");
        for (int j = 0; j < CommonVars.GRID_WIDTH; j++)
        {
            for (int i = 0; i < CommonVars.GRID_HEIGHT; i++)
            {
                if (_blocks[i][j] == 0)
                {
                    //get block from top
                    for (int k = i + 1; k < CommonVars.GRID_HEIGHT; k++)
                    {
                        if (_blocks[k][j] != 0)
                        {
                            (_blocks[i][j],_blocks[k][j]) = (_blocks[k][j],_blocks[i][j]);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void FillEmptyBlocks()
    {
        //Debug.Log("Fill empty blocks");
        for (int i = 0; i < CommonVars.GRID_HEIGHT; i++)
        {
            for (int j = 0; j < CommonVars.GRID_WIDTH; j++)
            {
                if (_blocks[i][j] == 0)
                {
                    _blocks[i][j] = Random.Range(1, 5);
                }
            }
        }
    }
    
    #endregion

    #region create_blocks
    private void CreateStoneBlocks()
    {
        Vector2 firstPosition = Vector3.zero;
        firstPosition.x = CommonVars.GRID_SIZE * -2.5f;
        firstPosition.y = CommonVars.GRID_SIZE * -2f;
        
        _puzzleBlocks = new Stone[CommonVars.GRID_HEIGHT][];
        for (int i = 0; i < CommonVars.GRID_HEIGHT; i++)
        {
            _puzzleBlocks[i] = new Stone[CommonVars.GRID_WIDTH];
            for (int j = 0; j < CommonVars.GRID_WIDTH; j++)
            {
                GameObject newBlock = Instantiate(stonePrefab, transform);
                newBlock.name = "Stone_"+i+" "+j;
                newBlock.transform.localPosition = new Vector2(firstPosition.x + (j * CommonVars.GRID_SIZE),
                    firstPosition.y + (i * CommonVars.GRID_SIZE));
                _puzzleBlocks[i][j] = newBlock.GetComponent<Stone>();
                _puzzleBlocks[i][j].InitCat(_blocks[i][j], j, i);
            }
        }
    }
    
    #endregion
    
    #region switch_blocks
    
    public IEnumerator SequenceSwitchBlock(int curPosX,int curPosY,int nexPosX,int nexPosY)
    {
        if (!isLocked)
        {
            isLocked = true;
        
            Stone curStone = _puzzleBlocks[curPosY][curPosX];
            Stone nexStone = _puzzleBlocks[nexPosY][nexPosX];
            Vector2 curPosition = curStone.transform.localPosition;
            Vector2 nexPosition = nexStone.transform.localPosition;
            
            curStone.gameObject.transform.DOLocalMove(nexPosition, 0.2f);
            nexStone.gameObject.transform.DOLocalMove(curPosition, 0.2f);

            yield return new WaitForSeconds(0.2f);

            bool canDestroy = false;
            int numClean = 0;

            if (curStone.isColorBomb)
            {
                int colorPicked = _blocks[nexPosY][nexPosX];
                if (colorPicked < 5)
                {
                    ClearBombColor(curPosX, curPosY, nexPosX, nexPosY, colorPicked);
                    canDestroy = true;
                }
            }
            else if (nexStone.isColorBomb)
            {
                int colorPicked = _blocks[curPosY][curPosX];
                if (colorPicked < 5)
                {
                    ClearBombColor(nexPosX, nexPosY, curPosX, curPosY, colorPicked);
                    canDestroy = true;
                }
            }
            else if (curStone.isHorizontalBomb)
            {
                SwitchBlock(curPosX, curPosY, nexPosX, nexPosY);
                CleanHorizontalBomb(nexPosY, nexPosX);
                canDestroy = true;
            }
            else if (nexStone.isHorizontalBomb)
            {
                SwitchBlock(curPosX, curPosY, nexPosX, nexPosY);
                CleanHorizontalBomb(curPosY, curPosX);
                canDestroy = true;
            }
            else if (curStone.isVerticalBomb)
            {
                SwitchBlock(curPosX, curPosY, nexPosX, nexPosY);
                CleanVerticalBomb(nexPosY, nexPosX);
                canDestroy = true;
            }
            else if (nexStone.isVerticalBomb)
            {
                SwitchBlock(curPosX, curPosY, nexPosX, nexPosY);
                CleanVerticalBomb(curPosY, curPosX);
                canDestroy = true;
            }
            else if (CanSwitchStoneBlock(curPosX, curPosY, nexPosX, nexPosY))
            {
                canDestroy = true;
            }

            if (canDestroy)
            {
                numClean++;
                CleanMatchedBlocks();
                DestroyedBlocks();
                yield return new WaitForSeconds(0.3f);
                
                float duration = MovedDestroyedStoneBlocks();
                yield return new WaitForSeconds(duration);
                
                while (CheckBlock(_blocks))
                {
                    numClean++;
                    CleanMatchedBlocks();
                    DestroyedBlocks();
                    yield return new WaitForSeconds(0.3f);
                
                    duration = MovedDestroyedStoneBlocks();
                    yield return new WaitForSeconds(duration);
                }

                if (numClean > 2)
                {
                    SoundController.Instance.PlayRandomKittenSFX();
                    UIController.Instance.StartAnimateBonus(numClean);
                }
            }
            else
            {
                curStone.gameObject.transform.DOLocalMove(curPosition, 0.2f);
                nexStone.gameObject.transform.DOLocalMove(nexPosition, 0.2f);
                yield return new WaitForSeconds(.3f);
            }

            isLocked = false;
        }
    }

    private bool CanSwitchStoneBlock(int curPosX, int curPosY, int nextPosX, int nextPosY)
    {
        SwitchBlock(curPosX, curPosY, nextPosX, nextPosY);
        
        if (CheckBlock(_blocks, false, curPosX, curPosY, nextPosX, nextPosY))
        {
            //SwitchBlock(curPosX, curPosY, nextPosX, nextPosY);
            return true;
        }
        else
        {
            //switch back
            SwitchBlock(curPosX, curPosY, nextPosX, nextPosY);
            return false;
        }
    }

    private void SwitchBlock(int curPosX, int curPosY, int nexPosX, int nexPosY)
    {
        (_blocks[curPosY][curPosX], _blocks[nexPosY][nexPosX]) =
            (_blocks[nexPosY][nexPosX], _blocks[curPosY][curPosX]);

        (_puzzleBlocks[curPosY][curPosX], _puzzleBlocks[nexPosY][nexPosX]) =
            (_puzzleBlocks[nexPosY][nexPosX], _puzzleBlocks[curPosY][curPosX]);
        
        _puzzleBlocks[curPosY][curPosX].SetBlock(curPosX, curPosY);
        _puzzleBlocks[nexPosY][nexPosX].SetBlock(nexPosX, nexPosY);
    }
    
    #endregion
    
    #region clear_stone

    private void ClearBombColor(int curPosX, int curPosY, int nextPosX, int nextPosY, int colorPicked)
    {
        SwitchBlock(curPosX, curPosY, nextPosX, nextPosY);
        destroyedBlock = new int[CommonVars.GRID_HEIGHT,CommonVars.GRID_WIDTH] ;
        for (int i = 0; i < CommonVars.GRID_HEIGHT; i++)
        {
            for (int j = 0; j < CommonVars.GRID_WIDTH; j++)
            {
                if (_blocks[i][j] == colorPicked)
                {
                    destroyedBlock[i, j] = CommonVars.MARK_DESTROYED;
                    GameController.Instance.CreateTrailObject(_puzzleBlocks[curPosY][curPosX].transform.position,
                        _puzzleBlocks[i][j].transform.position);
                }
            }
        }
        destroyedBlock[curPosY, curPosX] = CommonVars.MARK_DESTROYED;
        destroyedBlock[nextPosY, nextPosX] = CommonVars.MARK_DESTROYED;

        playSoundBomb = true;
    }

    private void DestroyedBlocks()
    {
        bool playUpgrade = false;
        for (int i = 0; i < CommonVars.GRID_HEIGHT; i++)
        {
            for (int j = 0; j < CommonVars.GRID_WIDTH; j++)
            {
                switch (destroyedBlock[i, j])
                {
                    case CommonVars.MARK_HORIZONTAL_BOMB:
                        playUpgrade = true;
                        _puzzleBlocks[i][j].PlayUpgrade(true, false, false);
                        break;
                    case CommonVars.MARK_VERTICAL_BOMB:
                        playUpgrade = true;
                        _puzzleBlocks[i][j].PlayUpgrade(false, true, false);
                        break;
                    case CommonVars.MARK_COLOR_BOMB: 
                        playUpgrade = true;
                        _puzzleBlocks[i][j].PlayUpgrade(false, false, true);
                        break;
                    case CommonVars.MARK_DESTROYED: 
                        _puzzleBlocks[i][j].PlayDestroyed();
                        break;
                }
            }
        }
        //sound
        if (playUpgrade) SoundController.Instance.PlayUpgradeClip();
    }

    private float MovedDestroyedStoneBlocks()
    {
        float longestTime = 0f;
        
        //Debug.Log("Moved destroyed Stone Blocks");
        Vector2 firstPosition = Vector3.zero;
        firstPosition.x = CommonVars.GRID_SIZE * -2.5f;
        firstPosition.y = CommonVars.GRID_SIZE * -2f;
        
        for (int j = 0; j < CommonVars.GRID_WIDTH; j++)
        {
            int level = 0;
            for (int i = 0; i < CommonVars.GRID_HEIGHT; i++)
            {
                if (_blocks[i][j] == 0)
                {
                    Vector2 nexPosition = new Vector2(firstPosition.x + (j * CommonVars.GRID_SIZE),
                        firstPosition.y + (i * CommonVars.GRID_SIZE));

                    bool isFound = false;
                    //find block from top
                    for (int k = i + 1; k < CommonVars.GRID_HEIGHT; k++)
                    {
                        if (_blocks[k][j] != 0)
                        {
                            Stone curStone = _puzzleBlocks[k][j];
                            float waitTime = .4f;//.4f * (k - i);
                            if (longestTime < waitTime) longestTime = waitTime;
                            
                            curStone.gameObject.transform.DOLocalMove(nexPosition, waitTime).SetEase(Ease.OutBounce);
                            
                            SwitchBlock(j, i, j, k);
                            isFound = true;
                            break;
                        }
                    }

                    if (!isFound)
                    {
                        //if not found then create new one
                        _blocks[i][j] = Random.Range(1, 5);
                    
                        Stone newStone = _puzzleBlocks[i][j];
                        newStone.InitCat(_blocks[i][j], j, i);
                    
                        newStone.transform.localPosition = new Vector2(firstPosition.x + (j * CommonVars.GRID_SIZE),
                            firstPosition.y + ((CommonVars.GRID_HEIGHT + level) * CommonVars.GRID_SIZE));

                        float waitTime = .4f;// * (CommonVars.GRID_HEIGHT + level - i);
                        if (longestTime < waitTime) longestTime = waitTime;
                    
                        newStone.gameObject.transform.DOLocalMove(nexPosition, waitTime).SetEase(Ease.OutBounce);;

                        level++;
                    }
                }
            }
        }

        return longestTime;
    }
    
    #endregion
    
    #region auto clear bomb

    public void DoStartFinish()
    {
        Debug.Log("Do Start Finish");
        StartCoroutine(CleanUpBombs());
    }

    private IEnumerator CleanUpBombs()
    {
        while (isLocked)
        {
            yield return new WaitForSeconds(0.2f);
        }
        
        int numClean = 0;
        while (ExplodeAllBombs())
        {
            numClean++;
            CleanMatchedBlocks();
            DestroyedBlocks();
            yield return new WaitForSeconds(0.3f);
                
            float duration = MovedDestroyedStoneBlocks();
            yield return new WaitForSeconds(duration);
                
            while (CheckBlock(_blocks))
            {
                numClean++;
                CleanMatchedBlocks();
                DestroyedBlocks();
                yield return new WaitForSeconds(0.3f);
                
                duration = MovedDestroyedStoneBlocks();
                yield return new WaitForSeconds(duration);
            }
        }

        if (numClean > 2)
        {
            SoundController.Instance.PlayRandomKittenSFX();
            UIController.Instance.StartAnimateBonus(numClean);
        }
        
        yield return new WaitForSeconds(1f);
        GameController.Instance.CheckSwitchScene();
    }

    private bool ExplodeAllBombs()
    {
        bool hasBomb = false;
        int numBomb = 0;
        for (int i = 0; i < CommonVars.GRID_HEIGHT; i++)
        {
            for (int j = 0; j < CommonVars.GRID_WIDTH; j++)
            {
                Stone stone = _puzzleBlocks[i][j];
                if (stone.isColorBomb)
                {
                    RandomCleanBomb(i,j);
                    hasBomb = true;
                }
                else if (stone.isHorizontalBomb)
                {
                    CleanHorizontalBomb(i, j);
                    hasBomb = true;
                }
                else if (stone.isVerticalBomb)
                {
                    CleanVerticalBomb(i, j);
                    hasBomb = true;
                }
            }
        }
        Debug.Log("TOtal bomb left:"+numBomb);
        return hasBomb;
    }

    private void RandomCleanBomb(int posX, int posY)
    {
        int colorPicked = Random.Range(1, 5);
                    
        destroyedBlock = new int[CommonVars.GRID_HEIGHT,CommonVars.GRID_WIDTH] ;
        for (int i = 0; i < CommonVars.GRID_HEIGHT; i++)
        {
            for (int j = 0; j < CommonVars.GRID_WIDTH; j++)
            {
                if (_blocks[i][j] == colorPicked)
                {
                    destroyedBlock[i, j] = CommonVars.MARK_DESTROYED;
                    GameController.Instance.CreateTrailObject(_puzzleBlocks[posX][posY].transform.position,
                        _puzzleBlocks[i][j].transform.position);
                }
            }
        }
        destroyedBlock[posX, posY] = CommonVars.MARK_DESTROYED;
        playSoundBomb = true;
    }

    private void CleanHorizontalBomb(int posX, int posY)
    {
        for (int index = 0; index < CommonVars.GRID_WIDTH; index++)
        {
            if (index != posY)
            {
                if (_puzzleBlocks[posX][index].isVerticalBomb)
                {
                    _puzzleBlocks[index][posY].isVerticalBomb = false;
                    CleanVerticalBomb(posX,index);
                }
                else if (_puzzleBlocks[posX][index].isColorBomb)
                {
                    _puzzleBlocks[index][posY].isColorBomb = false;
                    RandomCleanBomb(posX, index);
                }
            }
            
            destroyedBlock[posX, index] = CommonVars.MARK_DESTROYED;
        }

        GameController.Instance.CreateSmokeObject(
            _puzzleBlocks[posX][posY].transform.position,
            _puzzleBlocks[posX][0].transform.position);
        GameController.Instance.CreateSmokeObject(
            _puzzleBlocks[posX][posY].transform.position,
            _puzzleBlocks[posX][CommonVars.GRID_WIDTH - 1].transform.position);
        playSoundWind = true;
    }

    private void CleanVerticalBomb(int posX, int posY)
    {
        for (int index = 0; index < CommonVars.GRID_HEIGHT; index++)
        {
            if (index != posX)
            {
                if (_puzzleBlocks[index][posY].isHorizontalBomb)
                {
                    _puzzleBlocks[index][posY].isHorizontalBomb = false;
                    CleanHorizontalBomb(index, posY);
                }
                else if (_puzzleBlocks[index][posY].isColorBomb)
                {
                    _puzzleBlocks[index][posY].isColorBomb = false;
                    RandomCleanBomb(index, posY);
                }
            }

            destroyedBlock[index, posY] = CommonVars.MARK_DESTROYED;
        }
        
        GameController.Instance.CreateSmokeObject(
            _puzzleBlocks[posX][posY].transform.position,
            _puzzleBlocks[0][posY].transform.position);
        GameController.Instance.CreateSmokeObject(
            _puzzleBlocks[posX][posY].transform.position,
            _puzzleBlocks[CommonVars.GRID_HEIGHT - 1][posY].transform.position);
        playSoundWind = true;
    }
    
    #endregion
}
