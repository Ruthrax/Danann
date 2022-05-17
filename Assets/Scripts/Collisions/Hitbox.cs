using System.Collections;
using UnityEngine;
using System;

public class Hitbox : MonoBehaviour
{
    #region Hitbox Visualization
#if UNITY_EDITOR
    Color _currentColor;
    Color _sphereColor = new Color(1f, 0.1f, 0.1f, 0.2f);
    Color _sphereWireColor = new Color(0.5f, 0f, 0f, 0.5f);
    Mesh _mesh;

    private void OnDrawGizmos()
    {
        SOMeshes.Init();
        _mesh = SOMeshes.Instance.HitboxDebugSphere;
        Gizmos.color = _sphereColor;
        Gizmos.DrawMesh(_mesh, -1, transform.position, Quaternion.identity, new Vector3(_radius, _radius, _radius));
        Gizmos.color = _sphereWireColor;
        Gizmos.DrawWireMesh(_mesh, -1, transform.position, Quaternion.identity, new Vector3(_radius, _radius, _radius));
    }
#endif
    #endregion
    [SerializeField] float _radius;
    Hurtbox[] _hurtboxesToFocus;
    string _attackName;
    int _attackDamage;

    public void Setup(Hurtbox[] hurtboxesToFocus, string attackName, int attackDamage)
    {
        _hurtboxesToFocus = hurtboxesToFocus;
        _attackDamage = attackDamage;
        _attackName = attackName;
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        CheckForHit();
    }

    public void CheckForHit()
    {
        if (_hurtboxesToFocus.Length == 0)
        {
            Debug.LogError("Initialized incorrectly: no hurtboxes");
            return;
        }
        foreach (Hurtbox hurtbox in _hurtboxesToFocus)
        {
            if (CheckDistance(hurtbox))
                hurtbox.TakeHit(_attackName, _attackDamage);
        }
    }

    private bool CheckDistance(Hurtbox hurtbox)
    {
        float distance = (hurtbox.transform.position - transform.position).sqrMagnitude;
        if (distance < (_radius / 2 + hurtbox.Radius / 2) * (_radius / 2 + hurtbox.Radius / 2))
            return true;
        return false;
    }
}