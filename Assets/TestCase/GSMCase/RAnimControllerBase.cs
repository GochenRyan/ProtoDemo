using System;
using System.Collections.Generic;
using UnityEngine;
using ZeroPass;

public class RAnimControllerBase : MonoBehaviour
{
    public struct AnimEntry
    {
        public string anim;

        public PlayMode mode;

        public float speed;

        public float timeOffset;
    }

    private Animator animator;
    private Queue<AnimEntry> animQueue = new Queue<AnimEntry>();
    private bool stopped = true;
    private AnimEntry? curAnim = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsPlaying() && animQueue.Count > 0)
        {
            PlayNext();
        }
    }

    private bool IsPlaying()
    {
        var state = animator.GetCurrentAnimatorStateInfo(0);
        return state.normalizedTime < 1f || animator.IsInTransition(0);
    }

    private void PlayNext()
    {
        stopped = false;

        var entry = animQueue.Dequeue();
        animator.speed = entry.speed;
        animator.Play(entry.anim, 0, entry.timeOffset);

        if (curAnim?.mode == PlayMode.Loop  && animQueue.Count == 0)
        {
            animQueue.Enqueue((AnimEntry)curAnim);
        }
    }

    public void Play(string anim_name, PlayMode mode = PlayMode.Once, float speed = 1f, float time_offset = 0f)
    {
        if (!stopped)
        {
            Stop();
        }
        Queue(anim_name, mode, speed, time_offset);
    }

    public void Play(string[] anim_names, PlayMode mode = PlayMode.Once, float speed = 1f, float time_offset = 0f)
    {
        if (!stopped)
        {
            Stop();
        }

        for (int i = 0; i < anim_names.Length - 1; i++)
        {
            Queue(anim_names[i], PlayMode.Once, speed, time_offset);
        }

        Queue(anim_names[anim_names.Length - 1], mode, speed, time_offset);
    }

    public void Queue(string anim_name, PlayMode mode = PlayMode.Once, float speed = 1f, float time_offset = 0f)
    {
        animQueue.Enqueue(new AnimEntry
        {
            anim = anim_name,
            mode = mode,
            speed = speed,
            timeOffset = time_offset
        });

        if (animQueue.Count == 1 && stopped)
        {
            PlayNext();
        }
    }

    public void Stop()
    {
        animQueue.Clear();
        stopped = true;
        curAnim = null;
    }
}
