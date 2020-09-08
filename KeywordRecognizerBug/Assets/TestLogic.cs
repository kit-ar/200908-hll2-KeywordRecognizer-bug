using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class TestLogic : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer;

    void Start()
    {
        StartCoroutine(Test());
    }

    //void OnApplicationFocus(bool hasFocus)
    //{
    //    if (hasFocus)
    //    {
    //        // regained focus
    //        Debug.Log("Application gained focus.");
    //    }
    //    else
    //    {
    //        Debug.Log("Application lost focus.");

    //        //KitVoiceCommands.Instance.StopListening();

    //        //if (m_mainFlowState == EMAINFLOWSTATE.AssemblyMode)
    //        //{
    //        //    // pause
    //        //    m_assemblyModeFlow.Pause();
    //        //}
    //        //SoundManager.Instance.PlayMarkerRejectedSoundFX(); note: sound doesn't play (app sleeps)
    //    }
    //}

    private IEnumerator Test()
    {
        Debug.Log("Starting test in 2s...");
        yield return new WaitForSeconds(2);

        List<string> _keywords = new List<string>() { "Test Voice Command", "Test Other" }; 

        Debug.Log("Initializing KeywordRecognizer and registering voice commands: 'Test Voice Command' & 'Test Other'");

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(_keywords.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

        keywordRecognizer.Start();

        Debug.Log("Started recognition. Waiting forever, just say 'Test Voice Command' or 'Test Other' and check that the recognition message appears.");
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
        builder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
        builder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
        Debug.Log(builder.ToString());
    }
}
