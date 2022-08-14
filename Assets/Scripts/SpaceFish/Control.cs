using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { START, PLAYERTURN, AITURN, WON, LOST }

public class Control : MonoBehaviour
{
    Helper helper;
    public GameState state;
    
    public Board board;
    public GameObject[,] boardObject;

    private HumanPlayer player;
    private BotPlayer bot;

    [SerializeField] private Transform cam;
    public GameOverScreen gameOverScreen;

    void Start()
    {
        state = GameState.START;
        
        helper = gameObject.GetComponent<Helper>();

        InitBoard();
        helper.SetBoard(board);

        InitPlayers();
        
        StartCoroutine(PlayerTurn());  //randomize playerTurn/botTurn
    }

    private IEnumerator PlayerTurn()
    {
        state = GameState.PLAYERTURN;

        if (board.winner != null)
        {
            print(board.winner);
            StartCoroutine(GameOver((int)board.winner));
            yield break;
        }

        yield return StartCoroutine(player.MakeMove(board, helper));

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(BotTurn());
    }

    //regular function?
    private IEnumerator BotTurn()
    {
        state = GameState.AITURN;

        if (board.winner != null)
        {
            StartCoroutine(GameOver((int)board.winner));
            yield break;
        }

        yield return StartCoroutine(bot.MakeMove(board, helper));

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(PlayerTurn());
    }


    public IEnumerator GameOver(int winner)
    {
        // play a gameover sound here
        yield return new WaitForSeconds(3.0f);
        gameOverScreen.Setup(winner);
    }


    //Init
    private void InitBoard(bool havePieceLookAt = false)
    {
        board = new Board();
        //cam.transform.position = new Vector3((float)Board.LENGTH_X / 2 - 0.5f, (float)Board.LENGTH_Y / 2 - 0.5f, -10f);

        GenerateTiles(Board.LENGTH_X, Board.LENGTH_Y);
        GeneratePieces();

        if (havePieceLookAt)
        {
            GameObject pieceLookAt = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pieceLookAt.transform.position = new Vector3((float)Board.LENGTH_X / 2 - 0.5f, (float)Board.LENGTH_Y / 2 - 0.5f, 3f);
            pieceLookAt.tag = "PieceLookAt";
        }
    }

    private void InitPlayers()
    {
        //randomize player 1
        player = gameObject.AddComponent<HumanPlayer>();
        player.playerNumber = 1;

        bot = gameObject.AddComponent<BotPlayer>();
        bot.SetPlayerNumber(2);
    }

    private void GenerateTiles(int boardWidth, int boardHight)
    {
        boardObject = new GameObject[boardWidth, boardHight];

        for (int y = 0; y < boardHight; y++)
            for (int x = 0; x < boardWidth; x++)
                boardObject[x, y] = helper.GenerateTile(x, y, transform);

    }

    private void GeneratePieces()
    {
        for (int i = 0; i < Board.LENGTH; i++)
            helper.GeneratePiece(board.Get(i), i);
    }

}
