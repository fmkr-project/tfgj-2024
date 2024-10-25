using System;
using System.Collections;
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
        private readonly List<string> _diffMenuItemNames = new List<string> {"Easy", "Less easy", "Akyuu", "Back!"};

        public const float StartYAnchor = 0.54f;

        private void Start()
        {
            foreach (var itemName in _mainMenuItemNames)
            {
                MakeNewMenuItem(itemName);
            }
        }

        public IEnumerator ChangeMenuState(MenuState newState)
        {
            if (menuState == newState) yield break;

            if (newState == MenuState.DiffMenu)
            {
                menuState = MenuState.DiffMenu;
                for (var i = 0; i < GetMainMenuItemCount(); i++)
                {
                    StartCoroutine(_menuItems[i].Yoink(_diffMenuItemNames[i]));
                }
                
                yield break;
            }

            if (newState == MenuState.MainMenu)
            {
                menuState = MenuState.MainMenu;
                for (var i = 0; i < GetDiffMenuItemCount(); i++)
                {
                    StartCoroutine(_menuItems[i].Yoink(_mainMenuItemNames[i]));
                }
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
            return _diffMenuItemNames.Count;
        }

        public string GetTextUnderArrow(int pos)
        {
            try
            {
                return menuState == MenuState.MainMenu ? _mainMenuItemNames[pos] : _diffMenuItemNames[pos];
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.LogWarning("Invalid arrow position");
                return "";
            }
        }
    }
}
