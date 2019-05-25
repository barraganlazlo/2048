using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartieSave : MonoBehaviour
{

    public string pseudo;
    public LinkedList<int> scores;
    public bool result;
    public LinkedList<int[,]> deplacements;
    public int nbDeplacements;

    private const string SAVEURL = "http://webinfo.iutmontp.univ-montp2.fr/~barraganl/2048/SavePartie.php";

    public void InitPartieSave()
    {
        scores = new LinkedList<int>();
        deplacements = new LinkedList<int[,]>();
    }

    public void SavePartie(string pseudo, bool result)
    {
        this.pseudo = pseudo;
        this.result = result;
        StartCoroutine(Save(pseudo));
    }

    public void SaveDeplacement(int[,] d, int score)
    {
        scores.AddLast(score);
        deplacements.AddLast(d);

    }

    IEnumerator Save(string pseudo)
    {
        Debug.Log("saving");
        WWWForm form = new WWWForm();
        form.AddField("pseudo", pseudo);
        form.AddField("finalScore", "" + scores.Last.Value);
        form.AddField("scores", ScoreToString());
        form.AddField("result", result.ToString());
        form.AddField("deplacements", DeplacementsToString());
        form.AddField("nbDeplacements", deplacements.Count - 1);

        WWW data = new WWW(SAVEURL, form);
        yield return data;
        if (!string.IsNullOrEmpty(data.error))
        {
            Debug.Log("erreur envoi" + data.error);
        }
        Debug.Log(data.text);
    }

    public string DeplacementsToString()
    {
        string toret = "";
        foreach (int[,] d in deplacements)
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    toret += d[x, y] + ".";
                }
            }
            toret = toret.TrimEnd('.');
            toret += ":";
        }
        toret = toret.TrimEnd(':');
        return toret;
    }

    public string ScoreToString()
    {
        string toret = "";
        foreach (int s in scores)
        {
            toret += s + ":";
        }
        toret = toret.TrimEnd(':');
        return toret;
    }
}
