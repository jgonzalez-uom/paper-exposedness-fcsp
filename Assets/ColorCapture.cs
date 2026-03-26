using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorCapture : MonoBehaviour
{
    public Camera[] recordingCameras;
    public Animator animator;
    public string format;
    [HideInInspector]
    public static float distanceFromTarget = 0;
    private bool recordingInProgress = false;

    [SerializeField]
    private RecordingAnimationClass[] animations;
    public int framesPerSecond = 20;

    public enum captureMode { UnityScreenshot, SplitMethod};
    public captureMode captureModeSelected;
    public int numRows = 5;
    public int numColumns = 5;
    public RenderTexture outputRect;

    [System.Serializable]
    private class RecordingAnimationClass
    {
        public AnimationClip animation;
        public KeyCode startRecordingKey;
        public UnityEngine.Events.UnityEvent OnStart;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var r in animations)
        {
            if (!recordingInProgress && Input.GetKeyDown(r.startRecordingKey))
            {
                if (captureModeSelected == captureMode.UnityScreenshot)
                    StartCoroutine(RecordAnimation(r));
                else
                    StartCoroutine(RecordAnimationRenderTexture(r));
                //StartCoroutine(RecordAnimationRenderTexture(r));
            }
        }

    }

    private IEnumerator RecordAnimation(RecordingAnimationClass anim)
    {
        recordingInProgress = true;

        anim.OnStart.Invoke();

        string clipName = System.DateTime.Now.ToString().Replace('/', '-').Replace(' ', '_').Replace(':','-');
        AnimationClip animation = anim.animation;
        int frameCount = Mathf.RoundToInt(animation.length * framesPerSecond) - 1;

        System.IO.Directory.CreateDirectory(string.Format("{0}/{1}/", Application.persistentDataPath, clipName));

        for (int n = 0; n < frameCount; n++)
        {
            animator.Play(animation.name, -1, (float)n / frameCount);

            foreach (var c in recordingCameras)
            {
                foreach (var t in recordingCameras)
                    t.gameObject.SetActive(false);
                c.gameObject.SetActive(true);

                Debug.Log("Capturing...");
                yield return new WaitForEndOfFrame();

                Texture2D texture;

                texture = ScreenCapture.CaptureScreenshotAsTexture();
                texture.filterMode = FilterMode.Point;

                Debug.Log("Encoding...");
                byte[] test = ImageConversion.EncodeToPNG(texture);
                System.IO.File.WriteAllBytes(string.Format("{0}/{1}/{1}_{2}_{3}m.{4}", Application.persistentDataPath, clipName, n, distanceFromTarget.ToString(), format), test);
                Debug.Log("Clearing memory...");
                test = null;
                texture = null;
                Destroy(texture);
                Debug.Log("Memory clear?");
            }
        }

        recordingInProgress = false;

        Debug.Log("Done!");
    }

    private IEnumerator RecordAnimationRenderTexture(RecordingAnimationClass anim)
    {
        recordingInProgress = true;

        anim.OnStart.Invoke();

        string clipName = System.DateTime.Now.ToString().Replace('/', '-').Replace(' ', '_').Replace(':', '-');
        AnimationClip animation = anim.animation;
        int frameCount = Mathf.RoundToInt(animation.length * framesPerSecond) - 1;

        System.IO.Directory.CreateDirectory(string.Format("{0}/{1}/", Application.persistentDataPath, clipName));

        for (int n = 0; n < frameCount; n++)
        {
            animator.Play(animation.name, -1, (float)n / frameCount);

            foreach (var c in recordingCameras)
            {
                foreach (var t in recordingCameras)
                    t.gameObject.SetActive(false);
                c.gameObject.SetActive(true);

                RenderTexture.active = c.targetTexture;
                Camera.main.Render();

                Debug.Log("Capturing...");
                yield return new WaitForEndOfFrame();

                int width = Camera.main.pixelWidth / numColumns;
                int height = Camera.main.pixelHeight / numRows;
                int startX, startY;

                for (int x = 0; x < numColumns; x++)
                {
                    for (int y = 0; y < numRows; y++)
                    {
                        startX = x * width;
                        startY = y * height;

                        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

                        Rect rex = new Rect(startX, startY, width, height);


                        tex.ReadPixels(rex, 0, 0);
                        tex.Apply();

                        // Encode texture into PNG
                        var bytes = tex.EncodeToPNG();
                        Destroy(tex);

                        string fileName = string.Format("{0}_({1},{2})_{3}m.{4}", clipName, x, y, distanceFromTarget.ToString(), "png");
                        System.IO.File.WriteAllBytes(string.Format("{0}/{1}/{2}", Application.persistentDataPath, clipName, fileName), bytes);

                        Destroy(tex);
                        tex = null;
                    }
                }
            }
        }

        recordingInProgress = false;

        Debug.Log("Done!");
    }

}
