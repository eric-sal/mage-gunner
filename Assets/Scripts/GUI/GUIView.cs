using UnityEngine;
using System;
using System.Collections;

public class GUIView : MonoBehaviour {
    // Coins
    private GUIText _marioCoins;
    private CharacterState _mario;
    private GUIText _twinCoins;
    private CharacterState _twin;

    // Timer
    private GUIText _timeValue;
    private GameTimer _gameTimer;

    public void Awake() {
        // Coins
        _marioCoins = (GUIText)GameObject.Find("GUI/MarioCoins/CoinValue").GetComponent("GUIText");
        _mario = (CharacterState)GameObject.Find("MarioSprite").GetComponent("CharacterState");
        _twinCoins = (GUIText)GameObject.Find("GUI/MarioTwinCoins/CoinValue").GetComponent("GUIText");
        _twin = (CharacterState)GameObject.Find("MarioTwinSprite").GetComponent("CharacterState");

        // Timer
        _timeValue = (GUIText)GameObject.Find("GUI/TimeValue").GetComponent("GUIText");
        _gameTimer = (GameTimer)GameObject.Find("SceneController").GetComponent("GameTimer");
    }

    public void Update() {
        // Coins
        _marioCoins.text = _mario.coinCount.ToString("D2");
        _twinCoins.text = _twin.coinCount.ToString("D2");

        // Timer
        _timeValue.text = Math.Floor(_gameTimer.currentTime).ToString();
    }
}
