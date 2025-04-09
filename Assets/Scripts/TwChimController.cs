using System;
using System.Collections;
using GaussianSplatting.Runtime;
using UnityEngine;

public class TwChimController : MonoBehaviour
{
    public Vector3 initPosition;
    public float speed = 1f;
    
    private PlayerController _player;
    private State _state;
    private GaussianSplatRenderer _splat;

    private enum State
    {
        Uninitialized,
        Idle,
        Turned,
        Chasing,
    }
    
    private void Start()
    {
        _player = FindFirstObjectByType<PlayerController>();
        if (_player == null)
        {
            Debug.LogError("PlayerController not found in the scene.");
        }

        _state = State.Uninitialized;
        
        _splat = GetComponentInChildren<GaussianSplatRenderer>();
        _splat.m_RenderMode = GaussianSplatRenderer.RenderMode.Hidden;
    }
    
    private void Update()
    {
        if (_player == null)
        {
            return;
        }

        switch (_state)
        {
            case State.Idle:
                if (Vector3.Distance(transform.position, _player.transform.position) < 8f)
                {
                    _state = State.Turned;
                    StartCoroutine(Turn());
                }
                break;
            case State.Turned:
                if (Vector3.Distance(transform.position, _player.transform.position) < 5f)
                {
                    _state = State.Chasing;
                }
                break;
            case State.Chasing:
                if (Vector2.Distance(
                    new Vector2(transform.position.x, transform.position.z),
                    new Vector2(_player.transform.position.x, _player.transform.position.z)
                ) < 1f)
                {
                    _state = State.Uninitialized;
                    _splat.m_RenderMode = GaussianSplatRenderer.RenderMode.Hidden;
                }
                else
                {
                    var y = transform.position.y;
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        _player.transform.position,
                        Time.deltaTime * speed
                    );
                    transform.position = new Vector3(transform.position.x, y, transform.position.z);

                    var forwardTarget = new Vector3(
                        _player.transform.position.x,
                        transform.position.y,
                        _player.transform.position.z
                    );
                    transform.forward = -(forwardTarget - transform.position).normalized;
                }
                break;
            case State.Uninitialized:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnAppearanceEnter()
    {
        if (_state != State.Uninitialized)
        {
            return;
        }
        
        _state = State.Idle;
        _splat.m_RenderMode = GaussianSplatRenderer.RenderMode.Splats;
        transform.position = initPosition;
        transform.rotation = Quaternion.identity;
    }

    public void OnAppearanceExit()
    {
        _state = State.Uninitialized;
        _splat.m_RenderMode = GaussianSplatRenderer.RenderMode.Hidden;
    }

    private IEnumerator Turn()
    {
        var target = -transform.forward;
        var start = transform.forward;
        const float duration = 1f;
        var elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var t = elapsed / duration;
            transform.forward = Vector3.Slerp(start, target, t);
            yield return null;
        }
        
        transform.forward = target;
    }
}
