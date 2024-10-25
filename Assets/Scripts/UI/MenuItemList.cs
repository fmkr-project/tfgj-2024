using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{
    public enum MenuState
    {
        MainMenu,
        DiffMenu
    }
    
    public class MenuItemList : MonoBehaviour
    {
        [SerializeField] private GameObject menuItemPrefab;
        private List<MenuItem> _menuItems = new();
        public MenuState menuState = MenuState.MainMenu;
        
        private readonly List<string> _mainMenuItemNames = new List<string> { "Play!", "Collection", "Settings", "Quit!" };

        public const float StartYAnchor = 0.5f;

        private void Start()
        {
            foreach (var itemName in _mainMenuItemNames)
            {
                MakeNewMenuItem(itemName);
            }
        }

        private void MakeNewMenuItem(string menuItemName)
        {
            var newObject = Instantiate(menuItemPrefab, transform);
            newObject.SetActive(true);
            var newMenuItem = newObject.GetComponent<MenuItem>();
            newMenuItem.SetStartYAnchor(StartYAnchor - 0.12f * _menuItems.Count);
            newMenuItem.SetLabel(menuItemName);
            _menuItems.Add(newMenuItem);
        }

        public int GetMainMenuItemCount()
        {
            return _mainMenuItemNames.Count;
        }

        public int GetDiffMenuItemCount()
        {
            // TODO 
            return 2;
        }
    }
}
