using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// an enum to handle all the possibe scoring events
public enum eScoreEvent
{
    draw,
    mine,
    mineGold,
    gameWin,
    gameLoss
}

public class ScoreManager : MonoBehaviour
{
    static private ScoreManager S;

    static public int SCORE_FROM_PREV_ROUND = 0;
    static public int HIGH_SCORE = 0;

    [Header("set Dynamically")]
    public int chain = 0;
    public int scoreRun = 0;
    public int score = 0;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("ERROR: ScoreManager.Awake(); S is already set!");
        }

        // check for highscore
        if (PlayerPrefs.HasKey("ProspectorHighScore"))
        {
            HIGH_SCORE = PlayerPrefs.GetInt("ProspectorHighScore");
        }
        // add the score from the last round, which will be > 0 if it was a win
        score += SCORE_FROM_PREV_ROUND;
        // and reset
        SCORE_FROM_PREV_ROUND = 0;
    }

    static public void EVENT(eScoreEvent evt)
    {
        try
        {
            S.Event(evt);
        }
        catch (System.NullReferenceException nre)
        {
            Debug.LogError("ScoreManager:EVENT() called while S = null.\n" + nre);
        }
    }

    void Event(eScoreEvent evt)
    {
        switch (evt)
        {
            // same things need to happen regardless of the game outcome
            case eScoreEvent.draw:          // drawing a card 
            case eScoreEvent.gameWin:       // won the round
            case eScoreEvent.gameLoss:      // lost the round
                chain = 0;                  // resets the score chain
                score += scoreRun;          // add scoreRun to total score
                scoreRun = 0;               // reset scoreRun
                break;

            case eScoreEvent.mine:          // remove a mine card
                chain++;                    // increase the score chain
                scoreRun += chain;          // add score for this card to run
                break;
        }

        // handles win/loss
        switch (evt)
        {
            case eScoreEvent.gameWin:
                // if its a win, add the score to the next round
                SCORE_FROM_PREV_ROUND = score;
                print("You won this round! Keep it up. Round score: " + score);
                break;

            case eScoreEvent.gameLoss:
                // if its a loss, check against the high score
                if (HIGH_SCORE <= score)
                {
                    print("You got the high score! High score: " + score);
                    HIGH_SCORE = score;
                    PlayerPrefs.SetInt("ProspectorHighScore", score);
                }
                else
                {
                    print("Your final score for the game was: " + score);
                }
                break;

            default:
                print("score: " + score + " scoreRun: " + scoreRun + " chain: " + chain);
                break;
        }
    }

    static public int CHAIN { get { return S.chain; } }
    static public int SCORE { get { return S.score; } }
    static public int SCORE_RUN { get { return S.scoreRun; } }
}