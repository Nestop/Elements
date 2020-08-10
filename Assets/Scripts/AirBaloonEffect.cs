using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameLogic;

namespace GameEffects
{
    public class AirBaloonEffect : MonoBehaviour
    {
        [SerializeField] private int count = 3;
        [SerializeField] private float scale = 1f;
        [SerializeField] private float minSpawnTime = 0f;
        [SerializeField] private float maxSpawnTime = 3f;
        [SerializeField] private float minSpeed = 1f;
        [SerializeField] private float maxSpeed = 1f;
        [SerializeField] private Sprite[] sprites = default;

        private Transform[] objPool;
        private SpriteRenderer[] objPoolSprites;
        private int[] objDirection;
        private float[] objSpeed;
        private float[] objSinusOffset;
        private bool[] objLaunched;
        

        private float leftBorder, rightBorder, topBorder, bottomBorder;
        private float topSpawnBorder, bottomSpawnBorder;

        private void Start()
        {
            Camera cam = GameManager.instance.MainCamera;

            leftBorder   = cam.ScreenToWorldPoint(Vector3.zero).x - 1f;
            bottomBorder = cam.ScreenToWorldPoint(Vector3.zero).y;
            rightBorder  = cam.ScreenToWorldPoint(new Vector3(Screen.width,0f,0f)).x + 1f;
            topBorder    = cam.ScreenToWorldPoint(new Vector3(0f,Screen.height,0f)).y + 2f;

            bottomSpawnBorder = cam.ScreenToWorldPoint(new Vector3(0f,Screen.height*0.4f)).y;
            topSpawnBorder    = cam.ScreenToWorldPoint(new Vector3(0f,Screen.height*0.9f,0f)).y;

            objPool = new Transform[count];
            objPoolSprites = new SpriteRenderer[count];
            objDirection = new int[count];
            objLaunched = new bool[count];
            objSpeed = new float[count];
            objSinusOffset = new float[count];

            for (int i = 0; i < count; i++)
            {
                objPool[i] = new GameObject("balloon",typeof(SpriteRenderer)).transform;
                objPool[i].transform.SetParent(transform);
                objPool[i].transform.localScale = Vector3.one*scale;
                objPoolSprites[i] = objPool[i].GetComponent<SpriteRenderer>();
                objPoolSprites[i].sortingOrder = -100;
                LaunchBalloon(i);
            }
        }

        private void LaunchBalloon(int i)
        {
            StartCoroutine(LaunchBalloonCoroutine(i,Random.Range(minSpawnTime,maxSpawnTime)));
        }

        private IEnumerator LaunchBalloonCoroutine(int i, float time)
        {
            yield return new WaitForSeconds(time);

            objLaunched[i] = true;
            objPoolSprites[i].sprite = sprites[ Random.Range(0, sprites.Length) ];
            Vector3 position = new Vector3();
            objDirection[i] = (int)Mathf.Sign(Random.Range(-1,1));
            objSpeed[i] = Random.Range(minSpeed,maxSpeed);
            objSinusOffset[i] = Random.Range(0f,15f);
            position.x = objDirection[i] > 0 ? leftBorder: rightBorder;
            position.y = Random.Range(bottomSpawnBorder, topSpawnBorder);
            objPool[i].position = position;
        }

        void Update()
        {
            for( int i = 0; i<count; i++)
            {
                Vector3 position = objPool[i].position;
                position.x += objDirection[i]*objSpeed[i]*Time.deltaTime;
                position.y += (0.0015f*Mathf.Sin(Time.time*0.4f+objSinusOffset[i]))*objSpeed[i];
                objPool[i].position = position; 
                if(objLaunched[i] && (position.x<leftBorder || position.x>rightBorder))
                {
                    objLaunched[i] = false;
                    LaunchBalloon(i);
                }
                //objPoolSprites[i].bounds.
            }
        }
    }
}
