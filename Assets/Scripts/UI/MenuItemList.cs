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
        
        private readonly List<string> _mainMenuItemNames = new List<string> { "Tutorial", "Play!", "Collection", "Credits", "Quit!" };
        private readonly List<string> _diffMenuItemNames = new List<string> {"Easy", "Less easy", "Akyuu", "Back!"};

        public const float EndYAnchor = 0.18f;

        private void Start()
        {
            foreach (var itemName in _mainMenuItemNames)
            {
                MakeNewMenuItem(itemName, _mainMenuItemNames.Count);
            }
        }

        public IEnumerator ChangeMenuState(MenuState newState)
        {
            if (menuState == newState) yield break;

            if (newState == MenuState.DiffMenu)
            {
                menuState = MenuState.DiffMenu;
                // First items are hidden
                for (var i = 0; i < GetMainMenuItemCount() - GetDiffMenuItemCount(); i++)
                {
                    StartCoroutine(_menuItems[i].Hide());
                }
                
                // Remaining items are shown with the correct text
                for (var i = GetMainMenuItemCount() - GetDiffMenuItemCount(); i < GetMainMenuItemCount(); i++)
                {
                    var corrected = i - GetMainMenuItemCount() + GetDiffMenuItemCount();
                    StartCoroutine(_menuItems[i].Yoink(_diffMenuItemNames[corrected]));
                }
                
                yield break;
            }

            if (newState == MenuState.MainMenu)
            {
                menuState = MenuState.MainMenu;
                // Show hidden items first
                for (var i = 0; i < GetMainMenuItemCount() - GetDiffMenuItemCount(); i++)
                {
                    StartCoroutine(_menuItems[i].Show());
                }
                
                // Change text for already shown items
                for (var i = GetMainMenuItemCount() - GetDiffMenuItemCount(); i < GetMainMenuItemCount(); i++)
                {
                    StartCoroutine(_menuItems[i].Yoink(_mainMenuItemNames[i]));
                }
            }
        }

        public void DiscreteChangeMenuState(MenuState newState)
        {
            // Same as ChangeMenuState, but does not animate the boxes.
            
            if (menuState == newState) return;

            if (newState == MenuState.DiffMenu)
            {
                menuState = MenuState.DiffMenu;
                // All items stay hidden
                // Relevant items have their text changed
                for (var i = GetMainMenuItemCount() - GetDiffMenuItemCount(); i < GetMainMenuItemCount(); i++)
                {
                    var corrected = i - GetMainMenuItemCount() + GetDiffMenuItemCount();
                    _menuItems[i].SetLabel(_diffMenuItemNames[corrected]);
                }
            }

            if (newState == MenuState.MainMenu)
            {
                menuState = MenuState.MainMenu;
                // Do nothing with hidden items
                // For already shown items, hide them and change their text
                for (var i = GetMainMenuItemCount() - GetDiffMenuItemCount(); i < GetMainMenuItemCount(); i++)
                {
                    _menuItems[i].SetLabel(_mainMenuItemNames[i]);
                }
            }
        }

        private void MakeNewMenuItem(string menuItemName, int limit)
        {
            var newObject = Instantiate(menuItemPrefab, transform);
            newObject.SetActive(true);
            var newMenuItem = newObject.GetComponent<MenuItem>();
            newMenuItem.SetStartYAnchor(EndYAnchor + 0.12f * (limit - _menuItems.Count - 1));
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

        public IEnumerator HideAllItems()
        {
            for (var i = menuState == MenuState.DiffMenu ? 1 : 0; i < _menuItems.Count; i++)
                StartCoroutine(_menuItems[i].Hide());
            yield return null;
        }

        public IEnumerator ShowAllItems()
        {
            foreach (var item in _menuItems)
                StartCoroutine(item.Show());
            yield return null;
        }
    }
}
