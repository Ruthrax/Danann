using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackShake
{
    static public Dictionary<string, Vector2> ShakeValue = new Dictionary<string, Vector2>()
    {
        {Ccl_Attacks.LIGHTATTACK1, new Vector2(0.05f, 0.2f)},
        {Ccl_Attacks.LIGHTATTACK2, new Vector2(0.1f, 0.3f)},
        {Ccl_Attacks.LIGHTATTACK3, new Vector2(0.2f, 0.75f)},
        {Ccl_Attacks.TRIANGLEBOOM, new Vector2(0.3f, 0.8f)},
        {Ccl_Attacks.TRIANGLETICK, new Vector2(0.08f, 0.3f)},
        {Ccl_Attacks.DASHONSPEAR, new Vector2(0.2f, 0.75f)},
        {Ccl_Attacks.SPEARSWINGL, new Vector2(0.15f, 0.4f)},
        {Ccl_Attacks.SPEARSWINGR, new Vector2(0.15f, 0.4f)},
        {Ccl_Attacks.TRAVELINGSPEAR, new Vector2(0.1f, 0.2f)},
        {Danu_Attacks.DASH, new Vector2(0.1f, 0.3f)},
        {Danu_Attacks.DASH2, new Vector2(0.1f, 0.3f)},
        {Danu_Attacks.PROJECTILE, new Vector2(0.1f, 0.3f)},
        {Danu_Attacks.SLAM1, new Vector2(0.1f, 0.3f)},
        {Danu_Attacks.SLAM2, new Vector2(0.1f, 0.3f)},
        {Danu_Attacks.SLAM3, new Vector2(0.1f, 0.3f)},
        {Danu_Attacks.TP, new Vector2(0.1f, 0.3f)}
    };

    static public Dictionary<string, Vector3> RumbleValue = new Dictionary<string, Vector3>()
    {
        {Ccl_Attacks.LIGHTATTACK1, new Vector3(0.1f,0.1f, 0.2f)},
        {Ccl_Attacks.LIGHTATTACK2, new Vector3(0.12f,0.15f, 0.3f)},
        {Ccl_Attacks.LIGHTATTACK3, new Vector3(0.2f,0.25f, 0.42f)},
        {Ccl_Attacks.TRIANGLEBOOM,    new Vector3(0.3f,0.4f, 0.4f)},
        {Ccl_Attacks.TRIANGLETICK,    new Vector3(0.1f,0.1f, 0.2f)},
        {Ccl_Attacks.DASHONSPEAR, new Vector3(0.2f,0.3f, 0.5f)},
        {Ccl_Attacks.SPEARSWINGL, new Vector3(0.12f,0.2f, 0.3f)},
        {Ccl_Attacks.SPEARSWINGR, new Vector3(0.12f,0.3f, 0.2f)},
        {Ccl_Attacks.TRAVELINGSPEAR, new Vector3(0.1f,0.05f, 0.1f)},
        {Danu_Attacks.DASH, new Vector3(0.12f,0.15f, 0.3f)},
        {Danu_Attacks.DASH2, new Vector3(0.12f,0.15f, 0.3f)},
        {Danu_Attacks.PROJECTILE, new Vector3(0.12f,0.15f, 0.3f)},
        {Danu_Attacks.SLAM1, new Vector3(0.12f,0.15f, 0.3f)},
        {Danu_Attacks.SLAM2, new Vector3(0.12f,0.15f, 0.3f)},
        {Danu_Attacks.SLAM3, new Vector3(0.12f,0.15f, 0.3f)},
        {Danu_Attacks.TP, new Vector3(0.12f,0.15f, 0.3f)}
    };
}