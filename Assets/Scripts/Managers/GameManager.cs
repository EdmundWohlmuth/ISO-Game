using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public static Camera mainCam;
    public int woodPool;
    public int stonePool;
    public int foodPool;

    public Mesh ProgressMesh;
    public Material ProgressMat;
    public static int progressMaterialProperty = -1;
    public MaterialPropertyBlock mpb;

    private void Awake()
    {
        mainCam = Camera.main;
        mpb = new MaterialPropertyBlock();
        progressMaterialProperty = Shader.PropertyToID("_Progress");

        if (gameManager == null)
        {
            gameManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (gameManager != this && gameManager != null)
        {
            Destroy(this.gameObject);
        }
    }

    public void DisplayProgress(Vector3 pos, float height, float progressAmmount, float size = 0.5f)
    {
        pos += Vector3.up * height;
        mpb.SetFloat(progressMaterialProperty, progressAmmount);
        Vector3 cameraDirection = pos - mainCam.transform.position; // ? == if, : == else
        Quaternion rotation = cameraDirection == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation( cameraDirection - mainCam.transform.up);
        Graphics.DrawMesh(gameManager.ProgressMesh, Matrix4x4.TRS(pos, rotation, Vector3.one * size), gameManager.ProgressMat, 7, null, 0, mpb);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        woodPool = 0;
        stonePool = 0;
        foodPool = 0;
    }
}
