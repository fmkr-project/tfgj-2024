using System;
using System.Collections;
using UnityEngine;

namespace UI
{
    public class MainMenuScreen : MonoBehaviour
    {
        private Arrow _arrow;
        private MenuItemList _menuItemList;
        private Fader _fader;
        
        private DialogueManager _dialogueManager;
        
        // Flags
        private bool _isInGame;

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
            _arrow.endYAnchor = MenuItemList.EndYAnchor;
            _arrow.UpdateAnchors();
            
            _fader = GetComponentInChildren<Fader>();

            _dialogueManager = GetComponentInChildren<DialogueManager>();
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
                if (_isInGame)
                {
                    return;
                }
                if (_menuItemList.menuState == MenuState.MainMenu)
                { 
                    switch (_menuItemList.GetTextUnderArrow(_arrow.position)) 
                    {
                        case "Tutorial":
                            StartCoroutine(StartTutorial());
                            _isInGame = true;
                            break;
                        case "Play!":
                            StartCoroutine(_menuItemList.ChangeMenuState(MenuState.DiffMenu));
                            _arrow.maxPosition = _menuItemList.GetDiffMenuItemCount();
                            _arrow.position = 0;
                            _arrow.UpdateAnchors();
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
                            _arrow.maxPosition = _menuItemList.GetMainMenuItemCount();
                            _arrow.position = 0;
                            _arrow.UpdateAnchors();
                            break;
                        default:
                            Debug.Log("TODO");
                            break;
                    }
                }
            }
        }

        private IEnumerator StartTutorial()
        {
            _arrow.Hide();
            StartCoroutine(_menuItemList.HideAllItems());
            yield return new WaitForSeconds(0.25f);

            StartCoroutine(_dialogueManager.KeineAppear("Lorem ipsum dolor sit amet"));
            // Boilerplate yay!
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            while (!Input.GetKeyUp(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.ChangeOtherTalking(new Who("Rumia"));
            StartCoroutine(_dialogueManager.OtherAppear("Is that so...?"));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            while (!Input.GetKeyUp(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);

            StartCoroutine(_dialogueManager.OtherRetire());
            yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());
            _dialogueManager.ChangeOtherTalking(new Who("Dai"));
            StartCoroutine(_dialogueManager.OtherAppear("Yay, I can talk!"));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            while (!Input.GetKeyUp(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            
            
            yield return null;
        }
    }
}
