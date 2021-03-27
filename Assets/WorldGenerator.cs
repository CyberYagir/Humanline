using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{

    public float lacunar;
    public float persistance;
    public int octaves;
    public float rounder;
    public Texture2D noise;
    public Vector2Int mapSize;
    public float mapScale;
    public Vector2Int offcets;
    public Vector2Int cords;
    [Space]
    public GameObject forest;
    public GameObject[] rocks;
    public GameObject hub;
    [Space]
    public Tilemap darkTilemap;
    public Tilemap waterTilemap;
    public TileBase dark;
    public TileBase water;
    public GameObject oneEnemie;
    public GameObject treesHolder;
    public GameObject envHolder;
    public Transform enemiesholder;
    public int seed = -1;

    public GameObject loadCanvas;
    public TMP_Text loadProgress;
    public TMP_Text townName;
    public int dayCount = 0;
    private void Start()
    {
        if (PlayerPrefs.HasKey("Seed"))
        {
            var p = PlayerPrefs.GetString("Seed");
            seed = int.Parse((p[0] + p[1] + p[2] + p[6] + p[4] + p[9] + p[7]).ToString());
            cords.x = int.Parse((p[3] + p[4] + p[5] + p[2] + p[6] + p[9]).ToString());
            cords.y = int.Parse((p[6] + p[7] + p[8] + p[3] + p[9] + p[1]).ToString());
            offcets.x = int.Parse((p[4] + p[7] + p[1]).ToString());
            offcets.y = int.Parse((p[1] + p[8] + p[6]).ToString());
        }
        if (PlayerPrefs.GetString("Town").Replace(" ", "") != "")
        {
            townName.text = "Town: " + PlayerPrefs.GetString("Town").Replace(" ", "");
        }
        else
        {
            townName.text = "Town: NoName";
        }
        AudioListener.volume = PlayerPrefs.GetFloat("Volume", 1);

        StartCoroutine(Gen());

    }

    public void SpawnEnemies()
    {
        dayCount++;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Cave");
        objs = objs.OrderBy(x => Vector2.Distance(this.transform.position, x.transform.position)).ToArray();
        objs = objs.ToList().FindAll(x => x.GetComponent<BuildObject>() != null).ToArray();
        for (int i = 0; i < dayCount; i++)
        {
           var gm =  Instantiate(oneEnemie.gameObject, objs[i].transform.position + new Vector3(0, -0.6f, 0), Quaternion.identity);
            gm.transform.parent = enemiesholder;
            gm.GetComponent<Enemie>().moveToHub = true;
        }

    }

    private void Update()
    {
    }
    public IEnumerator Gen()
    {
        loadCanvas.SetActive(true);
        loadProgress.text = "Building noises";
        noise = _CalcNoise(cords, mapSize, mapScale, offcets);
        var riverTex = _CalcNoise(cords, mapSize * 2, 1.6f, offcets, 10.3f, 0.89f, 3, 0.2f);
        TileBase[] waters = new Tile[riverTex.width * riverTex.width];
        Vector3Int[] waterPos = new Vector3Int[riverTex.width * riverTex.width];
        for (int x = 0; x < riverTex.width; x++)
        {
            for (int y = 0; y < riverTex.height; y++)
            {
                if (riverTex.GetPixel(x, y).r > 0.8f)
                {
                    waters[y * riverTex.width + x] = water;
                    waterPos[y * riverTex.width + x] = waterTilemap.WorldToCell(new Vector3(x / (2.5f * 2), y / (2.5f * 2), 0));
                    continue;
                }
            }
        }
        yield return new WaitForSeconds(0.2f);

        loadProgress.text = "Water build";
        waterTilemap.SetTiles(waterPos, waters);
        waterTilemap.gameObject.AddComponent<TilemapCollider2D>();
        bool[] fulled = new bool[noise.width * noise.height];

        var rn = new System.Random(seed);
        var hubpos = new Vector2(rn.Next((noise.width / 2) - (noise.width / 3), (noise.width / 2) + (noise.width / 3)) / 2.5f,
           rn.Next((noise.height / 2) - (noise.height / 3), (noise.height / 2) + (noise.height / 3)) / 2.5f);
        while (waterTilemap.GetTile(waterTilemap.WorldToCell(hubpos)) != null)
        {
            hubpos = new Vector2(rn.Next((noise.width / 2) - (noise.width / 3), (noise.width / 2) + (noise.width / 3)) / 2.5f,
              rn.Next((noise.height / 2) - (noise.height / 3), (noise.height / 2) + (noise.height / 3)) / 2.5f);
        }

        var hu = Instantiate(hub, hubpos, Quaternion.identity); ;

        int k = 0;
        loadProgress.text = "Forest build : 0%";
        yield return new WaitForSeconds(0.2f);
        for (int x = 0; x < noise.width; x++)
        {
            k++;
            if (k > 10)
            {
                yield return new WaitForSeconds(0.001f);
                k = 0;
            }
            for (int y = 0; y < noise.height; y++)
            {
                loadProgress.text = "Forest build : " + (((float)((float)x + (float)y) / (float)((float)noise.width + (float)noise.height)) * 100).ToString("0.00") + "%";

                if (waterTilemap.GetTile(waterTilemap.WorldToCell(new Vector3(x / (2.5f), y / (2.5f), 0))) != null || Dist(hu.transform.position, new Vector2((float)x / 2.5f, (float)y / 2.5f)) < 4f)
                {
                    if (Dist(hu.transform.position, new Vector2((float)x / 2.5f, (float)y / 2.5f)) < 4f){ darkTilemap.SetTile(darkTilemap.WorldToCell(new Vector3((float)x / 2.5f, (float)y / 2.5f)), null); 
                    }
                    fulled[y * noise.width + x] = true; continue;
                }

                if (noise.GetPixel(x, y).r > 0.8f)
                {
                    fulled[y * noise.width + x] = true;
                    var gm = Instantiate(forest.gameObject, new Vector3((float)x / 2.5f, (float)y / 2.5f, 0), Quaternion.identity);
                    gm.transform.parent = treesHolder.transform;
                }
            }
        }
        float x1 = 0;
        float x2 = 0;
        float y1 = 0;
        float y2 = 0;
        var vObjects = FindObjectsOfType<VirtualObject>().ToList();
        var rnd = rn;

        k = 0; ;
        loadProgress.text = "Resources build : 0%";
        yield return new WaitForSeconds(0.2f);
        for (int x = 0; x < noise.width; x++)
        {
            k++;
            if (k > 10)
            {
                yield return new WaitForSeconds(0.001f);
                k = 0;
            }
            for (int y = 0; y < noise.height; y++)
            {
                if (fulled[y * noise.width + x] == true) continue;
                var canSpawn = true;


                loadProgress.text = "Resources build : " + (((float)((float)x + (float)y) / (float)((float)noise.width + (float)noise.height)) * 100).ToString("0.00") + "%";

                for (int i = 0; i < vObjects.Count; i++)
                {
                    if (Dist(vObjects[i].transform.position, new Vector2((float)x / 2.5f, (float)y / 2.5f)) <= 3.5f)
                    {
                        canSpawn = false;
                        break;
                    }
                }
                if (canSpawn)
                {

                    var id = rnd.Next(0, rocks.Length);
                    if (rocks[id].GetComponent<VirtualObject>().VirtualName == "Enemie")
                    {
                        if (Dist(hu.transform.position, new Vector2((float)x / 2.5f, (float)y / 2.5f)) <= 20f) continue;
                    }
                    var gm = Instantiate(rocks[id], new Vector3((float)x / 2.5f, (float)y / 2.5f, 0), Quaternion.identity);
                    if (gm.GetComponent<VirtualObject>().VirtualName == "Enemie")
                    {
                        gm.transform.parent = enemiesholder.transform;
                    }
                    else
                    {

                        gm.transform.parent = envHolder.transform;
                    }
                    vObjects.Add(gm.GetComponent<VirtualObject>());
                }
            }
        }

        yield return new WaitForSeconds(0.2f);

        Camera.main.transform.position = new Vector3(hu.transform.position.x, hu.transform.position.y, -10);
        yield return new WaitForSeconds(0.2f);

        loadProgress.text = "Paths scan";
        FindObjectOfType<AstarPath>().Scan();
        loadCanvas.SetActive(false);
    }

    public float Dist(Vector2 one, Vector2 two)
    {
       float x1 = one.x;
       float x2 = two.x;
             
       float y1 = one.y;
       float y2 = two.y;

        return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);

    }
    public Texture2D _CalcNoise(Vector2Int cords, Vector2Int mapSize, float scale, Vector2 offcet)
    {
        print("Gen");
        Color[] pix = new Color[mapSize.x * mapSize.y];
        float maxNoise = float.MinValue;
        float minNoise = float.MaxValue;
        Texture2D noiseTex = new Texture2D(mapSize.x, mapSize.y);
        for (float y = 0; y < noiseTex.height; y++)
        {
            for (float x = 0; x < noiseTex.width; x++)
            {
                float amplitude = 1;
                float freq = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float xCoord = (((cords.x * mapSize.x) + (x / noiseTex.width) * (scale * 2)) + offcet.x) * freq;
                    float yCoord = (((cords.y * mapSize.y) + (y / noiseTex.height) * (scale * 2)) + offcet.y) * freq;

                    float sample = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;

                    noiseHeight += sample * amplitude;
                    amplitude *= persistance;
                    freq *= lacunar;
                }
                if (noiseHeight > maxNoise)
                {
                    maxNoise = noiseHeight;
                }
                if (noiseHeight < minNoise)
                {
                    minNoise = noiseHeight;
                }
                pix[(int)y * noiseTex.width + (int)x] = new Color(noiseHeight, noiseHeight, noiseHeight);
            }
        }
        for (int y = 0; y < noiseTex.height; y++)
        {
            for (int x = 0; x < noiseTex.width; x++)
            {
                float sample = pix[(int)y * noiseTex.width + (int)x].r;
                sample = Mathf.InverseLerp(minNoise, maxNoise, sample);
                sample = sample <= rounder ? sample = 1 : sample = 0;
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
            }
        }
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
        return noiseTex;
    } //NO WHILE


    public Texture2D _CalcNoise(Vector2Int cords, Vector2Int mapSize, float scale, Vector2 offcet, float pers, float lac, int oct, float rounder)
    {

        Color[] pix = new Color[mapSize.x * mapSize.y];
        float maxNoise = float.MinValue;
        float minNoise = float.MaxValue;
        Texture2D noiseTex = new Texture2D(mapSize.x, mapSize.y);
        for (float y = 0; y < noiseTex.height; y++)
        {
            for (float x = 0; x < noiseTex.width; x++)
            {
                float amplitude = 1;
                float freq = 1;
                float noiseHeight = 0;

                for (int i = 0; i < oct; i++)
                {
                    float xCoord = (((cords.x * mapSize.x) + (x / noiseTex.width) * (scale * 2)) + offcet.x) * freq;
                    float yCoord = (((cords.y * mapSize.y) + (y / noiseTex.height) * (scale * 2)) + offcet.y) * freq;

                    float sample = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;

                    noiseHeight += sample * amplitude;
                    amplitude *= pers;
                    freq *= lac;
                }
                if (noiseHeight > maxNoise)
                {
                    maxNoise = noiseHeight;
                }
                if (noiseHeight < minNoise)
                {
                    minNoise = noiseHeight;
                }
                pix[(int)y * noiseTex.width + (int)x] = new Color(noiseHeight, noiseHeight, noiseHeight);
            }
        }
        for (int y = 0; y < noiseTex.height; y++)
        {
            for (int x = 0; x < noiseTex.width; x++)
            {
                float sample = pix[(int)y * noiseTex.width + (int)x].r;
                sample = Mathf.InverseLerp(minNoise, maxNoise, sample);
                sample = sample <= rounder ? sample = 1 : sample = 0;
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
            }
        }
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
        return noiseTex;
    } //NO WHILE
}
