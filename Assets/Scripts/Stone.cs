using System;
using System.Collections;
using DG.Tweening;
using Interfaces;
using TMPro;
using UnityEngine;

public class Stone : MonoBehaviour, IPuzzleBlock
{
    [SerializeField] GameObject[] catsTransforms;
    [SerializeField] GameObject bombTransform;
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


    public void InitCat(int catNumber, int posX, int posY, bool horiBomb = false, bool vertiBomb = false, bool colorBomb = false)
    {
        mainCamera ??= Camera.main;
        isHorizontalBomb = horiBomb;
        isVerticalBomb = vertiBomb;
        isColorBomb = colorBomb;
        bombTransform.SetActive(isHorizontalBomb || isVerticalBomb);

        tmpPoint.gameObject.SetActive(false);
        tmpPoint.rectTransform.localPosition = _firstLocalPosition;
        
        foreach(GameObject catTrans in catsTransforms)
        {
            catTrans.SetActive(false);
        }

        if (isColorBomb)
        {
            _catType = 7;
            _catObject = catsTransforms[_catType - 1];
            _animator = null;
            _catObject.SetActive(true);
        }
        else
        {
            _catType = catNumber;
            _catObject = catsTransforms[_catType - 1];
            _animator = _catObject.GetComponent<Animator>();
            _catObject.SetActive(true);
        }
        
        SetBlock(posX, posY);
    }

    public void PlayDestroyed()
    {
        if (_animator)
        {
            _animator.Play("smile");
            //this.gameObject.SetActive(false);
        }

        tmpPoint.gameObject.SetActive(true);
        tmpPoint.rectTransform.DOLocalMoveY(tmpPoint.rectTransform.localPosition.y + 1f, 0.2f);
        StartCoroutine(HidePuzzle());
    }

    public void PlayUpgrade(bool horiBomb = false, bool vertiBomb = false, bool colorBomb = false)
    {
        if (!isColorBomb)
        {
            _animator.Play("smile");
        }

        isHorizontalBomb = horiBomb;
        isVerticalBomb = vertiBomb;
        isColorBomb = colorBomb;

        if (isColorBomb)
        {
            foreach(GameObject catTrans in catsTransforms)
            {
                catTrans.SetActive(false);
            }
            _catType = 6;
            _catObject = catsTransforms[_catType - 1];
            _animator = null;
            _catObject.SetActive(true);
        }
        bombTransform.SetActive(isHorizontalBomb || isVerticalBomb);
    }

    IEnumerator HidePuzzle()
    {
        yield return new WaitForSeconds(.2f);
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
        _moved = true;
        _touchPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
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
    }
}
