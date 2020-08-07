using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private Element[,] elements;

    private int width, height, steps4win, steps = 0;
    private float marginHorizontal, itemScale, unitsOnHorizontal;
    private bool needDestroy = false;
    private bool blockControl = false;
    private bool needRefresh = false;
    private bool isPlayerMove = false;
    private bool twoElements;
    private GameObject gameEffects;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float destroyTime = 1.2f;

    public void GetElements(Element[,] elements)
    {
        if (this.elements != null)
        {
            foreach (var element in this.elements)
            {
                if (element != null)
                    Destroy(element.transform.gameObject);
            }
        }
        if (gameEffects == null)
        {
            gameEffects = Instantiate(LevelManager.instance.GameEffects, Vector3.zero, Quaternion.identity);
        }

        this.elements = elements;
        width = LevelManager.instance.Width;
        height = LevelManager.instance.Height;
        marginHorizontal = LevelManager.instance.MarginHorizontal;
        itemScale = LevelManager.instance.ItemScale;
        unitsOnHorizontal = LevelManager.instance.UnitsOnHorizontal;
        steps4win = LevelManager.instance.Steps;
        blockControl = false;
        steps = 0;
    }

    public void Swipe(int i1, int j1, int i2, int j2)
    {
        if (blockControl) return;
        if (elements[i1, j1] == null || i2 < 0 || i2 >= height || j2 < 0 || j2 >= width) return;
        steps++;
        blockControl = true;
        isPlayerMove = true;
        if (i2 + 1 < height && elements[i2 + 1, j2] == null || (i1 - 1 > -1 && elements[i1 - 1, j1] != null))
        {
            needRefresh = true;
        }
        if (elements[i2, j2] == null)
        {
            if (i1 != i2) 
            {   
                blockControl = false;
                return;
            }
            twoElements = false;
            StartCoroutine(TranslateTo(i1, j1, i2, j2));
        }
        else
        {
            twoElements = true;
            StartCoroutine(TranslateTo(i1, j1, i2, j2));
        }
    }


    private void Fall(int i1, int j1, int i2, int j2)
    {
        if (elements[i2, j2] == null)
        {
            StartCoroutine(TranslateTo(i1, j1, i2, j2));
            elements[i1, j1].spriteRenderer.sortingOrder = -(width * i2 - j2);
            elements[i2, j2] = elements[i1, j1];
            elements[i1, j1] = null;
        }
    }

    IEnumerator TranslateTo(int i1, int j1, int i2, int j2)
    {
        if (elements[i2, j2] == null)
        {

            Transform trans1 = elements[i1, j1].transform;
            Vector3 origPos1 = GetCoord(i1, j1);
            Vector3 origPos2 = GetCoord(i2, j2);
            Vector3 delta = origPos2 - origPos1;

            float deltaX, deltaY;
            deltaX = Mathf.Abs(trans1.localPosition.x - origPos2.x);
            deltaY = Mathf.Abs(trans1.localPosition.y - origPos2.y);
            trans1.localPosition += delta * speed * Time.deltaTime;

            delta = new Vector3(delta.x == 0 ? 0 : delta.x / Mathf.Abs(delta.x), delta.y == 0 ? 0 : delta.y / Mathf.Abs(delta.y), 0f);
            while (deltaX > Mathf.Abs(trans1.localPosition.x - origPos2.x) || deltaY > Mathf.Abs(trans1.localPosition.y - origPos2.y))
            {
                yield return null;
                deltaX = Mathf.Abs(trans1.localPosition.x - origPos2.x);
                deltaY = Mathf.Abs(trans1.localPosition.y - origPos2.y);

                trans1.localPosition += delta * speed * Time.deltaTime;
            }
            trans1.localPosition = origPos2;
        }
        else
        {
            Transform trans1 = elements[i1, j1].transform;
            Transform trans2 = elements[i2, j2].transform;
            Vector3 origPos1 = GetCoord(i1, j1);
            Vector3 origPos2 = GetCoord(i2, j2);
            Vector3 delta = origPos2 - origPos1;

            float deltaX, deltaY;
            deltaX = Mathf.Abs(trans1.localPosition.x - origPos2.x);
            deltaY = Mathf.Abs(trans1.localPosition.y - origPos2.y);
            trans1.localPosition += delta * speed * Time.deltaTime;
            trans2.localPosition -= delta * speed * Time.deltaTime;
            delta = new Vector3(delta.x == 0 ? 0 : delta.x / Mathf.Abs(delta.x), delta.y == 0 ? 0 : delta.y / Mathf.Abs(delta.y), 0f);
            while (deltaX > Mathf.Abs(trans1.localPosition.x - origPos2.x) || deltaY > Mathf.Abs(trans1.localPosition.y - origPos2.y))
            {
                yield return null;
                deltaX = Mathf.Abs(trans1.localPosition.x - origPos2.x);
                deltaY = Mathf.Abs(trans1.localPosition.y - origPos2.y);

                trans1.localPosition += delta * speed * Time.deltaTime;
                trans2.localPosition -= delta * speed * Time.deltaTime;
            }
            trans1.localPosition = origPos2;
            trans2.localPosition = origPos1;
        }
        if (isPlayerMove)
        {
            isPlayerMove = false;
            if (twoElements)
            {
                int sortingOrder = elements[i1, j1].spriteRenderer.sortingOrder;
                elements[i1, j1].spriteRenderer.sortingOrder = elements[i2, j2].spriteRenderer.sortingOrder;

                elements[i2, j2].spriteRenderer.sortingOrder = sortingOrder;
                Element element = elements[i1, j1];
                elements[i1, j1] = elements[i2, j2];
                elements[i2, j2] = element;
                CheckMatch(i1, j1);
                CheckMatch(i2, j2);
                if (needDestroy)
                {
                    DestroyElements();
                    Invoke("Refresh", destroyTime);
                }
                else
                if (needRefresh)
                {
                    Refresh();
                }
                else
                {
                    blockControl = false;
                    Check4win();
                }
            }
            else
            {
                elements[i1, j1].spriteRenderer.sortingOrder = -(width * i2 - j2);
                elements[i2, j2] = elements[i1, j1];
                elements[i1, j1] = null;
                CheckMatch(i2, j2);
                if (needDestroy)
                {
                    DestroyElements();
                    Invoke("Refresh", destroyTime);
                }
                else
                if (needRefresh)
                {
                    Refresh();
                }
                else
                {
                    blockControl = false;
                    Check4win();
                }
            }
        }
    }

    private void CheckMatch(int i, int j)
    {
        List<Element> collapseItemV = new List<Element>(); // Vertical
        List<Element> collapseItemH = new List<Element>(); // Horizontal
        int matchV = 0,
            matchH = 0;
        int id = elements[i, j].id;

        for (int k = i + 1; k < height; k++)
        { // Проверка на совпадения от элемента вниз
            if (elements[k, j] != null && id == elements[k, j].id)
            {
                collapseItemV.Add(elements[k, j]);
                matchV++;
            }
            else break;
        }
        for (int k = i - 1; k >= 0; k--)
        { // Проверка на совпадения от элемента вверх
            if (elements[k, j] != null && id == elements[k, j].id)
            {
                collapseItemV.Add(elements[k, j]);
                matchV++;
            }
            else break;
        }
        for (int k = j + 1; k < width; k++)
        { // Проверка на совпадения от элемента вправо
            if (elements[i, k] != null && id == elements[i, k].id)
            {
                collapseItemH.Add(elements[i, k]);
                matchH++;
            }
            else break;
        } // Проверка на совпадения от элемента влево
        for (int k = j - 1; k >= 0; k--)
        {
            if (elements[i, k] != null && id == elements[i, k].id)
            {
                collapseItemH.Add(elements[i, k]);
                matchH++;
            }
            else break;
        }

        bool isCollapse = false;
        if (matchV > 1)
        {
            isCollapse = true;
            foreach (var collItem in collapseItemV)
            {
                collItem.Destroy = true;
            }
        }
        if (matchH > 1)
        {
            isCollapse = true;
            foreach (var collItem in collapseItemH)
            {
                collItem.Destroy = true;
            }
        }
        if (isCollapse)
        {
            elements[i, j].Destroy = true;
            needDestroy = true;
        }
    }

    private void CheckAllMatches()
    {
        // Проверяем все совпадения
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                if (elements[i, j] != null)
                    CheckMatch(i, j);
            }
        // Если было хоть одно совпадение, то обновляем сетку
        if (needDestroy)
        {
            DestroyElements();
            Invoke("Refresh", destroyTime);
        }
        else
        {
            blockControl = false;
            Check4win();

        }
    }

    private void DestroyElements()
    {
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                if (elements[i, j] != null && elements[i, j].Destroy)
                {
                    elements[i, j].animator.SetBool("Destroy", true);
                }
            }
        Invoke("Destroy", destroyTime);
    }

    private void Destroy()
    {
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                if (elements[i, j] != null && elements[i, j].Destroy)
                {
                    Destroy(elements[i, j].transform.gameObject);
                    elements[i, j] = null;
                }
            }
    }

    private void Refresh()
    {
        needRefresh = false;
        needDestroy = false;
        bool shift = false;
        int shifts = 0, maxShifts = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = height - 1; j > -1; j--)
            {
                if (elements[j, i] == null)
                {
                    shift = true;
                    shifts++;
                    continue;
                }
                if (shift)
                {
                    if (shifts > maxShifts)
                    {
                        maxShifts = shifts;
                    }
                    for (int k = height - 1; k > j; k--)
                    {
                        if (elements[k, i] == null)
                        {
                            twoElements = false;
                            Fall(j, i, k, i);
                            break;
                        }
                    }
                }
            }
            shift = false;
            shifts = 0;
        }
        Invoke("CheckAllMatches", itemScale * maxShifts / speed);
    }

    private Vector3 GetCoord(int i, int j)
    {
        float x = marginHorizontal - unitsOnHorizontal / 2f + itemScale / 2f + j * itemScale;
        float y = (height - 1) * itemScale / 2f - i * itemScale;
        return new Vector3(x, y, 0f);
    }

    private void Check4win()
    {
        bool win = true;
        foreach (var element in elements)
        {
            if (element != null)
            {
                win = false;
                break;
            }
        }
        if (win)
        {
            blockControl = true;
            GUIManager.instance.Win(true);
        }
        else
        if (steps == steps4win)
        {
            blockControl = true;
            GUIManager.instance.Win(false);
        }
    }
}
