using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UI;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameDifficulty currentDifficulty;
    private List<OuterCard> _ignoredCards = new();
    
    private MainMenuScreen _mainMenuScreen;

    private List<OuterCard> _pool;
    
    // Save related
    private string pathDir;

    public void SaveData()
    {
        var iProgPath = Path.Combine(pathDir, "ip.json");
        var progPath = Path.Combine(pathDir, "p.json");
        var unPath = Path.Combine(pathDir, "u.json");
        var tiPath = Path.Combine(pathDir, "t.json");
        var btPath = Path.Combine(pathDir, "b.json");
        var oUnPath = Path.Combine(pathDir, "ou.json");
        try
        {
            Directory.CreateDirectory(pathDir);
            
            // Safeguard against data loss caused by incorrect outer / inner format.
            if (CardManager.SaveT().Count == 0 || CardManager.SaveP().Count == 0 ||
                CardManager.SaveU().Count == 0) return;
                
            var p = JsonConvert.SerializeObject(CardManager.SaveP(), Formatting.Indented);
            var u = JsonConvert.SerializeObject(CardManager.SaveU(), Formatting.Indented);
            var t = JsonConvert.SerializeObject(CardManager.SaveT(), Formatting.Indented);
            var b = JsonConvert.SerializeObject(CardManager.SaveB(), Formatting.Indented);
            
            using var uf = new FileStream(unPath, FileMode.Create);
            using var pf = new FileStream(progPath, FileMode.Create);
            using var tf = new FileStream(tiPath, FileMode.Create);
            using var bf = new FileStream(btPath, FileMode.Create);
            
            using var uw = new StreamWriter(uf);
            using var pw = new StreamWriter(pf);
            using var tw = new StreamWriter(tf);
            using var bw = new StreamWriter(bf);
            uw.Write(u);
            pw.Write(p);
            tw.Write(t);
            bw.Write(b);
        }
        catch (Exception e)
        {
            print("todo");
            throw;
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    public void LoadData()
    {
        var iProgPath = Path.Combine(pathDir, "ip.json");
        var progPath = Path.Combine(pathDir, "p.json");
        var unPath = Path.Combine(pathDir, "u.json");
        var tiPath = Path.Combine(pathDir, "t.json");
        var bfPath = Path.Combine(pathDir, "b.json");
        var oUnPath = Path.Combine(pathDir, "ou.json");

        Dictionary<string, List<int>> fp = new();
        Dictionary<string, bool> fu = new();
        Dictionary<string, float> ft = new();
        Dictionary<string, float> fb = new();

        if (File.Exists(unPath))
        {
            try
            {
                var pData = "";
                using FileStream pfs = new FileStream(unPath, FileMode.Open);
                using StreamReader psr = new StreamReader(pfs);
                pData = psr.ReadToEnd();
                fu = JsonConvert.DeserializeObject<Dictionary<string, bool>>(pData);
            }
            catch (Exception e)
            {
                print(e);
                throw;
            }
        }
        
        if (File.Exists(progPath))
        {
            try
            {
                var pData = "";
                using FileStream pfs = new FileStream(progPath, FileMode.Open);
                using StreamReader psr = new StreamReader(pfs);
                pData = psr.ReadToEnd();
                fp = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(pData);
            }
            catch (Exception e)
            {
                print(e);
                throw;
            }
        }
        
        if (File.Exists(tiPath))
        {
            try
            {
                var pData = "";
                using FileStream pfs = new FileStream(tiPath, FileMode.Open);
                using StreamReader psr = new StreamReader(pfs);
                pData = psr.ReadToEnd();
                ft = JsonConvert.DeserializeObject<Dictionary<string, float>>(pData);
            }
            catch (Exception e)
            {
                print(e);
                throw;
            }
        }

        if (File.Exists(bfPath))
        {
            try
            {
                var pData = "";
                using FileStream pfs = new FileStream(bfPath, FileMode.Open);
                using StreamReader psr = new StreamReader(pfs);
                pData = psr.ReadToEnd();
                fb = JsonConvert.DeserializeObject<Dictionary<string, float>>(pData);
            }
            catch (Exception e)
            {
                print(e);
                throw;
            }
        }
        
        if (fp.Count > 0 && fu.Count > 0)
        {
            CardManager.LoadP(fp);
            CardManager.LoadU(fu);
            CardManager.LoadT(ft);
            CardManager.LoadB(fb);
        }
    }
    
    private void Awake()
    {
        pathDir = Application.persistentDataPath;
        _mainMenuScreen = transform.Find("/Canvas").GetComponent<MainMenuScreen>();
        
        CardManager.LoadCards();
        LoadData();
    }

    private void Start()
    {
        _mainMenuScreen.callTutorialTurn1.AddListener(ForceTutorial1);
        _mainMenuScreen.callTutorialTurn2.AddListener(ForceTutorial2);
        _mainMenuScreen.callNextTurn.AddListener(GetCards);
        _mainMenuScreen.callGame.AddListener(InitializeGame);
    }

    private void InitializeGame()
    {
        _ignoredCards = new List<OuterCard>();
        currentDifficulty = _mainMenuScreen.selectedDifficulty;
    }

    public void Turn()
    {
    }

    private void UpdateCards(string innerSt, string outerSt)
    {
        _mainMenuScreen.UpdateCardDisplay(
            CardManager.ForcePullInner(innerSt),
            CardManager.ForcePullOuter(outerSt)
            );
    }

    private void ForceTutorial1()
    {
        UpdateCards("Akyuu's birth", "American Civil War");
    }

    private void ForceTutorial2()
    {
        UpdateCards("Akyuu's birth", "Okinawa restitution");
    }

    private void GetCards()
    {
        var lowerLim = currentDifficulty switch
        {
            GameDifficulty.Easy => 25,
            GameDifficulty.LessEasy => 10,
            GameDifficulty.Akyuu => 1,
            GameDifficulty.Extra => 1,
            _ => throw new ArgumentException("Unknown game difficulty!")
        };
        var upperLim = currentDifficulty switch
        {
            GameDifficulty.Easy => 999,
            GameDifficulty.LessEasy => 35,
            GameDifficulty.Akyuu => 10,
            GameDifficulty.Extra => 42,
            _ => throw new ArgumentException("Unknown game difficulty!")
        };
        
        InnerCard inner = null;
        OuterCard outer = null;
        var rerolls = 0;
        // There should always be at least 8 cards for each inner and for each difficulty.
        // Safeguard just in case.
        while (outer is null && rerolls < 4 * CardManager.GetInnerCount())
        {
            try
            {
                inner = CardManager.PullInner();
                outer = CardManager.PullOuter(inner, lowerLim, upperLim, currentDifficulty, _ignoredCards);
            }
            catch (ArgumentOutOfRangeException e)
            {
                print("Rerolling due to card exhaustion.");
                rerolls++;
            }
        }

        // TODO this is absolutely not elegant
        UpdateCards(inner.ShortTitle, outer.ShortTitle);
        _ignoredCards.Add(outer);
    }
}