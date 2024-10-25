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
            _arrow.startYAnchor = MenuItemList.StartYAnchor;
            _arrow.UpdateAnchors();
            
            _fader = GetComponentInChildren<Fader>();
        }

        private void Start()
        {
            StartCoroutine(_fader.FadeIn(0.5f));
        }

        private void Update()
        {
            // Arrow controls
            if (Input.GetKeyDown(KeyCode.W))
                _arrow.MoveUp();
            if (Input.GetKeyDown(KeyCode.S))
                _arrow.MoveDown();
            
            if (Input.GetKeyDown(KeyCode.Return)) 
            { 
                if (_menuItemList.menuState == MenuState.MainMenu)
                { 
                    switch (_menuItemList.GetTextUnderArrow(_arrow.position)) 
                    {
                        case "Play!":
                            StartCoroutine(_menuItemList.ChangeMenuState(MenuState.DiffMenu));
                            break;
                        default:
                            Debug.Log("TODO");
                            break;
                    } 
                }

                if (_menuItemList.menuState == MenuState.DiffMenu)
                {
                    switch (_menuItemList.GetTextUnderArrow(_arrow.position))
                    {
                        case "Back!":
                            StartCoroutine(_menuItemList.ChangeMenuState(MenuState.MainMenu));
                            break;
                        default:
                            Debug.Log("TODO");
                            break;
                    }
                }
            }
        }
    }
}
