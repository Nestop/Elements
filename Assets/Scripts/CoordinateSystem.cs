using UnityEngine;

namespace GameLogic
{
    static public class CoordinateSystem
    {
        static public int Width => _width;
        static public int Height => _height;
        static public float ElementScale => _elementScale;
        static public float ElementPixelScale => _elementPixelScale;
        static public float MarginHorizontal => _marginHorizontal;
        static public float UnitsOnHorizontal => _unitsOnHorizontal;
        static public float UnitsOnVertical => _unitsOnVertical;
        static public float PixelsPerUnit => _pixelsPerUnit;

        static public float LeftCameraBorder => _leftCameraBorder;
        static public float BottomCameraBorder => _bottomCameraBorder;
        static public float RightCameraBorder => _rightCameraBorder;
        static public float TopCameraBorder => _topCameraBorder;
        

        static private int _width;
        static private int _height;
        static private float _elementScale;
        static private float _elementPixelScale;
        static private float _marginHorizontal;
        static private float _unitsOnHorizontal;
        static private float _unitsOnVertical;
        static private float _pixelsPerUnit;

        static private float _leftCameraBorder;
        static private float _bottomCameraBorder;
        static private float _rightCameraBorder;
        static private float _topCameraBorder;

        static public void SetCoordinateSystem(int width, int height, float marginHorizontal)
        {
            _width = width;
            _height = height;
            _marginHorizontal = marginHorizontal;
            Camera cam = GameManager.instance.MainCamera;
            _unitsOnHorizontal =
                cam.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f)).x -
                cam.ScreenToWorldPoint(Vector3.zero).x;
            _unitsOnVertical = 
                cam.ScreenToWorldPoint(new Vector3(0f, Screen.height, 0f)).y -
                cam.ScreenToWorldPoint(Vector3.zero).y;
            _elementScale = (_unitsOnHorizontal - _marginHorizontal * 2f) / _width;
            _pixelsPerUnit = Screen.width / _unitsOnHorizontal;
            _elementPixelScale = _pixelsPerUnit*_elementScale;
            
            _leftCameraBorder   = cam.ScreenToWorldPoint(Vector3.zero).x;
            _bottomCameraBorder = cam.ScreenToWorldPoint(Vector3.zero).y;
            _rightCameraBorder  = cam.ScreenToWorldPoint(new Vector3(Screen.width,0f,0f)).x;
            _topCameraBorder    = cam.ScreenToWorldPoint(new Vector3(0f,Screen.height,0f)).y;
        }

        static public Vector3 GetPosition(int i, int j)
        {
            float x = _marginHorizontal - _unitsOnHorizontal / 2f + _elementScale / 2f + j * _elementScale;
            float y = (_height - 1) * _elementScale / 2f - i * _elementScale;
            return new Vector3(x, y, 0f);
        }

        static public int GetSortingOrder(int i, int j)
        {
            return -(_width * i - j);
        }
    }
}