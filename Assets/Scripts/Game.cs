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
        var oUnPath = Path.Combine(pathDir, "ou.json");
        try
        {
            Directory.CreateDirectory(pathDir);
            var p = JsonConvert.SerializeObject(CardManager.SaveP(), Formatting.Indented);
            var u = JsonConvert.SerializeObject(CardManager.SaveU(), Formatting.Indented);
            var t = JsonConvert.SerializeObject(CardManager.SaveT(), Formatting.Indented);
            using var uf = new FileStream(unPath, FileMode.Create);
            using var pf = new FileStream(progPath, FileMode.Create);
            using var tf = new FileStream(tiPath, FileMode.Create);
            using var uw = new StreamWriter(uf);
            using var pw = new StreamWriter(pf);
            using var tw = new StreamWriter(tf);
            uw.Write(u);
            pw.Write(p);
            tw.Write(t);
            /*
            var iprog = JsonConvert.SerializeObject(CardManager.SaveInnerProgress());
            var oprog = JsonConvert.SerializeObject(CardManager.SaveOuterProgress());
            Debug.Log(iprog);
            var iunlock = JsonConvert.SerializeObject(CardManager.SaveInnerUnlocks());
            var ounlock = JsonConvert.SerializeObject(CardManager.SaveOuterUnlocks());
            using FileStream ipfs = new FileStream(iProgPath, FileMode.Create);
            using StreamWriter ipsw = new StreamWriter(ipfs);
            using FileStream iufs = new FileStream(iUnPath, FileMode.Create);
            using StreamWriter iusw = new StreamWriter(iufs);
            using FileStream opfs = new FileStream(oProgPath, FileMode.Create);
            using StreamWriter opsw = new StreamWriter(opfs);
            using FileStream oufs = new FileStream(oUnPath, FileMode.Create);
            using StreamWriter ousw = new StreamWriter(oufs);
            ipsw.Write(iprog);
            iusw.Write(iunlock);
            opsw.Write(oprog);
            ousw.Write(ounlock);*/
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
        var oUnPath = Path.Combine(pathDir, "ou.json");

        Dictionary<string, List<int>> fp = new();
        Dictionary<string, bool> fu = new();
        Dictionary<string, float> ft = new();

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
/*
        if (File.Exists(iProgPath))
        {
            try
            {
                var pData = "";
                using FileStream pfs = new FileStream(iProgPath, FileMode.Open);
                using StreamReader psr = new StreamReader(pfs);
                pData = psr.ReadToEnd();
                var temp = JsonConvert.DeserializeObject<Dictionary<string, History>>(pData);
                print(temp);
                finalPi = JsonConvert.DeserializeObject<Dictionary<InnerCard, History>>(temp);
            }
            catch (Exception e)
            {
                print(e);
                throw;
            }
        }

        if (File.Exists(oProgPath))
        {
            try
            {
                var pData = "";
                using FileStream pfs = new FileStream(oProgPath, FileMode.Open);
                using StreamReader psr = new StreamReader(pfs);
                pData = psr.ReadToEnd();
                var temp = JsonConvert.DeserializeObject<Dictionary<string, History>>(pData).ToString();
                finalPo = JsonConvert.DeserializeObject<Dictionary<OuterCard, History>>(temp);
            }
            catch (Exception e)
            {
                print(e);
                throw;
            }
        }
        
        if (File.Exists(iUnPath))
        {
            try
            {
                var uData = "";
                using FileStream ufs = new FileStream(iUnPath, FileMode.Open);
                using StreamReader usr = new StreamReader(ufs);
                uData = usr.ReadToEnd();
                var temp = JsonConvert.DeserializeObject<Dictionary<string, bool>>(uData).ToString();
                finalUi = JsonConvert.DeserializeObject<Dictionary<InnerCard, bool>>(temp);
            }
            catch (Exception e)
            {
                print("nope");
                throw;
            }
        }

        if (File.Exists(oUnPath))
        {
            try
            {
                var uData = "";
                using FileStream ufs = new FileStream(oUnPath, FileMode.Open);
                using StreamReader usr = new StreamReader(ufs);
                uData = usr.ReadToEnd();
                var temp = JsonConvert.DeserializeObject<Dictionary<string, bool>>(uData).ToString();
                finalUo = JsonConvert.DeserializeObject<Dictionary<OuterCard, bool>>(temp);
            }
            catch (Exception e)
            {
                print("nope");
                throw;
            }
        }*/


        
        if (fp.Count > 0 && fu.Count > 0)
        {
            CardManager.LoadP(fp);
            CardManager.LoadU(fu);
            CardManager.LoadT(ft);
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
            GameDifficulty.Easy => 10,
            GameDifficulty.LessEasy => 5,
            GameDifficulty.Akyuu => 1,
            _ => throw new ArgumentException("Unknown game difficulty!")
        };
        var upperLim = currentDifficulty switch
        {
            GameDifficulty.Easy => 999,
            GameDifficulty.LessEasy => 25,
            GameDifficulty.Akyuu => 10,
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