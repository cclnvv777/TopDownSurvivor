using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TopDownSurvivor.Core;
using TopDownSurvivor.Enemies;
using TopDownSurvivor.Player;

namespace TopDownSurvivor.Editor
{
    /// <summary>
    /// 一键搭建场景：创建玩家、GameManager，并配置相机。
    /// 菜单：Tools &gt; TopDownSurvivor &gt; Setup Scene
    /// </summary>
    public static class TopDownSurvivorSceneSetup
    {
        private const string MenuPath = "Tools/TopDownSurvivor/Setup Scene";
        private const string PlaceholderAssetPath = "Assets/Sprites/PlayerPlaceholder.png";

        [MenuItem(MenuPath)]
        public static void SetupScene()
        {
            SetupPlayer();
            SetupEnemy();
            SetupGameManager();
            SetupCamera();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[TopDownSurvivor] 场景搭建完成：Player / Enemy / GameManager / Main Camera 已就绪。");
        }

        private static void SetupPlayer()
        {
            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                player = new GameObject("Player");
            }

            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                sr = player.AddComponent<SpriteRenderer>();
            }

            sr.sprite = EnsurePlaceholderSprite();
            sr.color = new Color(0.35f, 0.75f, 1f); // 淡蓝色占位方块

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = player.AddComponent<Rigidbody2D>();
            }

            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            CircleCollider2D col = player.GetComponent<CircleCollider2D>();
            if (col == null)
            {
                col = player.AddComponent<CircleCollider2D>();
            }

            col.radius = 0.5f;

            if (player.GetComponent<PlayerMovement>() == null)
            {
                player.AddComponent<PlayerMovement>();
            }
        }

        private static void SetupEnemy()
        {
            GameObject enemy = GameObject.Find("Enemy");
            if (enemy == null)
            {
                enemy = new GameObject("Enemy");
            }

            SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                sr = enemy.AddComponent<SpriteRenderer>();
            }

            sr.sprite = EnsurePlaceholderSprite();
            sr.color = new Color(1f, 0.35f, 0.35f); // 红色占位方块

            if (enemy.GetComponent<Enemy>() == null)
            {
                enemy.AddComponent<Enemy>();
            }

            // 初始放在玩家右上方，方便观察追击
            enemy.transform.position = new Vector3(3f, 3f, 0f);
        }

        private static void SetupGameManager()
        {
            GameObject gm = GameObject.Find("GameManager");
            if (gm == null)
            {
                gm = new GameObject("GameManager");
            }

            if (gm.GetComponent<GameManager>() == null)
            {
                gm.AddComponent<GameManager>();
            }
        }

        private static void SetupCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                camObj.AddComponent<AudioListener>();
                cam = camObj.AddComponent<Camera>();
            }

            cam.orthographic = true;
            cam.orthographicSize = 6f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.08f, 0.08f, 0.12f);
        }

        /// <summary>
        /// 生成并持久化一张白色占位图，作为玩家的临时精灵。
        /// </summary>
        private static Sprite EnsurePlaceholderSprite()
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(PlaceholderAssetPath);
            if (sprite != null)
            {
                return sprite;
            }

            const int size = 32;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[size * size];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }

            tex.SetPixels(pixels);
            tex.Apply();

            byte[] png = tex.EncodeToPNG();
            Object.DestroyImmediate(tex);

            string fullPath = Path.Combine(Application.dataPath, "Sprites", "PlayerPlaceholder.png");
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            File.WriteAllBytes(fullPath, png);

            AssetDatabase.ImportAsset(PlaceholderAssetPath);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(PlaceholderAssetPath);
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false;
                importer.filterMode = FilterMode.Point;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(PlaceholderAssetPath);
        }
    }
}
