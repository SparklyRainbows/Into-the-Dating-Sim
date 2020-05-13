using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

public class Unit : MonoBehaviour {
    private TextMeshPro damageForecast;

    public Tile tile;
    [HideInInspector] public Tile prevTile;
    public Directions dir;
    public UnitType type;

    protected SpriteRenderer renderer;
    protected Animator animator;

    public Animator statusAnimator;

    protected virtual void Start() {
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        damageForecast = GetComponentInChildren<TextMeshPro>();
        HideDamageForecast();
    }

    public virtual void Place(Tile target) {
        if (tile != null && tile.content == gameObject)
            tile.content = null;
        
        tile = target;

        if (target != null) {
            if (target.content != null &&
                target.content.GetComponent<UnitStats>().HasStatus(StatusEffects.BURN)) {
                GetComponent<UnitStats>().AddStatus(StatusEffects.BURN);
            }

            target.content = gameObject;
        }
    }

    public void Match() {
        transform.localPosition = tile.GetPosition();
        transform.localEulerAngles = dir.ToEuler();
    }

    public void Die() {
        if (tile != null && tile.content == gameObject)
            tile.content = null;

        Destroy(gameObject);
    }

    public virtual bool KnockbackAble() {
        return true;
    }

    #region Animations
    public virtual void InitAnimator(UnitInfo info) {

    }

    protected virtual void ReplaceAnimation(AnimationClip oldClip, UnitInfo info, List<KeyValuePair<AnimationClip, AnimationClip>> anims) {

    }

    public virtual float PlayAttack(Tile target) {
        return 0;
    }

    public void PlayStatus(StatusEffects status) {
        if (status == StatusEffects.BURN) {
            statusAnimator.SetBool("Burn", true);
        }
        if (status == StatusEffects.FREEZE) {
            statusAnimator.SetBool("Freeze", true);
        }
    }

    public void StopPlayingStatus(StatusEffects status) {
        if (status == StatusEffects.BURN) {
            statusAnimator.SetBool("Burn", false);
        }
        if (status == StatusEffects.FREEZE) {
            statusAnimator.SetBool("Freeze", false);
        }
    }
    #endregion

    #region Damage
    public void TakeDamage() {
        if (type != UnitType.TERRAIN) {
            StartCoroutine(FlashRed());
        }
    }

    private IEnumerator FlashRed() {
        Color temp = renderer.color;

        renderer.color = Color.red;
        yield return new WaitForSeconds(.1f);
        renderer.color = temp;
    }
    #endregion

    #region Forecast
    public void HideDamageForecast() {
        damageForecast.enabled = false;
    }

    public void ShowDamageForecast(int damage) {
        damageForecast.enabled = true;
        damageForecast.text = damage.ToString();
    }
    #endregion
}

public enum UnitType {
    PLAYER,
    ENEMY,
    TERRAIN
}