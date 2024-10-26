using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class MainMenuScreen : MonoBehaviour
    {
        public UnityEvent callTutorialTurn1; // For tutorial
        public UnityEvent callTutorialTurn2; // For tutorial
        public UnityEvent callNextTurn; // Tell Game to update the cards using CardManager
        public UnityEvent callGame; // Tell Game to look for the selected difficulty
        
        private Arrow _arrow;
        private MenuItemList _menuItemList;
        private Fader _fader;
        
        private DialogueManager _dialogueManager;
        private UICardManager _uiCardManager;
        private CollectionMenu _collectionMenu;
        private bool _isCollectionMenuOpen;

        public GameDifficulty selectedDifficulty;
        public int turnNumber;

        private InnerCard _inner;
        private OuterCard _outer;
        
        // Flags
        private bool _isInGame;

        private void Awake()
        {
            callTutorialTurn1 = new UnityEvent();
            callTutorialTurn2 = new UnityEvent();
            callNextTurn = new UnityEvent();
            callGame = new UnityEvent();
            
            _arrow = GetComponentInChildren<Arrow>();
            _menuItemList = GetComponentInChildren<MenuItemList>();
            _arrow.maxPosition = _menuItemList.menuState switch
            {
                MenuState.MainMenu => _menuItemList.GetMainMenuItemCount(),
                MenuState.DiffMenu => _menuItemList.GetDiffMenuItemCount(),
                _ => _arrow.maxPosition
            };
            _arrow.endYAnchor = MenuItemList.EndYAnchor;
            
            _fader = GetComponentInChildren<Fader>();

            _dialogueManager = GetComponentInChildren<DialogueManager>();
            _uiCardManager = GetComponentInChildren<UICardManager>();
            _collectionMenu = GetComponentInChildren<CollectionMenu>();
        }

        private void Start()
        {
            _arrow.UpdateAnchors();
            StartCoroutine(_fader.FadeIn(0.5f));
        }

        private void Update()
        {
            // Controls in the collection menu
            if (_isCollectionMenuOpen)
            {
                if (!_collectionMenu.isReady) return;
                
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    StartCoroutine(_collectionMenu.Hide());
                    _isCollectionMenuOpen = false;
                }
                if (Input.GetKeyDown(KeyCode.A))
                    _collectionMenu.PreviousPage();
                if (Input.GetKeyDown(KeyCode.D))
                    _collectionMenu.NextPage();
                if (Input.GetKeyDown(KeyCode.W))
                    _collectionMenu.MoveArrowUp();
                if (Input.GetKeyDown(KeyCode.S))
                    _collectionMenu.MoveArrowDown();
                if (Input.GetKeyDown(KeyCode.Q))
                    _collectionMenu.SwitchSide(CollectionSide.Inner);
                if (Input.GetKeyDown(KeyCode.E))
                    _collectionMenu.SwitchSide(CollectionSide.Outer);

                return;
            }
            
            // Arrow controls
            if (Input.GetKeyDown(KeyCode.W) && !_isInGame)
                _arrow.MoveUp();
            if (Input.GetKeyDown(KeyCode.S) && !_isInGame)
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
                            break;
                        case "Play!":
                            StartCoroutine(_menuItemList.ChangeMenuState(MenuState.DiffMenu));
                            _arrow.maxPosition = _menuItemList.GetDiffMenuItemCount();
                            _arrow.position = 0;
                            _arrow.UpdateAnchors();
                            break;
                        case "Collection":
                            _isCollectionMenuOpen = true;
                            _collectionMenu.ShowFrom(0);
                            StartCoroutine(_collectionMenu.Show());
                            break;
                        default:
                            Debug.Log("TODO");
                            break;
                    } 
                }

                else if (_menuItemList.menuState == MenuState.DiffMenu)
                {
                    switch (_menuItemList.GetTextUnderArrow(_arrow.position))
                    {
                        case "Easy":
                            selectedDifficulty = GameDifficulty.Easy;
                            StartCoroutine(StartGame());
                            break;
                        case "Less easy":
                            selectedDifficulty = GameDifficulty.LessEasy;
                            StartCoroutine(StartGame());
                            break;
                        case "Akyuu":
                            selectedDifficulty = GameDifficulty.Akyuu;
                            StartCoroutine(StartGame());
                            break;
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
            _isInGame = true;
            StartCoroutine(_menuItemList.HideAllItems());
            yield return new WaitForSeconds(0.25f);

            // Introduction dialogue.
            // Context, minimal backstory.
            StartCoroutine(_dialogueManager.KeineAppear("Lorem ipsum dolor sit amet"));
            // Boilerplate yay!
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.ChangeOtherTalking(new Who("Rumia"));
            StartCoroutine(_dialogueManager.OtherAppear("Is that so...?"));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            StartCoroutine(_dialogueManager.OtherRetire());
            yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());
            _dialogueManager.ChangeOtherTalking(new Who("Dai"));
            StartCoroutine(_dialogueManager.OtherAppear("Yay, I can talk!"));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.KeineChangeDialogue("Ce train JUJU(ko) a pour destination Juvisy. Il desservira toutes les gares d'Invalides à Bibliothèque François Mitterrand et toutes les gares des Ardoines à Juvisy.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            StartCoroutine(_dialogueManager.OtherRetire());
            yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());
            _dialogueManager.ChangeOtherTalking(new Who("Akyuu"));
            StartCoroutine(_dialogueManager.OtherAppear("I think I'm dead."));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            // Prepare the cards for the first tutorial turn.
            callTutorialTurn1.Invoke();
            
            StartCoroutine(_dialogueManager.KeineRetire());
            StartCoroutine(_dialogueManager.OtherRetire());
            yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());

            // Forced game.
            StartCoroutine(_uiCardManager.ShowCards());
            yield return new WaitForSeconds(_uiCardManager.GetAnimationTime());
            // Keine pops and talks about the commands.
            // During this part, the player can't skip to pressing W.
            StartCoroutine(_dialogueManager.KeineAppear("I should give actual advice to the player about how to play."));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            // Dai actually listens. Crazy, right.
            _dialogueManager.ChangeOtherTalking(new Who("Dai"));
            StartCoroutine(_dialogueManager.OtherAppear("Frogs go mlem mlem, snakes go pbbtpptbpbptbpt, fairies go pichu~n"));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            // Everyone unpops.
            StartCoroutine(_dialogueManager.KeineRetire());
            StartCoroutine(_dialogueManager.OtherRetire());
            yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());
            
            // Wait for player input. The other input (wrong answer) is disabled.
            // Don't use this code for the main loop.
            while (!Input.GetKeyDown(KeyCode.W)) yield return new WaitForSeconds(Time.deltaTime);
            StartCoroutine(_uiCardManager.Choose(Selected.Up, true));
            while (!Input.GetKeyUp(KeyCode.W)) yield return new WaitForSeconds(Time.deltaTime);
            
            // Wait for animation to end
            yield return new WaitForSeconds(_uiCardManager.AnswerAnimationDuration());
            
            // Cards disappear, Keine pops.
            StartCoroutine(_uiCardManager.HideCards());
            StartCoroutine(_dialogueManager.KeineAppear("It's Keine again!"));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.ChangeOtherTalking(new Who("Dai"));
            StartCoroutine(_dialogueManager.OtherAppear("Object event."));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            callTutorialTurn2.Invoke();
            
            StartCoroutine(_dialogueManager.KeineRetire());
            StartCoroutine(_dialogueManager.OtherRetire());
            yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());
            
            // Second forced game.
            // Japanese items are explained (F_kokunai).
            // The player can't yet press S.
            StartCoroutine(_uiCardManager.ShowCards());
            yield return new WaitForSeconds(_uiCardManager.GetAnimationTime());
            
            StartCoroutine(_dialogueManager.KeineAppear("I should give more advice, but I'm going to say some nonsense for now."));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            // Dai again.
            _dialogueManager.ChangeOtherTalking(new Who("Dai"));
            StartCoroutine(_dialogueManager.OtherAppear("Does the existence of Lunate Elf implies that there may be a Lunate Zwölf?"));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            // Everyone unpops.
            StartCoroutine(_dialogueManager.KeineRetire());
            StartCoroutine(_dialogueManager.OtherRetire());
            yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());
            
            // Wait for player input. The other input (wrong answer) is disabled.
            // Again, don't use this code for the main loop.
            while (!Input.GetKeyDown(KeyCode.S)) yield return new WaitForSeconds(Time.deltaTime);
            StartCoroutine(_uiCardManager.Choose(Selected.Down, true));
            while (!Input.GetKeyUp(KeyCode.S)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(_uiCardManager.AnswerAnimationDuration());
            
            // Cards disappear. Keine.
            StartCoroutine(_uiCardManager.HideCards());
            StartCoroutine(_dialogueManager.KeineAppear("Boo! This would have been scarier if I was that karakasa."));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.ChangeOtherTalking(new Who("Cirno"));
            StartCoroutine(_dialogueManager.OtherAppear("Rindfleischetikettierungsüberwachungsaufgabenübertragungsgesetz."));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            // Tutorial ends.
            // Other thresholds should be explained here.
            StartCoroutine(_dialogueManager.KeineRetire());
            StartCoroutine(_dialogueManager.OtherRetire());
            yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());
            _isInGame = false;
            
            // Back to main menu. (we were already on the main menu)
            _arrow.Show();
            StartCoroutine(_menuItemList.ShowAllItems());
            
            yield return null;
        }

        private IEnumerator StartGame()
        {
            _arrow.Hide();
            _isInGame = true;
            turnNumber = 0;
            StartCoroutine(_menuItemList.HideAllItems());
            yield return new WaitForSeconds(0.25f);
            
            callGame.Invoke();
            
            // TODO Prepare a different dialogue for all 3 difficulties.
            
            // Introduction dialogues.
            // TODO maybe different introduction dialogues in the same difficulty.
            if (selectedDifficulty == GameDifficulty.Easy)
            {
                StartCoroutine(_dialogueManager.KeineAppear("njut njut njut njut njut njut njut njut njut"));
                while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);
                
                _dialogueManager.ChangeOtherTalking(new Who("Rumia"));
                StartCoroutine(_dialogueManager.OtherAppear("*insert Polish noises*"));
                while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);
                
                StartCoroutine(_dialogueManager.KeineRetire());
                StartCoroutine(_dialogueManager.OtherRetire());
                yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());
            }
            else if (selectedDifficulty == GameDifficulty.LessEasy)
            {
                _dialogueManager.ChangeOtherTalking(new Who("Cirno"));
                StartCoroutine(_dialogueManager.OtherAppear("Speech order can be inverted! Although it's not particularly elegant."));
                while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);

                StartCoroutine(_dialogueManager.KeineAppear("yeah"));
                while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);
                
                StartCoroutine(_dialogueManager.KeineRetire());
                StartCoroutine(_dialogueManager.OtherRetire());
                yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());
            }
            else
            {
                StartCoroutine(_dialogueManager.KeineAppear("hej"));
                while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);

                _dialogueManager.ChangeOtherTalking(new Who("Kosuzu"));
                StartCoroutine(_dialogueManager.OtherAppear("ayo"));
                while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);

                StartCoroutine(_dialogueManager.KeineRetire());
                StartCoroutine(_dialogueManager.OtherRetire());
                yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());
            }
            
            // Game loop.
            // Keep playing until the player either gets an X or finishes 6 cards.
            // Randomly introduce some ambience dialogue.
            while (_uiCardManager.lastWasCorrect && turnNumber < 6)
            {
                callNextTurn.Invoke();
                
                StartCoroutine(_uiCardManager.ShowCards());
                yield return new WaitForSeconds(_uiCardManager.GetAnimationTime());
                
                // Wait for player input.
                while (!Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.S))
                    yield return new WaitForSeconds(Time.deltaTime);
                var selected = Input.GetKeyDown(KeyCode.W) ? Selected.Up : Selected.Down;
                StartCoroutine(_uiCardManager.Choose(selected, false));
                while (!Input.GetKeyUp(KeyCode.W) && !Input.GetKeyDown(KeyCode.S))
                    yield return new WaitForSeconds(Time.deltaTime);
                yield return new WaitForSeconds(_uiCardManager.AnswerAnimationDuration());
                
                StartCoroutine(_uiCardManager.HideCards());
                yield return new WaitForSeconds(_uiCardManager.GetAnimationTime());
                turnNumber++;
            }
            
            // End dialogue.
            // Separate player win and player loss.
            if (selectedDifficulty == GameDifficulty.Easy)
            {
                StartCoroutine(_dialogueManager.KeineAppear("| || || |-"));
                while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);
                
                _dialogueManager.ChangeOtherTalking(new Who("Dai"));
                StartCoroutine(_dialogueManager.OtherAppear("Is this loss?"));
                while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);
                
                StartCoroutine(_dialogueManager.KeineRetire());
                StartCoroutine(_dialogueManager.OtherRetire());
                yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());
            }
            else if (selectedDifficulty == GameDifficulty.LessEasy)
            {
                _dialogueManager.ChangeOtherTalking(new Who("Cirno"));
                StartCoroutine(_dialogueManager.OtherAppear("I like spaghetti code!"));
                while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);

                StartCoroutine(_dialogueManager.KeineAppear("Please don't do this here."));
                while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);
                
                StartCoroutine(_dialogueManager.KeineRetire());
                StartCoroutine(_dialogueManager.OtherRetire());
                yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());
            }
            else
            {
                StartCoroutine(_dialogueManager.KeineAppear("ahoj!"));
                while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);

                _dialogueManager.ChangeOtherTalking(new Who("Akyuu"));
                StartCoroutine(_dialogueManager.OtherAppear("ohayou (and not oha 24)"));
                while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
                yield return new WaitForSeconds(Time.deltaTime);

                StartCoroutine(_dialogueManager.KeineRetire());
                StartCoroutine(_dialogueManager.OtherRetire());
                yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());
            }
            
            // Return to the main menu. Change items to their main menu counterpart (we were in diff).
            _arrow.Show();
            _menuItemList.DiscreteChangeMenuState(MenuState.MainMenu);
            _arrow.maxPosition = _menuItemList.GetMainMenuItemCount();
            _arrow.position = 0;
            _arrow.UpdateAnchors();
            StartCoroutine(_menuItemList.ShowAllItems());

            _uiCardManager.lastWasCorrect = true;
            _isInGame = false;
            
            yield return null;
        }

        public void UpdateCardDisplay(InnerCard inner, OuterCard outer)
        {
            _inner = inner;
            _outer = outer;
            _uiCardManager.UpdateCards(inner, outer);
        }

        private Selected SelectedOfInput(KeyCode keyCode)
        {
            return keyCode == KeyCode.W ? Selected.Up : Selected.Down;
        }
    }
}
