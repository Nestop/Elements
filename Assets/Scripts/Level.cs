using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    [System.Serializable]
    public class Level
    {
        public int Width, Height;
        public Sprite Background;
        public GameObject GameEffect;
        public int[] ElementsID;
        public float MarginHorizontal;
        public int Steps;
    }
}
