//Author: Small Hedge Games
//Date: 05/04/2024
using UnityEngine;
using System.Collections.Generic;

namespace SHG.AnimatorCoder
{
    /// <summary> Class the manages the hashes of animations and parameters </summary>
    public class AnimatorValues
    {
        private Dictionary<string, int> animation = new();
        private bool initialized = false;
        /// <summary> Returns the animation hash array </summary>
        public bool Initialized { get { return initialized; } }

        /// <summary>
        /// Initializes the animation hashes using an array of animation names
        /// </summary>
        /// <param name="animationStrings"> The string array of animation names</param>
        public void Initialize(string[] animationStrings)
        {
            if (initialized) return;
            initialized = true;

            for (int i = 0; i < animationStrings.Length; i++)
            {
                animation[animationStrings[i]] = Animator.StringToHash(animationStrings[i]);
            }
        }

        public int GetHash(string animationName)
        {
            try
            {
                return animation[animationName];
            }
            catch
            {
                Debug.LogError("AnimatorValues Error: Animation name " + animationName + " not found. Did you initialize the Animator Values?");
                return -1;
            }
        }

        public string GetName(int hash)
        {
            foreach (var kvp in animation)
            {
                if (kvp.Value == hash)
                    return kvp.Key;
            }
            
            Debug.LogError("AnimatorValues Error: Hash " + hash + " not found.");
            return null;
        }
    }
    /// <summary> Complete list of all animation state names </summary>
    public enum Animations
    {
        //Change the list below to your animation state names
        IDLE,
        RUN,
        ATTACK1,
        ATTACK2,
        HIT,
        JUMP,
        FALL,
        RESET  //Keep Reset
    }

    /// <summary> Complete list of all animator parameters </summary>
    public enum Parameters
    {
        //Change the list below to your animator parameters
        GROUNDED,
        FALLING
    }
}


