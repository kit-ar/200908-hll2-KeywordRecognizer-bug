using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;

#if ENABLE_WINMD_SUPPORT
   using Windows.ApplicationModel;
   using Windows.ApplicationModel.Core;
#endif

public class TestLogic : MonoBehaviour
{
    private bool _fNeedEngineReset = false;

    KeywordRecognizer _keywordRecognizer;

    List<string> _keywords = new List<string>() { "Test Voice Command", "Test Other" };

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

#if ENABLE_WINMD_SUPPORT
    private void OnResuming(object sender, object args)
    {
        _fNeedEngineReset = true;
    }
#endif

    void Update()
    {
        if (_fNeedEngineReset)
        {
            UnityEngine.Debug.Log("Suspend / Resume Speech Engine Reset");

            _fNeedEngineReset = false;

            PhraseRecognitionSystem.Shutdown();
            PhraseRecognitionSystem.Restart();
        }
    }

    private IEnumerator Test()
    {
        Debug.Log("Starting test in 2s...");
        yield return new WaitForSeconds(2);

        Debug.Log("Initializing KeywordRecognizer and registering voice commands: 'Test Voice Command' & 'Test Other'");

        PhraseRecognitionSystem.OnStatusChanged += StatusHandler;

#if ENABLE_WINMD_SUPPORT
        CoreApplication.Resuming += this.OnResuming;
#endif

        // Tell the KeywordRecognizer about our keywords.
        _keywordRecognizer = new KeywordRecognizer(_keywords.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        _keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

        _keywordRecognizer.Start();

        Debug.Log("Started recognition. Waiting forever, just say 'Test Voice Command' or 'Test Other' and check that the recognition message appears.");
    }

    private void StatusHandler(SpeechSystemStatus status)
    {
        Debug.Log("SPEECH API STATUS CHANGED: " + status);

        if(status != SpeechSystemStatus.Running)
        {
            RebindVoiceCommands();
        }
    }

    private void RebindVoiceCommands()
    {
        _keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
        _keywordRecognizer.Dispose();

        // Tell the KeywordRecognizer about our keywords.
        _keywordRecognizer = new KeywordRecognizer(_keywords.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        _keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

        _keywordRecognizer.Start();
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
