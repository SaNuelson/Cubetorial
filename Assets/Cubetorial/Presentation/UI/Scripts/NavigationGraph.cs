using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cubetorial.Presentation.UI.Scripts
{
    [CreateAssetMenu(fileName = "NavigationGraph", menuName = "Cubetorial/UI Navigation Graph", order = 20)]
    public sealed class NavigationGraph : ScriptableObject
    {
        public string initialScreenId;
        public List<NavigationScreen> screens = new();

        public NavigationScreen FindScreen(string screenId)
        {
            return screens.Find(screen => screen.id == screenId);
        }
    }

    [Serializable]
    public sealed class NavigationScreen
    {
        public string id;
        public VisualTreeAsset asset;
        public List<NavigationLink> links = new();
    }

    [Serializable]
    public sealed class NavigationLink
    {
        public string buttonName;
        public string targetScreenId;
    }
}
