using UnityEngine;

public class AnimationController : MonoBehaviour
{
    enum AnimationState
    {
        Idle,
        Walk,
        Death
    } 
    
    private AnimationState _currentState;
    
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _currentState = AnimationState.Idle;
    }
    
    public void SetIdleState()
    {
        if (_currentState == AnimationState.Idle)
        {
            return;
        }
        _currentState = AnimationState.Idle;
        
        _animator.SetLayerWeight(1, 0);
        _animator.SetLayerWeight(0, 1);
        _animator.SetLayerWeight(2, 0);
    }
    
    public void SetWalkState()
    {
        if (_currentState == AnimationState.Walk)
        {
            return;
        }
        _currentState = AnimationState.Walk;
        
        _animator.SetLayerWeight(0, 0);
        _animator.SetLayerWeight(1, 1);
        _animator.SetLayerWeight(2, 0);
    }

    private void SetDeathState()
    {
        if (_currentState == AnimationState.Death)
        {
            return;
        }
        _currentState = AnimationState.Death;
        
        _animator.SetLayerWeight(0, 0);
        _animator.SetLayerWeight(1, 0);
        _animator.SetLayerWeight(2, 1);
    }
}
