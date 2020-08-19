using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using GameLogic;

[CustomEditor(typeof(LevelManager))]
public class EGLevel : Editor
{

    private LevelManager levelManager;
    Level currLvl;
    private GameManager gameManager;

    private int brushElementID = -1; 

    private const int BUTTON_SIZE = 50;

    private void OnEnable()
    {
        levelManager = target as LevelManager;
        gameManager = GameManager.Get();

        if (levelManager.levels == null)
        {
            levelManager.levels = new List<Level>();
        }
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(string.Format("Уровень {0} ",levelManager.CurrentLevelNum));
        if (GUILayout.Button("<-",GUILayout.Width(30f)))
        {
            levelManager.CurrentLevelNum--;
        }
        levelManager.CurrentLevelNum = EditorGUILayout.IntField( levelManager.CurrentLevelNum,GUILayout.Width(30f));
        if (GUILayout.Button("->",GUILayout.Width(30f)))
        {
            levelManager.CurrentLevelNum++;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);

        if(levelManager.CurrentLevelNum > levelManager.levels.Count)
        {
            if (GUILayout.Button("Создать уровень"))
            {
                levelManager.levels.Add(new Level());
                levelManager.levels[levelManager.CurrentLevelNum-1].ElementsID = new int[0];
            }
            return;  
        }
        ShowLevelData();
        if(ShowElementsPalette() == false) 
        {
            return;
        }
        CheckLevelSize();
        ShowLevel();       
    }

    private void ShowLevelData()
    {
        currLvl = levelManager.levels[levelManager.CurrentLevelNum-1];
        currLvl.Width = EditorGUILayout.IntField("Ширина", currLvl.Width);
        currLvl.Height = EditorGUILayout.IntField("Высота", currLvl.Height);
        currLvl.MarginHorizontal = EditorGUILayout.FloatField("Горизонтальный отступ", currLvl.MarginHorizontal);
        currLvl.Steps = EditorGUILayout.IntField("Ходов для победы", currLvl.Steps);
        currLvl.GameEffect = (GameObject)EditorGUILayout.ObjectField("Эффекты", currLvl.GameEffect, typeof(GameObject), false);
        currLvl.Background = (Sprite)EditorGUILayout.ObjectField("Фон", currLvl.Background, typeof(Sprite), false);
    }

    private bool ShowElementsPalette()
    {
        GUILayout.Label("Элементы закраски:");
        if (gameManager.Elements.Length == 0)
        {
            EditorGUILayout.HelpBox("Необходимо добавить элементы закраски в GameManager.", MessageType.Warning);
            EditorGUILayout.ObjectField(gameManager.gameObject, typeof(GameManager), false);
            return false;
        }
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete", GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE)))
        {
            brushElementID = -1;
        }
        for (int i = 0; i < gameManager.Elements.Length; i++)
        {
            if (gameManager.Elements[i] != null)
                if (GUILayout.Button(gameManager.Elements[i].Icon, GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE)))
                {
                    brushElementID = i;
                }
        }
        GUILayout.EndHorizontal();
        return true;
    }

    private void CheckLevelSize()
    {
        int elementsCount = currLvl.Height * currLvl.Width;
        if (currLvl.ElementsID.GetLength(0) != elementsCount)
        {
            currLvl.ElementsID = new int[elementsCount];
            for(int i = 0; i < currLvl.ElementsID.Length; i++)
            {
                currLvl.ElementsID[i] = -1;
            }
        }
    }

    private void ShowLevel()
    {
        GUILayout.Label("Уровень:");
        for (int i = 0; i < currLvl.Height; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = i * currLvl.Width; j < (i + 1) * currLvl.Width; j++)
            {
                if (currLvl.ElementsID[j] == -1)
                {
                    if (GUILayout.Button("", GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE)))
                    {
                        currLvl.ElementsID[j] = brushElementID; 
                    }
                    continue;
                }
                if (GUILayout.Button(gameManager.Elements[currLvl.ElementsID[j]].Icon, GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE)))
                {
                    currLvl.ElementsID[j] = brushElementID;
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
