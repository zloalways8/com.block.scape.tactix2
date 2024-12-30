using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager ins;

    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverBestScoreText;
    public TextMeshProUGUI gameOverScoreText;
    public GameObject scoreLayer;
    public Image offlineLabel;
   // public Button continueButton;
   // public GameObject addedPointsShell;

    public Button[] sound;
    public Sprite[] onOff;
    private bool isSound = true;

    public AdsService adsService;

    [HideInInspector]
    public bool gameOver = false;
    [HideInInspector]
    public bool paused = false;
    [HideInInspector]
    public bool firstBeatenScore;
    [HideInInspector]
    public bool continueGame;
    [HideInInspector]
    public bool waitingForAd;
    [HideInInspector]
    public int bestScore;
    [HideInInspector]
    public int score = 0;

    public void OnSoundClicked()
    {
        isSound = !isSound;
        if (isSound)
        {
            for (int i = 0; i < sound.Length; i++)
            {
                sound[i].image.sprite = onOff[0];
                AudioManager.ins.UnmuteMusic();
                AudioManager.ins.UnmuteSound();
            }
        }
        else
        {
            for (int i = 0; i < sound.Length; i++)
            {
                sound[i].image.sprite = onOff[1];
                AudioManager.ins.MuteMusic();
                AudioManager.ins.MuteSound();
            }
        }
    }

    public void ChangePoints(int e, int l)
    {
//        RectTransform trans = addedPointsShell.GetComponent<RectTransform>();
      //  trans.anchoredPosition = Camera.main.WorldToScreenPoint(InputManager.ins.lastPosition);

        int points = (BoardManager.BOARD_SIZE + e / 5) * l;
        points += (int)(points * (l / 3.0f - 0.333f));

        //TextMeshProUGUI t = addedPointsShell.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
       // t.text = "+" + points.ToString();

        scoreText.GetComponent<ScoreAddAnimation>().enabled = true;
        scoreText.GetComponent<ScoreAddAnimation>().SetAnimation(points, score, 0.4f);
        score += points;

        if (bestScore < score)
        {
            bestScore = score;
            bestScoreText.text = bestScore.ToString();
        }

        //addedPointsShell.GetComponent<Animator>().Play("Fade in");

    }

    public void RestartGame()
    {
        gameOver = false;
        if (score > bestScore)
        {
            bestScore = score;
        }

        bestScoreText.text = bestScore.ToString();
        firstBeatenScore = continueGame = true;
        score = 0;
        scoreText.text = score.ToString();

        for (int y = 0; y < BoardManager.BOARD_SIZE; y++)
        {
            for (int x = 0; x < BoardManager.BOARD_SIZE; x++)
            {
                if (BoardManager.ins.boardBlocks[x, y])
                {
                    Destroy(BoardManager.ins.boardBlocks[x, y].gameObject);
                    BoardManager.ins.boardBlocks[x, y] = null;
                }
            }
        }

        for (int i = 0; i < BoardManager.BLOCKS_AMOUNT; i++)
        {
            Destroy(BoardManager.ins.blocks[i].gameObject);
            int x = BoardManager.Rand(0, BoardManager.BLOCK_PREFABS_AMOUNT);
            BoardManager.ins.blocks[i] = BoardManager.ins.SpawnBlock(i, x);
        }
    }

    public void PauseGame()
    {
        paused = true;
        Time.timeScale = 0.0f;
    }

    public void UnpauseGame()
    {
        paused = false;
        Time.timeScale = 1.0f;
    }

    public void SetGameOver()
    {
        gameOver = true;

        gameOverBestScoreText.text = bestScore.ToString();
        gameOverScoreText.text = score.ToString();
    }

    public void FadeBlocks()
    {
        if (!ScenesManager.ins.transition)
            StartCoroutine(WaitForFade());
    }

    public IEnumerator WaitForFade()
    {
        gameOver = true;

        ScenesManager.ins.transition = true;
        
        for (int y = BoardManager.BOARD_SIZE - 1; y >= 0; y--)
        {
            for (int x = 0; x < BoardManager.BOARD_SIZE; x++)
            {
                BlockTile b = BoardManager.ins.boardBlocks[x, y];
                if (b)
                    b.Fade(0.25f, new Color(0.09f, 0.122f, 0.153f));

                if (x % 2 == 0)
                   yield return new WaitForSeconds(0.01f);
            }
        }

        yield return new WaitForSeconds(0.25f);
        
        ScenesManager.ins.LoadGameOverScreen();
    }

    public void ContinueGame()
    {
        gameOver = false;
        continueGame = false;

        ChangeBlocksColor();
        ScenesManager.ins.LoadGameFromGameOverScreen();
    }

    public void DestroyBlocks()
    {
        int a = (int)Random.Range(0, BoardManager.BOARD_SIZE - 2.001f);
        if (BoardManager.ins.blocks[0].size.x >= BoardManager.ins.blocks[0].size.y)
        {
            for (int y = a; y < a + 3; y++)
            {
                for (int x = 0; x < BoardManager.BOARD_SIZE; x++)
                {
                    BlockTile b = BoardManager.ins.boardBlocks[x, y];
                    if (b)
                        b.Destroy(0.25f);

                    BoardManager.ins.boardBlocks[x, y] = null;
                }
            }
        }
        else
        {
            for (int x = a; x < a + 3; x++)
            {
                for (int y = 0; y < BoardManager.BOARD_SIZE; y++)
                {
                    BlockTile b = BoardManager.ins.boardBlocks[x, y];
                    if (b)
                        b.Destroy(0.25f);

                    BoardManager.ins.boardBlocks[x, y] = null;
                }
            }
        }
        
        BoardManager.ins.CheckBoard();
    }

    public void ChangeBlocksColor()
    {
        for (int y = 0; y < BoardManager.BOARD_SIZE; y++)
            for (int x = 0; x < BoardManager.BOARD_SIZE; x++)
                if (BoardManager.ins.boardBlocks[x, y])
                    BoardManager.ins.boardBlocks[x, y].Fade(0.0f, BoardManager.ins.boardBlocks[x, y].defaultColor);
    }

    private void Awake()
	{
        if (!ins)
            ins = this;

        Application.targetFrameRate = 60;
        adsService = new AdsService();
        adsService.Init();
    }
    
}
