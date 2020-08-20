using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameGUI;
using CS = GameLogic.CoordinateSystem;

namespace GameLogic
{
    [RequireComponent(typeof(Game))]
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance = null;
        public Game game;
        public int CurrentLevelNum 
        {
            get{ return currentLevelNum;}
            set
            { 
                if (value < 1) { value = 1; }
                if (value > levels.Count) { value = levels.Count+1; }
                currentLevelNum = value;
            }
        }
        public List<Level> levels;
        public int loadedLevelNum;

        private int currentLevelNum = 1;
        private List<GameObject> elementsPool;

        private void Start()
        {
            if (instance == null || instance.Equals(this))
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
            
            elementsPool = new List<GameObject>();
            game = GetComponent<Game>();
        }

        public void LoadLevel(int levelNum)
        {
            loadedLevelNum = levelNum - 1;
            Level level = levels[loadedLevelNum];
            CS.SetCoordinateSystem(level.Width, level.Height, level.MarginHorizontal);
            GUIManager.instance.LoadLevel();
            GameManager gameManager = GameManager.instance;
            gameManager.SwipeScreen.UpdateSize();
            gameManager.Background.sprite = level.Background;
            foreach(var elem in elementsPool)
            {
                elem.SetActive(false);
            }
            GameObject element;
            int elementsPoolNum = 0;
            Element[,] elements = new Element[level.Height, level.Width];
            for (int i = 0; i < level.Height; i++)
                for (int j = 0; j < level.Width; j++)
                {
                    int id = level.Width * i + j;
                    if (level.ElementsID[id] == -1) continue;

                    element = GetElement(elementsPoolNum++);
                    element.SetActive(true);
                    element.transform.localScale = Vector2.one * CS.ElementScale;
                    element.transform.localPosition = CS.GetPosition(i,j);
                    SpriteRenderer sprRend = element.GetComponent<SpriteRenderer>();
                    sprRend.sortingOrder = CS.GetSortingOrder(i, j);
                    Animator animator = element.GetComponent<Animator>();
                    animator.runtimeAnimatorController = gameManager.Elements[level.ElementsID[id]].Controller;
                    animator.SetFloat("Random",Random.Range(0f,1f));
                    elements[i, j] = new Element(level.ElementsID[id], element.transform, sprRend);
                }
            game.GetElements(elements);
        }

        public void LoadNextLevel()
        {
            int levelNum = loadedLevelNum + 2;
            levelNum = levelNum > levels.Count ? 1 : levelNum;
            LoadLevel(levelNum);
        }

        private GameObject GetElement(int i)
        {
            if(i>elementsPool.Count-1)
            {
                GameObject element = new GameObject("Element", typeof(SpriteRenderer), typeof(Animator));
                elementsPool.Add(element);
                return element;
            }
            else
            {
                return elementsPool[i];
            }
        }
    }
}
