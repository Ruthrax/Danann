using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
public class BossHealth : EntityHP
{
    private bool _isBlinking;
    const float _blinkingDuration = 0.3f;
    float _blinkingT = 0;
    [SerializeField]Image shieldBar;
    [SerializeField]Image shieldRemnants;
    [SerializeField]GameObject shieldGO;
    [SerializeField] PlayerFeedbacks _playerFeedbacks;
    DanuAI agent;
    [Required][SerializeField] GameObject _body;
    public void SetBody(GameObject nbody){_body=nbody;}
    private float oldValue;
    private float accel;
    private int shieldPoint;
    [SerializeField] private int maxShieldPoint;
    private float shieldRemnantTime;
    private bool activateShieldRemnant;
    private float oldShieldValue;
    [SerializeField] RectTransform invulText;
    [SerializeField] GameObject dmText;
    void Awake()
    {
        agent = GetComponent<DanuAI>();
        shieldRemnants.fillAmount=0;
        //_maxHealthPoints = 500;
    }
    public void ActivateShield()
    {
        shieldPoint=maxShieldPoint;
        shieldGO.SetActive(true);
        shieldBar.fillAmount=1;
        agent.UpdateShield(shieldPoint);

    }
    public void DesactivateShieldFromDM()
    {
        shieldPoint=0;
        shieldBar.fillAmount=0;
        shieldGO.SetActive(false);
        agent.UpdateShield(shieldPoint);
    }
    override protected void DamageFeedback(string attackName, int plasmaRegainValue, float amount)
    {
        base.DamageFeedback(attackName,plasmaRegainValue,amount);
        SoundManager.Instance.PlayHitSound(attackName);
        _playerFeedbacks.StartShake(AttackShake.ShakeValue[attackName].x, AttackShake.ShakeValue[attackName].y);
        _playerFeedbacks.StartRumble(AttackShake.RumbleValue[attackName].x, AttackShake.RumbleValue[attackName].y, AttackShake.RumbleValue[attackName].z);
        _isBlinking = true;
        _blinkingT = _blinkingDuration;
        if (plasmaRegainValue > 0) _playerplasma.IncreasePlasma(plasmaRegainValue);
    }

    private void HandlePostDamageBlinking()
    {
        _blinkingT -= Time.unscaledDeltaTime;
        if (_blinkingT <= 0) ResetBlinking();
    }

    public override bool TakeDamage(float amount, string attackName, int plasmaRegainValue, int revengeGain = 0, GameObject obj = null)
    {
        if (!activateRemnant)
            oldValue = (HealthPoints / _maxHealthPoints);
        accel = 0;          
        float percent = (HealthPoints / _maxHealthPoints) * 100;
        bool isDistance =attackName == Ccl_Attacks.TRAVELINGSPEAR; 
        isDistance=isDistance || attackName == Ccl_Attacks.SPEARSWINGL;
        isDistance=isDistance|| attackName == Ccl_Attacks.SPEARSWINGR;
        isDistance=isDistance||attackName==Ccl_Attacks.TRIANGLETICK;
        isDistance=isDistance||attackName==Danu_Attacks.PROJECTILE ;
        if (agent.IsDM())
            return base.TakeDamage(0, attackName, plasmaRegainValue, revengeGain);
        if (agent.IsShielded() )
        {
            if (attackName==Ccl_Attacks.TRIANGLEBOOM)
            {
                shieldPoint=0;
                shieldBar.fillAmount=shieldPoint*0.25f;
                activateShieldRemnant=true;
                agent.UpdateShield(shieldPoint);
                DesactivateShield();
            }
            else if ( !isDistance)
            {

                shieldPoint--;
                shieldBar.fillAmount=shieldPoint*0.25f;
                activateShieldRemnant=true;
                agent.UpdateShield(shieldPoint);
                accel=0;
                if (shieldPoint<=0)
                {
                    DesactivateShield();
                }
                return base.TakeDamage(0, attackName, plasmaRegainValue, revengeGain);
            }
            else
            {
            InvulnerableShieldingText();
            return base.TakeDamage(0, attackName, plasmaRegainValue, revengeGain);
            }

        }
        if (((HealthPoints-amount)/_maxHealthPoints)*100<=5 && !agent.HasDM())
        {
            amount = (int)(HealthPoints - ((5f / 100f) * _maxHealthPoints)) + 1;
            agent.launchDM();
            DMText();
            SoundManager.Instance.PlayDMTransition();
            return base.TakeDamage(amount, attackName, plasmaRegainValue, revengeGain);
        }
        if (percent < 70 && agent.GetPhase() == 1)
            agent.NextPhase();

        return base.TakeDamage(amount, attackName, plasmaRegainValue, revengeGain);
    }
    public void InvulnerableShieldingText()
    {
        invulText.gameObject.SetActive(true);
    }
    public void DMText()
    {
        dmText.SetActive(true);
    }
    private void DesactivateShield()
    {
        shieldGO.SetActive(false);
        agent.UpdateShield(shieldPoint);
    }

    private void ResetBlinking()
    {
        _body.SetActive(true);
        _isBlinking = false;
    }

    void Update()
    {
        if (_isBlinking) HandlePostDamageBlinking();
        UpdateRemnant();
        accel = Mathf.Clamp(accel + Time.deltaTime, 0, 1);

    }
    private void UpdateRemnant() 
    {
        if (!activateRemnant)
            return;
        remnantTime += Time.deltaTime * accel;
        float value = Mathf.InverseLerp(0, _maxHealthPoints, HealthPoints);
        value = Mathf.Lerp(0, 1, value);
        _healthBarRemnant.fillAmount = Mathf.Lerp(oldValue, value, remnantTime / maxRemnantTime);

        if (remnantTime >= maxRemnantTime)
        {
            remnantTime = 0;
            activateRemnant = false;
        }
    }
    void FixedUpdate()
    {
        if (_isBlinking) _body.SetActive(!_body.activeSelf);
    }

    [Button]
    protected override void Die()
    {
        SoundManager.Instance.PlayBossDie();
        UiManager.Instance.PreWinScreen();
        base.Die();
        Time.timeScale = 0.2f;
        this.enabled=false;
    }
}
