using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameGUI;

namespace GameLogic
{
    [RequireComponent(typeof(LevelManager))]
    public class Game : MonoBehaviour
    {
        private Element[,] elements;

        private int width, height, steps4win, steps = 0;
        private float marginHorizontal, elementScale, unitsOnHorizontal;
        private bool needDestroy = false;
        private bool blockControl = false;
        private bool needRefresh = false;
        private bool isPlayerMove = false;
        private bool twoElements;
        private GameObject gameEffect;
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
            if (LevelManager.instance.GameEffect!= null && gameEffect == null)
            {
                gameEffect = Instantiate(LevelManager.instance.GameEffect, Vector3.zero, Quaternion.identity);
            }

            this.elements = elements;
            width = LevelManager.instance.Width;
            height = LevelManager.instance.Height;
            marginHorizontal = LevelManager.instance.MarginHorizontal;
            elementScale = LevelManager.instance.ElementScale;
            unitsOnHorizontal = LevelManager.instance.UnitsOnHorizontal;
            steps4win = LevelManager.instance.Steps;
            blockControl = false;
            steps = 0;
        }

        public void Swipe(int i1, int j1, int i2, int j2)
        {
            if (blockControl) return;
            if (elements[i1, j1] == null || i2 < 0 || i2 >= height || j2 < 0 || j2 >= width) return;
            blockControl = true;
            isPlayerMove = true;
            if (( i2 + 1 < height && elements[i2 + 1, j2] == null ) || ( i1 - 1 > -1 && elements[i1 - 1, j1] != null ))
            {
                needRefresh = true;
            }
            if (elements[i2, j2] == null)
            {
                if (j1 == j2) 
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
            steps++;
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
                Vector3 startPos1 = GetPosition(i1, j1);
                Vector3 startPos2 = GetPosition(i2, j2);
                Vector3 delta = startPos2 - startPos1;
                Vector3 offset = delta.normalized  * speed * Time.deltaTime;

                float deltaX, deltaY;
                deltaX = Mathf.Abs(trans1.localPosition.x - startPos2.x);
                deltaY = Mathf.Abs(trans1.localPosition.y - startPos2.y);
                trans1.localPosition += offset;

                delta = new Vector3(delta.x == 0 ? 0 : delta.x / Mathf.Abs(delta.x), delta.y == 0 ? 0 : delta.y / Mathf.Abs(delta.y), 0f);
                while (deltaX > Mathf.Abs(trans1.localPosition.x - startPos2.x) || deltaY > Mathf.Abs(trans1.localPosition.y - startPos2.y))
                {
                    yield return null;
                    deltaX = Mathf.Abs(trans1.localPosition.x - startPos2.x);
                    deltaY = Mathf.Abs(trans1.localPosition.y - startPos2.y);

                    trans1.localPosition += offset;
                }
                trans1.localPosition = startPos2;
            }
            else
            {
                Transform trans1 = elements[i1, j1].transform;
                Transform trans2 = elements[i2, j2].transform;
                Vector3 origPos1 = GetPosition(i1, j1);
                Vector3 origPos2 = GetPosition(i2, j2);
                Vector3 delta = origPos2 - origPos1;
                Vector3 offset = delta * speed * Time.deltaTime;

                float deltaX, deltaY;
                deltaX = Mathf.Abs(trans1.localPosition.x - origPos2.x);
                deltaY = Mathf.Abs(trans1.localPosition.y - origPos2.y);
                trans1.localPosition += offset;
                trans2.localPosition -= offset;
                delta = new Vector3(delta.x == 0 ? 0 : delta.x / Mathf.Abs(delta.x), delta.y == 0 ? 0 : delta.y / Mathf.Abs(delta.y), 0f);
                while (deltaX > Mathf.Abs(trans1.localPosition.x - origPos2.x) || deltaY > Mathf.Abs(trans1.localPosition.y - origPos2.y))
                {
                    yield return null;
                    deltaX = Mathf.Abs(trans1.localPosition.x - origPos2.x);
                    deltaY = Mathf.Abs(trans1.localPosition.y - origPos2.y);

                    trans1.localPosition += offset;
                    trans2.localPosition -= offset;
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
            List<Element> collapseItemHorizontal = new List<Element>();
            List<Element> collapseItemVertical   = new List<Element>();
            int matchHorizontal = 0;
            int matchVertical = 0;
            int id = elements[i, j].id;

            for (int k = i + 1; k < height; k++)
            { // Проверка на совпадения от элемента вниз
                if(!Matches(elements[k, j], id, ref collapseItemVertical, ref matchVertical)) 
                {
                    break;
                }
            }
            for (int k = i - 1; k >= 0; k--)
            { // Проверка на совпадения от элемента вверх
                if(!Matches(elements[k, j], id, ref collapseItemVertical, ref matchVertical)) 
                {
                    break;
                }
            }
            for (int k = j + 1; k < width; k++)
            { // Проверка на совпадения от элемента вправо
                if(!Matches(elements[i, k], id, ref collapseItemHorizontal, ref matchHorizontal)) 
                {
                    break;
                }
            } // Проверка на совпадения от элемента влево
            for (int k = j - 1; k >= 0; k--)
            {
                if(!Matches(elements[i, k], id, ref collapseItemHorizontal, ref matchHorizontal)) 
                {
                    break;
                }
            }

            if (CheckOnCollapse(collapseItemHorizontal, matchHorizontal) | CheckOnCollapse(collapseItemVertical, matchVertical))
            {
                elements[i, j].Destroy = true;
                needDestroy = true;
            }
        }

        private bool Matches(Element element, int id, ref List<Element> collection, ref int matches)
        {
            if (element != null && id == element.id)
                {
                    collection.Add(element);
                    matches++;
                    return true;
                }
            else return false;
        }

        private bool CheckOnCollapse(List<Element> elements, int matches)
        {
            if (matches >= 2)
            {
                foreach (var element in elements)
                {
                    element.Destroy = true;
                }
                return true;
            }
            else return false;
        }

        private void CheckAllMatches()
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    if (elements[i, j] != null)
                        CheckMatch(i, j);
                }
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
            Invoke("CheckAllMatches", elementScale * maxShifts / speed);
        }

        private Vector3 GetPosition(int i, int j)
        {
            float x = marginHorizontal - unitsOnHorizontal / 2f + elementScale / 2f + j * elementScale;
            float y = (height - 1) * elementScale / 2f - i * elementScale;
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
            if (steps >= steps4win)
            {
                blockControl = true;
                GUIManager.instance.Win(false);
            }
        }
    }
}