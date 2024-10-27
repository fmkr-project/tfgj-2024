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

        private Logo _logo;
        private Arrow _arrow;
        private MenuItemList _menuItemList;
        private Fader _fader;
        
        private DialogueManager _dialogueManager;
        private UICardManager _uiCardManager;
        private CollectionMenu _collectionMenu;
        private bool _isCollectionMenuOpen;
        private CreditsMenu _creditsMenu;
        private bool _isCreditsMenuOpen;

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
            
            _logo = GetComponentInChildren<Logo>();
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
            _creditsMenu = GetComponentInChildren<CreditsMenu>();
        }

        private void Start()
        {
            StartCoroutine(_arrow.UpdateAnchors());
            StartCoroutine(_fader.FadeIn(0.5f));
        }

        private void Update()
        {
            if (_isCreditsMenuOpen)
            {
                if (!_creditsMenu.isReady) return;
                
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    StartCoroutine(_creditsMenu.Hide());
                    _isCreditsMenuOpen = false;
                }

                return;
            }
            
            // Controls in the collection menu
            if (_isCollectionMenuOpen)
            {
                if (_collectionMenu.canClose)
                {
                    _isCollectionMenuOpen = false;
                    _collectionMenu.canClose = false;
                    return;
                }
                
                if (!_collectionMenu.isReady) return;
                
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    StartCoroutine(_collectionMenu.Hide());
                    _collectionMenu.isReady = false;
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
                    SoundManager.Fire("tick");
                    switch (_menuItemList.GetTextUnderArrow(_arrow.position)) 
                    {
                        case "Tutorial":
                            StartCoroutine(StartTutorial());
                            break;
                        case "Play!":
                            StartCoroutine(_menuItemList.ChangeMenuState(MenuState.DiffMenu));
                            StartCoroutine(_logo.Hide());
                            _arrow.maxPosition = _menuItemList.GetDiffMenuItemCount();
                            _arrow.position = 0;
                            StartCoroutine(_arrow.UpdateAnchors());
                            break;
                        case "Collection":
                            _isCollectionMenuOpen = true;
                            _collectionMenu.ShowFrom(0);
                            StartCoroutine(_collectionMenu.Show());
                            break;
                        case "Credits":
                            _isCreditsMenuOpen = true;
                            StartCoroutine(_creditsMenu.Show());
                            break;
                        case "Quit!":
                            Application.Quit();
                            break;
                        default:
                            Debug.Log("TODO");
                            break;
                    } 
                }

                else if (_menuItemList.menuState == MenuState.DiffMenu)
                {
                    SoundManager.Fire("tick");
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
                            StartCoroutine(_logo.Show());
                            _arrow.maxPosition = _menuItemList.GetMainMenuItemCount();
                            _arrow.position = 0;
                            StartCoroutine(_arrow.UpdateAnchors());
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
            StartCoroutine(_arrow.Hide());
            StartCoroutine(_logo.Hide());
            _isInGame = true;
            StartCoroutine(_menuItemList.HideAllItems());
            yield return new WaitForSeconds(0.25f);

            
            // Introduction dialogue.
            // Context, minimal backstory.
            StartCoroutine(_dialogueManager.KeineAppear("Hi Kozusu."));
            // Boilerplate yay!
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            // Kenesuzu is real.
            _dialogueManager.ChangeOtherTalking(new Who("Kosuzu"));
            StartCoroutine(_dialogueManager.OtherAppear("Morning! What's all this paper on your desk? An essay?"));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            /*
            StartCoroutine(_dialogueManager.OtherRetire());
            yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());
            _dialogueManager.ChangeOtherTalking(new Who("Dai"));
            StartCoroutine(_dialogueManager.OtherAppear("Yay, I can talk!"));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);*/
            
            _dialogueManager.KeineChangeDialogue("Not exactly. I'm trying to figure out when do events in the Outside World are... reflected in Gensokyo.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.KeineChangeDialogue("As you surely know, people and major events are forgotten in the Outside World. And in some cases, their legacy appear here.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.OtherChangeDialogue("I'm not really sure to understand... Last day, I had a young man who was trying to look for a book written by a certain Marguerite Duras, and...");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.KeineChangeDialogue("Marguerite Duras, 1914-1996. A French writer. She died 28 years ago... thanks for providing me once again with some proof for my theorem!");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.OtherChangeDialogue("Your... theorem?");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.KeineChangeDialogue("Of course! The Keine Theorem. Although someone called it the \"kein Theorem\" for some reason...");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.KeineChangeDialogue("Here, have a look.");
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
            StartCoroutine(_dialogueManager.KeineAppear("So, according to the Keine Theorem, <b>events</b> happening in the Outside World are forgotten after <b>15 years</b>."));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            StartCoroutine(_dialogueManager.OtherAppear("Why 15 and not another arbitrarily chosen number?"));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.KeineChangeDialogue("Well, it more or less matches the Chronicle, and it works on my set of data.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.OtherChangeDialogue("What's the size of your data anyway...?");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.KeineChangeDialogue("...Two.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.KeineChangeDialogue("Anyway, you certainly know when the American Civil War ended.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.OtherChangeDialogue("1865. So that means that by 1880, people would have forgotten everything about it? That's quite unnatural given how major of a conflict this was...");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.KeineChangeDialogue("If the Keine Theorem says so, it's true, don't argue about it!");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.KeineChangeDialogue("Anyway, we can conclude that the Civil War was remembered in Gensokyo before the birth of the ninth Child of Miare.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.OtherChangeDialogue("That seems logical. Ish.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.KeineChangeDialogue("To select the card that happened in the Outside World, you should press <b>W</b>.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.OtherChangeDialogue("I don't know who you're talking to but this person ought to press <b>W</b>.");
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
            
            // Wait for animation to end
            yield return new WaitForSeconds(_uiCardManager.AnswerAnimationDuration());
            
            // Cards disappear, Keine pops.
            StartCoroutine(_uiCardManager.HideCards());
            StartCoroutine(_dialogueManager.OtherAppear("If events are forgotten after 15 years, then why does that theorem still work with Marguerite Dumas?"));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            StartCoroutine(_dialogueManager.KeineAppear("It's because she was a <b>person</b>. According to the Keine Theorem, people are forgotten <b>25 years</b> after their death."));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.KeineChangeDialogue("As she died 28 years ago, humans in Gensokyo start to acknowledge her existence.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.OtherChangeDialogue("Still a bit weird, but I'll pass.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.KeineChangeDialogue("The Theorem also uses three more categories. <b>Places</b>, such as the abandoned island of Hashima (Gunkanjima), are forgotten after <b>10 years</b>. For <b>tech</b> such as the Game Boy, it's <b>30 years</b> after their introduction.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.OtherChangeDialogue("Oh, and I'll suppose that <b>pieces of technology that are discontinued</b> also get forgotten?");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.KeineChangeDialogue("Correct! <b>Dead technologies</b>, as I like to call them, are forgotten <b>instantly</b>.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.OtherChangeDialogue("That's, erm...");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.KeineChangeDialogue("Please don't complain about the Theorem. It even has a Seal of Approval (tm), so it must be a tautology!");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.OtherChangeDialogue("That's <i>your</i> Seal of Approval.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.KeineChangeDialogue("Trademark!");
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
            
            StartCoroutine(_dialogueManager.KeineAppear("Okinawa Prefecture became Japanese again in Season 87 (1972)."));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            StartCoroutine(_dialogueManager.OtherAppear("So people forgot about it in... 1987!"));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.KeineChangeDialogue("Not exactly. I forgot to talk about an important detail.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.KeineChangeDialogue("This event took place in <b>modern-day Japan</b>. According once again to the Keine Theorem, the time it takes for people to forget about it is <b>doubled</b>.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.OtherChangeDialogue("Oh, I see. So it was forgotten around... 2002? Before Akyuu was born?");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            _dialogueManager.KeineChangeDialogue("That's correct. Since the event that was remembered first in Gensokyo is the one that happened inside the Barrier, you should press <b>S</b>.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.OtherChangeDialogue("Yeah, press <b>S</b> already! I still don't know who you are talking to.");
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
            yield return new WaitForSeconds(_uiCardManager.AnswerAnimationDuration());
            
            // Cards disappear. Keine.
            StartCoroutine(_uiCardManager.HideCards());
            StartCoroutine(_dialogueManager.KeineAppear("Last exception, <b>pieces of technology</b> expire in 30 years <b>regardless if they were produced in Japan or not</b>."));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            
            StartCoroutine(_dialogueManager.OtherAppear("<b>Foreign people</b> are forgotten after <b>25 years</b>, <b>people born in Japan</b> after <b>50 years</b>, <b>technology</b> after <b>30 years</b> regardless of origin, and <b>dead tech</b> is forgotten <b>instantly</b>."));
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.KeineChangeDialogue("That's correct.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.OtherChangeDialogue("<b>Japanese places</b> are forgotten after <b>20 years</b> since <b>foreign ones</b> take <b>10 years</b> to be forgotten. For <b>events that took place in Japan</b>, it's <b>30 years</b>, and for <b>foreign ones</b>, it's <b>15</b>.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.KeineChangeDialogue("That's also correct! I'd give you a star for mastering the Theorem, but I think you're too old for that.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.OtherChangeDialogue("...Old!? Well, I think that's a very weird Theorem to begin with.");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            _dialogueManager.KeineChangeDialogue("Not with the Seal of Approval (tm)!");
            while (!Input.GetKeyDown(KeyCode.Return)) yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);

            // Tutorial ends.
            StartCoroutine(_dialogueManager.KeineRetire());
            StartCoroutine(_dialogueManager.OtherRetire());
            yield return new WaitForSeconds(_dialogueManager.GetAnimationWaitTime());
            _isInGame = false;
            
            // Back to main menu. (we were already on the main menu)
            StartCoroutine(_arrow.Show());
            StartCoroutine(_logo.Show());
            StartCoroutine(_menuItemList.ShowAllItems());
            
            yield return null;
        }

        private IEnumerator StartGame()
        {
            StartCoroutine(_arrow.Hide());
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
            StartCoroutine(_arrow.Show());
            _menuItemList.DiscreteChangeMenuState(MenuState.MainMenu);
            _arrow.maxPosition = _menuItemList.GetMainMenuItemCount();
            _arrow.position = 0;
            StartCoroutine(_arrow.UpdateAnchors());
            StartCoroutine(_logo.Show());
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
