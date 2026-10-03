using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class FrameTimeLogger : MonoBehaviour
{
    [SerializeField] private string label = "run";
    [SerializeField] private float warmupSeconds = 10f;
    [SerializeField] private float sampleSeconds = 30f;
    [SerializeField] private bool quitWhenDone = true;

    private float[] _frameMs;
    private int _count;
    private float _elapsed;
    private bool _done;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
        _frameMs = new float[200_000];
    }

    private void Update()
    {
        if (_done) return;

        float dt = Time.unscaledDeltaTime;
        _elapsed += dt;

        if (_elapsed < warmupSeconds) return;

        if (_elapsed < warmupSeconds + sampleSeconds)
        {
            if (_count < _frameMs.Length) _frameMs[_count++] = dt * 1000f;
            return;
        }

        Save();
        _done = true;
        if (quitWhenDone) Application.Quit();
    }

    private void Save()
    {
        var sb = new StringBuilder(_count * 8);
        sb.AppendLine("frame_ms");
        for (int i = 0; i < _count; i++)
            sb.AppendLine(_frameMs[i].ToString("F4", CultureInfo.InvariantCulture));

        string path = Path.Combine(Application.persistentDataPath, label + ".csv");
        File.WriteAllText(path, sb.ToString());
        Debug.Log($"FrameTimeLogger: saved {_count} frames to {path}");
    }
}
