using System.Collections.Generic;
using Abu.Tools;
using RuntimeHandle;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCamera : MonoBehaviour
{
    static AudioSource backMusic;
    
    
    Camera Camera { get; set; }

    
    [SerializeField] RenderTexture tempTex;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Texture2D targetSilhouette;
    [SerializeField] Texture2D currentSilhouette;
    [SerializeField] Camera safeCamera;
    [SerializeField] Image confidenceImage;
    [SerializeField] GameObject blocker;
    
    [SerializeField] GameObject targetObjects;
    [SerializeField] GameObject realObjects;
    [SerializeField] Material whiteUnlit;

    [SerializeField] float confidence = 0.002f;
    [SerializeField] CanvasGroup group;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip music;

    [SerializeField] GameObject help;

    [SerializeField] MeshRenderer mesh;
    

    float lastCheckTime = -1;

    int Width  => 1600 / 8;
    int Height => 1200 / 8;
    
    void Awake()
    {

        if (backMusic == null)
        {
            GameObject soundGo =  new GameObject("Sound");
            DontDestroyOnLoad(soundGo);
            backMusic = soundGo.AddComponent<AudioSource>();
            backMusic.spatialize = false;
            backMusic.clip = music;
            backMusic.loop = true;
            backMusic.volume = 0.65f;
            backMusic.Play();
        }
        
        Camera = Camera.main;

        tempTex = RenderTexture.GetTemporary(Width, Height, 0);

        targetSilhouette  = new Texture2D(Width, Height);
        currentSilhouette = new Texture2D(Width, Height);

        realObjects.SetActive(false);
        targetObjects.SetActive(true);
        
        RenderSilhouette(targetSilhouette);
        
        sprite.sprite = Sprite.Create(targetSilhouette, 
            new Rect(0, 0, currentSilhouette.width, currentSilhouette.height),
            new Vector2(0.5f, 0.5f));

        
        realObjects.SetActive(true);
        targetObjects.SetActive(false);

        sprite.transform.localScale = ScreenScaler.ScaleToFillScreen(sprite);
    }

    float lerpStartTime = -1;
    Matrix4x4 startMatrix;
    Vector3 startPos;
    Quaternion startRot;

    public bool isWin;
    bool winTransitionStarted;
    
    
    
    
    void Update()
    {

        
        if (Input.GetKeyDown(KeyCode.H))
        {
            safeCamera.enabled = !safeCamera.enabled;
        }
        
        // safeCamera.enabled = Vector3.Distance(safeCamera.transform.position, transform.position) > 0.3f;

        if (!isWin)
        {

            // if (Input.GetKeyDown(KeyCode.Tab))
            // {
            //     transform.position = safeCamera.transform.position;
            //     transform.rotation = safeCamera.transform.rotation;
            // }


            if (Time.time - lastCheckTime > 0.01f)
            {
                lastCheckTime = Time.time;

                //Perform check

                RenderSilhouette(currentSilhouette);

                int diffPixels = ComparePixels(targetSilhouette, currentSilhouette);

                float percent = (float)diffPixels / (Width * Height);

                print(percent);

                Color color = Color.Lerp(Color.cyan, Color.red, Mathf.InverseLerp(confidence, 0.07f, percent));
                mesh.sharedMaterial.SetColor("_Color1", color);
                // confidenceImage.color = color;

                if (percent < confidence)
                {
                    isWin = true;
                }
            }
        }


        if (!winTransitionStarted && isWin)
        {
            winTransitionStarted = true;
            lerpStartTime = Time.time;
            startMatrix = Camera.projectionMatrix;
            startPos = transform.position;
            startRot = transform.rotation;
            audioSource.Play();
            blocker.SetActive(true);
        }

        if (lerpStartTime > 0)
        {
            float phase = Mathf.Min(Time.time - lerpStartTime, 2f) / 2f;
            
            // print("PHASE " + phase);

            FindObjectOfType<RuntimeTransformHandle>().transform.localScale = Vector3.one * (1 - phase);  
            phase *= phase;
            
            Matrix4x4 matrix = MatrixLerp(startMatrix, safeCamera.projectionMatrix, phase);
            Camera.projectionMatrix = matrix;

            group.alpha = phase;
            
            transform.position = Vector3.Lerp(startPos, safeCamera.transform.position, phase);
            transform.rotation = Quaternion.Lerp(startRot, safeCamera.transform.rotation, phase);
        }
        
    }

    [ContextMenu("Text")]
    void Test()
    {
        // RenderSilhouette(currentSilhouette);
        //
        // int diffPixels = ComparePixels(targetSilhouette, currentSilhouette);
        //
        // float percent = (float)diffPixels / (Width * Height);
        //
        // print("==========");
        // print(diffPixels);
        // print(percent);
        
        audioSource.Play();
    }
    
    int ComparePixels(Texture2D source, Texture2D target)
    {
        Color32[] sourcePixels = source.GetPixels32();
        Color32[] targetPixels = target.GetPixels32();

        int diff = 0;
        
        for (int i = 0; i < source.width * source.height; i++)
        {
            Color32 sPix = sourcePixels[i];
            Color32 tPix = targetPixels[i];

            if (sPix.r != tPix.r)
            {
                diff++;
            }
        }

        return diff;
    }
    
    void RenderSilhouette(Texture2D tex)
    {
        Dictionary<MeshRenderer, Material> materials = new Dictionary<MeshRenderer, Material>();

        foreach (GameObject selectable in GameObject.FindGameObjectsWithTag("Selectable"))
        {
            MeshRenderer mesh = selectable.GetComponent<MeshRenderer>();
            
            if (mesh != null)
            {
                materials.Add(mesh, mesh.sharedMaterial);
                mesh.sharedMaterial = whiteUnlit;
            }
        }
        
        Rect cachedRect = safeCamera.rect;
        safeCamera.rect = new Rect(0, 0, 1, 1);
        
        safeCamera.targetTexture = tempTex;

        safeCamera.clearFlags = CameraClearFlags.SolidColor;
        safeCamera.backgroundColor = Color.black;
        
        safeCamera.Render();

        foreach (var kvp in materials)
            kvp.Key.sharedMaterial = kvp.Value;
        
        safeCamera.targetTexture = null;
        
        RenderTexture.active = tempTex;
        tex.ReadPixels(new Rect(0, 0, tempTex.width, tempTex.height), 0, 0);
        tex.Apply();

        safeCamera.rect = cachedRect;
    }
    
    
    public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time)
    {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(from[i], to[i], time);
        return ret;
    }


    public void NextLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        LevelTransition.LoadLevel(index + 1);
    }
    
    
    public void RestartLevel()
    {
        LevelTransition.LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void ToggleHelp()
    {
        help.SetActive(!help.activeSelf);
    }
    
    
}
