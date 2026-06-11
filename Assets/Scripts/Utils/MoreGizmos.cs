using UnityEditor;
using UnityEngine;

namespace Utils
{
    public static class MoreGizmos
    {
        public static void HighlightObjects(params GameObject[] objects)
        {
            using (new Handles.DrawingScope(Color.yellow))
            {
                foreach (var go in objects)
                {
                    Handles.DrawWireCube(go.transform.position, Vector3.one * 1.1f);
                }
            }
        }
    }
}