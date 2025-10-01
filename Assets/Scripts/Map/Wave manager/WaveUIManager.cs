using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Map.Wave_manager
{
    public class WaveUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject backdrop;
        [SerializeField] private TextMeshProUGUI waveCounterText;
        [SerializeField] private int timerBeforeWaveStart;
        [SerializeField] private int timer;

        public void StartCountdown(Action spawnEnemyCallback)
        {
            backdrop.SetActive(true);
            timer = timerBeforeWaveStart;
            StartCoroutine(StartCountdownCoroutine(spawnEnemyCallback));
        }

        IEnumerator StartCountdownCoroutine(Action spawnEnemyCallback)
        {
            while (timer > 0)
            {
                waveCounterText.text = timer.ToString();
                yield return new WaitForSeconds(1);
                timer -= 1;   
            }

            spawnEnemyCallback();
            backdrop.SetActive(false);
        }
    }
    
}
