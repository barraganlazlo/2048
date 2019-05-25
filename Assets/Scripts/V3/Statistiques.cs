using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Statistiques : MonoBehaviour
{
    public Joueur joueur;
    public string pseudo;
    public Text uiPseudo;
    public Text uiHighScore;
    public Text uiScoreMoyen;
    public Text uiTotalPlayed;

    private const string LOADURLHS = "http://webinfo.iutmontp.univ-montp2.fr/~barraganl/2048/LoadYourHighScore.php";
    private const string LOADURLAVG = "http://webinfo.iutmontp.univ-montp2.fr/~barraganl/2048/LoadMoyenneScore.php";
    private const string LOADURLTOTAL = "http://webinfo.iutmontp.univ-montp2.fr/~barraganl/2048/LoadTotalPlayed.php";

    public void SetPseudo(string pseudo)
    {
        uiPseudo.text = pseudo;
        this.pseudo = pseudo;
    }
    public void SetScoreMoyen(string scoreMoyen)
    {
        uiScoreMoyen.text = scoreMoyen;
    }
    public void SetHighScore(string highScore)
    {
        uiHighScore.text = highScore;
    }
    public void SetTotalPlayed(string totalPlayed)
    {
        uiTotalPlayed.text = totalPlayed;
    }
    public IEnumerator LoadHSV2(string pseudo)
    {
        WWWForm form = new WWWForm();
        form.AddField("pseudo", pseudo);
        WWW data = new WWW(LOADURLHS, form);
        yield return data;
        if (data.error != null)
        {
            Debug.Log("erreur reception" + data.error);
        }
        else if (data.text != "")
        {
            SetHighScore(data.text);
        }
        else
        {
            SetHighScore("0");
        }
    }

    public IEnumerator LoadAVG(string pseudo)
    {
        WWWForm form = new WWWForm();
        form.AddField("pseudo", pseudo);
        WWW data = new WWW(LOADURLAVG, form);
        yield return data;
        if (data.error != null)
        {
            Debug.Log("erreur reception" + data.error);
        }
        else if (data.text != "")
        {
            string scm = data.text.Split('.')[0];
            SetScoreMoyen(scm);
        }
        else
        {
            SetScoreMoyen("0");
        }
    }
    public IEnumerator LoadTOTAL(string pseudo)
    {
        Debug.Log("loading");
        WWWForm form = new WWWForm();
        form.AddField("pseudo", pseudo);
        WWW data = new WWW(LOADURLTOTAL, form);
        yield return data;
        if (data.error != null)
        {
            Debug.Log("erreur reception" + data.error);
        }
        else if (data.text != "")
        {
            Debug.Log(data.text);
            SetTotalPlayed(data.text);
        }
        else
        {
            SetTotalPlayed("0");
        }
    }
    public void OnEnable()
    {
        if (pseudo == null || pseudo == "")
        {
            SetPseudo(joueur.pseudo);
        }
        StartCoroutine(LoadTOTAL(pseudo));
        StartCoroutine(LoadAVG(pseudo));
        StartCoroutine(LoadHSV2(pseudo));
    }
}

