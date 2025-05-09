using System;
using System.Collections;
using DG.Tweening;
using Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Stone : MonoBehaviour, IPuzzleBlock
{
    [SerializeField] TextMeshProUGUI tmpPoint;

    private GameObject _catObject;
    private Animator _animator;

    public int _catType;

    public int _posX = 0;
    public int _posY = 0;
    private bool _moved = false;

    public bool isHorizontalBomb = false;
    public bool isVerticalBomb = false;
    public bool isColorBomb = false;

    private Camera mainCamera;

    private Vector3 _firstLocalPosition;

    private void Awake()
    {
        _firstLocalPosition = tmpPoint.rectTransform.localPosition;
    }

    private void Start()
    {
        mainCamera ??= Camera.main;
    }

    public void InitCat(int catNumber, int posX, int posY, bool horiBomb = false, bool vertiBomb = false,
        bool colorBomb = false)
    {
        isHorizontalBomb = horiBomb;
        isVerticalBomb = vertiBomb;
        isColorBomb = colorBomb;

        tmpPoint.gameObject.SetActive(false);
        tmpPoint.rectTransform.localPosition = _firstLocalPosition;

        _catType = catNumber;

        Transform stone;
        if (isHorizontalBomb)
        {
            _catType = 5;
            stone = StoneFactory.Instance.GetHorizontal();
        
        }
        else if (isVerticalBomb)
        {
            _catType = 6;
            stone = StoneFactory.Instance.GetVertical();
        
        }
        else if (isColorBomb)
        {
            _catType = 7;
            stone = StoneFactory.Instance.GetBomb();
        }
        else
        {
            stone = StoneFactory.Instance.GetStone(_catType - 1);
        }
        stone.SetParent(this.transform);
        stone.localPosition = Vector3.zero;
        stone.gameObject.SetActive(true);
        this.transform.localScale = Vector3.one;
        
        _catObject = stone.gameObject;
        _animator = stone.GetComponent<Animator>();
        
        SetBlock(posX, posY);
    }

    public void PlayDestroyed()
    {
        _catObject.transform.SetParent(null);
        _catObject.SetActive(false);
        if (_catType == 5)
        {
            StoneFactory.Instance.ReturnHorizontal(_catObject.transform);
        }
        else if (_catType == 6)
        {
            StoneFactory.Instance.ReturnVertical(_catObject.transform);
        }
        else if (_catType == 7)
        {
            StoneFactory.Instance.ReturnBomb(_catObject.transform);
        }
        else
        {
            StoneFactory.Instance.ReturnStone(_catType - 1, _catObject.transform);
        }

        Transform splash = VFXFactory.Instance.GetSplashObject();
        splash.transform.position = this.transform.position;

        StartCoroutine(HidePuzzle());
    }
    
    public void PlayUpgrade(bool horiBomb = false, bool vertiBomb = false, bool colorBomb = false)
    {
        isHorizontalBomb = horiBomb;
        isVerticalBomb = vertiBomb;
        isColorBomb = colorBomb;

        _catObject.transform.SetParent(null);
        _catObject.SetActive(false);
        if (_catType == 5)
        {
            StoneFactory.Instance.ReturnHorizontal(_catObject.transform);
        }
        else if (_catType == 6)
        {
            StoneFactory.Instance.ReturnVertical(_catObject.transform);
        }
        else if (_catType == 7)
        {
            StoneFactory.Instance.ReturnBomb(_catObject.transform);
        }
        else
        {
            StoneFactory.Instance.ReturnStone(_catType - 1, _catObject.transform);
        }

        if (isHorizontalBomb)
        {
            _catType = 5;
            Transform stone = StoneFactory.Instance.GetHorizontal();
            stone.SetParent(this.transform);
            stone.localPosition = Vector3.zero;
            stone.gameObject.SetActive(true);
            _catObject = stone.gameObject;

        }
        else if (isVerticalBomb)
        {
            _catType = 6;
            Transform stone = StoneFactory.Instance.GetVertical();
            stone.SetParent(this.transform);
            stone.localPosition = Vector3.zero;
            stone.gameObject.SetActive(true);
            _catObject = stone.gameObject;

        }
        else if (isColorBomb)
        {
            _catType = 7;
            Transform stone = StoneFactory.Instance.GetBomb();
            stone.SetParent(this.transform);
            stone.localPosition = Vector3.zero;
            stone.gameObject.SetActive(true);
            _catObject = stone.gameObject;
        }
        
    }

    IEnumerator HidePuzzle()
    {
        yield return new WaitForSeconds(.1f);

        float value = GameController.Instance.levelScore * 100;
        tmpPoint.text = CommonVars.GetValueString(value);
        tmpPoint.gameObject.SetActive(true);
        //tmpPoint.rectTransform.DOLocalMoveY(tmpPoint.rectTransform.localPosition.y + 1f, 0.2f);

        yield return new WaitForSeconds(.3f);
        this.transform.position = new Vector2(10f, 10f);
    }

    public void SetBlock(int posX, int posY)
    {
        _posX = posX;
        _posY = posY;
    }

    private Vector2 _touchPosition;

    private void OnMouseDown()
    {
        if (GameController.Instance._gameMode == CommonVars.GameMode.Play)
        {
            _moved = true;
            _touchPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseDrag()
    {
        if (!_moved) return;

        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diffPosition = mousePosition - _touchPosition;

        float diffHorizontal = Mathf.Abs(diffPosition.x);
        float diffVertical = Mathf.Abs(diffPosition.y);

        int nexPosX, nexPosY;
        nexPosX = _posX;
        nexPosY = _posY;

        if (diffHorizontal > diffVertical && diffHorizontal >= CommonVars.THRESHOLD_MOVE)
        {
            if (diffPosition.x > 0)
            {
                nexPosX = _posX + 1;
                if (nexPosX >= CommonVars.GRID_WIDTH) nexPosX = _posX;
            }
            else if (diffPosition.x < 0)
            {
                nexPosX = _posX - 1;
                if (nexPosX < 0) nexPosX = _posX;
            }
        }
        else if (diffVertical >= diffHorizontal && diffVertical >= CommonVars.THRESHOLD_MOVE)
        {
            if (diffPosition.y > 0)
            {
                nexPosY = _posY + 1;
                if (nexPosY >= CommonVars.GRID_HEIGHT) nexPosY = _posY;
            }
            else if (diffPosition.y < 0)
            {
                nexPosY = _posY - 1;
                if (nexPosY < 0) nexPosY = _posY;
            }
        }

        if (_posX != nexPosX || _posY != nexPosY)
        {
            _moved = false;
            StartCoroutine(PuzzleController.Instance.SequenceSwitchBlock(_posX, _posY, nexPosX, nexPosY));
        }
    }

    
    private void OnMouseUp()
    {
        _moved = false;
        _touchPosition = this.transform.position;
        this.transform.DOShakeScale(0.4f, CommonVars.shakeStrength);
    }
}
