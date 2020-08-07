using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelManager))]
public class EGLevel : Editor
{

    private LevelManager gl;
    private GameManager gm;

    private int s = -1; // Номер элемента закраски

    private int buttonSize = 50;

    private void OnEnable()
    {
        gl = (LevelManager)target;
        gm = GameManager.Get();

        if (gl.ElementsID == null)
        {
            gl.ElementsID = new int[0];
        }
    }

    public override void OnInspectorGUI()
    {
        gl.Width = EditorGUILayout.IntField("Ширина", gl.Width);
        gl.Height = EditorGUILayout.IntField("Высота", gl.Height);
        gl.MarginHorizontal = EditorGUILayout.FloatField("Горизонтальный отступ", gl.MarginHorizontal);
        gl.Steps = EditorGUILayout.IntField("Ходов для победы", gl.Steps);
        gl.GameEffects = (GameObject)EditorGUILayout.ObjectField("Эффекты", gl.GameEffects, typeof(GameObject), true);
        gl.Background = (Sprite)EditorGUILayout.ObjectField("Фон", gl.Background, typeof(Sprite), true);

        GUILayout.Label("Элементы закраски:");
        if (gm.Elements.Length == 0)
        {
            EditorGUILayout.HelpBox("Необходимо добавить элементы закраски в GameManager.", MessageType.Warning);
            EditorGUILayout.ObjectField(gm.gameObject, typeof(GameManager), false);
            return;
        }
        // Вывод элементов закраски
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
        {
            s = -1;
        }
        for (int i = 0; i < gm.Elements.Length; i++)
        {
            if (gm.Elements[i] != null)
                if (GUILayout.Button(gm.Elements[i].Icon.texture, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                {
                    s = i;
                }
        }
        GUILayout.EndHorizontal();

        // Проверка на соотвествие размера карты
        if (gl.ElementsID.GetLength(0) != gl.Height * gl.Width)
        {
            gl.ElementsID = new int[gl.Height * gl.Width];
        }

        // Построение карты
        GUILayout.Label("Карта:");
        for (int i = 0; i < gl.Height; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = i * gl.Width; j < (i + 1) * gl.Width; j++)
            {
                if (gl.ElementsID[j] == -1)
                {
                    if (GUILayout.Button("", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                    {
                        gl.ElementsID[j] = s;
                    }
                    continue;
                }
                if (GUILayout.Button(gm.Elements[gl.ElementsID[j]].Icon.texture, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                {
                    gl.ElementsID[j] = s;
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
