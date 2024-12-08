using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    private Animator _animator;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetHeal()
    {
        _animator.SetTrigger("heal");
    }
    
    public void SetIdling(bool value)
    {
        _animator.SetBool("isIdling", value);
    }
}
