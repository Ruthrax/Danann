using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using NaughtyAttributes;
using System;

public class SpearFeedbacks : MonoBehaviour
{
    #region anim
    [SerializeField] Transform _mesh;
    Vector3 _swingingRot = new Vector3(0, 90, 90);
    bool _isSwinging = false;
    float _swingAnimT = 0;
    [Required][SerializeField] TrailRenderer _bladeTrail;

    [Required][SerializeField] CinemachineTargetGroup _targetGroup;

    #endregion

    #region Audio
    [Required][SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _reattach;
    [SerializeField] AudioClip _swing;

    private void PlaySound(AudioClip clip, float volume)
    {
        _audioSource.PlayOneShot(clip, volume);
    }

    public void PlayReattach()
    {
        PlaySound(_reattach, 1f);
    }

    public void PlaySwing()
    {
        PlaySound(_swing, 1f);
    }
    #endregion

    void Start()
    {
        _isSwinging = false;
    }

    public void SetCameraTargetWeight(int target, int weight)
    {
        _targetGroup.m_Targets[target].weight = weight;
    }

    #region animation
    [Button]
    public void StartSwingAnimation()
    {
        SetMeshRotation(_swingingRot);
        _swingAnimT = 0;
        _isSwinging = true;
        EnableTrail();
    }

    public void StopSwingAnimation()
    {
        _isSwinging = false;
        DisableTrail();
        SetMeshRotation(Vector3.zero);
    }

    public void EnableTrail()
    {
        _bladeTrail.emitting = true;
    }
    public void DisableTrail()
    {
        _bladeTrail.emitting = false;
    }

    void AnimateSwing()
    {
        _swingAnimT += Time.deltaTime * 0.6f;
        SetMeshRotation(_mesh.rotation.eulerAngles + Vector3.up * Mathf.Lerp(0, 360, _swingAnimT));
    }

    public void SetMeshRotation(Vector3 rotation)
    {
        _mesh.rotation = Quaternion.Euler(rotation);
    }

    public void SetMeshForward(Vector3 direction)
    {
        _mesh.forward = direction;
    }

    public void SetMeshUp(Vector3 direction)
    {
        _mesh.up = direction;
    }
    #endregion

    void Update()
    {
        if (_isSwinging)
            AnimateSwing();
    }
}