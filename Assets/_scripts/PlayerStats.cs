﻿using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PlayerStats : MonoBehaviour
{
    public static PlayerStats SingleInstance;
    private float _currentScore;
    private float _currentEnergy;
    private CollectableType _currentCollectable;

    [SerializeField, Range(20.0f, 30.0f), Tooltip("Entre mas pequeño el crecimiento de ganancias de puntos es menor")]
    private float scoreDivider;

    private float _scoreMultiplier;
    [SerializeField] private float initialEnergy;
    [Range(5, 10)] public float energyPerObj, energyLostPerBadObj;
    [SerializeField, Range(0, 2.0f)] private float energyLostPerSecond;
    [SerializeField] private float timeBetweenShoot;
    private float _currentShootTime;
    private bool _canShoot;

    private void Awake()
    {
        _canShoot = true;
        if (SingleInstance == null)
            SingleInstance = this;
    }

    private void Start()
    {
        RestartValues();
    }

    private void Update()
    {
        if (_canShoot || GameManager.SingleInstance.GetCurrentGameState() != GameState.InGame) return;
        _currentShootTime += Time.deltaTime;
        if (_currentShootTime >= timeBetweenShoot)
        {
            RestartShoot();
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.SingleInstance.GetCurrentGameState() != GameState.InGame) return;

        ChangeEnergy(energyLostPerSecond * Time.fixedDeltaTime, false);
        _currentScore += Time.deltaTime * _scoreMultiplier;
        _scoreMultiplier += Time.deltaTime / scoreDivider;
        if (_currentEnergy <= 0)
            GameManager.SingleInstance.GameOver();
    }

//Collectables
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Collectable")) return;
        var collectable = other.GetComponent<Collectable>();
        collectable.BeCollected();
    }

    private void RestartValues()
    {
        _currentShootTime = 0;
        _currentCollectable = CollectableType.Null;
        _currentEnergy = initialEnergy;
        _currentScore = 0;
        _scoreMultiplier = 1;
    }

    public float GetCurrentScore()
    {
        return _currentScore;
    }

    public float GetCurrentEnergy()
    {
        return _currentEnergy;
    }

    public void ChangeCollectableType(int newCollectableTypeType)
    {
        _currentCollectable = GameManager.SingleInstance.collectables[newCollectableTypeType].type;
    }

    public void ChangeCollectableType(CollectableType newCollectableTypeType)
    {
        _currentCollectable = newCollectableTypeType;
    }

    public CollectableType GetCurrentCollectable()
    {
        return _currentCollectable;
    }

    public void ChangeEnergy(float energyWin, bool win = true)
    {
        _currentEnergy = Mathf.Clamp(win ? _currentEnergy + energyWin : _currentEnergy - energyWin, 0, initialEnergy);
    }

    public bool Shoot()
    {
        if (_canShoot)
        {
            _canShoot = false;
            return !_canShoot;
        }

        return _canShoot;
    }


    private void RestartShoot()
    {
        _currentShootTime = 0;
        _canShoot = true;
        InGameGUI.SingleInstace.SwitchSprite(ref InGameGUI.SingleInstace.redButtonImg, InGameGUI.SingleInstace
            .redButton);
    }
}