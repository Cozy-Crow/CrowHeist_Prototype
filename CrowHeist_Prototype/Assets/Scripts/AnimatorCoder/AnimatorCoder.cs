//Author: Small Hedge Games
//Date: 02/07/2024

using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace SHG.AnimatorCoder
{
    public abstract class AnimatorCoder : MonoBehaviour
    {
        /// <summary> The baseline animation logic on a specific layer </summary>
        public abstract void DefaultAnimation(int layer);
        private Animator animator = null;
        private string[] currentAnimation;
        private bool[] layerLocked;
        private ParameterDisplay[] parameters;
        private Coroutine[] currentCoroutine;
        private const string RESET = "Reset";
        private AnimatorValues animatorValues = new AnimatorValues();

        /// <summary> Sets up the Animator Brain </summary>
        public void Initialize(Animator animator = null)
        {
            if(animatorValues.Initialized == false)
            {
                LogError("Please initialize Animator Values before calling Initialize() in AnimatorCoder");
                return;
            }

            if(animator == null)
                this.animator = GetComponent<Animator>();
            else
                this.animator = animator;
                
            currentCoroutine = new Coroutine[this.animator.layerCount];
            layerLocked = new bool[this.animator.layerCount];
            currentAnimation = new string[this.animator.layerCount];

            for (int i = 0; i < this.animator.layerCount; ++i)
            {
                layerLocked[i] = false;

                int hash = this.animator.GetCurrentAnimatorStateInfo(i).shortNameHash;
                currentAnimation[i] = animatorValues.GetName(hash);
            }

            string[] names = Enum.GetNames(typeof(Parameters));
            parameters = new ParameterDisplay[names.Length];
            for (int i = 0; i < names.Length; ++i)
            {
                parameters[i].name = names[i];
                parameters[i].value = false;
            }
        }

        /// <summary> Returns the current animation that is playing </summary>
        public string GetCurrentAnimation(int layer)
        {
            try
            {
                return currentAnimation[layer];
            }
            catch
            {
                LogError("Can't retrieve Current Animation. Fix: Initialize() in Start() and don't exceed number of animator layers");
                return RESET;
            }
        }

        /// <summary> Sets the whole layer to be locked or unlocked </summary>
        public void SetLocked(bool lockLayer, int layer)
        {
            try
            {
                layerLocked[layer] = lockLayer;
            }
            catch
            {
                LogError("Can't retrieve Current Animation. Fix: Initialize() in Start() and don't exceed number of animator layers");
            }
        }

        public bool IsLocked(int layer)
        {
            try
            {
                return layerLocked[layer];
            }
            catch
            {
                LogError("Can't retrieve Current Animation. Fix: Initialize() in Start() and don't exceed number of animator layers");
                return false;
            }
        }

        /// <summary> Sets an animator parameter </summary>
        public void SetBool(Parameters id, bool value)
        {
            try
            {
                parameters[(int)id].value = value;
            }
            catch
            {
                LogError("Please Initialize() in Start()");
            }
        }

        /// <summary> Returns an animator parameter </summary>
        public bool GetBool(Parameters id)
        {
            try
            {
                return parameters[(int)id].value;
            }
            catch
            {
                LogError("Please Initialize() in Start()");
                return false;
            }
        }

        /// <summary> Takes in the animation details and the animation layer, then attempts to play the animation </summary>
        public bool Play(AnimationData data, int layer = 0)
        {
            try
            {
                if (data.animation == "Reset")
                {
                    DefaultAnimation(layer);
                    return false;
                }

                if (layerLocked[layer] || currentAnimation[layer] == data.animation) return false;

                if (currentCoroutine[layer] != null) StopCoroutine(currentCoroutine[layer]);
                layerLocked[layer] = data.lockLayer;
                currentAnimation[layer] = data.animation;

                animator.CrossFade(animatorValues.GetHash(currentAnimation[layer]), data.crossfade, layer);

                if (data.nextAnimation != null)
                {
                    currentCoroutine[layer] = StartCoroutine(Wait());
                    IEnumerator Wait()
                    {
                        animator.Update(0);
                        float delay = animator.GetNextAnimatorStateInfo(layer).length;
                        if (data.crossfade == 0) delay = animator.GetCurrentAnimatorStateInfo(layer).length;
                        yield return new WaitForSeconds(delay - data.nextAnimation.crossfade);
                        SetLocked(false, layer);
                        Play(data.nextAnimation, layer);
                    }
                }

                return true;
            }
            catch
            {
                LogError("Please Initialize() in Start()");
                return false;
            }
        }

        private void LogError(string message)
        {
            Debug.LogError("AnimatorCoder Error: " + message);
        }
    }

    /// <summary> Holds all data about an animation </summary>
    [Serializable]
    public class AnimationData
    {
        public string animation;
        /// <summary> Should the layer lock for this animation? </summary>
        public bool lockLayer;
        /// <summary> Should an animation play immediately after? </summary>
        public AnimationData nextAnimation;
        /// <summary> Should there be a transition time into this animation? </summary>
        public float crossfade = 0;

        /// <summary> Sets the animation data </summary>
        public AnimationData(string animation = "Reset", bool lockLayer = false, AnimationData nextAnimation = null, float crossfade = 0)
        {
            this.animation = animation;
            this.lockLayer = lockLayer;
            this.nextAnimation = nextAnimation;
            this.crossfade = crossfade;
        }
    }

    /// <summary> Allows the animation parameters to be shown in debug inspector </summary>
    [Serializable]
    public struct ParameterDisplay
    {
        [HideInInspector] public string name;
        public bool value;
    }
}
