using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Expression 
{
    Normal, Anger, Fear, Hurt, Happy, Sad,
}

public class HamsterStateMachine : MonoBehaviour
{
    public Material[] eyeMaterials;

    [SerializeField] private SkinnedMeshRenderer leftEye;
    [SerializeField] private SkinnedMeshRenderer rightEye;
    [SerializeField] private Animator animator;

    public void SetExpression(Expression expression)
    {
        leftEye.material = eyeMaterials[((int)expression)];
        rightEye.material = eyeMaterials[((int)expression)];
    }

    public void SetState(string state, bool value)
    {
        animator.SetBool(state, value);
    }

    public void SetSpeed (float speed)
    {
        animator.speed = speed;
        animator.SetFloat("currentSpeed", speed);
    }

    public void ResetState()
    {
        animator.SetBool("isStunned", false);
        animator.SetBool("isTurning", false);
        animator.SetFloat("currentSpeed", 0.5f);
        animator.speed = 1f;
        SetExpression(Expression.Normal);
        animator.ResetTrigger("celebrate");
        animator.SetTrigger("idle");
    }

    public void StartAnim(string trigger, Expression expression)
    {
        ResetState();
        animator.ResetTrigger("idle");
        animator.SetTrigger(trigger);
        SetExpression(expression);
    }
}
