using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TopDownSurvivor.Core;
using TopDownSurvivor.Enemies;
using TopDownSurvivor.Player;
using TopDownSurvivor.Systems;
using TopDownSurvivor.Weapons;

namespace TopDownSurvivor.Editor
{
    /// <summary>
    /// 一键搭建场景：创建玩家、敌人、刷怪器、GameManager，并配置相机。
    /// 菜单：Tools &gt; TopDownSurvivor &gt; Setup Scene
    /// </summary>
    public static class TopDownSurvivorSceneSetup
    {
        private const string MenuPath = "Tools/TopDownSurvivor/Setup Scene";
        private const string PlaceholderAssetPath = "Assets/Sprites/PlayerPlaceholder.png";
        private const string BulletPrefabPath = "Assets/Prefabs/Bullet.prefab";
        private const string EnemyPrefabPath = "Assets/Prefabs/Enemy.prefab";
        private const string RangedEnemyPrefabPath = "Assets/Prefabs/RangedEnemy.prefab";
        private const string EnemyBulletPrefabPath = "Assets/Prefabs/EnemyBullet.prefab";

        [MenuItem(MenuPath)]
        public static void SetupScene()
        {
            SetupPlayer();
            SetupEnemy();
            SetupEnemySpawner();
            SetupGameManager();
            SetupCamera();
            SetupExperienceSystem();
            SetupUpgradeManager();
            SetupHUD();
            SetupMenu();
            SetupAudioManager();
            SetupDamagePopupManager();

            var activeScene = EditorSceneManager.GetActiveScene();
            if (string.IsNullOrEmpty(activeScene.path))
            {
                Debug.LogWarning("[TopDownSurvivor] 当前是未保存的空场景，请先双击打开 Assets/Scenes/SampleScene，再运行本工具。");
            }
            else
            {
                EditorSceneManager.SaveScene(activeScene);
            }

            Debug.Log("[TopDownSurvivor] 场景搭建完成：Player / Enemy / EnemySpawner / GameManager / Main Camera 已就绪。");
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

            // 自动攻击：挂组件并自动设置子弹预制体引用
            PlayerAutoAttack autoAttack = player.GetComponent<PlayerAutoAttack>();
            if (autoAttack == null)
            {
                autoAttack = player.AddComponent<PlayerAutoAttack>();
            }

            Bullet bulletPrefab = AssetDatabase.LoadAssetAtPath<Bullet>(BulletPrefabPath);
            SetReference(autoAttack, "_bulletPrefab", bulletPrefab);

            if (player.GetComponent<PlayerExperience>() == null)
            {
                player.AddComponent<PlayerExperience>();
            }

            if (player.GetComponent<PlayerHealth>() == null)
            {
                player.AddComponent<PlayerHealth>();
            }

            if (player.GetComponent<OrbitingWeapon>() == null)
            {
                player.AddComponent<OrbitingWeapon>();
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

        private static void SetupEnemySpawner()
        {
            GameObject spawner = GameObject.Find("EnemySpawner");
            if (spawner == null)
            {
                spawner = new GameObject("EnemySpawner");
            }

            EnemySpawner es = spawner.GetComponent<EnemySpawner>();
            if (es == null)
            {
                es = spawner.AddComponent<EnemySpawner>();
            }

            // 自动设置敌人预制体引用
            Enemy enemyPrefab = AssetDatabase.LoadAssetAtPath<Enemy>(EnemyPrefabPath);
            SetReference(es, "_enemyPrefab", enemyPrefab);

            // 自动设置远程怪预制体引用
            RangedEnemy rangedPrefab = EnsureRangedEnemyPrefab();
            SetReference(es, "_rangedEnemyPrefab", rangedPrefab);
        }

        private static RangedEnemy EnsureRangedEnemyPrefab()
        {
            RangedEnemy ranged = AssetDatabase.LoadAssetAtPath<RangedEnemy>(RangedEnemyPrefabPath);
            if (ranged == null)
            {
                GameObject go = new GameObject("RangedEnemy");
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = EnsurePlaceholderSprite();
                sr.color = new Color(1f, 0.6f, 0.2f); // 橙色
                CircleCollider2D col = go.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                col.radius = 0.5f;
                go.AddComponent<RangedEnemy>();
                ranged = PrefabUtility.SaveAsPrefabAsset(go, RangedEnemyPrefabPath).GetComponent<RangedEnemy>();
                Object.DestroyImmediate(go);
            }

            // 设置远程怪的子弹引用
            EnemyBullet bullet = EnsureEnemyBulletPrefab();
            SerializedObject so = new SerializedObject(ranged);
            SerializedProperty prop = so.FindProperty("_bulletPrefab");
            if (prop != null && prop.objectReferenceValue == null)
            {
                prop.objectReferenceValue = bullet;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(ranged);
                AssetDatabase.SaveAssets();
            }

            return ranged;
        }

        private static EnemyBullet EnsureEnemyBulletPrefab()
        {
            EnemyBullet bullet = AssetDatabase.LoadAssetAtPath<EnemyBullet>(EnemyBulletPrefabPath);
            if (bullet != null)
            {
                return bullet;
            }

            GameObject go = new GameObject("EnemyBullet");
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = EnsurePlaceholderSprite();
            sr.color = new Color(1f, 0.5f, 0f); // 橙色子弹
            go.AddComponent<EnemyBullet>();

            EnemyBullet prefab = PrefabUtility.SaveAsPrefabAsset(go, EnemyBulletPrefabPath).GetComponent<EnemyBullet>();
            Object.DestroyImmediate(go);
            return prefab;
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

            if (cam.GetComponent<CameraShake>() == null)
            {
                cam.gameObject.AddComponent<CameraShake>();
            }
        }

        private static void SetupExperienceSystem()
        {
            // 经验掉落器：订阅敌人死亡事件，生成经验宝石
            GameObject dropper = GameObject.Find("ExperienceDropper");
            if (dropper == null)
            {
                dropper = new GameObject("ExperienceDropper");
            }

            ExperienceDropper ed = dropper.GetComponent<ExperienceDropper>();
            if (ed == null)
            {
                ed = dropper.AddComponent<ExperienceDropper>();
            }

            ExperienceGem gemPrefab = EnsureGemPrefab();
            SetReference(ed, "_gemPrefab", gemPrefab);
        }

        private static void SetupUpgradeManager()
        {
            GameObject um = GameObject.Find("UpgradeManager");
            if (um == null)
            {
                um = new GameObject("UpgradeManager");
            }

            if (um.GetComponent<UpgradeManager>() == null)
            {
                um.AddComponent<UpgradeManager>();
            }
        }

        private static void SetupHUD()
        {
            GameObject hud = GameObject.Find("HUDManager");
            if (hud == null)
            {
                hud = new GameObject("HUDManager");
            }

            if (hud.GetComponent<HUDManager>() == null)
            {
                hud.AddComponent<HUDManager>();
            }
        }

        private static void SetupMenu()
        {
            GameObject menu = GameObject.Find("MenuManager");
            if (menu == null)
            {
                menu = new GameObject("MenuManager");
            }

            if (menu.GetComponent<MenuManager>() == null)
            {
                menu.AddComponent<MenuManager>();
            }
        }

        private static void SetupAudioManager()
        {
            GameObject am = GameObject.Find("AudioManager");
            if (am == null)
            {
                am = new GameObject("AudioManager");
            }

            if (am.GetComponent<AudioManager>() == null)
            {
                am.AddComponent<AudioManager>();
            }
        }

        private static void SetupDamagePopupManager()
        {
            GameObject dpm = GameObject.Find("DamagePopupManager");
            if (dpm == null)
            {
                dpm = new GameObject("DamagePopupManager");
            }

            if (dpm.GetComponent<DamagePopupManager>() == null)
            {
                dpm.AddComponent<DamagePopupManager>();
            }
        }

        /// <summary>
        /// 确保经验宝石预制体存在，不存在则自动创建并保存。
        /// </summary>
        private static ExperienceGem EnsureGemPrefab()
        {
            const string gemPath = "Assets/Prefabs/ExperienceGem.prefab";
            ExperienceGem gem = AssetDatabase.LoadAssetAtPath<ExperienceGem>(gemPath);
            if (gem != null)
            {
                return gem;
            }

            GameObject go = new GameObject("ExperienceGem");
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = EnsurePlaceholderSprite();
            sr.color = new Color(0.4f, 1f, 0.4f); // 绿色

            CircleCollider2D col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.4f;

            go.AddComponent<ExperienceGem>();

            ExperienceGem prefab = PrefabUtility.SaveAsPrefabAsset(go, gemPath).GetComponent<ExperienceGem>();
            Object.DestroyImmediate(go);
            return prefab;
        }

        /// <summary>
        /// 通过 SerializedObject 设置组件上的私有 [SerializeField] 字段（用于自动配置预制体引用）。
        /// </summary>
        private static void SetReference(UnityEngine.Object target, string fieldName, UnityEngine.Object value)
        {
            SerializedObject so = new SerializedObject(target);
            SerializedProperty prop = so.FindProperty(fieldName);
            if (prop != null)
            {
                prop.objectReferenceValue = value;
                so.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 生成并持久化一张白色占位图，作为临时精灵。
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
