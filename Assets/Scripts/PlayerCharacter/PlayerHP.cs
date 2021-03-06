using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using Cinemachine;
using UnityEngine.InputSystem;

public class PlayerHP : EntityHP
{
    #region Variables for feedbacks: + time slowdown


    //time slow
    const float _slowdownLength = 0.03f;
    float _slowdownT;
    private const float _timeScalePostHit = 0.05f;
    private bool _timeIsSlow;

    //blinking (same length as invul)
    private bool _isBlinking;
    [SerializeField] GameObject _body;
    #endregion

    //invul
    const float _invulerabilityLength = 0.9f;
    float _invulerabilityT;
    float _tookAHit;
    bool _isShieldingForTheFirstTime = true;
    [SerializeField] ParticleSystem _ShieldCounterVfx;

    //Init
    PlayerFeedbacks _playerFeedbacks;
    PlayerPlasma _playerPlasma;
    Ccl_FSM _fsm;
    [SerializeField] DanuAI _danuAI;
    [SerializeField] BossHealth _bossHealth;

    Hurtbox _hurtbox;
    private float _regenT;
    private bool _canRegen;
    private float maxRegenT = 3f;

    void Awake()
    {
        _hurtbox = GetComponent<Hurtbox>();
        _playerFeedbacks = GetComponent<PlayerFeedbacks>();
        _playerPlasma = GetComponent<PlayerPlasma>();
        _fsm = GetComponent<Ccl_FSM>();
        _maxHealthPoints = 5;
    }

    override protected void DamageFeedback(string attackName = "", int plasmaRegainValue = 0, float amount = 0)
    {
        base.DamageFeedback(attackName, plasmaRegainValue, amount);
        _playerFeedbacks.PlayPlayerHurtSfx();
        SlowDownTime();
        StartInvul();
        _playerFeedbacks.StartShake(.3f, 1f);
        _playerFeedbacks.StartRumble(.3f, 0.6f, 0.9f);
        StartRegenCooldown();
    }

    private void StartRegenCooldown()
    {
        _regenT = maxRegenT;
        _canRegen = false;
    }

    void Update()
    {
        if (_invulerabilityT > 0) HandlePostDamageInvul();
        if (_timeIsSlow) HandlePostDamageTimeSlow();
        if (!_canRegen)
        {
            _regenT -= Time.deltaTime;
            if (_regenT <= 0) _canRegen = true;
        }
        else if (HealthPoints < _maxHealthPoints)
        {
            HealthPoints = Mathf.Clamp(HealthPoints + Time.deltaTime * 0.25f, 0, _maxHealthPoints);
            UpdateHealthBar();
        }
    }

    void FixedUpdate()
    {
        if (_isBlinking) _body.gameObject.SetActive(!_body.activeSelf);
    }

    protected override void Shield(GameObject obj, int plasmaRegainValue, string attackName)
    {
        Ccl_StateShielding stateShielding = _fsm.currentState as Ccl_StateShielding;
        if (_fsm.currentState.Name == Ccl_StateNames.SHIELDING) stateShielding.ShieldT = 0f;

        if (_playerPlasma.PlasmaPoints > 0)
        {
            _bossHealth.TakeDamage(plasmaRegainValue * 5, attackName, 0);
            if (!_isShieldingForTheFirstTime) _playerPlasma.SpendPlasma("Renvoi");
            _playerFeedbacks.PlayShieldTriggerSfx();
            _ShieldCounterVfx.Clear();
            _ShieldCounterVfx.Play();
        }
        else if (IsInvulnerable)
        {
            SoundManager.Instance.PlayBlockedHit();
        }

        _isShieldingForTheFirstTime = false;
        bool attackIsMelee = Danu_Attacks.AttackIsMelee[attackName];
        if (attackIsMelee)
        {
            _danuAI.Stun(2);
            obj.transform.parent.gameObject.SetActive(false); // renvoyer le projo un jour
        }
        else
            obj.SetActive(false); // renvoyer le projo un jour

    }


    protected override void Die()
    {
        _playerFeedbacks.StopShake();
        _playerFeedbacks.StopRumble();
        UiManager.Instance.DisplayLossScreen();
        SoundManager.Instance.PlayDeathSound();
        base.Die();
    }

    #region slow down time after damage taken + camera shake
    void SlowDownTime()
    {
        Time.timeScale = _timeScalePostHit;
        _timeIsSlow = true;
        _slowdownT = _slowdownLength;
    }
    private void HandlePostDamageTimeSlow()
    {
        _slowdownT -= Time.deltaTime;
        if (_slowdownT < 0) ResetSlowdown();
    }
    private void ResetSlowdown()
    {
        Time.timeScale = 1f;
        _timeIsSlow = false;
    }
    #endregion

    #region invulnerability time after damage taken + character's body is blinking
    void StartInvul()
    {
        IsInvulnerable = true;
        _invulerabilityT = _invulerabilityLength;
        _isBlinking = true;
    }

    private void HandlePostDamageInvul()
    {
        _invulerabilityT -= Time.deltaTime;
        if (_invulerabilityT < 0) ResetInvulerability();
    }

    public void ResetInvulerability()
    {
        if (_fsm.currentState.Name != Ccl_StateNames.SHIELDING)
        {
            IsInvulnerable = false;
            IsShielding = false;
            _isShieldingForTheFirstTime = true;
        }
        _hurtbox.ForgetAllAttacks();
        ResetBlinking();
    }

    //post damage blinking while invul
    private void ResetBlinking()
    {
        _body.SetActive(true);
        _isBlinking = false;
    }
    #endregion
}
