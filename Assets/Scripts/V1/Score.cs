using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Score
{
    public Partie partie;

    public int score;
    public int highScore;
    public int yourHighScore;

    private Text UiScore;
    private Text UiHighScore;
    private Text UiYourHighScore;
    private Text UiHighScorePlayer;

    private const string LOADURL1 = "http://webinfo.iutmontp.univ-montp2.fr/~barraganl/2048/LoadHighScore.php";
    private const string LOADURL2 = "http://webinfo.iutmontp.univ-montp2.fr/~barraganl/2048/LoadYourHighScore.php";

    private void SetHighScore(int s,string p)
    {
        highScore = s;
        UiHighScore.text = "" + s;
        UiHighScorePlayer.text = p;
    }
    private void SetHighScore(int s)
    {
        highScore = s;
        UiHighScore.text = "" + s;
    }
    private void SetYourHighScore(int s)
    {
        yourHighScore = s;
        UiYourHighScore.text = "" + s;
    }
    public IEnumerator LoadYHS(string pseudo)
    {
        WWWForm form = new WWWForm();
        form.AddField("pseudo", pseudo);
        WWW data = new WWW(LOADURL2, form);
        yield return data;
        if (data.error!=null)
        {
            Debug.Log("erreur reception" + data.error);
        }
        else if (data.text!="")
        {
            SetYourHighScore(int.Parse(data.text));
        }
        else
        {
            SetYourHighScore(0);
        }
    }
    
    public IEnumerator LoadHS()
    {
        WWW data = new WWW(LOADURL1);
        yield return data;
        if (data.error != null)
        {
            Debug.Log("erreur reception" + data.error);
        }
        else if (data.text != ""&& data.text != ":")
        {
            string[] st = data.text.Split(':');
            SetHighScore(int.Parse(st[0]),st[1]);
        }
        else
        {
            SetHighScore(0,"");
        }
    }
    public void AddScore(int a)
    {
        score += 2 * (int)Mathf.Pow(2, a);  // Ajoute cette fusion au score.  
        UiScore.text = "" + score;
        if (score>highScore)
        {
            SetHighScore(score);
        }
        if (score>yourHighScore)
        {
            SetYourHighScore(score);
        }
    }
    public void RegarderScore(int s)
    {
        score = s;
        UiScore.text = "" + score;
    }

    public Score(Partie partie)
    {
        this.partie = partie;

        score = 0;

        //récupération de l'interface graphique
        UiScore = GameObject.Find("ScorePointsText").GetComponent<Text>();
        UiHighScore = GameObject.Find("HighScorePointsText").GetComponent<Text>();
        UiYourHighScore = GameObject.Find("YourHighScorePointsText").GetComponent<Text>();
        UiHighScorePlayer = GameObject.Find("HighScorePlayerText").GetComponent<Text>();        

        //initialisation de l'interface graphique
        UiScore.text = "0";
    }
}