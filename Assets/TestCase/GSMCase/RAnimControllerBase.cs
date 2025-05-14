using System;
using System.Collections.Generic;
using UnityEngine;

public class RAnimControllerBase : MonoBehaviour
{
    public struct AnimEntry
    {
        public enum EntryType { Animation, Callback }

        public EntryType type;
        public string anim;
        public PlayMode mode;
        public float speed;
        public float timeOffset;

        public Action callback;
        /// <summary>
        /// For Loop animations: 
        /// true = invoke only once after first loop
        /// false = invoke after every loop completion
        /// </summary>
        public bool invokeOnce;
    }

    private Animator animator;
    private Queue<AnimEntry> animQueue = new Queue<AnimEntry>();
    private AnimEntry? curAnim = null;
    public string initialAnim;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (!string.IsNullOrEmpty(initialAnim))
        {
            Play(initialAnim, PlayMode.Loop, 1f, 0f);
        }
    }

    private void Update()
    {
        if (!IsPlaying() && animQueue.Count > 0)
        {
            PlayNext();
        }
    }

    public void PlayNext()
    {
        Queue<AnimEntry> repeatActionQueue = new Queue<AnimEntry>();
        while (animQueue.Count > 0 && animQueue.Peek().type == AnimEntry.EntryType.Callback)
        {
            var actionEntry = animQueue.Dequeue();
            actionEntry.callback?.Invoke();
            if (!actionEntry.invokeOnce && curAnim != null && curAnim.Value.mode == PlayMode.Loop)
            {
                repeatActionQueue.Enqueue(actionEntry);
            }
        }

        var entry = animQueue.Dequeue();
        animator.speed = entry.speed;
        animator.Play(entry.anim, 0, entry.timeOffset);

        curAnim = entry;

        if (curAnim.HasValue && curAnim.Value.mode == PlayMode.Loop && animQueue.Count == 0)
        {
            animQueue.Enqueue((AnimEntry)curAnim);
            while (repeatActionQueue.Count > 0)
            {
                animQueue.Enqueue(repeatActionQueue.Dequeue());
            }
        }
    }

    private bool IsPlaying()
    {
        var state = animator.GetCurrentAnimatorStateInfo(0);
        return state.normalizedTime < 1f || animator.IsInTransition(0);
    }


    public RAnimControllerBase Play(string anim_name, PlayMode mode = PlayMode.Once, float speed = 1f, float time_offset = 0f)
    {
        Stop();
        Queue(anim_name, mode, speed, time_offset);
        PlayNext();
        return this;
    }

    public RAnimControllerBase Play(string[] anim_names, PlayMode mode = PlayMode.Once, float speed = 1f, float time_offset = 0f)
    {
        Stop();

        for (int i = 0; i < anim_names.Length - 1; i++)
        {
            Queue(anim_names[i], PlayMode.Once, speed, time_offset);
        }

        Queue(anim_names[anim_names.Length - 1], mode, speed, time_offset);
        PlayNext();

        return this;
    }

    public RAnimControllerBase Queue(string anim_name, PlayMode mode = PlayMode.Once, float speed = 1f, float time_offset = 0f)
    {
        animQueue.Enqueue(new AnimEntry
        {
            type = AnimEntry.EntryType.Animation,
            anim = anim_name,
            mode = mode,
            speed = speed,
            timeOffset = time_offset,
            callback = null,
            invokeOnce = true
        });

        return this;
    }

    public void Queue(Action animEvent, bool invokeOnce = true)
    {
        animQueue.Enqueue(new AnimEntry
        {
            type = AnimEntry.EntryType.Callback,
            callback = animEvent,
            invokeOnce = invokeOnce
        });
    }

    public void Stop()
    {
        animQueue.Clear();
        curAnim = null;
    }
}
