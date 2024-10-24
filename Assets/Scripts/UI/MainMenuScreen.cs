using System;
using UnityEngine;

namespace UI
{
    public class MainMenuScreen : MonoBehaviour
    {
        private Arrow _arrow;
        private MenuItemList _menuItemList;
        private Fader _fader;

        private void Awake()
        {
            _arrow = GetComponentInChildren<Arrow>();
            _menuItemList = GetComponentInChildren<MenuItemList>();
            _arrow.maxPosition = _menuItemList.menuState switch
            {
                MenuState.MainMenu => _menuItemList.GetMainMenuItemCount(),
                MenuState.DiffMenu => _menuItemList.GetDiffMenuItemCount(),
                _ => _arrow.maxPosition
            };
            _fader = GetComponentInChildren<Fader>();
        }

        private void Start()
        {
            StartCoroutine(_fader.FadeIn(0.5f));
        }
    }
}
