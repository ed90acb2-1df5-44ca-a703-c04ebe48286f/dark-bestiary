using System;
using System.Collections;
using UnityEngine;

namespace DarkBestiary
{
    public class Timer : Singleton<Timer>
    {
        public static IEnumerator Coroutine(YieldInstruction instruction, Action action)
        {
            yield return instruction;
            action();
        }

        private static IEnumerator Coroutine(CustomYieldInstruction instruction, Action action)
        {
            yield return instruction;
            action();
        }

        public void RestartCoroutine(IEnumerator coroutine)
        {
            StopCoroutine(coroutine);
            StartCoroutine(coroutine);
        }

        public void WaitUntil(Func<bool> predicate, Action action)
        {
            StartCoroutine(Coroutine(new WaitUntil(predicate), action));
        }

        public void Wait(float seconds, Action action)
        {
            StartCoroutine(Coroutine(new WaitForSeconds(seconds), action));
        }

        public void WaitForFixedUpdate(Action action)
        {
            StartCoroutine(Coroutine(new WaitForFixedUpdate(), action));
        }

        public void WaitForEndOfFrame(Action action)
        {
            StartCoroutine(Coroutine(new WaitForEndOfFrame(), action));
        }
    }
}