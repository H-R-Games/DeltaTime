using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A static class for general helpful methods
/// </summary>

namespace rene_roid
{
    public static class Helpers
    {
        /// <summary>
        /// Return the camera
        /// </summary>
        private static Camera _camera;
        public static Camera Camera
        {
            get
            {
                if (_camera == null) _camera = Camera.main;
                return _camera;
            }
        }


        /// <summary>
        /// Cache WaitForSeconds
        /// </summary>
        private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
        public static WaitForSeconds GetWait(float time)
        {
            if (WaitDictionary.TryGetValue(time, out var wait)) return wait;

            WaitDictionary[time] = new WaitForSeconds(time);
            return WaitDictionary[time];
        }


        /// <summary>
        /// Tells you if the pointer is over any UI element
        /// </summary>
        private static PointerEventData _eventDataCurrentPosition;
        private static List<RaycastResult> _results;
        public static bool IsOverUi()
        {
            _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            _results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
            return _results.Count > 0;
        }


        /// <summary>
        /// Returns canvas item in worldspace
        /// </summary>
        /// <param name="element">Target element in canvas</param>
        /// <returns></returns>
        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera, out var result);
            return result;
        }


        /// <summary>
        /// Destroy all child objects of this transform (Unintentionally evil sounding).
        /// Use it like so:
        /// <code>
        /// transform.DestroyChildren();
        /// </code>
        /// </summary>
        public static void DestroyChildren(this Transform t)
        {
            foreach (Transform child in t) Object.Destroy(child.gameObject);
        }


        /// <summary>
        /// Convert a number in a range to the percentage of the number in that range
        /// Ex: min = 10, max = 20, actual = 15, result = 50%
        /// </summary>
        /// <param name="actualVar"></param>
        /// <param name="minVar"></param>
        /// <param name="maxVar"></param>
        /// <returns></returns>
        public static float FromRangeToPercentage(float actualVar, float minVar, float maxVar) // Converting a number from a range to the percentage of that number in the range
        {
            float result;
            actualVar = Mathf.Clamp(actualVar, minVar, maxVar); // Making sure number doesnt go above or below range to avoid negative or + 100%
            result = ((actualVar - minVar) / (maxVar - minVar)) * 100; // From 0-1 to 0-100

            return result;
        }

        /// <summary>
        /// Same as before but the other way arround
        /// </summary>
        /// <param name="percentage"></param>
        /// <param name="minVar"></param>
        /// <param name="maxVar"></param>
        /// <returns></returns>
        public static float FromPercentageToRange(float percentage, float minVar, float maxVar) // Converting a parcentage to the percentage number of a range
        {
            float result;
            percentage /= 100; // Converting percentage (0 - 100) to 0 - 1
            result = ((maxVar - minVar) * percentage) + minVar;

            return result;
        }

        /// <summary>
        /// Transform from range to range
        /// </summary>
        /// <param name="actualVar"></param>
        /// <param name="range1min"></param>
        /// <param name="range1max"></param>
        /// <param name="range2min"></param>
        /// <param name="range2max"></param>
        /// <returns></returns>
        public static float FromRangeToRange(float actualVar, float range1min, float range1max, float range2min, float range2max)
        {
            float result;

            float transformation1 = FromRangeToPercentage(actualVar, range1min, range1max);
            float trasformation2 = FromPercentageToRange(transformation1, 0, 100);
            result = FromPercentageToRange(trasformation2, range2min, range2max);

            return result;
        }
    }
}
