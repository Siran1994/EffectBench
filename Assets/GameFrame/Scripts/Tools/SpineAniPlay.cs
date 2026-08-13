using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

public class SpineAniPlay : MonoBehaviour
{
    public SkeletonAnimation SpineAni;

    public string Aniname;

    private void OnEnable()
    {
        PlayAni(Aniname, false);
    }

    public void PlayAni(string state, bool isLoop)
    {
        SpineAni.ClearState();
        SpineAni.timeScale = 1f;
        SpineAni.AnimationState.SetAnimation(0, state, isLoop);
        SpineAni.AnimationState.Complete += OnComplet;
    }

    void OnComplet(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == Aniname)
        {
            PlayAni("daiji", true);
        }
    }
}
