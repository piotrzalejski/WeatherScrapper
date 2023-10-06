using System.Speech.Synthesis;

public class Speech
{
    public static void SpeakResp(string message)
    {
        using var synth = new SpeechSynthesizer();

        //configure audio output
        synth.SetOutputToDefaultAudioDevice();
        synth.SelectVoiceByHints(VoiceGender.Neutral);

        //speak
        synth.Speak(message);
    }
}