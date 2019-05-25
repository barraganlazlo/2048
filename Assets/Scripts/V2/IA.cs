using System;
using UnityEngine;

public class IA : MonoBehaviour
{

    public enum STRAT { Circulaire, Aleatoire, Coin, Alternance }

    private STRAT stratégie;

    public Partie partie;
    public string pseudo;

    public void ChoisirStratégie(int i)
    {
        stratégie = (STRAT)i;
    }

    public void LancerIA()
    {
        pseudo = Enum.GetName(typeof(STRAT), stratégie);
        if (partie.state==Partie.STATE.IA || partie.state == Partie.STATE.Init)
        {
            return;
        }
        partie.InitNewGame();
        partie.state = Partie.STATE.IA;
        if (stratégie == STRAT.Circulaire)
        {
            StratégieCirculaire();
        }
        else if (stratégie == STRAT.Aleatoire)
        {
            StratégieAléatoire();
        }
        else if (stratégie == STRAT.Coin)
        {
            StratégieCoin();
        }
        else if (stratégie == STRAT.Alternance)
        {
            StratégieAlternance();
        }
    }

    public void StratégieCirculaire()
    {      
        bool enCours = true;
        Grille.DIRECTION direction = Grille.DIRECTION.Bas;
        int i = UnityEngine.Random.Range(1, 5);
        while (enCours)
        {
            switch (i)
            {
                case 1:
                    direction = Grille.DIRECTION.Bas;
                    break;
                case 2:
                    direction = Grille.DIRECTION.Droite;
                    break;
                case 3:
                    direction = Grille.DIRECTION.Haut;                    
                    break;
                case 4:
                    direction = Grille.DIRECTION.Gauche;
                    i = 0;
                    break;
                default:
                    Debug.Log("error");
                    break;
            }
            i += 1;
            partie.Deplacement(direction,pseudo);
            enCours = partie.state == Partie.STATE.IA;
        }
    }

    public void StratégieAléatoire()
    {
        bool enCours = true;
        Grille.DIRECTION direction = Grille.DIRECTION.Bas;
        int i;
        while (enCours)
        {
            i = UnityEngine.Random.Range(1, 5);
            switch (i)
            {
                case 1:
                    direction = Grille.DIRECTION.Bas;
                    break;
                case 2:
                    direction = Grille.DIRECTION.Droite;
                    break;
                case 3:
                    direction = Grille.DIRECTION.Gauche;
                    break;
                case 4:
                    direction = Grille.DIRECTION.Haut;
                    break;
                default:
                    Debug.Log("error");
                    break;
            }
            partie.Deplacement(direction, pseudo);
            enCours = partie.state == Partie.STATE.IA;
        }
    }


    public void StratégieCoin()
    {
        bool enCours = true;
        Grille.DIRECTION direction = Grille.DIRECTION.Bas;
        while(enCours)
        {
            if (partie.MaxCaseCoin())
            {
                bool gauche = partie.grille.DeplacementPossible(Grille.DIRECTION.Gauche);
                bool haut = partie.grille.DeplacementPossible(Grille.DIRECTION.Haut);


                if (gauche && haut)
                {
                    int n = UnityEngine.Random.Range(1, 3);
                    switch (n)
                    {
                        case 1:
                            direction = Grille.DIRECTION.Gauche;
                            break;
                        case 2:
                            direction = Grille.DIRECTION.Haut;
                            break;
                        default:
                            Debug.Log("error");
                            break;
                    }
                }
                else if (gauche)
                {
                    direction = Grille.DIRECTION.Gauche;
                }
                else if (haut)
                {
                    direction = Grille.DIRECTION.Haut;
                }
                else
                {
                    int n = UnityEngine.Random.Range(1, 3);
                    switch (n)
                    {
                        case 1:
                            direction = Grille.DIRECTION.Droite;
                            break;
                        case 2:
                            direction = Grille.DIRECTION.Bas;
                            break;
                        default:
                            Debug.Log("error");
                            break;
                    }
                }
            }
            else
            {
                int[] posCaseMax = new int[2];
                posCaseMax = partie.grille.PositionMaxCaseGrille();
                if (posCaseMax[0] > posCaseMax[1] && partie.grille.DeplacementPossible(Grille.DIRECTION.Gauche))
                {
                    direction = Grille.DIRECTION.Gauche;
                }
                else if (partie.grille.DeplacementPossible(Grille.DIRECTION.Haut))
                {
                    direction = Grille.DIRECTION.Haut;
                }
                else
                {
                    int n = UnityEngine.Random.Range(1, 3);
                    switch (n)
                    {
                        case 1:
                            direction = Grille.DIRECTION.Droite;
                            break;
                        case 2:
                            direction = Grille.DIRECTION.Bas;
                            break;
                        default:
                            Debug.Log("error");
                            break;
                    }
                }
            }
            partie.Deplacement(direction,pseudo);
            enCours = partie.state == Partie.STATE.IA;
        }
    }

    public void StratégieAlternance()
    {
        bool enCours = true;
        Grille.DIRECTION direction = Grille.DIRECTION.Bas;
        int i = 1;
        while (enCours)
        {
            if(partie.grille.DeplacementPossible(Grille.DIRECTION.Droite) || 
               partie.grille.DeplacementPossible(Grille.DIRECTION.Haut) ||
               partie.grille.DeplacementPossible(Grille.DIRECTION.Gauche))
            {
                switch (i)
                {
                    case 1:
                        direction = Grille.DIRECTION.Droite;
                        break;
                    case 2:
                        direction = Grille.DIRECTION.Haut;
                        break;
                    case 3:
                        direction = Grille.DIRECTION.Gauche;
                        break;
                    case 4:
                        direction = Grille.DIRECTION.Haut;
                        i = 0;
                        break;
                    default:
                        Debug.Log("error");
                        break;
                }
                i += 1;
                partie.Deplacement(direction,pseudo);
                enCours = partie.state == Partie.STATE.IA;
            }
            else
            {
                direction = Grille.DIRECTION.Bas;
                partie.Deplacement(direction,pseudo);
                enCours = partie.state == Partie.STATE.IA;
                direction = Grille.DIRECTION.Haut;
                partie.Deplacement(direction,pseudo);
                enCours = partie.state == Partie.STATE.IA;
            }
        }
    }
}
