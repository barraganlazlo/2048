using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class Partie : MonoBehaviour
{
    public enum STATE { Init, EnCours, Gagnée, Perdue, IA }
    public static Sprite[] images;
    public Grille grille;
    public Score scoreObject;
    public int winValue;
    public STATE state;
    public GameObject UiState;
    public Joueur joueur;
    public GameObject InputPseudo;
    public GameObject grillePrefab;
    public IA ia;
    public GameObject PlayBar;
    PartieSave save;

    public int version;

    private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.

    public LinkedListNode<int> nodeScore;
    public LinkedListNode<int[,]> nodeDeplacement;

    // Use this for initialization
    void Start()
    {
        state = STATE.Init;
        LoadSprites();
        AskPseudo();
    }

    void Update()
    {
        if (Input.GetButtonUp("Start") && state != STATE.Init && state != STATE.Init)
        {
            InitNewGame();
        }
        if (state == STATE.EnCours)
        {
            CheckInputs();
        }
    }

    private void LoadSprites()
    {
        images = new Sprite[14];
        for (int i = 0; i < 14; i++)
        {
            string path = "Sprites/" + i;
            images[i] = Resources.Load<Sprite>(path);
        }
    }

    private void CheckInputs()
    {
#if UNITY_IOS || UNITY_ANDROID
        //Check if Input has registered more than zero touches
        if (Input.touchCount > 0)
        {
            //Store the first touch detected.
            Touch myTouch = Input.touches[0];
            //Check if the phase of that touch equals Began
            if (myTouch.phase == TouchPhase.Began)
            {
                //If so, set touchOrigin to the position of that touch
                touchOrigin = myTouch.position;
            }
            //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                //Set touchEnd to equal the position of this touch
                Vector2 touchEnd = myTouch.position;

                //Calculate the difference between the beginning and end of the touch on the x axis.
                float x = touchEnd.x - touchOrigin.x;

                //Calculate the difference between the beginning and end of the touch on the y axis.
                float y = touchEnd.y - touchOrigin.y;

                //Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
                touchOrigin.x = -1;

                //Check if the difference along the x axis is greater than the difference along the y axis.
                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    if (x > 0.5f)
                    {
                        Deplacement(Grille.DIRECTION.Droite, joueur.pseudo);
                    }
                    else if(x<-0.5f)
                    {
                        Deplacement(Grille.DIRECTION.Gauche, joueur.pseudo);
                    }
                }
                else
                {
                    if (y>0.2f)
                    {
                        Deplacement(Grille.DIRECTION.Haut, joueur.pseudo);
                    }
                    else if(y<-0.2f)
                    {
                        Deplacement(Grille.DIRECTION.Bas, joueur.pseudo);
                    }
                }
            }
        }


#else
        if (Input.GetButtonDown("Up"))
        {
            Deplacement(Grille.DIRECTION.Haut, joueur.pseudo);
        }
        if (Input.GetButtonDown("Down"))
        {
            Deplacement(Grille.DIRECTION.Bas, joueur.pseudo);
        }
        if (Input.GetButtonDown("Left"))
        {
            Deplacement(Grille.DIRECTION.Gauche, joueur.pseudo);
        }
        if (Input.GetButtonDown("Right"))
        {
            Deplacement(Grille.DIRECTION.Droite, joueur.pseudo);
        }
#endif
    }

    public void InitNewGame()
    {
        if (state == STATE.IA)
        {
            return;
        }
        state = STATE.Init;
        PlayBar.SetActive(false);

        // detruit ancienne grille
        if (grille != null)
        {
            Destroy(grille.gameObject);
        }

        //crée une nouvelle grille
        grille = Instantiate<GameObject>(grillePrefab).GetComponent<Grille>();
        grille.GenerationGrille(this, 4);

        //Initialisation score
        scoreObject = new Score(this);
        //chargement du highscore
        StartCoroutine(scoreObject.LoadHS());
        StartCoroutine(scoreObject.LoadYHS(joueur.pseudo));

        UiState.transform.parent.gameObject.SetActive(false);
        if (save != null)
        {
            Destroy(save.gameObject);
        }
        save = new GameObject().AddComponent<PartieSave>();
        save.InitPartieSave();
        SaveDeplacement();
        state = STATE.EnCours;
    }

    private void CheckWin(string pseudo)
    {
        bool win = grille.CheckValue(winValue);
        if (win)
        {
            state = STATE.Gagnée;
            UiState.transform.parent.gameObject.SetActive(true);
            UiState.GetComponent<Text>().text = "Gagné";
            UiState.gameObject.SetActive(true);
            SavePartie(pseudo, true);
            RegarderPartie();
        }
    }

    private void CheckLose(string pseudo)
    {
        bool lose = !grille.DeplacementPossible(Grille.DIRECTION.Haut) && !grille.DeplacementPossible(Grille.DIRECTION.Bas) && !grille.DeplacementPossible(Grille.DIRECTION.Gauche) && !grille.DeplacementPossible(Grille.DIRECTION.Droite);
        // si aucun deplacement possible
        if (lose)
        {
            state = STATE.Perdue;
            UiState.transform.parent.gameObject.SetActive(true);
            UiState.GetComponent<Text>().text = "Perdu";
            UiState.gameObject.SetActive(true);
            SavePartie(pseudo, false);
            RegarderPartie();
        }
    }

    private void AskPseudo()
    {
        InputPseudo.SetActive(true);
    }

    public void StartGame()
    {
        if (joueur.pseudo != null && joueur.pseudo != "")
        {
            foreach (string s in Enum.GetNames(typeof(IA.STRAT)))
            {
                if (s == joueur.pseudo)
                {
                    return;
                }
            }            
            InputPseudo.SetActive(false);
            InitNewGame();
        }
    }

    public void Deplacement(Grille.DIRECTION direction, string pseudo)
    {

        if (grille.DeplacementPossible(direction))
        {
            grille.Deplacement(direction);
            SaveDeplacement();
            CheckLose(pseudo);
            CheckWin(pseudo);
        }
    }

    public int NbFusionsPossibles(Grille.DIRECTION dir)
    {
        int nb = 0;
        Grille simulation = grille.Simulate();
        for (int index = 0; index < simulation.dimension; index++)
        {
            simulation.DecalerSousTableau(index, dir);
        }
        nb = simulation.CountFusionsPossible(dir);
        Destroy(simulation.gameObject);
        Debug.Log("destroyed");
        return nb;

    }

    public bool MaxCaseCoin()
    {
        return (grille.Cases[0, 0].GetPuis() == grille.MaxCaseGrille());
    }

    public void SaveDeplacement()
    {
        int[,] d = grille.ConvertToInt();
        save.SaveDeplacement(d, scoreObject.score);
    }

    public void SavePartie(string pseudo, bool result)
    {
        save.SavePartie(pseudo, result);
    }

    public void RegarderPartie()
    {
        PlayBar.SetActive(true);
        nodeScore = save.scores.Last;
        nodeDeplacement = save.deplacements.Last;
    }
    public void RegarderActuel()
    {
        int s = nodeScore.Value;
        int[,] d = nodeDeplacement.Value;
        grille.RegarderDeplacement(d);
        scoreObject.RegarderScore(s);
    }
    public void RegarderSuivant()
    {
        if (nodeScore == save.scores.Last)
        {
            return;
        }
        nodeScore = nodeScore.Next;
        nodeDeplacement = nodeDeplacement.Next;
        RegarderActuel();
    }
    public void RegarderPrecedent()
    {
        if (nodeScore == save.scores.First)
        {
            return;
        }
        nodeScore = nodeScore.Previous;
        nodeDeplacement = nodeDeplacement.Previous;
        RegarderActuel();
    }
    public void RegarderDebut()
    {
        nodeScore = save.scores.First;
        nodeDeplacement = save.deplacements.First;
        RegarderActuel();
    }
    public void RegarderFin()
    {
        nodeScore = save.scores.Last;
        nodeDeplacement = save.deplacements.Last;
        RegarderActuel();
    }
}
