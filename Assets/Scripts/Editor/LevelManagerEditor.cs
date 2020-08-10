using UnityEngine;
using UnityEditor;

using GameLogic;

[CustomEditor(typeof(LevelManager))]
public class EGLevel : Editor
{

    private LevelManager gameLevel;
    private GameManager gameManager;

    private int brushElementID = -1; 

    private const int BUTTON_SIZE = 50;

    private void OnEnable()
    {
        gameLevel = target as LevelManager;
        gameManager = GameManager.Get();

        if (gameLevel.ElementsID == null)
        {
            gameLevel.ElementsID = new int[0];
        }
    }

    public override void OnInspectorGUI()
    {
        ShowInputData();
        if(ShowElementsPalette() == false) 
        {
            return;
        }
        CheckLevelSize();
        ShowLevel();       
    }

    private void ShowInputData()
    {
        gameLevel.Width = EditorGUILayout.IntField("Ширина", gameLevel.Width);
        gameLevel.Height = EditorGUILayout.IntField("Высота", gameLevel.Height);
        gameLevel.MarginHorizontal = EditorGUILayout.FloatField("Горизонтальный отступ", gameLevel.MarginHorizontal);
        gameLevel.Steps = EditorGUILayout.IntField("Ходов для победы", gameLevel.Steps);
        gameLevel.GameEffect = (GameObject)EditorGUILayout.ObjectField("Эффекты", gameLevel.GameEffect, typeof(GameObject), false);
        gameLevel.Background = (Sprite)EditorGUILayout.ObjectField("Фон", gameLevel.Background, typeof(Sprite), false);
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
        int elementsCount = gameLevel.Height * gameLevel.Width;
        if (gameLevel.ElementsID.GetLength(0) != elementsCount)
        {
            gameLevel.ElementsID = new int[elementsCount];
            for(int i = 0; i < gameLevel.ElementsID.Length; i++)
            {
                gameLevel.ElementsID[i] = -1;
            }
        }
    }

    private void ShowLevel()
    {
        GUILayout.Label("Уровень:");
        for (int i = 0; i < gameLevel.Height; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = i * gameLevel.Width; j < (i + 1) * gameLevel.Width; j++)
            {
                if (gameLevel.ElementsID[j] == -1)
                {
                    if (GUILayout.Button("", GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE)))
                    {
                        gameLevel.ElementsID[j] = brushElementID; 
                    }
                    continue;
                }
                if (GUILayout.Button(gameManager.Elements[gameLevel.ElementsID[j]].Icon, GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE)))
                {
                    gameLevel.ElementsID[j] = brushElementID;
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
